using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;
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
        private HashSet<string> purchasedProducts = new();
        private readonly Dictionary<string, Action<bool>> purchaseCallbacks = new();
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

            try
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log("[GameServices][IAP] Unity Services initialized.");
                }
                else
                {
                    Debug.Log($"[GameServices][IAP] Unity Services state: {UnityServices.State}");
                }

                storeController = UnityIAPServices.StoreController();

                storeController.OnProductsFetched += OnProductsFetched;
                storeController.OnProductsFetchFailed += OnProductsFetchFailed;
                storeController.OnPurchasePending += OnPurchasePending;
                storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
                storeController.OnPurchaseFailed += OnPurchaseFailed;
                storeController.OnStoreDisconnected += OnStoreDisconnected;

                await storeController.Connect();
                FetchProducts();
            }
            catch (Exception ex)
            {
                isInitializing = false;
                productsFetched = false;
                Debug.LogError($"[GameServices][IAP] Initialization failed: {ex.Message}");
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
        
        private IAPProductData GetProductByName(string productName)
        {
            return iapConfig.products.Find(x => x.productName == productName);
        }

        public void Purchase(string productName, Action<bool> onPurchaseCompleted = null)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[GameServices][IAP] Store not initialized or products not fetched yet.");
                onPurchaseCompleted?.Invoke(false);
                return;
            }

            if (string.IsNullOrWhiteSpace(productName))
            {
                Debug.LogWarning("[GameServices][IAP] Product id is empty.");
                onPurchaseCompleted?.Invoke(false);
                return;
            }
            
            var productData = GetProductByName(productName);
            if (productData == null)
            {
                Debug.LogWarning($"[GameServices][IAP] Product not found in config: {productName}");
                onPurchaseCompleted?.Invoke(false);
                return;
            }

            string storeId = productData.GetStoreID();
            Product product = storeController.GetProductById(storeId);
            if (product == null)
            {
                Debug.LogWarning($"[GameServices][IAP] Product not found: {productName}::{storeId}");
                onPurchaseCompleted?.Invoke(false);
                return;
            }

            if (onPurchaseCompleted != null)
                purchaseCallbacks[storeId] = onPurchaseCompleted;

            storeController.PurchaseProduct(product);
        }

        public void RestorePurchases(Action<bool, List<string>> onRestoreCompleted = null)
        {
            if (storeController == null)
            {
                onRestoreCompleted?.Invoke(false, new List<string>());
                return;
            }

            storeController.RestoreTransactions((success, error) =>
            {
                Debug.Log($"[GameServices][IAP] Restore: {success} | {error}");

                storeController.FetchPurchases();

                var restoredProducts = new List<string>(purchasedProducts);

                onRestoreCompleted?.Invoke(success, restoredProducts);
            });
        }

        public bool IsPurchased(string productName)
        {
            if (storeController == null || string.IsNullOrWhiteSpace(productName))
                return false;
            var productData = GetProductByName(productName);
            var product = storeController.GetProductById(productData.GetStoreID());
            return product != null && purchasedProducts.Contains(productData.GetStoreID());
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
            
            purchasedProducts.Clear();

            foreach (var product in products)
            {
                if (product == null)
                    continue;
                
                purchasedProducts.Add(product.definition.id);
            }
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
                purchasedProducts.Add(productId);
                
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
                InvokePurchaseCallback(productId, true);
            }
        }

        private void OnPurchaseFailed(FailedOrder order)
        {
            Debug.LogWarning($"[GameServices][IAP] Purchase failed: {order.FailureReason}");

            foreach (var item in order.CartOrdered.Items())
            {
                string productId = item.Product.definition.id;
                InvokePurchaseCallback(productId, false);
            }
        }

        private void InvokePurchaseCallback(string productId, bool success)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return;

            if (!purchaseCallbacks.TryGetValue(productId, out Action<bool> callback))
                return;

            purchaseCallbacks.Remove(productId);
            callback?.Invoke(success);
        }
#endif
    }
}
