using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
	public static AdManager Instance { get; private set; }
	private InterstitialAd interstitialAd;

	// Google'ın standart test Geçiş Reklamı ID'si
	private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
	private BannerView bannerView;

	// Test Banner ID'si
	private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";

	void Awake()
	{
		if (Instance != null && Instance != this) Destroy(gameObject);
		else Instance = this;
	}

	void Start()
	{
		MobileAds.Initialize(initStatus => { });
		LoadBannerAd();
	}

	public void LoadBannerAd()
	{
		// Eğer zaten bir banner varsa yok et
		if (bannerView != null)
		{
			bannerView.Destroy();
		}

		// Ekranın altına (Bottom) yerleştirilen standart banner
		bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

		var adRequest = new AdRequest();
		bannerView.LoadAd(adRequest);

		// Başlangıçta reklamı gizle (Menüde görünmesin diye)
		bannerView.Hide();
	}
	public void LoadInterstitialAd()
	{
		// Eski reklamı temizle
		if (interstitialAd != null)
		{
			interstitialAd.Destroy();
			interstitialAd = null;
		}

		var adRequest = new AdRequest();
		InterstitialAd.Load(interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
		{
			if (error != null || ad == null) return;
			interstitialAd = ad;

			// Reklam kapandığında yenisini otomatik yükle ki bir sonraki sefer hazır olsun
			interstitialAd.OnAdFullScreenContentClosed += () => { LoadInterstitialAd(); };
		});
	}

	public void ShowInterstitial()
	{
		if (interstitialAd != null && interstitialAd.CanShowAd())
		{
			interstitialAd.Show();
		}
		else
		{
			// Reklam hazır değilse bile yenisini yüklemeye çalış
			LoadInterstitialAd();
		}
	}
	public void ShowBanner()
	{
		if (bannerView != null) bannerView.Show();
	}

	public void HideBanner()
	{
		if (bannerView != null) bannerView.Hide();
	}
}