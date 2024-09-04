using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STools.Tool_Manager
{
    public enum Color_DBG : byte
    {
        Default,
        /// <summary>
        /// (same as cyan) 同青色	#00ffffff	
        /// </summary>
        Aqua,
        /// <summary>
        /// 黑色	#000000ff	
        /// </summary>
        Black,
        /// <summary>
        /// 蓝色	#0000ffff	
        /// </summary>
        Blue,
        /// <summary>
        /// 棕色	#a52a2aff	
        /// </summary>
        Brown,
        /// <summary>
        /// (same as aqua) 青色	#00ffffff	
        /// </summary>
        Cyan,
        /// <summary>
        /// 深蓝色	#0000a0ff	
        /// </summary>
        Darkblue,
        /// <summary>
        /// (same as magenta) 紫红色（同洋红）	#ff00ffff	
        /// </summary>
        Fuchsia,
        /// <summary>
        ///  绿色	#008000ff	
        /// </summary>
        Green,
        /// <summary>
        /// 灰色	#808080ff	
        /// </summary>
        Grey,
        /// <summary>
        /// 浅蓝色	#add8e6ff	
        /// </summary>
        Lightblue,
        /// <summary>
        /// 青橙绿	#00ff00ff	
        /// </summary>
        Lime,
        /// <summary>
        /// (same as fuchsia) 洋红色（同紫红色）	#ff00ffff
        /// </summary>
        Magenta,
        /// <summary>
        /// 褐红色	#800000ff	
        /// </summary>
        Maroon,
        /// <summary>
        /// 海军蓝	#000080ff	
        /// </summary>
        Navy,
        /// <summary>
        /// 橄榄色	#808000ff	
        /// </summary>
        Olive,
        /// <summary>
        /// 橙黄色	#ffa500ff	
        /// </summary>
        Orange,
        /// <summary>
        /// 紫色	#800080ff	
        /// </summary>
        Purple,
        /// <summary>
        /// 红色	#ff0000ff	
        /// </summary>
        Red,
        /// <summary>
        /// 银灰色	#c0c0c0ff	
        /// </summary>
        Silver,
        /// <summary>
        /// 蓝绿色	#008080ff	
        /// </summary>
        Teal,
        /// <summary>
        /// 白色	#ffffffff	
        /// </summary>
        White,
        /// <summary>
        /// 黄色	#ffff00ff	
        /// </summary>
        Yellow,
    }
    public static class Manager_DBG
    {
        public static void DBG(Color_DBG[] colors, params object[] args)
        {
            string str = string.Empty;
            string colorStr = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                if (i > colors.Length - 1)
                {
                    switch (colors[colors.Length - 1])
                    {
                        case Color_DBG.Default:
                            colorStr = "silver";
                            break;
                        case Color_DBG.Aqua:
                            colorStr = "aqua";
                            break;
                        case Color_DBG.Black:
                            colorStr = "black";
                            break;
                        case Color_DBG.Blue:
                            colorStr = "blue";
                            break;
                        case Color_DBG.Brown:
                            colorStr = "brown";
                            break;
                        case Color_DBG.Cyan:
                            colorStr = "cyan";
                            break;
                        case Color_DBG.Darkblue:
                            colorStr = "darkblue";
                            break;
                        case Color_DBG.Fuchsia:
                            colorStr = "fuchsia";
                            break;
                        case Color_DBG.Green:
                            colorStr = "green";
                            break;
                        case Color_DBG.Grey:
                            colorStr = "grey";
                            break;
                        case Color_DBG.Lightblue:
                            colorStr = "lightblue";
                            break;
                        case Color_DBG.Lime:
                            colorStr = "lime";
                            break;
                        case Color_DBG.Magenta:
                            colorStr = "magenta";
                            break;
                        case Color_DBG.Maroon:
                            colorStr = "maroon";
                            break;
                        case Color_DBG.Navy:
                            colorStr = "navy";
                            break;
                        case Color_DBG.Olive:
                            colorStr = "olive";
                            break;
                        case Color_DBG.Orange:
                            colorStr = "orange";
                            break;
                        case Color_DBG.Purple:
                            colorStr = "purple";
                            break;
                        case Color_DBG.Red:
                            colorStr = "red";
                            break;
                        case Color_DBG.Silver:
                            colorStr = "silver";
                            break;
                        case Color_DBG.Teal:
                            colorStr = "teal";
                            break;
                        case Color_DBG.White:
                            colorStr = "white";
                            break;
                        case Color_DBG.Yellow:
                            colorStr = "yellow";
                            break;
                        default:
                            colorStr = "silver";
                            break;
                    }
                }
                else
                {
                    switch (colors[i])
                    {
                        case Color_DBG.Default:
                            colorStr = "silver";
                            break;
                        case Color_DBG.Aqua:
                            colorStr = "aqua";
                            break;
                        case Color_DBG.Black:
                            colorStr = "black";
                            break;
                        case Color_DBG.Blue:
                            colorStr = "blue";
                            break;
                        case Color_DBG.Brown:
                            colorStr = "brown";
                            break;
                        case Color_DBG.Cyan:
                            colorStr = "cyan";
                            break;
                        case Color_DBG.Darkblue:
                            colorStr = "darkblue";
                            break;
                        case Color_DBG.Fuchsia:
                            colorStr = "fuchsia";
                            break;
                        case Color_DBG.Green:
                            colorStr = "green";
                            break;
                        case Color_DBG.Grey:
                            colorStr = "grey";
                            break;
                        case Color_DBG.Lightblue:
                            colorStr = "lightblue";
                            break;
                        case Color_DBG.Lime:
                            colorStr = "lime";
                            break;
                        case Color_DBG.Magenta:
                            colorStr = "magenta";
                            break;
                        case Color_DBG.Maroon:
                            colorStr = "maroon";
                            break;
                        case Color_DBG.Navy:
                            colorStr = "navy";
                            break;
                        case Color_DBG.Olive:
                            colorStr = "olive";
                            break;
                        case Color_DBG.Orange:
                            colorStr = "orange";
                            break;
                        case Color_DBG.Purple:
                            colorStr = "purple";
                            break;
                        case Color_DBG.Red:
                            colorStr = "red";
                            break;
                        case Color_DBG.Silver:
                            colorStr = "silver";
                            break;
                        case Color_DBG.Teal:
                            colorStr = "teal";
                            break;
                        case Color_DBG.White:
                            colorStr = "white";
                            break;
                        case Color_DBG.Yellow:
                            colorStr = "yellow";
                            break;
                        default:
                            colorStr = "silver";
                            break;
                    }
                }

                str += "<color=" + colorStr + ">{" + i + "}</color>";
            }
            Debug.LogFormat(str, args);
        }
    }
}