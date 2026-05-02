#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input Sistemi için gerekli
using System.IO;

public class ScreenshotCapturer : MonoBehaviour
{
	[Header("Ayarlar")]
	public string folderName = "OyunEkranGoruntuleri";
	public int resolutionUpscale = 2;

	// Yeni input sisteminde tuş kontrolü için
	private UnityEngine.InputSystem.Controls.KeyControl sKey;

	void Start()
	{
		// Klavyedeki S tuşunu tanımlıyoruz
		sKey = Keyboard.current.sKey;

		// Bu objeyi sadece Editor'da çalışacak şekilde etiketleyelim
		gameObject.tag = "EditorOnly";
	}

	void Update()
	{
		// Yeni Input Sistemi: S tuşuna basıldığı an (wasPressedThisFrame)
		if (sKey != null && sKey.wasPressedThisFrame)
		{
			Capture();
		}
	}

	public void Capture()
	{
		string directoryPath = Path.Combine(Application.dataPath, "../" + folderName);
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}

		string fileName = "Screen_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
		string fullPath = Path.Combine(directoryPath, fileName);

		ScreenCapture.CaptureScreenshot(fullPath, resolutionUpscale);

		Debug.Log($"<color=#00ff00><b>[SCREENSHOT]</b></color> Kaydedildi: {fullPath}");
	}
}
#endif