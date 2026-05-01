using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static event Action<SwipeDirection> OnSwipeDetected;
	public static InputManager Instance { get; private set; }

	[Header("Ayarlar")]
	[SerializeField] private float swipeThreshold = 50f; 

	private Vector2 startPosition;
	private bool isDragging = false;
	public enum SwipeDirection { Up, Down, Left, Right }

	void Awake()
	{
		if (Instance != null && Instance != this) Destroy(this);
		else Instance = this;
	}

	void Update()
	{
		HandleInput();
	}

	private void HandleInput()
	{
		// 1. Dokunma veya Tıklama Başlangıcı
		if (Pointer.current.press.wasPressedThisFrame)
		{
			startPosition = Pointer.current.position.ReadValue();
			isDragging = true;
		}

		// 2. Dokunma veya Tıklama Bitişi
		if (Pointer.current.press.wasReleasedThisFrame && isDragging)
		{
			Vector2 endPosition = Pointer.current.position.ReadValue();
			isDragging = false;
			DetectSwipe(endPosition);
		}
	}

	private void DetectSwipe(Vector2 endPosition)
	{
		Vector2 delta = endPosition - startPosition;


		if (delta.magnitude > swipeThreshold)
		{
			if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				if (delta.x > 0) SendSwipe(SwipeDirection.Right);
				else SendSwipe(SwipeDirection.Left);
			}
			else
			{
				// Dikey hareket
				if (delta.y > 0) SendSwipe(SwipeDirection.Up);
				else SendSwipe(SwipeDirection.Down);
			}
		}
	}

	private void SendSwipe(SwipeDirection direction)
	{
		OnSwipeDetected?.Invoke(direction);
		Debug.Log("Gelen yön: " + direction.ToString());
	}
}