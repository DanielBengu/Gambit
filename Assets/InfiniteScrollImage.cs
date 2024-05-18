using UnityEngine;

public class InfiniteScrollImage : MonoBehaviour
{
    public float scrollSpeed = 1f; // Speed of scrolling
    private RectTransform rectTransform;
    private float imageWidth;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        imageWidth = rectTransform.rect.width;
    }

    void Update()
    {
        // Move the image to the left
        rectTransform.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);

        // Check if the image has completely moved out of the screen
        if (rectTransform.anchoredPosition.x <= - (imageWidth * rectTransform.localScale.x))
        {
            // Reset its position to the right end of the other image
            rectTransform.anchoredPosition += new Vector2(2 * imageWidth * rectTransform.localScale.x, 0);
        }
    }
}
