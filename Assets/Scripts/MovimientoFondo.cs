using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
	[SerializeField] private RawImage background;
	[SerializeField] private float speed = 0.05f;

	private void Update()
	{
		Rect uvRect = background.uvRect;

		uvRect.y += speed * Time.deltaTime;

		if (uvRect.y >= 1f)
		{
			uvRect.y -= 1f;
		}

		background.uvRect = uvRect;
	}
}