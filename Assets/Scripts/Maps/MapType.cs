// マップのタイプを定義する列挙型
public enum MapType
{
    Safe,           // 安全マップ（敵生成なし）
    Normal,          // 通常マップ（敵生成あり）
    Challenge,      // チャレンジマップ（強敵あり）
    Treasure        // 宝箱マップ（敵生成なし、宝箱のみ）
}
