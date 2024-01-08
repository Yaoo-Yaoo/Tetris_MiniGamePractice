using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Model
{
    [Serializable]
    public class ItemData
    {
        public E_Item_Type itemType;
        public Color itemColor = Color.white;
        public List<ItemValueMatrix> itemValues = new List<ItemValueMatrix>();
    }

    [Serializable]
    public class ItemValueMatrix
    {
        public E_Item_Value_Matrix_Type matrixType;
        [Tooltip("row for E_Item_Value_Matrix_Type.R2xC4; column for E_Item_Value_Matrix_Type.R4xC2")]
        public E_Slot_Value[] lineOne = new E_Slot_Value[4];
        public E_Slot_Value[] lineTwo = new E_Slot_Value[4];
    }
}
