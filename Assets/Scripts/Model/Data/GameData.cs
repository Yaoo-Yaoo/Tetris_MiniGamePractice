using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Model
{
    public class GameData
    {
        public GameSettingSO gameSetting;
        public Dictionary<int, SlotData[]> allSlots = new Dictionary<int, SlotData[]>();
        public Queue<int> itemQueue = new Queue<int>();
        public E_Item_Type currentItemType;
        public int currentValue = 0;
        private int m_score = -1;
        public int score
        {
            get => m_score;
            set
            {
                if (m_score != value)
                {
                    m_score = value;
                    EventManager.Instance.onGameScoreChanged.Trigger(value);
                }
            }
        }
        public Dictionary<int, SlotData[]> nextItemSlots = new Dictionary<int, SlotData[]>();
    }

    public struct SlotPosition
    {
        public int x;
        public int y;

        public SlotPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    //Slots related
    public static partial class GameDataExtension
    {
        public static void InitSlots(this GameData gameData, GameSettingSO gameSetting, Transform slotsParent, E_Panel_Type panelType)
        {
            gameData.gameSetting = gameSetting;
            float slotLength = gameSetting.emptySlotPrefab.GetComponent<RectTransform>().sizeDelta.x;
            int row;
            int column;
            Vector3 startPos;
            GameObject slotPrefab;
            if (panelType == E_Panel_Type.Main)
            {
                row = gameSetting.row;
                column = gameSetting.column;
                startPos = gameSetting.startPos;
                slotPrefab = gameSetting.emptySlotPrefab;
            }
            else
            {
                row = gameSetting.nextItemPanelRow;
                column = gameSetting.nextItemPanelColumn;
                startPos = gameSetting.nextItemStartPos;
                slotPrefab = gameSetting.nextItemEmptySlotPrefab;
            }
            for (int i = 0; i < row; i++)
            {
                SlotData[] currentRowSlots = new SlotData[column];
                for (int j = 0; j < currentRowSlots.Length; j++)
                {
                    GameObject currentSlotObj = GameObject.Instantiate(slotPrefab);
                    currentSlotObj.transform.SetParent(slotsParent);
                    currentSlotObj.transform.localPosition = startPos + Vector3.right * j * slotLength + Vector3.up * i * slotLength;
                    currentSlotObj.transform.localScale = Vector3.one;
                    currentSlotObj.name = $"{i},{j}";
                    currentRowSlots[j] = new SlotData(currentSlotObj, i, j, E_Slot_Value.Empty);
                }

                if (panelType == E_Panel_Type.Main)
                {
                    if (!gameData.allSlots.ContainsKey(i))
                        gameData.allSlots.Add(i, currentRowSlots);
                }
                else
                {
                    if (!gameData.nextItemSlots.ContainsKey(i))
                        gameData.nextItemSlots.Add(i, currentRowSlots);
                }
            }
        }

        private static bool TryGetLine(this GameData gameData, int rowIndex, out SlotData[] slotsData)
        {
            slotsData = gameData.GetLine(rowIndex);
            if (slotsData == null) return false;
            return true;
        }

        private static SlotData[] GetLine(this GameData gameData, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= gameData.gameSetting.row) return null;
            return gameData.allSlots[rowIndex];
        }

        public static bool CheckIfLineEmpty(this GameData gameData, int rowIndex)
        {
            if (gameData.TryGetLine(rowIndex, out SlotData[] slotsData))
            {
                for (int i = 0; i < slotsData.Length; i++)
                {
                    if (slotsData[i].GetValue() == E_Slot_Value.Occupied)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void FallLine(this GameData gameData, int fallingRow, int fallingStep)
        {
            for (int i = 0; i < fallingStep; i++)
            {
                gameData.SwapLine(fallingRow - i, fallingRow - i - 1);
            }
        }

        private static void SwapLine(this GameData gameData, int lineOne, int lineTwo)
        {
            for (int i = 0; i < gameData.gameSetting.column; i++)
            {
                gameData.SwapSlot(new SlotPosition(lineOne, i), new SlotPosition(lineTwo, i));
            }
        }

        public static void FadeLineColor(this GameData gameData, int rowIndex, float alpha)
        {
            if (gameData.TryGetLine(rowIndex, out SlotData[] slotsData))
            {
                for (int i = 0; i < slotsData.Length; i++)
                {
                    Color newColor = slotsData[i].GetColor();
                    newColor.a = alpha;
                    slotsData[i].SetColor(newColor);
                }
            }
        }

        public static bool ClearLine(this GameData gameData, int rowIndex)
        {
            if (gameData.TryGetLine(rowIndex, out SlotData[] originSlotsData))
            {
                for (int i = 0; i < originSlotsData.Length; i++)
                {
                    originSlotsData[i].SetValue(E_Slot_Value.Empty);
                }
                return true;
            }
            return true;
        }

        private static bool TryGetSlot(this GameData gameData, SlotPosition slotPosition, out SlotData slotData)
        {
            return gameData.TryGetSlot(slotPosition.x, slotPosition.y, out slotData);
        }

        private static bool TryGetSlot(this GameData gameData, int rowIndex, int columnIndex, out SlotData slotData)
        {
            slotData = gameData.GetSlot(rowIndex, columnIndex);
            if (slotData == null) return false;
            return true;
        }

        private static SlotData GetSlot(this GameData gameData, SlotPosition slotPosition)
        {
            return gameData.GetSlot(slotPosition.x, slotPosition.y);
        }

        private static SlotData GetSlot(this GameData gameData, int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= gameData.gameSetting.row || columnIndex < 0 || columnIndex >= gameData.gameSetting.column) return null;
            return gameData.allSlots[rowIndex][columnIndex];
        }

        private static bool SetSlot(this GameData gameData, SlotPosition slotPosition, E_Slot_Value slotValue, Color color = default)
        {
            return gameData.SetSlot(slotPosition.x, slotPosition.y, slotValue, color);
        }

        private static bool SetSlot(this GameData gameData, int rowIndex, int columnIndex, E_Slot_Value slotValue, Color color = default)
        {
            if (gameData.TryGetSlot(rowIndex, columnIndex, out SlotData originSlotData))
            {
                originSlotData.SetValue(slotValue, color);
                return true;
            }
            return false;
        }

        private static void SwapSlot(this GameData gameData, SlotPosition one, SlotPosition two)
        {
            SlotData slotOne = gameData.GetSlot(one);
            E_Slot_Value tempValue = slotOne.GetValue();
            Color tempColor = slotOne.GetColor();

            SlotData slotTwo = gameData.GetSlot(two);
            gameData.SetSlot(one, slotTwo.GetValue(), slotTwo.GetColor());
            gameData.SetSlot(two, tempValue, tempColor);
        }

        private static SlotPosition VerticalMoveSlot(this GameData gameData, SlotPosition slotPosition, int fallStep)
        {
            if (gameData.TryGetSlot(slotPosition, out SlotData slotData))
            {
                SlotPosition newSlotPosition = new SlotPosition(slotPosition.x - fallStep, slotPosition.y);
                gameData.SetSlot(newSlotPosition, slotData.GetValue(), slotData.GetColor());
                gameData.SetSlot(slotPosition, E_Slot_Value.Empty);
                return newSlotPosition;
            }
            return slotPosition;
        }

        private static SlotPosition HorizontalMoveSlot(this GameData gameData, SlotPosition slotPosition, int moveStep)
        {
            if (gameData.TryGetSlot(slotPosition, out SlotData lastSlot))
            {
                SlotPosition newSlotPosition = new SlotPosition(slotPosition.x, slotPosition.y + moveStep);
                gameData.SetSlot(newSlotPosition, E_Slot_Value.Occupied, lastSlot.GetColor());
                gameData.SetSlot(slotPosition, E_Slot_Value.Empty);
                return newSlotPosition;
            }
            return slotPosition;
        }

        public static bool CheckIfItemLinesBonus(this GameData gameData, List<SlotPosition> slotsPosition, out List<int> bonusLineIndices)  //检查有方块下落的行
        {
            bonusLineIndices = new List<int>();
            List<int> slotsRowIndices = GetSlotsRowIndices(slotsPosition);

            for (int i = 0; i < slotsRowIndices.Count; i++)
            {
                if (gameData.TryGetLine(slotsRowIndices[i], out SlotData[] currentRowSlotsData))
                {
                    bool isBonus = true;
                    for (int j = 0; j < currentRowSlotsData.Length; j++)
                    {
                        if (currentRowSlotsData[j].GetValue() != E_Slot_Value.Occupied)
                        {
                            isBonus = false;
                            break;
                        }
                    }
                    if (isBonus) bonusLineIndices.Add(slotsRowIndices[i]);
                }
            }

            if (bonusLineIndices != null && bonusLineIndices.Count > 0)
            {
                bonusLineIndices.SortList(true);
                return true;
            }
            return false;
        }

        private static void SortList(this List<int> list, bool isAcsending)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < list.Count - 1 - i; j++)
                {
                    if (isAcsending)  //升序
                    {
                        if (list[j] > list[j + 1]) list.Swap(j, j + 1);
                    }
                    else  //降序
                    {
                        if (list[j] < list[j + 1]) list.Swap(j, j + 1);
                    }
                }
            }
        }

        private static void Swap(this List<int> list, int i, int j)
        {
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static bool CheckIfHasBonus(this GameData gameData, out List<int> bonusLineIndices)  //检查全部
        {
            bonusLineIndices = new List<int>();

            for (int i = 0; i < gameData.gameSetting.row; i++)
            {
                bool isBonus = true;
                for (int j = 0; j < gameData.gameSetting.column; j++)
                {
                    if (gameData.GetSlot(i, j).GetValue() != E_Slot_Value.Occupied)
                    {
                        isBonus = false;
                        break;
                    }
                }
                if (isBonus) bonusLineIndices.Add(i);
            }

            if (bonusLineIndices != null && bonusLineIndices.Count > 0) bonusLineIndices.SortList(true);

            return false;
        }
    }

    //Items related
    public static partial class GameDataExtension
    {
        public static void InitItemsQueue(this GameData gameData)
        {
            for (int i = 0; i < 2; i++)
            {
                gameData.AddItem();
            }
        }

        private static void AddItem(this GameData gameData)
        {
            gameData.itemQueue.Enqueue(Random.Range(0, 7));
        }

        private static E_Item_Type GetNewItemType(this GameData gameData)
        {
            int newItemId = gameData.itemQueue.Dequeue();
            gameData.AddItem();
            return (E_Item_Type)newItemId;
        }

        private static E_Item_Type PeekNextItemType(this GameData gameData)
        {
            return (E_Item_Type)gameData.itemQueue.Peek();
        }

        public static List<SlotPosition> GenerateNextItem(this GameData gameData)
        {
            gameData.ClearNextItemSlots();

            List<SlotPosition> itemSlots = new List<SlotPosition>();

            int rowOneIndex = gameData.gameSetting.row - 1;
            int rowTwoIndex = gameData.gameSetting.row - 2;

            // for test
            // gameData.currentItemType = E_Item_Type.TShape;
            gameData.currentItemType = gameData.GetNewItemType();
            gameData.SetNextItemSlotsValue(gameData.PeekNextItemType());
            gameData.currentValue = 0;
            ItemData item = gameData.gameSetting.items[(int)gameData.currentItemType];
            for (int i = gameData.gameSetting.itemMatrixStartIndex; i < gameData.gameSetting.itemMatrixStartIndex + 4; i++)
            {
                E_Slot_Value rowOneValue = item.itemValues[gameData.currentValue].lineOne[i - gameData.gameSetting.itemMatrixStartIndex];
                gameData.SetSlot(rowOneIndex, i, rowOneValue, item.itemColor);
                if (rowOneValue == E_Slot_Value.Occupied) itemSlots.Add(new SlotPosition(rowOneIndex, i));

                E_Slot_Value rowTwoValue = item.itemValues[gameData.currentValue].lineTwo[i - gameData.gameSetting.itemMatrixStartIndex];
                gameData.SetSlot(rowTwoIndex, i, rowTwoValue, item.itemColor);
                if (rowTwoValue == E_Slot_Value.Occupied) itemSlots.Add(new SlotPosition(rowTwoIndex, i));
            }

            if (itemSlots == null || itemSlots.Count == 0) return null;

            //将列表按行数从小到大排序
            itemSlots.SortByColumnAcsending();
            return itemSlots;
        }

        private static void ClearNextItemSlots(this GameData gameData)
        {
            for (int i = 0; i < gameData.nextItemSlots.Count; i++)
            {
                for (int j = 0; j < gameData.nextItemSlots[i].Length; j++)
                {
                    gameData.nextItemSlots[i][j].SetValue(E_Slot_Value.Empty);
                }
            }
        }

        private static void SetNextItemSlotsValue(this GameData gameData, E_Item_Type itemType)
        {
            ItemData itemData = gameData.gameSetting.items[(int)itemType];
            ItemValueMatrix itemValueMatrix = itemData.itemValues[0];
            for (int i = 0; i < gameData.gameSetting.nextItemPanelColumn; i++)
            {
                if (itemValueMatrix.lineOne[i] == E_Slot_Value.Occupied)
                    gameData.nextItemSlots[1][i].SetValue(E_Slot_Value.Occupied, itemData.itemColor);

                if (itemValueMatrix.lineTwo[i] == E_Slot_Value.Occupied)
                    gameData.nextItemSlots[0][i].SetValue(E_Slot_Value.Occupied, itemData.itemColor);
            }
        }

        private static void SortByColumnAcsending(this List<SlotPosition> itemList)
        {
            for (int i = 0; i < itemList.Count - 1; i++)
            {
                for (int j = 0; j < itemList.Count - 1 - i; j++)
                {
                    if (itemList[j].y > itemList[j + 1].y)
                        itemList.Swap(j, j + 1);
                    else if (itemList[j].y == itemList[j + 1].y)
                        if (itemList[j].x > itemList[j + 1].x) itemList.Swap(j, j + 1);
                }
            }
        }

        private static void Swap(this List<SlotPosition> list, int i, int j)
        {
            SlotPosition temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static List<SlotPosition> CalculateItemVerticalTargetSlots(this GameData gameData, List<SlotPosition> itemSlots)
        {
            List<SlotPosition> targetFallDownSlots = new List<SlotPosition>();

            //找到每一列的边界
            Dictionary<int, int> columnTargetDistance = new Dictionary<int, int>();
            foreach (SlotPosition slot in itemSlots)
            {
                if (!columnTargetDistance.ContainsKey(slot.y))
                {
                    int boundaryRow = -1;
                    for (int i = slot.x - 1; i >= 0; i--)
                    {
                        if (gameData.TryGetSlot(i, slot.y, out SlotData slotData))
                        {
                            if (slotData.GetValue() == E_Slot_Value.Occupied)
                            {
                                boundaryRow = i + 1;
                                break;
                            }
                        }
                    }
                    if (boundaryRow == -1) boundaryRow = 0;
                    columnTargetDistance.Add(slot.y, slot.x - boundaryRow);
                }
            }

            //找到边界距离的最小值，即可计算下落的最近点
            int mincolumnTargetDistance = int.MaxValue;
            foreach (int column in columnTargetDistance.Keys)
            {
                if (columnTargetDistance[column] < mincolumnTargetDistance) mincolumnTargetDistance = columnTargetDistance[column];
            }

            //找到目标点并设定值
            foreach (SlotPosition slot in itemSlots)
            {
                targetFallDownSlots.Add(new SlotPosition(slot.x - mincolumnTargetDistance, slot.y));
                if (gameData.TryGetSlot(slot.x - mincolumnTargetDistance, slot.y, out SlotData slotData))
                {
                    if (slotData.GetValue() == E_Slot_Value.Empty)
                        gameData.SetSlot(slot.x - mincolumnTargetDistance, slot.y, E_Slot_Value.Target, gameData.gameSetting.fallTargetColor);
                }
            }

            return targetFallDownSlots;
        }

        public static void ClearItemVerticalTargetSlots(this GameData gameData, List<SlotPosition> targetSlots)
        {
            if (targetSlots == null || targetSlots.Count == 0) return;

            for (int i = 0; i < targetSlots.Count; i++)
            {
                if (gameData.TryGetSlot(targetSlots[i], out SlotData slotData))
                {
                    if (slotData.GetValue() == E_Slot_Value.Target)
                        gameData.SetSlot(targetSlots[i], E_Slot_Value.Empty);
                }
            }
            targetSlots.Clear();
        }

        private static bool CheckIfReachTarget(this GameData gameData, List<SlotPosition> itemSlots, List<SlotPosition> targetSlots)
        {
            //判断是否已经到达目标位置
            bool isSame = true;
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].x != targetSlots[i].x || itemSlots[i].y != targetSlots[i].y)
                {
                    isSame = false;
                    break;
                }
            }
            return isSame;
        }

        public static bool VerticalMoveItem(this GameData gameData, List<SlotPosition> itemSlots, List<SlotPosition> targetSlots, int fallStep)
        {
            if (gameData.CheckIfReachTarget(itemSlots, targetSlots)) return false;

            //下落
            for (int i = 0; i < itemSlots.Count; i++)
            {
                gameData.VerticalMoveSlot(itemSlots[i], fallStep);
                itemSlots[i] = new SlotPosition(itemSlots[i].x - 1, itemSlots[i].y);
            }
            return true;
        }

        public static void DirectlyMoveItemToTarget(this GameData gameData, List<SlotPosition> itemSlots, List<SlotPosition> targetSlots)
        {
            gameData.VerticalMoveItem(itemSlots, targetSlots, itemSlots[0].x - targetSlots[0].x);
        }

        public static bool CheckIfCanHorizontalMoveItem(this GameData gameData, List<SlotPosition> itemSlots, int moveStep)
        {
            if (moveStep > 0)
            {
                if (itemSlots[itemSlots.Count - 1].y + moveStep > gameData.gameSetting.column - 1) return false;

                //找到右侧边界
                Dictionary<int, int> rightBoundaries = new Dictionary<int, int>();
                for (int i = 0; i < itemSlots.Count; i++)
                {
                    if (rightBoundaries.ContainsKey(itemSlots[i].x))
                    {
                        if (itemSlots[i].y > rightBoundaries[itemSlots[i].x])
                        {
                            rightBoundaries[itemSlots[i].x] = itemSlots[i].y;
                        }
                    }
                    else
                    {
                        rightBoundaries.Add(itemSlots[i].x, itemSlots[i].y);
                    }
                }

                //检查右侧边界以右是否有方块
                for (int i = 1; i <= moveStep; i++)
                {
                    foreach (int row in rightBoundaries.Keys)
                    {
                        if (gameData.GetSlot(row, rightBoundaries[row] + i).GetValue() == E_Slot_Value.Occupied) return false;
                    }
                }
            }
            else if (moveStep < 0)
            {
                if (itemSlots[0].y + moveStep < 0) return false;

                //找到左侧边界
                Dictionary<int, int> leftBoundaries = new Dictionary<int, int>();
                for (int i = 0; i < itemSlots.Count; i++)
                {
                    if (leftBoundaries.ContainsKey(itemSlots[i].x))
                    {
                        if (itemSlots[i].y < leftBoundaries[itemSlots[i].x])
                        {
                            leftBoundaries[itemSlots[i].x] = itemSlots[i].y;
                        }
                    }
                    else
                    {
                        leftBoundaries.Add(itemSlots[i].x, itemSlots[i].y);
                    }
                }

                //检查左侧边界以左是否有方块
                for (int i = 1; i <= -moveStep; i++)
                {
                    foreach (int row in leftBoundaries.Keys)
                    {
                        if (gameData.GetSlot(row, leftBoundaries[row] - i).GetValue() == E_Slot_Value.Occupied) return false;
                    }
                }
            }
            return true;
        }

        public static void HorizontalMoveItem(this GameData gameData, List<SlotPosition> itemSlots, int moveStep)
        {
            if (moveStep > 0)
            {
                //先右后左移动
                for (int i = itemSlots.Count - 1; i >= 0; i--)
                {
                    itemSlots[i] = gameData.HorizontalMoveSlot(itemSlots[i], moveStep);
                }
            }
            else if (moveStep < 0)
            {
                //先左后右移动
                for (int i = 0; i < itemSlots.Count; i++)
                {
                    itemSlots[i] = gameData.HorizontalMoveSlot(itemSlots[i], moveStep);
                }
            }
        }

        public static bool TurnItem(this GameData gameData, List<SlotPosition> itemSlots)
        {
            //若为方块，则完全不改变
            if (gameData.currentItemType == E_Item_Type.Square) return false;

            //组合方块值+1
            ItemData itemData = gameData.gameSetting.items[(int)gameData.currentItemType];
            gameData.currentValue++;
            if (gameData.currentValue >= itemData.itemValues.Count)
                gameData.currentValue -= itemData.itemValues.Count;

            //找到基准点
            SlotPosition centerPos = itemSlots.GetReferenceCenter(itemData, gameData.currentValue);

            //计算新的位置
            List<SlotPosition> newItemSlots = new List<SlotPosition>();
            ItemValueMatrix itemValueMatrix = itemData.itemValues[gameData.currentValue];
            int[] itemSlotsBoundOffset = new int[2] { 0, 0 };
            bool isOutOfBound = false;
            for (int i = 0; i < itemValueMatrix.lineOne.Length; i++)
            {
                //第一排
                if (itemValueMatrix.lineOne[i] == E_Slot_Value.Occupied)
                {
                    SlotPosition position;
                    if (itemValueMatrix.matrixType == E_Item_Value_Matrix_Type.R2xC4)  //横
                        position = new SlotPosition(centerPos.x, centerPos.y + i - 1);
                    else  //纵
                        position = new SlotPosition(centerPos.x + 1 - i, centerPos.y);
                    //检查是否出边界
                    if (gameData.CheckIfOutOfBound(position, out int[] itemSlotsOffset))
                    {
                        isOutOfBound = true;
                        for (int j = 0; j < itemSlotsBoundOffset.Length; j++)
                        {
                            if (Mathf.Abs(itemSlotsOffset[j]) > Mathf.Abs(itemSlotsBoundOffset[j]))
                                itemSlotsBoundOffset[j] = itemSlotsOffset[j];
                        }
                    }
                    newItemSlots.Add(position);
                }
                //第二排
                if (itemValueMatrix.lineTwo[i] == E_Slot_Value.Occupied)
                {
                    SlotPosition position;
                    if (itemValueMatrix.matrixType == E_Item_Value_Matrix_Type.R2xC4)  //横
                        position = new SlotPosition(centerPos.x - 1, centerPos.y + i - 1);
                    else  //纵
                        position = new SlotPosition(centerPos.x + 1 - i, centerPos.y + 1);
                    //检查是否出边界
                    if (gameData.CheckIfOutOfBound(position, out int[] itemSlotsOffset))
                    {
                        isOutOfBound = true;
                        for (int j = 0; j < itemSlotsBoundOffset.Length; j++)
                        {
                            if (Mathf.Abs(itemSlotsOffset[j]) > Mathf.Abs(itemSlotsBoundOffset[j]))
                                itemSlotsBoundOffset[j] = itemSlotsOffset[j];
                        }
                    }
                    newItemSlots.Add(position);
                }
            }

            bool canTurn = true;
            for (int i = 0; i < newItemSlots.Count; i++)
            {
                if (isOutOfBound)
                    newItemSlots[i] = new SlotPosition(newItemSlots[i].x - itemSlotsBoundOffset[0], newItemSlots[i].y - itemSlotsBoundOffset[1]);
                if (!itemSlots.Contains(newItemSlots[i]) && gameData.GetSlot(newItemSlots[i]).GetValue() == E_Slot_Value.Occupied)
                {
                    canTurn = false;
                    break;
                }
            }
            if (!canTurn) return false;

            newItemSlots.SortByColumnAcsending();
            gameData.ClearItemSlots(itemSlots);
            itemSlots.Clear();
            for (int i = 0; i < newItemSlots.Count; i++)
            {
                itemSlots.Add(newItemSlots[i]);
                gameData.SetSlot(newItemSlots[i], E_Slot_Value.Occupied, itemData.itemColor);
            }
            return true;
        }

        private static bool CheckIfOutOfBound(this GameData gameData, SlotPosition position, out int[] itemSlotsOffset)
        {
            itemSlotsOffset = new int[2] { 0, 0 };  //x,y方向上的偏移量
            bool isOutOfBound = false;

            //x方向
            int xOffset = 0;
            if (position.x < 0)
                xOffset = position.x;
            else if (position.x > gameData.gameSetting.row - 1)
                xOffset = position.x - gameData.gameSetting.row + 1;
            if (xOffset != 0 && Mathf.Abs(xOffset) > Mathf.Abs(itemSlotsOffset[0]))
            {
                itemSlotsOffset[0] = xOffset;
                isOutOfBound = true;
            }

            //y方向
            int yOffset = 0;
            if (position.y < 0)
                yOffset = position.y;
            else if (position.y > gameData.gameSetting.column - 1)
                yOffset = position.y - gameData.gameSetting.column + 1;
            if (yOffset != 0 && Mathf.Abs(yOffset) > Mathf.Abs(itemSlotsOffset[1]))
            {
                itemSlotsOffset[1] = yOffset;
                isOutOfBound = true;
            }

            return isOutOfBound;
        }

        private static void ClearItemSlots(this GameData gameData, List<SlotPosition> itemSlots)
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                gameData.SetSlot(itemSlots[i], E_Slot_Value.Empty);
            }
        }

        private static SlotPosition GetReferenceCenter(this List<SlotPosition> itemSlots, ItemData itemData, int currentValue)
        {
            //R2xC4->R1xC2;R4xC2->R2xC1
            switch (itemData.itemType)
            {
                case E_Item_Type.Line:
                    return itemSlots[1];
                case E_Item_Type.ZShape:
                    return itemSlots[0];
                case E_Item_Type.ReverseZShape:
                    return itemSlots[1];
                case E_Item_Type.LShape:
                    if (currentValue == 0)
                        return itemSlots[0];
                    else if (currentValue == 1 || currentValue == 2)
                        return itemSlots[1];
                    else
                        return new SlotPosition(itemSlots[0].x + 1, itemSlots[0].y);
                case E_Item_Type.ReverseLShape:
                    if (currentValue == 0)
                        return new SlotPosition(itemSlots[1].x + 1, itemSlots[1].y);
                    else if (currentValue == 1)
                        return itemSlots[1];
                    else if (currentValue == 2)
                        return itemSlots[2];
                    else
                        return new SlotPosition(itemSlots[0].x - 1, itemSlots[0].y);
                case E_Item_Type.TShape:
                    if (currentValue == 0 || currentValue == 1 || currentValue == 2)
                        return itemSlots[1];
                    else
                        return itemSlots[0];
                default:
                    return default;
            }
        }

        private static List<int> GetSlotsRowIndices(List<SlotPosition> slotsPosition)
        {
            List<int> slotsRowIndices = new List<int>();
            foreach (SlotPosition slot in slotsPosition)
            {
                if (!slotsRowIndices.Contains(slot.x))
                    slotsRowIndices.Add(slot.x);
            }
            return slotsRowIndices;
        }
    }
}
