using System;
using System.Collections;
using System.Collections.Generic;
using Tetris.Model;
using UnityEngine;

namespace Tetris.Controller
{
    public class SlotsController : MonoBehaviour
    {
        [SerializeField] private GameSettingSO gameSetting;
        [SerializeField] private Transform slotsParent;
        private GameData gameData;
        private List<SlotPosition> currentItemSlots;
        private List<SlotPosition> targetFallDownSlots;
        private event Action onGenerateNextItem;
        [SerializeField] private Transform nextItemPanel;

        private void Awake()
        {
            gameData = new GameData();
            EventManager.Instance.onReceiveInput.Register(OnReceiveInput);
        }

        private void Start()
        {
            gameData.InitSlots(gameSetting, slotsParent, E_Panel_Type.Main);
            gameData.InitItemsQueue();
            gameData.InitSlots(gameSetting, nextItemPanel, E_Panel_Type.NextItem);
            onGenerateNextItem += OnGenerateNextItem;
            onGenerateNextItem?.Invoke();
            gameData.score = 0;
        }

        private void OnDestroy()
        {
            onGenerateNextItem -= OnGenerateNextItem;
            gameData.score = 0;
            EventManager.Instance.onReceiveInput.UnRegister(OnReceiveInput);
        }

        private void OnGenerateNextItem()
        {
            //Clear
            CancelInvoke("VerticalMoveItem");
            currentItemSlots = new List<SlotPosition>();
            targetFallDownSlots = new List<SlotPosition>();

            //Generate new item
            currentItemSlots = gameData.GenerateNextItem();
            targetFallDownSlots = gameData.CalculateItemVerticalTargetSlots(currentItemSlots);

            //Makes the generated item starts to fall down
            InvokeRepeating("VerticalMoveItem", gameSetting.fallInterval, gameSetting.fallInterval);
        }

        private void VerticalMoveItem()
        {
            if (!gameData.VerticalMoveItem(currentItemSlots, targetFallDownSlots, 1))
            {
                OnItemFallDown();
            }
        }

        private void OnItemFallDown()
        {
            if (gameData.CheckIfItemLinesBonus(targetFallDownSlots, out List<int> bonusLineIndices))
            {
                CancelInvoke("VerticalMoveItem");
                StartCoroutine(GenerateNextItemUntilBonusFinished(bonusLineIndices));
            }
            else
            {
                onGenerateNextItem?.Invoke();
            }
        }

        private IEnumerator GenerateNextItemUntilBonusFinished(List<int> bonusLineIndices)
        {
            //思路：
            //1.消除目标行
            //2.其余行下落
            //3.下落完成后，查看有无要继续消除的行
            //4.若有，则重复1&2&3
            //5.若没有，则继续生成下一个随机对象

            bool hasBonus = true;  //默认先处理一次
            while (hasBonus)
            {
                //计分
                gameData.score += gameSetting.singleLineScore * bonusLineIndices.Count;

                //颜色渐变消除目标行
                yield return StartCoroutine(FadeLines(bonusLineIndices, gameSetting.fadeDuration));

                //其余行下落
                yield return StartCoroutine(FallDownOtherLines(bonusLineIndices));

                //检查有无要继续消除的行
                bonusLineIndices.Clear();
                hasBonus = gameData.CheckIfHasBonus(out bonusLineIndices);

                yield return null;
            }
            //继续生成下一个随机对象
            onGenerateNextItem?.Invoke();
        }

        private IEnumerator FadeLines(List<int> bonusLineIndices, float duration)
        {
            float timer = 0f;
            float alpha = 1f;
            while (timer < 1)
            {

                timer += Time.deltaTime / duration;
                alpha -= (1 - 0.3f) / duration * Time.deltaTime;
                for (int i = 0; i < bonusLineIndices.Count; i++)
                {
                    gameData.FadeLineColor(bonusLineIndices[i], alpha);
                }
                yield return null;
            }
            for (int i = 0; i < bonusLineIndices.Count; i++)
            {
                gameData.ClearLine(bonusLineIndices[i]);
            }
        }

        private IEnumerator FallDownOtherLines(List<int> bonusLineIndices)
        {
            //找到有值的最大行
            int maxRowIndex = -1;
            for (int i = 0; i < gameSetting.row; i++)
            {
                if (bonusLineIndices.Contains(i)) continue;
                if (gameData.CheckIfLineEmpty(i))
                    break;
                else
                    maxRowIndex = i;
            }
            if (maxRowIndex == -1) yield return null;

            //下落
            for (int i = 0; i < bonusLineIndices.Count; i++)
            {
                for (int j = bonusLineIndices[i] + 1 - i; j <= maxRowIndex; j++)
                {
                    gameData.FallLine(j, 1);
                }

                maxRowIndex--;
                if (i == bonusLineIndices.Count - 1)
                    yield return null;
                else
                    yield return new WaitForSeconds(gameSetting.fallInterval);
            }
        }

        private void OnReceiveInput(E_Input_Type inputType)
        {
            switch (inputType)
            {
                case E_Input_Type.Left:
                    HorizontalMoveItem(-1);
                    break;
                case E_Input_Type.Right:
                    HorizontalMoveItem(1);
                    break;
                case E_Input_Type.Down:
                    VerticalMoveItem();
                    break;
                case E_Input_Type.StraightDown:
                    gameData.DirectlyMoveItemToTarget(currentItemSlots, targetFallDownSlots);
                    OnItemFallDown();
                    break;
                case E_Input_Type.Turn:
                    TurnItem();
                    break;
            }
        }

        private void HorizontalMoveItem(int moveStep)
        {
            if (gameData.CheckIfCanHorizontalMoveItem(currentItemSlots, moveStep))
            {
                gameData.ClearItemVerticalTargetSlots(targetFallDownSlots);
                gameData.HorizontalMoveItem(currentItemSlots, moveStep);
                targetFallDownSlots = gameData.CalculateItemVerticalTargetSlots(currentItemSlots);
            }
        }

        private void TurnItem()
        {
            if (gameData.TurnItem(currentItemSlots))
            {
                gameData.ClearItemVerticalTargetSlots(targetFallDownSlots);
                targetFallDownSlots = gameData.CalculateItemVerticalTargetSlots(currentItemSlots);
            }
        }
    }
}
