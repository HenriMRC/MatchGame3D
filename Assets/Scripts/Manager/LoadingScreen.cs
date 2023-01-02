using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ArkadiumTest.Manager
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TMP_Text _loadingText;
        [SerializeField]
        private TMP_Text _loadingPercent;

        [SerializeField]
        private RectTransform _loadingBar;

        internal void Show(Queue<ILoadProcess> processes, Action onEnded, float duration = 1f)
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowCoroutine(processes, onEnded, duration));
        }

        private IEnumerator ShowCoroutine(Queue<ILoadProcess> processes, Action onEnded, float duration)
        {
            float time = 0;
            if (duration == 0f)
            {
                _canvasGroup.alpha = 1;
                BaseManager.Instance?.SetMainAudioVolume(0);
            }
            else
                while (time < duration)
                {
                    yield return null;

                    time += Time.deltaTime;
                    time = Mathf.Min(time, duration);

                    _canvasGroup.alpha = time / duration;
                    BaseManager.Instance?.SetMainAudioVolume(1 - _canvasGroup.alpha);
                }

            StartProcess(processes, onEnded);
        }

        private void StartProcess(Queue<ILoadProcess> processes, Action onEnded)
        {
            _loadingText.gameObject.SetActive(true);
            _loadingPercent.gameObject.SetActive(true);

            StartCoroutine(ProcessCoroutine(processes, onEnded));
        }

        private IEnumerator ProcessCoroutine(Queue<ILoadProcess> processes, Action onEnded)
        {
            const int LOADING_MAX_LENGHT = 10;
            const float DURATION = 0.5f;
            float time = 0;
            float processesCount = processes.Count;
            float currentProcess = 0;

            while (processes.TryDequeue(out ILoadProcess process))
            {
                IEnumerator enumerator = process.StartProcess();

                do
                {
                    yield return null;

                    UpdateProgress((Mathf.Clamp01(process.Progress) + currentProcess) / processesCount);

                    time += Time.deltaTime;

                    if (time > DURATION)
                    {
                        time %= DURATION;

                        if (_loadingText.text.Length == LOADING_MAX_LENGHT)
                            _loadingText.text = "LOADING";
                        else
                            _loadingText.text += ".";
                    }
                } while (enumerator.MoveNext());

                currentProcess++;
            }

            UpdateProgress(1);

            Hide(onEnded);
        }

        private void UpdateProgress(float progress)
        {
            Vector2 anchor = _loadingBar.anchorMax;
            anchor.x = progress;
            _loadingBar.anchorMax = anchor;

            _loadingPercent.text = $"<mspace=0.8em>{(int)(progress * 100)}%</mspace>";
        }

        internal void Hide(Action onEnded)
        {
            StartCoroutine(HideCoroutine(onEnded, 0.5f));
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
                BaseManager.Instance?.SetMainAudioVolume(1 - _canvasGroup.alpha);
            }

            _loadingText.text = "LOADING";
            UpdateProgress(0);

            _canvasGroup.alpha = 0;
            BaseManager.Instance?.SetMainAudioVolume(1);

            gameObject.SetActive(false);

            onEnded();
        }
    }

    public interface ILoadProcess
    {
        public float Progress { get; }

        public IEnumerator StartProcess();
    }
}
