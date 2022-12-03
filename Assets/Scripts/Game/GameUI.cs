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

        public void UpdateUITimer(float time)
        {
            int timeInt = Mathf.RoundToInt(time);
            int minutes = timeInt / 60;
            int seconds = timeInt % 60;

            _tmp.text = $"<mspace=0.6em>{minutes}</mspace>:<mspace=0.6em>{seconds:00}</mspace>";
        }

        public void UpdateUIScore(float score)
        {
            _score.text = $"<mspace=0.6em>{score:0000}</mspace>";
        }
    }
}