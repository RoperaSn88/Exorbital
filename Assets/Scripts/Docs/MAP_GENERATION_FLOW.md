# マップ自動生成フロー

このドキュメントでは、MapBaseScriptVer2とMapGeneratorVer2を使用したマップの自動生成フローについて説明します。

## 概要

プロジェクトは2つの主要なスクリプトを使用して、動的に3Dダンジョンマップを生成します：

1. **MapBaseScriptVer2** - 個々のマップチャンクの情報を保持
2. **MapGeneratorVer2** - マップ生成のメインロジックを制御

## 主要なデータ構造

### MapBaseScriptVer2 の主要なプロパティ

- `NeighborClass MapInfo` - マップの接続情報（9方向の隣接マップ）
- `List<MapLoopholeVer2> Loopholes` - 他のマップへの出口のリスト
- `List<EnemySpawnClass> EnemySpawnPoses` - 敵のスポーン位置のリスト
- `List<Transform> TreasureSpawnPoses` - 宝箱のスポーン位置のリスト
- `Transform StartSpawnPos` - スタートマップでのプレイヤースポーン位置
- `Vector3 MapNumber` - マップのグリッド座標

### MapGeneratorVer2 の主要なプロパティ

- `MapBaseScriptVer2 StartMap` - スタートマップのプレハブ
- `List<MapBaseScriptVer2> MapMaterials` - 通常のマップのプレハブリスト
- `List<MapBaseScriptVer2> ChallengeMaps` - 強敵マップのプレハブリスト
- `List<MapBaseScriptVer2> MiniEnds` - 行き止まりマップのプレハブリスト
- `List<MapBaseScriptVer2> Ends` - ゴールマップのプレハブリスト
- `List<GameObject> Enemys` - 通常敵のプレハブリスト
- `List<GameObject> EliteEnemys` - 強敵のプレハブリスト

## マップ自動生成フロー（詳細）

### 1. 初期化 (Start)

```
MapGeneratorVer2.Start()
  └─> StartCoroutine(GenerateMap())
```

### 2. スタートマップの生成

```
GenerateMap() - ステップ1: スタートマップ
  1. StartMapをInstantiateして生成
  2. 位置をVector3.zeroに設定
  3. プレイヤーをStartSpawnPosに配置
  4. SelectTrueLoophole()で次のマップへの正しい出口を選択
  5. SaveMapNumbers()でマップの占有するグリッド座標を保存
  6. EnemySpawnPosesから敵スポーン位置を収集
```

**関連メソッド:**
- `MapBaseScriptVer2.SelectTrueLoophole()` - 使用可能な出口からランダムに選択
- `MapBaseScriptVer2.SaveMapNumbers()` - マップが占有するグリッド座標を保存

### 3. メインマップループの生成

```
while (MapCount > 0) {
  ステップA: マップタイプの選択
    - ランダムに特殊マップ（チャレンジマップ）か通常マップかを決定
    - SpecificMapInfo.randomerで確率を制御

  ステップB: 互換性のあるマップの選択
    while (候補マップが残っている) {
      1. ランダムにマップを選択（CopyMapMaterialsまたはCopyChallenges）
      2. MatchMapNum()で前のマップの出口と入口が一致するか確認
      3. CheckNotCoverMap()で既存マップと重ならないか確認
      4. CheckCanSetNextPoint()で他の出口が塞がれないか確認
      5. 条件を満たせば選択完了、満たさなければ除外して次へ
    }

  ステップC: 行き止まり処理
    - すべてのマップが配置不可の場合
    - MiniEndsから行き止まりマップを配置
    - Failsリストから別の出口を新しいルートとして選択

  ステップD: マップの生成とデータ収集
    1. 選択したマップをInstantiate
    2. 入口の位置を前のマップの出口に合わせる
    3. 出口の位置を計算して配置
    4. SaveMapNumbers()でグリッド座標を保存
    5. SelectTrueLoophole()で次の出口を選択
    6. EnemySpawnPosesとTreasureSpawnPosesを収集

  MapCount--
}
```

**重要なメソッド:**
- `MatchMapNum(int TrueNum, int SelectedNum)` - 出口番号の互換性チェック（1↔7, 3↔5）
- `CheckNotCoverMap()` - 新しいマップが既存マップと重ならないか確認
- `CheckCanSetNextPoint()` - 他の出口が使用可能か確認
- `ReverseNum(int i)` - 出口番号を入口番号に変換

