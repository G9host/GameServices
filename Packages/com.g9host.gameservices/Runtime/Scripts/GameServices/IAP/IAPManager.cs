using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using GameServices.Core;

namespace GameServices.IAP
{
    public class IAPManager : MonoBehaviour
    {
#if GAMESERVICES_UNITY_IAP
        private StoreController storeController;
        private IAPConfig iapConfig;
        private bool productsFetched;
        private bool isInitializing;

        public bool IsInitialized => storeController != null && productsFetched;

        public async void Initialize(IAPConfig config)
        {
            if (isInitializing || IsInitialized)
                return;

            iapConfig = config;

            if (iapConfig == null || !iapConfig.enableIAP)
            {
                Debug.Log("[GameServices][IAP] Config missing or disabled.");
                return;
            }

            isInitializing = true;
            productsFetched = false;

            storeController = UnityIAPServices.StoreController();

            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += OnPurchaseFailed;
            storeController.OnStoreDisconnected += OnStoreDisconnected;

            try
            {
                await storeController.Connect();
                FetchProducts();
            }
            catch (Exception ex)
            {
                isInitializing = false;
                Debug.LogError($"[GameServices][IAP] Store connection failed: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            if (storeController == null)
                return;

            storeController.OnProductsFetched -= OnProductsFetched;
            storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
            storeController.OnPurchasePending -= OnPurchasePending;
            storeController.OnPurchaseConfirmed -= OnPurchaseConfirmed;
            storeController.OnPurchaseFailed -= OnPurchaseFailed;
            storeController.OnStoreDisconnected -= OnStoreDisconnected;
        }

        private void FetchProducts()
        {
            var products = new List<ProductDefinition>();

            if (iapConfig.products == null)
            {
                Debug.LogError("[GameServices][IAP] Product list is null.");
                isInitializing = false;
                return;
            }

            foreach (var product in iapConfig.products)
            {
                string id = GetPlatformProductId(product);

                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning($"[GameServices][IAP] Skipped product '{product.productName}' because its platform id is empty.");
                    continue;
                }

                products.Add(new ProductDefinition(id, product.productType));
            }

            if (products.Count == 0)
            {
                Debug.LogError("[GameServices][IAP] No valid products to fetch.");
                isInitializing = false;
                return;
            }

            storeController.FetchProducts(products);
        }

        private static string GetPlatformProductId(IAPProductData product)
        {
#if UNITY_ANDROID
            return product.androidProductId;
#elif UNITY_IOS
            return product.iosProductId;
#elif UNITY_EDITOR_OSX
            return !string.IsNullOrWhiteSpace(product.iosProductId)
                ? product.iosProductId
                : product.androidProductId;
#else
            return !string.IsNullOrWhiteSpace(product.androidProductId)
                ? product.androidProductId
                : product.iosProductId;
#endif
        }

        public void Purchase(string productId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[GameServices][IAP] Store not initialized or products not fetched yet.");
                return;
            }

            if (string.IsNullOrWhiteSpace(productId))
            {
                Debug.LogWarning("[GameServices][IAP] Product id is empty.");
                return;
            }

            Product product = storeController.GetProductById(productId);
            if (product == null)
            {
                Debug.LogWarning($"[GameServices][IAP] Product not found: {productId}");
                return;
            }

            storeController.PurchaseProduct(product);
        }

        public void RestorePurchases()
        {
            if (storeController == null)
                return;

            storeController.RestoreTransactions((success, error) =>
            {
                Debug.Log($"[GameServices][IAP] Restore: {success} | {error}");
                storeController.FetchPurchases();
            });
        }

        public bool IsPurchased(string productId)
        {
            if (storeController == null || string.IsNullOrWhiteSpace(productId))
                return false;

            Product product = storeController.GetProductById(productId);

            return product != null &&
                   product.hasReceipt &&
                   product.definition.type == ProductType.NonConsumable;
        }

        public string GetLocalizedPrice(string productId)
        {
            if (storeController == null || string.IsNullOrWhiteSpace(productId))
                return string.Empty;

            Product product = storeController.GetProductById(productId);
            return product?.metadata?.localizedPriceString ?? string.Empty;
        }

        private void OnProductsFetched(List<Product> products)
        {
            productsFetched = true;
            isInitializing = false;
            Debug.Log($"[GameServices][IAP] Products fetched: {products.Count}");
        }

        private void OnProductsFetchFailed(ProductFetchFailed failed)
        {
            productsFetched = false;
            isInitializing = false;
            Debug.LogError($"[GameServices][IAP] Fetch failed: {failed.FailureReason}");
        }

        private void OnStoreDisconnected(StoreConnectionFailureDescription description)
        {
            productsFetched = false;
            isInitializing = false;
            Debug.LogWarning($"[GameServices][IAP] Store disconnected: {description.message}");
        }

        private void OnPurchasePending(PendingOrder order)
        {
            Debug.Log("[GameServices][IAP] Purchase pending.");

            // Grant the item here if you need server validation first, then confirm only after success.
            storeController.ConfirmPurchase(order);
        }

        private void OnPurchaseConfirmed(Order order)
        {
            Debug.Log("[GameServices][IAP] Purchase confirmed.");

            foreach (var item in order.CartOrdered.Items())
            {
                Product product = item.Product;
                string productId = product.definition.id;
                decimal price = product.metadata.localizedPrice;
                string currency = product.metadata.isoCurrencyCode;
                string transactionId = order.Info.TransactionID;

                Debug.Log($"[GameServices][IAP] Purchased: {productId}");
                
                if (product.definition.type == ProductType.Consumable)
                {
                    switch (productId)
                    {
                        case "":
                            break;
                    }
                }
                else if (product.definition.type == ProductType.NonConsumable)
                {
                    switch (productId)
                    {
                        case "":
                            break;
                    }
                }

                GameService.IAPPackPurchase(productId, currency, price, transactionId);
            }
        }

        private void OnPurchaseFailed(FailedOrder order)
        {
            Debug.LogWarning($"[GameServices][IAP] Purchase failed: {order.FailureReason}");
        }
#endif
    }
}
