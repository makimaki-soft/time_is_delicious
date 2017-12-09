using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Faces : MonoBehaviour
{

    private Sprite NormalSprite;
    [SerializeField]
    private Sprite SmileSprite;
    [SerializeField]
    private Sprite SadSprite;
    [SerializeField]
    private Sprite WorriedSprite;

    private Image ImageCompnent;

    // Use this for initialization
    void Start()
    {
        ImageCompnent = gameObject.GetComponent<Image>();
        NormalSprite = ImageCompnent.sprite;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Smile(float duration)
    {
        StartCoroutine(ExpressFeelings(duration, SmileSprite));
    }

    public void Sad(float duration)
    {
        StartCoroutine(ExpressFeelings(duration, SadSprite));
    }

    public void Thinking(float duration)
    {
        StartCoroutine(ExpressFeelings(duration, WorriedSprite));
    }

    private IEnumerator ExpressFeelings(float duraion, Sprite changeSprite)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duraion)
        {
            float baseTime = Time.time;

            yield return new WaitUntil(() =>
            {
                return Time.time - baseTime > 0.25f;
            });

            ImageCompnent.sprite = changeSprite;

            yield return new WaitUntil(() =>
            {
                return Time.time - baseTime > 0.5f;
            });

            ImageCompnent.sprite = NormalSprite;

            yield return new WaitUntil(() =>
            {
                return Time.time - baseTime > 0.75f;
            });

            ImageCompnent.sprite = changeSprite;

            yield return new WaitUntil(() =>
            {
                return Time.time - baseTime > 1f;
            });

            ImageCompnent.sprite = NormalSprite;
        }

        // Normalに戻して終わり
        ImageCompnent.sprite = NormalSprite;
    }
}