### 4. ゴールマップの生成

```
ゴールマップの生成:
  1. Endsリストから適切な方向のゴールマップを選択
  2. 最後のマップの出口に接続
  3. SaveMapNumbers()で座標を保存
```

### 5. 分岐ルート（失敗ルート）の生成

```
foreach (FailLoophoolClass fail in Fails) {
  1. 失敗リストの各出口について
  2. MiniMapCountMin～MiniMapCountMaxの長さのルートを生成
  3. 各ルートはMiniEndsで終了

  サブループ: 分岐マップの生成
    while (ran > 0) {
      - MapMaterialsからマップを選択
      - 同様の配置ロジック
      - さらに分岐がある場合、即座に行き止まりを配置
      - 敵・宝箱位置を収集
    }
}
```

### 6. NavMeshのビルド

```
NavMesh.BuildNavMesh()
  - すべてのマップ配置後にナビゲーションメッシュを構築
```

### 7. 敵と宝箱の生成

```
敵の生成:
  foreach (EnemySpawnClass cls in EnemySpawns) {
    - cls.Countsの数だけEnemysからランダムに敵を生成
    - cls.isMiddleがtrueの場合、EliteEnemysから強敵を生成
    - 生成された敵数をSceneManagerScript.instance.maxEnemyCountに追加
  }

宝箱の生成:
  foreach (Transform trans in TreasureSpawns) {
    - treasurePrefabを各位置にInstantiate
  }
```

### 8. ステージ開始

```
SceneManagerScript.instance.StartStage(this)
  - ステージの開始処理を実行
```

## 出口番号システム

マップは3x3グリッドの概念で接続されます：

```
1 2 3
4 5 6     5は中心（使用しない）
7 8 9
```

実際に使用される出口番号：
- **1** - 北（上）
- **3** - 東（右）
- **5** - 南（下）
- **7** - 西（左）

対応関係：
- 出口1 ↔ 入口7（北から入る↔南から出る）
- 出口3 ↔ 入口5（東から入る↔西から出る）
- 出口5 ↔ 入口3（南から入る↔北から出る）
- 出口7 ↔ 入口1（西から入る↔東から出る）

## グリッド座標システム

各マップは3Dグリッド座標（Vector3）を持ち、重複を防ぎます：
- `MapNumbers`リストにすべての占有座標を保存
- 新しいマップを配置する前にCheckNotCoverMap()で確認
- MapScaleClassで各マップの占有するグリッドのサイズを定義

## 敵の生成に関する現在の実装

現在のシステムでは：

1. **すべてのマップ**が敵を生成する可能性がある
   - 各MapBaseScriptVer2には`EnemySpawnPoses`リストがある
   - リストが空でなければ敵が生成される

2. **敵の生成はマップ配置後に一括で実行される**
   - GenerateMap()の最後に全EnemySpawnPosesを処理
   - 各スポーン位置でランダムに敵を選択して生成

3. **特殊マップ（チャレンジマップ）**
   - SpecificMapInfoで確率的に選択
   - ChallengeMapCountsで生成数を制限
   - 通常とは別のリスト（ChallengeMaps）から選択

## 改善の余地

現在の実装における課題：

1. **敵生成の柔軟性が低い**
   - すべてのマップが敵を生成するかしないかは、プレハブのEnemySpawnPosesリストに依存
   - コード上で動的に敵を生成するマップを制御できない

2. **マップタイプの明示的な分類がない**
   - 通常マップ、チャレンジマップの区別はあるが
   - "敵を生成するマップ"と"敵を生成しないマップ"の明示的な区別がない

3. **選択ロジックが複雑**
   - マップの選択と配置のロジックが密結合
   - 新しいマップタイプを追加しづらい

## まとめ

現在のマップ自動生成システムは：
- MapBaseScriptVer2でマップの情報を定義
- MapGeneratorVer2でInstantiateによる動的生成を実行
- グリッド座標システムで重複を防止
- 出口/入口の番号システムで接続を管理
- すべてのマップ配置後に敵と宝箱を生成

このフローを基に、敵を生成するマップと生成しないマップを明示的に分けて管理するシステムに改善することができます。
