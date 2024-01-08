using UnityEngine;

namespace Tetris.Controller
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private float horizontalHoldTriggerInterval = 0.1f;
        [SerializeField] private float verticalHoldTriggerInterval = 0.1f;
        private bool isLeftButtonHold = false;
        private bool isRightButtonHold = false;
        private bool isDownButtonHold = false;
        private float horizontalTimer = 0f;
        private float verticalTimer = 0f;

        private void Update()
        {
            //左
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                isLeftButtonHold = true;
                isRightButtonHold = false;
                horizontalTimer = horizontalHoldTriggerInterval;
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                isLeftButtonHold = false;
                horizontalTimer = 0f;
            }

            //右
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                isRightButtonHold = true;
                isLeftButtonHold = false;
                horizontalTimer = horizontalHoldTriggerInterval;
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                isRightButtonHold = false;
                horizontalTimer = 0f;
            }

            //加速下
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                isDownButtonHold = true;
                verticalTimer = verticalHoldTriggerInterval;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                isDownButtonHold = false;
                verticalTimer = 0f;
            }

            //直接下
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventManager.Instance.onReceiveInput.Trigger(E_Input_Type.StraightDown);
            }

            //旋转
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                EventManager.Instance.onReceiveInput.Trigger(E_Input_Type.Turn);
            }

            //水平输入的计时逻辑
            if (isLeftButtonHold || isRightButtonHold)
            {
                horizontalTimer += Time.deltaTime;
                if (horizontalTimer >= horizontalHoldTriggerInterval)
                {
                    if (isLeftButtonHold)
                        EventManager.Instance.onReceiveInput.Trigger(E_Input_Type.Left);
                    if (isRightButtonHold)
                        EventManager.Instance.onReceiveInput.Trigger(E_Input_Type.Right);
                    horizontalTimer = 0f;
                }
            }

            //竖直输入的计时逻辑
            if (isDownButtonHold)
            {
                verticalTimer += Time.deltaTime;
                if (verticalTimer >= verticalHoldTriggerInterval)
                {
                    EventManager.Instance.onReceiveInput.Trigger(E_Input_Type.Down);
                    verticalTimer = 0f;
                }
            }
        }
    }
}
