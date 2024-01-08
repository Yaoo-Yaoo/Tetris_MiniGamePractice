using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tetris.Controller
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Text scoreText;

        private void Awake()
        {
            EventManager.Instance.onGameScoreChanged.Register(OnGameScoreChanged);
        }

        private void OnDestroy()
        {
            EventManager.Instance.onGameScoreChanged.UnRegister(OnGameScoreChanged);
        }

        private void OnGameScoreChanged(int score)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
