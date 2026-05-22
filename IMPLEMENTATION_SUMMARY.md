# マップ自動生成の改善実装まとめ

## 実装内容

### 1. MapTypeの導入（MapBaseScriptVer2.cs）

マップのタイプを明示的に分類する列挙型を追加しました：

```csharp
public enum MapType
{
    Normal,          // 通常マップ（敵生成あり）
    Safe,           // 安全マップ（敵生成なし）
    Challenge,      // チャレンジマップ（強敵あり）
    Treasure        // 宝箱マップ（敵生成なし、宝箱のみ）
}
```

各MapBaseScriptVer2に`mapType`フィールドを追加：
- エディタから簡単に設定可能
- デフォルトはMapType.Normal

### 2. 敵生成判定メソッドの追加（MapBaseScriptVer2.cs）

マップタイプに基づいて敵や宝箱を生成するかを判定するメソッドを追加：

```csharp
/// <summary>
/// このマップが敵を生成するかどうかを返す
/// </summary>
public bool ShouldSpawnEnemies()
{
    return mapType == MapType.Normal || mapType == MapType.Challenge;
}

/// <summary>
/// このマップが宝箱を生成するかどうかを返す
/// </summary>
public bool ShouldSpawnTreasures()
{
    return mapType == MapType.Treasure || TreasureSpawnPoses.Count > 0;
}
```

### 3. マップ生成の追跡（MapGeneraterVer2.cs）

生成されたすべてのマップを追跡するリストを追加：

```csharp
private List<MapBaseScriptVer2> GeneratedMaps;
```

すべてのInstantiate箇所でGeneratedMapsに追加：
- スタートマップ
- メインループのマップ
- 行き止まりマップ
- ゴールマップ
- 分岐ルートのマップ

### 4. 敵生成ロジックの改善（MapGeneraterVer2.cs）

敵スポーン位置の収集時にマップタイプをチェック：

```csharp
// Before
if(GeneratedMap.EnemySpawnPoses.Count != 0)
    foreach(EnemySpawnClass trans in GeneratedMap.EnemySpawnPoses) EnemySpawns.Add(trans);

// After
if(GeneratedMap.ShouldSpawnEnemies() && GeneratedMap.EnemySpawnPoses.Count != 0)
    foreach(EnemySpawnClass trans in GeneratedMap.EnemySpawnPoses) EnemySpawns.Add(trans);
```

### 5. 統計情報の追加（MapGeneraterVer2.cs）

マップ生成の統計情報を収集・表示する機能を追加：

```csharp
[System.Serializable]
public class MapGenerationStats
{
    public int TotalMaps;
    public int NormalMaps;
    public int SafeMaps;
    public int ChallengeMaps;
    public int TreasureMaps;
    public int MapsWithEnemies;
    public int MapsWithoutEnemies;
}

public MapGenerationStats GetMapGenerationStats() { ... }
public void LogMapGenerationStats() { ... }
```

統計情報はマップ生成完了時に自動的にログに出力されます。

## 使用方法

### マッププレハブの設定

1. UnityエディタでMapBaseScriptVer2を持つマッププレハブを開く
2. Inspectorで「マップタイプ設定」セクションを見つける
3. `Map Type`ドロップダウンから適切なタイプを選択：
   - **Normal**: 通常の敵が出現するマップ
   - **Safe**: 敵が出現しない安全なマップ
   - **Challenge**: 強敵が出現するチャレンジマップ
   - **Treasure**: 敵が出現せず宝箱のみのマップ

### MapGeneratorVer2での設定

マップリストに異なるタイプのマップを混在させることができます：

- `MapMaterials`: 通常マップとSafeマップを混在可能
- `ChallengeMaps`: Challengeタイプのマップを配置
- `MiniEnds`: Safeタイプの行き止まりマップを推奨
- `Ends`: ゴールマップ（通常はSafe）

## 統計情報の確認

ゲーム実行時、マップ生成完了後にコンソールに以下のような統計情報が出力されます：

```
=== マップ生成統計 ===
総マップ数: 15
通常マップ: 8
安全マップ: 4
チャレンジマップ: 2
宝箱マップ: 1
敵生成あり: 10
敵生成なし: 5
```

## 利点

### 1. 明示的な制御
- マップごとに敵生成の有無を明示的に設定可能
- コードを変更せずにエディタから調整可能

### 2. 柔軟性の向上
- 同じマッププレハブを異なる目的で使用可能
- 新しいマップタイプを簡単に追加可能

### 3. デバッグの容易化
- 統計情報により生成されたマップの構成を確認可能
- マップタイプごとの分布を把握しやすい

### 4. ゲームバランスの調整
- 敵が出現するマップと出現しないマップの比率を調整
- プレイヤーの休憩ポイントを戦略的に配置

## 後方互換性

既存のマッププレハブは自動的に`MapType.Normal`として扱われるため、既存の機能は影響を受けません。必要に応じて個別にマップタイプを変更できます。

## 将来の拡張可能性

このシステムは以下のような拡張に対応可能です：

1. **新しいマップタイプの追加**
   - Boss戦専用マップ
   - ショップマップ
   - イベントマップ

2. **動的なマップタイプ変更**
   - ゲーム進行度に応じてマップタイプを変更
   - プレイヤーの状態に応じた適応的生成

3. **より詳細な統計**
   - 各タイプのマップがどの位置に生成されたか
   - 敵の総数や種類の分布

## コード変更箇所まとめ

### 変更ファイル

1. **Assets/Scripts/Maps/MapBaseScriptVer2.cs**
   - MapType列挙型を追加
   - mapTypeフィールドを追加
   - ShouldSpawnEnemies()メソッドを追加
   - ShouldSpawnTreasures()メソッドを追加

2. **Assets/Scripts/Maps/MapGeneraterVer2.cs**
   - GeneratedMapsリストを追加
   - すべてのマップ生成箇所でGeneratedMapsに追加
   - 敵スポーン位置収集時にShouldSpawnEnemies()を使用
   - MapGenerationStatsクラスを追加
   - GetMapGenerationStats()メソッドを追加
   - LogMapGenerationStats()メソッドを追加
   - GenerateMap()の最後で統計情報を出力

### 新規ファイル

1. **MAP_GENERATION_FLOW.md**
   - マップ自動生成フローの詳細ドキュメント

2. **IMPLEMENTATION_SUMMARY.md**（このファイル）
   - 実装内容のまとめ
