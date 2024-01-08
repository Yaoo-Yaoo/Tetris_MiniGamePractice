using UnityEngine;

namespace Tetris.Model
{
    [CreateAssetMenu(fileName = "GameSetting", menuName = "Game Setting")]
    public class GameSettingSO : ScriptableObject
    {
        [Header("主面板")]
        public int column = 10;
        public int row = 20;
        public GameObject emptySlotPrefab;
        public Vector3 startPos = new Vector3(-405f, -855f, 0f);

        [Header("游戏数据相关")]
        public float fallInterval = 1f;
        public float fadeDuration = 0.5f;
        public int singleLineScore = 100;

        [Header("砖块生成相关")]
        public int itemMatrixStartIndex = 3;
        public ItemData[] items = new ItemData[7];
        public Color fallTargetColor;

        [Header("右上角小面板——下一个砖块")]
        public GameObject nextItemEmptySlotPrefab;
        public int nextItemPanelColumn = 4;
        public int nextItemPanelRow = 2;
        public Vector3 nextItemStartPos = new Vector3(0, 0, 0);
    }
}
