namespace FTools.Community
{
    public enum RoomType : byte
    {
        Double = 0,
        Flexible
    }
    public enum RoomState : byte
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 匹配中
        /// </summary>
        Matching,
        /// <summary>
        /// 匹配中拒绝
        /// </summary>
        MatchingRefuse,
        /// <summary>
        /// 已匹配到
        /// </summary>
        Matched,
        /// <summary>
        /// 匹配到的拒绝
        /// </summary>
        MatchedRefuse,
        /// <summary>
        /// 匹配成功
        /// </summary>
        MatchedFinished
    }
}
