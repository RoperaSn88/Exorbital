namespace Manager.SelectElement
{
    public enum ElementUIMoveKinds
    {
        TopNull,
        /// <summary>
        /// 上側の何もないところから出てくる方法
        /// </summary>
        TopNullToTopMini,

        /// <summary>
        /// 上のミニからメインに移動する方法
        /// </summary>
        TopMiniToMain,

        /// <summary>
        /// メインから下のミニへ移動する方法
        /// </summary>
        MainToBottomMini,

        /// <summary>
        /// 下のミニから移動して消える方法 → TopNull
        /// </summary>
        BottomMiniToBottomNull,

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////
        /// </summary>

        BottomNull,
    
        /// <summary>
        /// 下の何もないところから出現する方法
        /// </summary>
        BottomNullToBottomMini,
        /// <summary>
        /// 下ミニからメイン
        /// </summary>
        BottomMiniToMain,

        /// <summary>
        /// メインから上ミニ
        /// </summary>
        MainToTopMini,

        /// <summary>
        /// 上ミニから消える
        /// </summary>
        TopMiniToTopNull,

    
    }    
}