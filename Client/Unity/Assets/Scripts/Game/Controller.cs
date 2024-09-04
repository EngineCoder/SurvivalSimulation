using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFO.STICK
{
    [System.Serializable]
    public struct PiecesStruct
    {
        //public Vector2 position;
        public Piece piece;
        public State state;
        public Vector2 index;
    }

    public class Controller : SingleMode<Controller>
    {
        public KeyCode changeTeam;
        public bool isRed = false;
        public PiecesStruct[] piecesStructs;
        public List<Piece> pieces = new List<Piece>();

        public int Red_xTx = 0;//横棍
        public int Red_yTy = 0;//纵棍
        public int Red_xyTxy = 0;//通天x=y
        public int Red_xy6Txy6 = 0;//通天x+y=6

        public int Red_LTThree = 0;//三斜
        public int Red_LDThree = 0;//三斜
        public int Red_RTThree = 0;//三斜
        public int Red_RDThree = 0;//三斜

        public int Red_LTFour = 0;//四斜
        public int Red_LDFour = 0;//四斜
        public int Red_RTFour = 0;//四斜
        public int Red_RDFour = 0;//四斜


        public int Red_rectangle = 0;//方


        public int Block_xTx = 0;//横棍
        public int Block_yTy = 0;//纵棍
        public int Block_xyTxy = 0;//通天x=y
        public int Block_xy6Txy6 = 0;//通天x+y=6


        public int Block_LTThree = 0;//三斜
        public int Block_LDThree = 0;//三斜
        public int Block_RTThree = 0;//三斜
        public int Block_RDThree = 0;//三斜

        public int Block_LTFour = 0;//四斜
        public int Block_LDFour = 0;//四斜
        public int Block_RTFour = 0;//四斜
        public int Block_RDFour = 0;//四斜

        public int Block_rectangle = 0;//方

        public GameObject checkerBoard;

        private void Awake()
        {
            //foreach (Transform item in checkerBoard.transform)
            //{
            //    pieces.Add(item.GetComponent<Piece>());
            //}
            //piecesStructs = new PiecesStruct[pieces.Count];
            //for (int i = 0; i < pieces.Count; i++)
            //{
            //    if ((i + 1) % 5 == 0)
            //    {
            //        piecesStructs[i].index = new Vector2((i + 1) / 5, 5);
            //    }
            //    else
            //    {
            //        piecesStructs[i].index = new Vector2((i + 1) / 5 + 1, (i + 1) % 5);
            //    }

            //    piecesStructs[i].piece = pieces[i];
            //    //piecesStructs[i].position = pieces[i].btnSelf.GetComponent<RectTransform>().localPosition;
            //    piecesStructs[i].state = pieces[i].pieceState;
            //}
        }
        private void Update()
        {
            if (Input.GetKeyDown(changeTeam))
            {
                Change();
            }
        }

        public void Change()
        {
            isRed = !isRed;
        }

        public void SetBlockAndRedToZero()
        {
            Red_xTx = 0;//横棍
            Red_yTy = 0;//纵棍
            Red_xyTxy = 0;//通天x=y
            Red_xy6Txy6 = 0;//通天x+y=6
            Red_LTThree = 0;//三斜
            Red_LDThree = 0;//三斜
            Red_RTThree = 0;//三斜
            Red_RDThree = 0;//三斜
            Red_LTFour = 0;//四斜
            Red_LDFour = 0;//四斜
            Red_RTFour = 0;//四斜
            Red_RDFour = 0;//四斜
            Red_rectangle = 0;//方


            Block_xTx = 0;//横棍
            Block_yTy = 0;//纵棍
            Block_xyTxy = 0;//通天x=y
            Block_xy6Txy6 = 0;//通天x+y=6
            Block_LTThree = 0;//三斜
            Block_LDThree = 0;//三斜
            Block_RTThree = 0;//三斜
            Block_RDThree = 0;//三斜
            Block_LTFour = 0;//四斜
            Block_LDFour = 0;//四斜
            Block_RTFour = 0;//四斜
            Block_RDFour = 0;//四斜

            Block_rectangle = 0;//方
        }
    }
}
