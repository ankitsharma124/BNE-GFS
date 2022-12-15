## Terraformを使ったCloudRunのblue / green deploy について

 - refs: https://cloud.google.com/run/docs/rollouts-rollbacks-traffic-migration
 - 対象resource: google_cloud_run_service.corebridge-XXX


## 0. 初回デプロイ:
  -  traffic {percent:100, tag: "blue" } でdeploy (blue)


## 1. greenデプロイ:
  - greenデプロイ用にimageタグを変更する
  - blueデプロイの traffic.revision_name に0.で生成されたrevision_nameを指定する
  - traffic {percet: 0, tag: "green"} で deploy(green)
  - terraform apply


## 2. Trafficの分散
  - greenデプロイの traffic.revision_name に1.で生成されたrevision_nameを指定する
  - 各々任意のtraffic.percentを設定してdeploy
    - ex. 30:70でtrafficを分散させる

```
       traffic {percet: 30, tag: "blue", revison_name: "blue"}
       traffic {percet: 70, tag: "green", revison_name: "green"}
```

  - terraform apply


### 分散状況をログエクスプローラにて確認する

```
severity>=DEFAULT
resource.type="cloud_run_revision"
resource.labels.revision_name="corebridge-XXXX"
```


## 3. blue / greenの切り替え
  - 1. でdeployした revisionを、traffic {percet: 100, tag: "blue"} に変更
  - 0. でdeployした revisionを、traffic {percet: 0, tag: "green"} に変更
  - terraform apply
