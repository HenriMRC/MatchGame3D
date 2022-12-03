using Codice.Client.BaseCommands;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ArkadiumTest.Manager
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private Coroutine _coroutine;

        [SerializeField]
        private TMP_Text _loadingText;
        [SerializeField]
        private TMP_Text _loadingPercent;

        [SerializeField]
        private RectTransform _loadingBar;

        internal void Show(Action onEnded, float duration = 1f)
        {
            gameObject.SetActive(true);
            _coroutine = StartCoroutine(ShowCoroutine(onEnded, duration));
        }

        private IEnumerator ShowCoroutine(Action onEnded, float duration)
        {
            float time = 0;
            if (duration == 0f)
                _canvasGroup.alpha = 1;
            else
                while (time < duration)
                {
                    yield return null;

                    time += Time.deltaTime;
                    time = Mathf.Min(time, duration);

                    _canvasGroup.alpha = time / duration;
                }

            yield return null;

            _coroutine = null;
            onEnded?.Invoke();
        }

        internal void StartProcess(Func<float> onUpdate)
        {
            _loadingText.gameObject.SetActive(true);
            _loadingPercent.gameObject.SetActive(true);

            _coroutine = StartCoroutine(ProcessCoroutine(onUpdate));
        }

        private IEnumerator ProcessCoroutine(Func<float> onUpdate)
        {
            const int LOADING_MAX_LENGHT = 10;
            const float DURATION = 0.5f;
            float time = 0;

            while (true)
            {
                yield return null;

                UpdateProgress(Mathf.Clamp01(onUpdate()));

                time += Time.deltaTime;

                if (time > DURATION)
                {
                    time %= DURATION;

                    if (_loadingText.text.Length == LOADING_MAX_LENGHT)
                        _loadingText.text = "LOADING";
                    else
                        _loadingText.text += ".";
                }
            }
        }

        private void UpdateProgress(float progress)
        {
            Vector2 anchor = _loadingBar.anchorMax;
            anchor.x = progress;
            _loadingBar.anchorMax = anchor;

            _loadingPercent.text = $"<mspace=0.8em>{(int)(progress * 100)}%</mspace>";
        }

        internal void EndProcess()
        {
            if (_coroutine != null && !_coroutine.Equals(null))
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            UpdateProgress(1);
        }

        internal void Hide(Action onEnded, float duration = 1f)
        {
            _coroutine = StartCoroutine(HideCoroutine(onEnded, duration));
        }

        private IEnumerator HideCoroutine(Action onEnded, float duration)
        {
            float time = duration;
            while (time > 0)
            {
                yield return null;

                time -= Time.deltaTime;
                time = Mathf.Max(time, 0);

                _canvasGroup.alpha = time / duration;
            }

            _coroutine = null;

            _loadingText.text = "LOADING";
            UpdateProgress(0);

            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
            
            onEnded?.Invoke();
        }
    }
}
