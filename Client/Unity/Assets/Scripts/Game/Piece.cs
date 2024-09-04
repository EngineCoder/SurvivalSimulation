using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UFO.STICK
{

    public class Piece : MonoBehaviour
    {
        public Button btnSelf;
        public State pieceState = State.isNull;
        public Vector2 pieceIndex = Vector2.zero;

        private void Awake()
        {
            btnSelf = GetComponent<Button>();
            btnSelf.onClick.AddListener(UpdateColor);
        }

        /// <summary>
        /// 更新棋子颜色
        /// </summary>
        public void UpdateColor()
        {
            if (pieceState == State.isNull)
            {
                if (Controller.Instance.isRed)
                {
                    UpdateState(State.isRed, Color.red);
                }
                else
                {
                    UpdateState(State.isBlack, Color.black);
                }
            }
            else
            {
                //操作无效
                Debug.Log("操作无效");
            }
        }

        /// <summary>
        /// 更新棋子的状态
        /// </summary>
        /// <param name="state"></param>
        /// <param name="color"></param>
        public void UpdateState(State state, Color color)
        {
            pieceState = state;
            btnSelf.image.color = color;

            //更新棋子的状态
            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
            {
                if (this == Controller.Instance.piecesStructs[i].piece)
                {
                    Controller.Instance.piecesStructs[i].state = state;
                    pieceIndex = Controller.Instance.piecesStructs[i].index;
                    break;
                }
            }

            #region 通天
            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
            {
                #region 棍，通天
                //横棍
                if ((Controller.Instance.piecesStructs[i].index.x == pieceIndex.x) && (Controller.Instance.piecesStructs[i].state == pieceState))
                {
                    if (pieceState == State.isRed)
                    {
                        Controller.Instance.Red_xTx++;
                    }
                    else
                    {
                        Controller.Instance.Block_xTx++;
                    }

                }
                //纵棍
                else if ((Controller.Instance.piecesStructs[i].index.y == pieceIndex.y) && (Controller.Instance.piecesStructs[i].state == pieceState))
                {
                    if (pieceState == State.isRed)
                    {
                        Controller.Instance.Red_yTy++;
                    }
                    else
                    {
                        Controller.Instance.Block_yTy++;
                    }
                }
                //通天x=y
                if ((pieceIndex.x == pieceIndex.y) && Controller.Instance.piecesStructs[i].index.x == Controller.Instance.piecesStructs[i].index.y && Controller.Instance.piecesStructs[i].state == pieceState)
                {

                    if (pieceState == State.isRed)
                    {
                        Controller.Instance.Red_xyTxy++;
                    }
                    else
                    {
                        Controller.Instance.Block_xyTxy++;
                    }
                }
                //通天x+y=6
                else if (((pieceIndex.x + pieceIndex.y) == 6) && ((Controller.Instance.piecesStructs[i].index.x + Controller.Instance.piecesStructs[i].index.y) == 6) && Controller.Instance.piecesStructs[i].state == pieceState)
                {

                    if (pieceState == State.isRed)
                    {
                        Controller.Instance.Red_xy6Txy6++;
                    }
                    else
                    {
                        Controller.Instance.Block_xy6Txy6++;
                    }

                }
                #endregion
            }
            #endregion

            #region 三斜
            //LT & LD 三斜
            if (pieceIndex.x == 3 && pieceIndex.y == 1)
            {
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LTThree++;
                    Controller.Instance.Red_LDThree++;
                }
                else
                {
                    Controller.Instance.Block_LTThree++;
                    Controller.Instance.Block_LDThree++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //条件：+=4 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTThree++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTThree++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                }
            }
            //LT 三斜
            else if (pieceIndex.x == 2 && pieceIndex.y == 2)
            {
                //Left Top Three
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LTThree++;
                }
                else
                {
                    Controller.Instance.Block_LTThree++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //Left Top Three & Left Down Three
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //条件：+=4 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTThree++;
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTThree++;
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                    //Left Top Three & Right Top Three
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTThree++;
                            Controller.Instance.Red_RTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTThree++;
                            Controller.Instance.Block_RTThree++;
                        }
                    }
                }
            }
            //LT & RT 三斜
            else if (pieceIndex.x == 1 && pieceIndex.y == 3)
            {
                //Left Top Three & Right Top Three
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LTThree++;
                    Controller.Instance.Red_RTThree++;
                }
                //Left Top Three & Right Top Three
                else
                {
                    Controller.Instance.Block_LTThree++;
                    Controller.Instance.Block_RTThree++;
                }

                //更新棋子的状态
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //Left Top Three
                    if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //条件：+=4 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTThree++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTThree++;
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTThree++;
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                    //Right Top Three
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTThree++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTThree++;
                        }
                    }
                }
            }
            //RT 三斜
            else if (pieceIndex.x == 2 && pieceIndex.y == 4)
            {
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RTThree++;
                }
                else
                {
                    Controller.Instance.Block_RTThree++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //Left Top Three & Left Down Three
                    if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //Left Top Three & Left Top Three
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTThree++;
                            Controller.Instance.Red_RTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTThree++;
                            Controller.Instance.Block_LTThree++;
                        }
                    }
                    //Left Top Three & Right Top Three
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTThree++;
                        }
                    }
                }
            }
            //RT & RD 三斜
            else if (pieceIndex.x == 3 && pieceIndex.y == 5)
            {
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RTThree++;
                    Controller.Instance.Red_RDThree++;
                }
                else
                {
                    Controller.Instance.Block_RTThree++;
                    Controller.Instance.Block_RDThree++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //Right Top Three
                    if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //Left Top Three & Left Top Three
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTThree++;
                        }
                    }
                    //Left Top Three & Right Top Three
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTThree++;
                        }
                    }
                }
            }
            //RD 三斜
            else if (pieceIndex.x == 4 && pieceIndex.y == 4)
            {
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RDThree++;
                }
                else
                {
                    Controller.Instance.Block_RDThree++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //Right Top Three
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //Left Top Three & Left Top Three
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDThree++;
                        }
                    }
                    //Left Top Three & Right Top Three
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDThree++;
                        }
                    }
                }
            }
            //LD & RD 三斜
            else if (pieceIndex.x == 5 && pieceIndex.y == 3)
            {
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RDThree++;
                    Controller.Instance.Red_LDThree++;
                }
                else
                {
                    Controller.Instance.Block_LDThree++;
                    Controller.Instance.Block_RDThree++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LD
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //Left Top Three & Left Top Three
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                    //LD
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDThree++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDThree++;
                        }
                    }
                }
            }
            //LD 三斜
            else if (pieceIndex.x == 4 && pieceIndex.y == 2)
            {
                //LD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LDThree++;
                }
                else
                {
                    Controller.Instance.Block_LDThree++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LD 
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //Left Top Three & Left Top Three
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                    //LD
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDThree++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDThree++;
                        }
                    }
                }
            }
            #endregion

            #region 四斜
            //LT 四斜
            if (pieceIndex.x == 4 && pieceIndex.y == 1)
            {
                //LT
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LTFour++;
                }
                else
                {
                    Controller.Instance.Block_LTFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                }
            }
            //LT 四斜
            else if (pieceIndex.x == 1 && pieceIndex.y == 4)
            {
                //LT
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LTFour++;
                }
                else
                {
                    Controller.Instance.Block_LTFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                }
            }
            //LT LD 四斜
            else if (pieceIndex.x == 3 && pieceIndex.y == 2)
            {
                //LT LD 
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LTFour++;
                    Controller.Instance.Red_LDFour++;
                }
                else
                {
                    Controller.Instance.Block_LTFour++;
                    Controller.Instance.Block_LDFour++;
                }

                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //LD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //LD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                }
            }
            //LT RD 四斜
            else if (pieceIndex.x == 2 && pieceIndex.y == 3)
            {
                //LT RD 
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LTFour++;
                    Controller.Instance.Red_RDFour++;
                }
                else
                {
                    Controller.Instance.Block_LTFour++;
                    Controller.Instance.Block_RDFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //LT 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LTFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //LT 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD 四斜 
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                }
            }
            //RD 四斜
            else if (pieceIndex.x == 1 && pieceIndex.y == 2)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RDFour++;
                }
                else
                {
                    Controller.Instance.Block_RDFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                }
            }
            //RD RT 四斜
            else if (pieceIndex.x == 3 && pieceIndex.y == 4)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RDFour++;
                    Controller.Instance.Red_RTFour++;
                }
                else
                {
                    Controller.Instance.Block_RDFour++;
                    Controller.Instance.Block_RTFour++;
                }

                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //RD 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                }
            }
            //RD 四斜
            else if (pieceIndex.x == 4 && pieceIndex.y == 5)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RDFour++;
                }
                else
                {
                    Controller.Instance.Block_RDFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 1 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RDFour++;
                        }
                    }
                }
            }
            //LD 四斜
            else if (pieceIndex.x == 2 && pieceIndex.y == 1)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LDFour++;
                }
                else
                {
                    Controller.Instance.Block_LDFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                }
            }
            //LD 四斜
            else if (pieceIndex.x == 4 && pieceIndex.y == 3)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LDFour++;
                    Controller.Instance.Red_RTFour++;
                }
                else
                {
                    Controller.Instance.Block_LDFour++;
                    Controller.Instance.Block_RTFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                }
            }
            //LD 四斜
            else if (pieceIndex.x == 5 && pieceIndex.y == 4)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_LDFour++;
                }
                else
                {
                    Controller.Instance.Block_LDFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 1 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_LDFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_LDFour++;
                        }
                    }
                }
            }
            //RT 四斜
            else if (pieceIndex.x == 5 && pieceIndex.y == 2)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RTFour++;
                }
                else
                {
                    Controller.Instance.Block_RTFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 2 && Controller.Instance.piecesStructs[i].index.y == 5 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                }
            }
            //RT 四斜
            else if (pieceIndex.x == 2 && pieceIndex.y == 5)
            {
                //RD
                if (pieceState == State.isRed)
                {
                    Controller.Instance.Red_RTFour++;
                }
                else
                {
                    Controller.Instance.Block_RTFour++;
                }
                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                {
                    //LT 四斜
                    if (Controller.Instance.piecesStructs[i].index.x == 4 && Controller.Instance.piecesStructs[i].index.y == 3 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 3 && Controller.Instance.piecesStructs[i].index.y == 4 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                    //RD 四斜
                    else if (Controller.Instance.piecesStructs[i].index.x == 5 && Controller.Instance.piecesStructs[i].index.y == 2 && Controller.Instance.piecesStructs[i].state == pieceState)
                    {
                        //RD
                        if (pieceState == State.isRed)
                        {
                            Controller.Instance.Red_RTFour++;
                        }
                        else
                        {
                            Controller.Instance.Block_RTFour++;
                        }
                    }
                }
            }
            #endregion



            #region 四方
            if (pieceIndex.x == 1)
            {
                if (pieceIndex.y == 1)
                {
                    for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1~2
                    {
                        for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//1~2
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                    }
                }
                else if (pieceIndex.y == 5)
                {
                    for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1-2
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//5-4
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                    }
                }
                else//y=2,3,4
                {
                    //先算顺时针的正方
                    for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1-2
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//2-1
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1-2
                        {
                            for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        //如果有四方
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                        }
                    }
                }
            }
            else if (pieceIndex.x == 2)
            {
                if (pieceIndex.y == 1)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//2-1
                    {
                        for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//1-2
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;

                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//2-3
                        {
                            for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//1-2
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x
                                        && Controller.Instance.piecesStructs[i].index.y == y
                                        && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                        }
                    }
                }
                else if (pieceIndex.y == 5)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//1-2
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//5-4
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1-2
                        {
                            for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//5-4
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x
                                        && Controller.Instance.piecesStructs[i].index.y == y
                                        && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                        }
                    }
                }
                else //y=2,3,4
                {
                    //先算顺时针的正方
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//2-1
                    {
                        for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//2-1
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//2-1
                        {
                            for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        //如果有四方
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                            for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//2-3
                            {
                                for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//2-1
                                {
                                    for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                    {
                                        if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                        {
                                            if (pieceState == State.isRed)
                                            {
                                                Controller.Instance.Red_rectangle++;
                                            }
                                            else
                                            {
                                                Controller.Instance.Block_rectangle++;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            //如果有四方
                            if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                            {

                            }
                            else
                            {
                                Controller.Instance.Red_rectangle = 0;
                                Controller.Instance.Block_rectangle = 0;
                                for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//2-3
                                {
                                    for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//2-3
                                    {
                                        for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                        {
                                            if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                            {
                                                if (pieceState == State.isRed)
                                                {
                                                    Controller.Instance.Red_rectangle++;
                                                }
                                                else
                                                {
                                                    Controller.Instance.Block_rectangle++;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                //如果有四方
                                if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                                {

                                }
                                else
                                {
                                    Controller.Instance.Red_rectangle = 0;
                                    Controller.Instance.Block_rectangle = 0;
                                }
                            }

                        }
                    }
                }
            }
            else if (pieceIndex.x == 3)
            {
                if (pieceIndex.y == 1)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//2-1
                    {
                        for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//1-2
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//2-3
                        {
                            for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//1-2
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x
                                        && Controller.Instance.piecesStructs[i].index.y == y
                                        && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                        }
                    }
                }
                else if (pieceIndex.y == 5)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//1-2
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//5-4
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Red_rectangle = 0;

                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1-2
                        {
                            for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//5-4
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x
                                        && Controller.Instance.piecesStructs[i].index.y == y
                                        && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                        }
                    }
                }
                else //y=2,3,4
                {
                    //先算顺时针的正方
                    for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1-2
                    {
                        for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//2-1
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {
                       
                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//2-1
                        {
                            for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        //如果有四方
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {
                    
                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                            for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//2-3
                            {
                                for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//2-1
                                {
                                    for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                    {
                                        if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                        {
                                            if (pieceState == State.isRed)
                                            {
                                                Controller.Instance.Red_rectangle++;
                                            }
                                            else
                                            {
                                                Controller.Instance.Block_rectangle++;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            //如果有四方
                            if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                            {
                      
                            }
                            else
                            {
                                Controller.Instance.Red_rectangle = 0;
                                Controller.Instance.Block_rectangle = 0;
                                for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//2-3
                                {
                                    for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//2-3
                                    {
                                        for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                        {
                                            if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                            {
                                                if (pieceState == State.isRed)
                                                {
                                                    Controller.Instance.Red_rectangle++;
                                                }
                                                else
                                                {
                                                    Controller.Instance.Block_rectangle++;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                //如果有四方
                                if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                                {
                      
                                }
                                else
                                {
                                    Controller.Instance.Red_rectangle = 0;
                                    Controller.Instance.Block_rectangle = 0;
                                }
                            }
                        }
                    }
                }
            }
            else if (pieceIndex.x == 4)
            {
                if (pieceIndex.y == 1)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//2-1
                    {
                        for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//1-2
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//2-3
                        {
                            for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//1-2
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x
                                        && Controller.Instance.piecesStructs[i].index.y == y
                                        && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Red_rectangle = 0;
                        }

                        }
                }
                else if (pieceIndex.y == 5)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//1-2
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//5-4
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Red_rectangle = 0;

                        for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//1-2
                        {
                            for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//5-4
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x
                                        && Controller.Instance.piecesStructs[i].index.y == y
                                        && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Red_rectangle = 0;
                        }
                        }
                }
                else //y=2,3,4
                {
                    //先算顺时针的正方
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//
                        {
                            for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        //如果有四方
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {
                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;

                            for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)//2-3
                            {
                                for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//2-1
                                {
                                    for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                    {
                                        if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                        {
                                            if (pieceState == State.isRed)
                                            {
                                                Controller.Instance.Red_rectangle++;
                                            }
                                            else
                                            {
                                                Controller.Instance.Block_rectangle++;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            //如果有四方
                            if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                            {

                            }
                            else
                            {
                                Controller.Instance.Red_rectangle = 0;
                                Controller.Instance.Block_rectangle = 0;
                                for (int x = (int)pieceIndex.x; x <= (int)pieceIndex.x + 1; x++)
                                {
                                    for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)
                                    {
                                        for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                        {
                                            if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                            {
                                                if (pieceState == State.isRed)
                                                {
                                                    Controller.Instance.Red_rectangle++;
                                                }
                                                else
                                                {
                                                    Controller.Instance.Block_rectangle++;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }

                                //如果有四方
                                if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                                {
                                }
                                else
                                {
                                    Controller.Instance.Red_rectangle = 0;
                                    Controller.Instance.Block_rectangle = 0;
                                }
                            }
                        }
                    }
                }
            }
            else if (pieceIndex.x == 5)
            {
                if (pieceIndex.y == 1)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)
                    {
                        for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                    }
                }
                else if (pieceIndex.y == 5)
                {
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x && Controller.Instance.piecesStructs[i].index.y == y && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                    }
                }
                else//2,3,4
                {
                    //先算顺时针的正方
                    for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//5-4
                    {
                        for (int y = (int)pieceIndex.y; y >= (int)pieceIndex.y - 1; y--)//2-1
                        {
                            for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                            {
                                if (Controller.Instance.piecesStructs[i].index.x == x
                                    && Controller.Instance.piecesStructs[i].index.y == y
                                    && Controller.Instance.piecesStructs[i].state == pieceState)
                                {
                                    if (pieceState == State.isRed)
                                    {
                                        Controller.Instance.Red_rectangle++;
                                    }
                                    else
                                    {
                                        Controller.Instance.Block_rectangle++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //如果有四方
                    if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                    {

                    }
                    else
                    {
                        Controller.Instance.Red_rectangle = 0;
                        Controller.Instance.Block_rectangle = 0;
                        //先算顺时针的正方
                        for (int x = (int)pieceIndex.x; x >= (int)pieceIndex.x - 1; x--)//5-4
                        {
                            for (int y = (int)pieceIndex.y; y <= (int)pieceIndex.y + 1; y++)//
                            {
                                for (int i = 0; i < Controller.Instance.piecesStructs.Length; i++)
                                {
                                    if (Controller.Instance.piecesStructs[i].index.x == x
                                        && Controller.Instance.piecesStructs[i].index.y == y
                                        && Controller.Instance.piecesStructs[i].state == pieceState)
                                    {
                                        if (pieceState == State.isRed)
                                        {
                                            Controller.Instance.Red_rectangle++;
                                        }
                                        else
                                        {
                                            Controller.Instance.Block_rectangle++;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        //如果有四方
                        if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
                        {

                        }
                        else
                        {
                            Controller.Instance.Red_rectangle = 0;
                            Controller.Instance.Block_rectangle = 0;
                        }
                    }
                }
            }
            #endregion

            #region 技能
            if (Controller.Instance.Red_xTx == 5 || Controller.Instance.Block_xTx == 5 || Controller.Instance.Red_yTy == 5 || Controller.Instance.Block_yTy == 5)
            {
                Debug.Log(pieceState.ToString() + ":棍");
            }
            else if (Controller.Instance.Red_xyTxy == 5 || Controller.Instance.Block_xyTxy == 5 || Controller.Instance.Red_xy6Txy6 == 5 || Controller.Instance.Block_xy6Txy6 == 5)
            {
                Debug.Log(pieceState.ToString() + ":通天");
            }
            else if (Controller.Instance.Red_LTThree == 3 || Controller.Instance.Block_LTThree == 3 ||
                     Controller.Instance.Red_LDThree == 3 || Controller.Instance.Block_LDThree == 3 ||
                     Controller.Instance.Red_RTThree == 3 || Controller.Instance.Block_RTThree == 3 ||
                     Controller.Instance.Red_RDThree == 3 || Controller.Instance.Block_RDThree == 3)//三斜
            {
                Debug.Log(pieceState.ToString() + ":三斜");
            }
            else if (Controller.Instance.Red_LTFour == 4 || Controller.Instance.Block_LTFour == 4 ||
                     Controller.Instance.Red_LDFour == 4 || Controller.Instance.Block_LDFour == 4 ||
                     Controller.Instance.Red_RDFour == 4 || Controller.Instance.Block_RDFour == 4 ||
                     Controller.Instance.Red_RTFour == 4 || Controller.Instance.Block_RTFour == 4)//四斜
            {
                Debug.Log(pieceState.ToString() + ":四斜");
            }
            else if (Controller.Instance.Red_rectangle == 4 || Controller.Instance.Block_rectangle == 4)
            {
                Debug.Log(pieceState.ToString() + ":四方");
            }
            #endregion

            Controller.Instance.SetBlockAndRedToZero();
        }
    }

    public enum State
    {
        isNull = 0,
        isRed,
        isBlack,
        isFalse,
        isGod//三斜、、、
    }
}
