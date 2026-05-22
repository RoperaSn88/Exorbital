# マップタイプによる自動生成システム

## 概要

このプロジェクトでは、マップの自動生成時に敵を生成するマップと生成しないマップを明示的に分けて管理できるようになりました。

## 主な機能

### 1. マップタイプの分類

4種類のマップタイプが利用可能です：

| タイプ | 説明 | 敵の生成 | 宝箱の生成 | 用途 |
|-------|------|---------|-----------|------|
| **Normal** | 通常マップ | ✅ あり | 可能 | 標準的な戦闘エリア |
| **Safe** | 安全マップ | ❌ なし | 可能 | 休憩ポイント、探索エリア |
| **Challenge** | チャレンジマップ | ✅ あり（強敵） | 可能 | ボス戦、強敵エリア |
| **Treasure** | 宝箱マップ | ❌ なし | 優先的に生成 | 報酬エリア |

## セットアップ手順

### ステップ1: マッププレハブの設定

1. Unityエディタで、マッププレハブを選択
2. Inspectorで`Map Base Script Ver2`コンポーネントを確認
3. 「マップタイプ設定」セクションで`Map Type`を選択
4. プレハブを保存

#### 設定例

```
通常の戦闘マップ → Normal
休憩用の広場 → Safe
ボス戦の部屋 → Challenge
宝箱だけの隠し部屋 → Treasure
行き止まりマップ → Safe
```

### ステップ2: MapGeneratorVer2での配置

`MapGeneratorVer2`コンポーネントで、各リストに適切なタイプのマップを配置します：

```csharp
// 通常ルートで使用するマップ
MapMaterials:
  - NormalタイプとSafeタイプを混在させる
  - 例: Normal 70%, Safe 30%

// チャレンジマップ専用
ChallengeMaps:
  - Challengeタイプのマップのみ配置

// 行き止まり用
MiniEnds:
  - Safeタイプを推奨（敵がいないので探索しやすい）

// ゴールマップ
Ends:
  - Safeタイプを推奨（ゴール到達の達成感のため）
```

## 使用例

### 例1: バランスの取れたダンジョン

```
MapCount: 10
MapMaterials:
  - 7個のNormalマップ
  - 3個のSafeマップ
ChallengeMaps:
  - 2個のChallengeマップ
ChallengeMapCounts: 1

結果:
  - 通常戦闘: 7マップ
  - 安全な休憩: 3マップ
  - チャレンジ: 1マップ
  - ゴール: 1マップ（Safe）
```

### 例2: 難易度の高いダンジョン

```
MapCount: 15
MapMaterials:
  - 12個のNormalマップ
  - 2個のSafeマップ
ChallengeMaps:
  - 3個のChallengeマップ
ChallengeMapCounts: 2

結果:
  - 通常戦闘: 12マップ
  - 安全な休憩: 2マップ（少なめで難易度アップ）
  - チャレンジ: 2マップ
  - ゴール: 1マップ
```

### 例3: 宝探しダンジョン

```
MapCount: 8
MapMaterials:
  - 4個のNormalマップ
  - 2個のSafeマップ
  - 2個のTreasureマップ

結果:
  - 戦闘と探索のバランスが取れた構成
  - Treasureマップで報酬を獲得
```

## デバッグ機能

### 統計情報の表示

ゲーム実行時、マップ生成完了後にコンソールに統計情報が表示されます：

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

この情報を使って：
- マップの構成比率を確認
- 意図した通りに生成されているか検証
- ゲームバランスの調整材料にする

## ゲームデザインへの活用

### 1. プレイヤー体験の設計

```
戦闘 → 戦闘 → 休憩(Safe) → 戦闘 → チャレンジ → 宝箱(Treasure)
```

このように、戦闘と休憩のリズムを作ることができます。

### 2. 難易度カーブの調整

- ステージ序盤: Safeマップを多めに配置
- ステージ中盤: Normalマップ中心
- ステージ終盤: Challengeマップを増やす

### 3. 探索の動機付け

- 分岐ルートにTreasureマップを配置
- 行き止まりにも価値を持たせる

## トラブルシューティング

### Q: 既存のマップが動作しなくなった

A: 既存のマッププレハブは自動的に`Normal`タイプとして扱われます。特に変更は不要ですが、より細かく制御したい場合は個別に設定してください。

### Q: Safeマップでも敵が出現する

A: 以下を確認してください：
1. マッププレハブの`Map Type`が`Safe`に設定されているか
2. `EnemySpawnPoses`リストに敵スポーン位置が設定されていないか（設定されていても無視されますが）

### Q: 統計情報が表示されない

A: コンソールの表示フィルタを確認してください。`Log`メッセージが有効になっている必要があります。

## 技術詳細

詳細な技術仕様については、以下のドキュメントを参照してください：

- `MAP_GENERATION_FLOW.md` - マップ自動生成フローの詳細
- `IMPLEMENTATION_SUMMARY.md` - 実装の詳細とコード変更内容

## コード例

### プログラムからマップタイプを確認

```csharp
MapBaseScriptVer2 map = GetComponent<MapBaseScriptVer2>();

// 敵を生成すべきか確認
if (map.ShouldSpawnEnemies()) {
    // 敵を生成
}

// 宝箱を生成すべきか確認
if (map.ShouldSpawnTreasures()) {
    // 宝箱を生成
}

// マップタイプを直接確認
switch (map.mapType) {
    case MapType.Normal:
        // 通常マップの処理
        break;
    case MapType.Safe:
        // 安全マップの処理
        break;
    // ...
}
```

### 統計情報の取得

```csharp
MapGeneraterVer2 generator = GetComponent<MapGeneraterVer2>();
MapGenerationStats stats = generator.GetMapGenerationStats();

Debug.Log($"敵が出現するマップ: {stats.MapsWithEnemies}");
Debug.Log($"安全なマップ: {stats.MapsWithoutEnemies}");
```

## まとめ

このシステムにより、マップの自動生成時に：

✅ 敵を生成するマップと生成しないマップを明確に分類
✅ エディタから簡単に設定可能
✅ 統計情報で生成結果を確認可能
✅ ゲームバランスの調整が容易

これらの機能を活用して、より洗練されたダンジョン体験を設計できます。
