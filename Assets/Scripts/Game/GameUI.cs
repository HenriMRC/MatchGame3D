using TMPro;
using UnityEngine;

namespace ArkadiumTest.Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _tmp;

        [SerializeField]
        private TMP_Text _score;

        [SerializeField]
        private TMP_Text _finalScore;

        [SerializeField]
        private Animator _animator;

        public void UpdateUITimer(float time)
        {
            int timeInt = Mathf.RoundToInt(time);
            int minutes = timeInt / 60;
            int seconds = timeInt % 60;

            _tmp.text = $"<mspace=0.6em>{minutes}</mspace>:<mspace=0.6em>{seconds:00}</mspace>";
        }

        public void UpdateUIScore(int score)
        {
            _score.text = $"<mspace=0.6em>{score:0000}</mspace>";
        }

        public void GameEnded(bool win, int score)
        {
            _animator.SetTrigger(win ? "Win" : "Lose");

            _finalScore.text = score.ToString();
        }
    }
}