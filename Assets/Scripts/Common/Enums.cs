namespace Tetris
{
    public enum E_Panel_Type
    {
        Main,  //用于处理逻辑的主面板
        NextItem  //用于显示下一个方块的面板
    }

    public enum E_Slot_Value
    {
        Target = -1,
        Empty = 0,
        Occupied = 1
    }

    public enum E_Item_Type
    {
        Line = 0,
        ZShape,
        ReverseZShape,
        Square,
        LShape,
        ReverseLShape,
        TShape

        //0000000000

        //            10
        //    1111    10
        //    0000    10
        //            10

        //            01
        //    0110    11
        //    0011    10
        //            00

        //            10
        //    0110    11
        //    1100    01
        //            00

        //    0110
        //    0110

        //            11            01
        //    0100    10    1110    01
        //    0111    10    0010    11
        //            00            00

        //            10            11
        //    0010    10    1110    01
        //    1110    11    1000    01
        //            00            00

        //            10            01
        //    0100    11    1110    11
        //    1110    10    0100    01
        //            00            00
    }

    public enum E_Item_Value_Matrix_Type
    {
        R2xC4,   //0000
                 //0000

        R4xC2    //00
                 //00
                 //00
                 //00
    }

    public enum E_Input_Type
    {
        Left,
        Right,
        Down,
        StraightDown,
        Turn
    }
}
