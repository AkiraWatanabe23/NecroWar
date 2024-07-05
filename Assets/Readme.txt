==ClientProgram の使い方==

開発時実行環境：2022.3.14f1
サンプルシーン：「Assets/Scenes/NetworkDemo/ClientDemoScene.unity」


1, ServerConnector.cs
　・サーバーに対する接続処理、各リクエストの大枠を実行する
　・詳細なリクエスト処理、結果の受け取りは ConnectorModel が実行する
　・「Assets/Prefabs/Network/ServerConnector.prefab, UerDataHolder.prefab」をシーン上に配置して使用する
　・基本的にリクエストを送る際は、「await Request(string ID, string RequestName)」の形式で実行する


2, UserDataHolder.cs
　・ゲームにおける各データを保持するクラス
　・このクラスにユーザーの固有IDを保持していて、各リクエストの際にはこのIDをキーとして渡し、処理を実行する
　・やり取りするデータの定義は、後述の「Datas.cs」に記述する


3, Datas.cs
　・サーバーとやり取りするデータを定義するクラス
　・「class AbstractData」を継承し、定義する
　・サーバーに送信する際はstring型に変換するため、それに対応できる型で定義すること


4, ConnectorModel.cs
　・サーバーへのリクエスト、結果の受け取りの詳細を記述したクラス
　・UnityWebRequestを用いている
　　・Get, Post, Putの3通りのリクエストが実装されている、実行したい内容によって呼び出しを分ける

　　　<<違い>>
　　　・Get  → キーを渡さずに、アクセスのみを行いたい場合（ex. 初期のテストリクエスト）
　　　・Post → キーを渡して処理を実行したい場合（ex. 自分のデータを取得したい ... IDがあればいける）
　　　・Put  → キー以外に追加でパラメータを渡して処理を実行したい場合（ex. スコアをサーバーに保存したい ... IDとスコア）


5, ConnectorView.cs
　・リクエストの結果等を画面に表示するためのUIが宣言されているクラス
　・プロジェクトごとに表示する内容が異なることが想定されるため、必要であれば編集する