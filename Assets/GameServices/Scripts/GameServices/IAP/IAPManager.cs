
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GameServices.IAP
{
    public class IAPManager : MonoBehaviour
    {
#if GameServices_UnityIAP
        private StoreController storeController;
        private IAPConfig iapConfig;

        public bool IsInitialized => storeController != null;

        public void Initialize(IAPConfig config)
        {
            iapConfig = config;

            if (iapConfig == null || !iapConfig.enableIAP)
            {
                Debug.Log("[GameServices][IAP] Config missing or disabled.");
                return;
            }

            storeController = UnityIAPServices.StoreController();

            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;

            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += OnPurchaseFailed;

            storeController.Connect();

            FetchProducts();
        }

        private void FetchProducts()
        {
            List<ProductDefinition> products =
                new List<ProductDefinition>();

            foreach (var product in iapConfig.products)
            {
                products.Add(
                    new ProductDefinition(
                        product.productId,
                        product.productType));
            }

            storeController.FetchProducts(products);
        }

        public void Purchase(string productId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning(
                    "[GameServices][IAP] Store not initialized.");
                return;
            }

            storeController.PurchaseProduct(productId);
        }

        public void RestorePurchases()
        {
            if (!IsInitialized)
                return;

            storeController.RestoreTransactions(
                (success, error) =>
                {
                    Debug.Log(
                        $"[GameServices][IAP] Restore: {success} | {error}");
                });
        }

        public bool IsPurchased(string productId)
        {
            if (!IsInitialized)
                return false;

            Product product =
                storeController.GetProductById(productId);

            return product != null &&
                   product.hasReceipt &&
                   product.definition.type ==
                   ProductType.NonConsumable;
        }

        public string GetLocalizedPrice(string productId)
        {
            if (!IsInitialized)
                return string.Empty;

            Product product =
                storeController.GetProductById(productId);

            if (product == null || product.metadata == null)
                return string.Empty;

            return product.metadata.localizedPriceString;
        }

        private void OnProductsFetched(
            List<Product> products)
        {
            Debug.Log(
                $"[GameServices][IAP] Products fetched: {products.Count}");
        }

        private void OnProductsFetchFailed(
            ProductFetchFailed failed)
        {
            Debug.LogError(
                $"[GameServices][IAP] Fetch failed: {failed.FailureReason}");
        }

        private void OnPurchasePending(
            PendingOrder order)
        {
            Debug.Log(
                "[GameServices][IAP] Purchase pending.");

            storeController.ConfirmPurchase(order);
        }

        private void OnPurchaseConfirmed(
            Order order)
        {
            Debug.Log(
                "[GameServices][IAP] Purchase confirmed.");

            foreach (var item in order.CartOrdered.Items())
            {
                string productId =
                    item.Product.definition.id;

                decimal price =
                    item.Product.metadata.localizedPrice;

                string currency =
                    item.Product.metadata.isoCurrencyCode;

                string transactionId =
                    order.Info.TransactionID;

                Debug.Log(
                    $"[GameServices][IAP] Purchased: {productId}");

                GameServices.IAPPackPurchase(
                    productId,
                    currency,
                    price,
                    transactionId);
            }
        }

        private void OnPurchaseFailed(
            FailedOrder order)
        {
            Debug.LogWarning(
                $"[GameServices][IAP] Purchase failed: {order.FailureReason}");
        }
#endif
    }
}

