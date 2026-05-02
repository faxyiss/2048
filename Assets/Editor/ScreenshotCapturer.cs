using UnityEngine;
using System.IO;

public class ScreenshotCapturer : MonoBehaviour
{
	[Header("Ayarlar")]
	public string folderName = "OyunEkranGoruntuleri";
	public int resolutionUpscale = 2; // 1=Ekran çözünürlüğü, 2=İki katı (Daha kaliteli)
	public KeyCode captureKey = KeyCode.S;

	void Update()
	{
		// Belirlediğin tuşa basınca ekran görüntüsü al
		if (Input.GetKeyDown(captureKey))
		{
			Capture();
		}
	}

	public void Capture()
	{
		// Klasör yoksa oluştur
		string directoryPath = Path.Combine(Application.dataPath, "../" + folderName);
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}

		// Dosya ismini tarih ve saatle benzersiz yap
		string fileName = "Screen_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
		string fullPath = Path.Combine(directoryPath, fileName);

		// Ekran görüntüsünü al ve kaydet
		ScreenCapture.CaptureScreenshot(fullPath, resolutionUpscale);

		Debug.Log("<color=green>Ekran Görüntüsü Kaydedildi: </color>" + fullPath);
	}
}
