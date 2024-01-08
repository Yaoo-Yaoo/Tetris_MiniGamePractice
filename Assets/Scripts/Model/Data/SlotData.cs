using UnityEngine;
using UnityEngine.UI;

namespace Tetris.Model
{
    public class SlotData
    {
        public GameObject slotObj;
        public int rowIndex;
        public int columnIndex;

        private E_Slot_Value slotValue;
        private Image slotImage;
        private Color defaultColor;

        public SlotData(GameObject slotObj, int rowIndex, int columnIndex, E_Slot_Value slotValue)
        {
            this.slotObj = slotObj;
            slotImage = slotObj.GetComponent<Image>();
            defaultColor = slotImage.color;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            SetValue(E_Slot_Value.Empty);
        }

        public void SetValue(E_Slot_Value slotValue, Color slotColor = default)
        {
            if (this.slotValue == slotValue) return;
            this.slotValue = slotValue;
            if (slotValue == E_Slot_Value.Occupied || slotValue == E_Slot_Value.Target)
                slotImage.color = slotColor;
            else
                slotImage.color = defaultColor;
        }

        public E_Slot_Value GetValue()
        {
            return slotValue;
        }

        public Color GetColor()
        {
            return slotImage.color;
        }

        public void SetColor(Color newColor)
        {
            slotImage.color = newColor;
        }
    }
}
