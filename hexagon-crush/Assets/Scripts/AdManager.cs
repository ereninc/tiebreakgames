using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour
{
    string App_ID = "ca-app-pub-8665605135575423~9711132005";
    string Interstitial_Ad_ID = "ca-app-pub-8665605135575423/3145723657";

    string App_IDtest = "ca-app-pub-3940256099942544/1033173712";
    private InterstitialAd interstitial;

    [System.Obsolete]
    void Start()
    {
        MobileAds.Initialize(App_IDtest);
        RequestInterstitial();
    }

    // Update is called once per frame
    void Update()
    {
        ShowInterstitialAd();
    }

    public void RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(App_IDtest);
        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void ShowInterstitialAd()
    {
        if (GridManager.instance._gameStatus == 9)
        {
            if (this.interstitial.IsLoaded())
            {
                Debug.Log("Reklam yüklendi.");
                this.interstitial.Show();
                Debug.Log("Reklam gösterildi.");
                StartCoroutine(ExecuteAfterTime(0.15f));
                GridManager.instance._gameStatus = 2;
                RequestInterstitial();
            }
        }
    }
    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        reloadSceneWithDelay();
    }

    void reloadSceneWithDelay()
    {
        SceneManager.LoadScene(0);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

}
