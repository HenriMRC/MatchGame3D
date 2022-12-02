using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ArkadiumTest.Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _tmp;

        internal void UpdateUITimer(float time)
        {
            int timeInt = Mathf.RoundToInt(time);
            int minutes = timeInt / 60;
            int seconds = timeInt % 60;

            _tmp.text = $"<mspace=0.6em>{minutes}</mspace>:<mspace=0.6em>{seconds:00}</mspace>";
        }
    }
}