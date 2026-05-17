using System;
using UnityEngine;
using UnityEngine.UI;
using GameServices.Core;
using TMPro;
using UnityEngine.Purchasing;

public class Testing : MonoBehaviour
{
    public int levelNumber, coins;
    public Button homeButton;
    public Product iapProduct;
    public Button adsButton, analyticsButton, iapButton;
    public Button showBanner, hideBanner, showInterstitials, showRewarded;
    public Button levelStart, levelFail,levelComplete, interstitialsWatched, rewardedWatched, IapPurchased;
    public Button noAds, starterPack, coinPack;
    public TMP_Text levelNumberTxt, coinsTxt, adsSummary, analyticsSummary, iapSummary;
    public Transform mainParent, adsParent, analyticsParent, iapParent;


    private void Start()
    {
        coinsTxt.text = "coins: "+ coins;
        levelNumberTxt.text = "Level: "+levelNumber;
        
        ButtonsActions();
    }

    private void ButtonsActions()
    {
        homeButton.onClick.AddListener(() =>
        {
            mainParent.gameObject.SetActive(true);
            adsParent.gameObject.SetActive(false);
            analyticsParent.gameObject.SetActive(false); 
            iapParent.gameObject.SetActive(false);
        });
        
        adsButton.onClick.AddListener(() =>
        {
            mainParent.gameObject.SetActive(false);
            adsParent.gameObject.SetActive(true);
        });

        analyticsButton.onClick.AddListener(() =>
        {
            mainParent.gameObject.SetActive(false);
            analyticsParent.gameObject.SetActive(true);
        });

        iapButton.onClick.AddListener(() =>
        {
            mainParent.gameObject.SetActive(false);
            iapParent.gameObject.SetActive(true);
        });


        showBanner.onClick.AddListener(GameService.ShowBanner);
        hideBanner.onClick.AddListener(GameService.HideBanner);

        showInterstitials.onClick.AddListener(() =>
        {
            GameService.ShowInterstitial("Whatever", () => { adsSummary.text = "ShowedInterstitials"; });
        });

        showRewarded.onClick.AddListener(() =>
        {
            GameService.ShowRewarded("Whatever", (s) => { adsSummary.text = s ? "Rewarded" : "Not Rewarded"; });
        });
        
        levelStart.onClick.AddListener(() =>
        {
            GameService.LevelStart(levelNumber, "");
            analyticsSummary.text = "Level_start_ " + levelNumber;
        });
        
        levelFail.onClick.AddListener(() =>
        {
            GameService.LevelEnd(levelNumber, false, "");
            analyticsSummary.text = "Level_Failed_ " + levelNumber;
        });
        
        levelComplete.onClick.AddListener(() =>
        {
            GameService.LevelEnd(levelNumber, true, "");
            analyticsSummary.text = "Level_Completed_ " + levelNumber;
            
            levelNumber += 1;
            levelNumberTxt.text ="Level " + levelNumber;
        });
        
        interstitialsWatched.onClick.AddListener(() =>
        {
            GameService.InterstitialAdWatched("");
            analyticsSummary.text = "Interstitial_Watched " + levelNumber;
        });
        
        rewardedWatched.onClick.AddListener(() =>
        {
            GameService.RewardedAdWatched("");
            analyticsSummary.text = "Rewarded_Watched " + levelNumber;
        });
        
        IapPurchased.onClick.AddListener(() =>
        {
            GameService.IAPPackPurchase(iapProduct.definition.id, iapProduct.metadata.isoCurrencyCode, iapProduct.metadata.localizedPrice, "");
            analyticsSummary.text = "Rewarded_Watched " + levelNumber;
        });
        
        noAds.onClick.AddListener(() =>
        {
            GameService.Purchase("NoAds");
            iapSummary.text = "NoAds Purchased";
        });
        
        starterPack.onClick.AddListener(() =>
        {
            GameService.Purchase("StarterPack");
            iapSummary.text = "StarterPack Purchased";
        });
        
        coinPack.onClick.AddListener(() =>
        {
            coins += 10;
            coinsTxt.text = "coins: "+coins;
            GameService.Purchase("Coins Pack");
            iapSummary.text = "CoinsPack Purchased";
        });
    }
}