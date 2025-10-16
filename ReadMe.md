# RF測試紀錄管理系統(RFTestRecordManagement) 專案計畫

---

### 1. 專案名稱

**「RF測試紀錄管理系統」**

---

### 2. 專案目的

本專案旨在建立一個 **Console 應用程式**，模擬RF測試時可以記錄量測功率的法規、無線技術、

測試頻段、功率量測值、測試結果和測試時間。

#### 核心功能
- **RF測試紀錄：** 新增、更新、刪除、查詢所有資料、查詢單筆資料和關鍵字查詢測試紀錄。

#### 階段目標 (Minimum Viable Product, MVP)
- **階段 1：Repository建立**
目的：將資料庫 **Json** 與 **SQL Server** 建立出來，方便資料儲存與匯出。
技術重點：練習 **Json** 與 **Dapper(ORM)** 的設計。

- **階段 2：RF測試紀錄管理**
目的：建立RF測試紀錄的 CRUD 功能 (新增、更新、刪除和查詢)。
技術重點：練習 **分層架構** 與 **資料存取分離** 的設計。
  
- **階段 3：單元測試**
目的： 驗證最小單位程式碼是否能正確運作預期的功能。
技術重點：練習 **xUnit測試架構** 與 **Moq模擬物件** 的功能。

---

### 3. 流程分析 (Use Case / 流程細節)

#### 主要流程
  
```mermaid
graph TD
    A[使用者啟動程式] --> B[初始化與設定讀取]
    B --> C{是否使用 Json?}
    C -->|否| D[建立 Dapper Repository 並測試連線]
    C -->|是| E[建立 JSON Repository]
    D --> F[初始化 Service 層（外部注入 Repository）]
    E --> F[初始化 Service 層（外部注入 Repository）]
    F --> G[顯示主選單（Console 介面）]
    G --> H1[新增測試紀錄]
    G --> H2[更新測試紀錄]
    G --> H3[刪除測試紀錄]
    G --> H4[查詢單筆紀錄]
    G --> H5[查詢所有紀錄]
    G --> H6[搜尋紀錄（依法規或無線技術）]
    G --> H7[匯出成 JSON]
    G --> H8[匯入 JSON]
    G --> I[離開系統]
```

#### 流程細節
新增紀錄

```mermaid
graph TD
    A[使用者選擇「新增測試紀錄」] --> B[輸入法規，技術,頻段,功率,結果,日期]
    B --> C[呼叫 Service.AddRecord（）]
    C --> D[驗證輸入是否合法]
    D -->|驗證失敗| E[顯示錯誤訊息,返回主選單]
    D -->|驗證通過| F[建立 RFTestRecord 物件]
    F --> G[呼叫 Repository.InsertRecord（record）]
    G --> H{目前使用的 Repository?}
    H -->|Dapper| I1[執行 SQL INSERT 指令]
    H -->|JSON| I2[將資料加入 List 並寫入 JSON 檔案]
    I1 -->|成功| J1[寫入 Repo_Dapper_action_log.txt]
    I1 -->|失敗| J2[寫入 Repo_Dapper_error_log.txt]
    I2 -->|成功| J3[寫入 Repo_Json_action_log.txt]
    I2 -->|失敗| J4[寫入 Repo_Json_error_log.txt]
    J1 --> K1[顯示新增成功]
    J3 --> K1[顯示新增成功]
    J2 --> K2[顯示錯誤訊息,返回主選單]
    J4 --> K2[顯示錯誤訊息,返回主選單]
    K1 --> L[返回主選單]
    K2 --> L
```
更新紀錄

```mermaid
graph TD
    A[使用者選擇「更新測試紀錄」] --> B[輸入欲更新的 RecordId]
    B --> C[呼叫 Service.GetRecordById（）]
    C -->|找不到| D[顯示「找不到該紀錄」並返回主選單]
    C -->|找到| E[顯示原始資料]
    E --> F[輸入新法規、技術、頻段、功率、結果、日期]
    F --> G[確認是否更新（Y/N）]
    G -->|N| H[取消操作返回主選單]
    G -->|Y| I[呼叫 Service.UpdateRecord（）]
    I --> J[驗證輸入]
    J -->|通過| K[Repository.UpdateRecord（record）]
    J -->|失敗| L[顯示錯誤訊息返回主選單]
    K --> M{Repository 類型?}
    M -->|Dapper| N1[執行 SQL UPDATE 指令]
    M -->|JSON| N2[更新 JSON 檔中對應紀錄]
    N1 -->|成功| O1[寫入 Repo_Dapper_action_log.txt]
    N1 -->|失敗| O2[寫入 Repo_Dapper_error_log.txt]
    N2 -->|成功| O3[寫入 Repo_Json_action_log.txt]
    N2 -->|失敗| O4[寫入 Repo_Json_error_log.txt]
    O1 --> P1[顯示更新成功]
    O2 --> P2[顯示錯誤訊息,返回主選單]
    O3 --> P1[顯示更新成功]
    O4 --> P2[顯示錯誤訊息,返回主選單]
    P1 --> Q[返回主選單]
    P2 --> Q
 ```

刪除紀錄

```mermaid
graph TD
    A[使用者選擇「刪除測試紀錄」] --> B[輸入欲刪除的 RecordId]
    B --> C[呼叫 Service.GetRecordById（）]
    C -->|找不到| D[顯示「找不到該紀錄」並返回主選單]
    C -->|找到| E[顯示原始資料]
    E -->F[確認是否刪除（Y/N）]
    F -->|N| G[取消操作返回主選單]
    F -->|Y| H[呼叫 Service.DeleteRecord（）]
    H -->I[驗證輸入]
    I -->|通過| J[Repository.DeleteRecord（recordId）]
    I -->|失敗| K[顯示錯誤訊息返回主選單]
    J -->L{Repository 類型?}
    L -->|Dapper| M1[執行 SQL DELETE 指令]
    L -->|JSON| M2[刪除 JSON 檔中對應紀錄]
    M1 -->|成功| N1[寫入 Repo_Dapper_action_log.txt]
    M1 -->|失敗| N2[寫入 Repo_Dapper_error_log.txt]
    M2 -->|成功| N3[寫入 Repo_Json_action_log.txt]
    M2 -->|失敗| N4[寫入 Repo_Json_error_log.txt]
    N1 --> O1[顯示刪除成功]
    N2 --> O2[顯示錯誤訊息,返回主選單]
    N3 --> O1[顯示刪除成功]
    N4 --> O2[顯示錯誤訊息,返回主選單]
    O1 --> P[返回主選單]
    O2 --> P
```

查詢單筆紀錄

```mermaid
graph TD
    A[使用者選擇「查詢單筆紀錄」] --> B[輸入欲查詢的 RecordId]
    B --> C[呼叫 Service.GetRecordById（）]
    C -->|找不到| D[顯示「找不到該紀錄」並返回主選單]
    C -->|找到| E[Repository.GetRecordById（recordId）]
    E -->F{Repository 類型?}
    F -->|Dapper| I1[執行 SQL SELECT * FROM RFTestRecord]
    F -->|JSON| I2[查詢 JSON 檔中對應紀錄]
    I1 -->|成功| J1[寫入 Repo_Dapper_action_log.txt]
    I1 -->|失敗| J2[寫入 Repo_Dapper_error_log.txt]
    I2 -->|成功| J3[寫入 Repo_Json_action_log.txt]
    I2 -->|失敗| J4[寫入 Repo_Json_error_log.txt]
    J1 --> K1[顯示查詢紀錄]
    J2 --> K2[顯示錯誤訊息,返回主選單]
    J3 --> K1
    J4 --> K2
    K1 --> L[返回主選單]
    K2 --> L
```

查詢所有紀錄

```mermaid
graph TD
    A[使用者選擇「查詢所有紀錄」] --> B[呼叫 Service.GetAllRecords（）]
    B -->|無資料| C[顯示「目前無任何紀錄」並返回主選單]
    B -->|有資料| D{Repository 類型?}
    D -->|Dapper| E1[執行 SQL SELECT * FROM RFTestRecord]
    D -->|JSON| E2[讀取 JSON 檔所有紀錄]
    E1 -->|成功| F1[寫入 Repo_Dapper_action_log.txt]
    E1 -->|失敗| F2[寫入 Repo_Dapper_error_log.txt]
    E2 -->|成功| F3[寫入 Repo_Json_action_log.txt]
    E2 -->|失敗| F4[寫入 Repo_Json_error_log.txt]
    F1 --> G1[顯示所有紀錄列表（RecordId、法規、技術、頻段、功率、結果、日期）]
    F3 --> G1
    F2 --> G2[顯示錯誤訊息並返回主選單]
    F4 --> G2
    G1 --> H[返回主選單]
    G2 --> H
```  

匯出所有紀錄至 JSON 檔案

```mermaid
graph TD
    A[使用者選擇「匯出所有紀錄」] --> B[呼叫 Service.GetAllRecords（）]
    B -->|無資料| C[顯示「目前無可匯出紀錄」並返回主選單]
    B -->|有資料| D[呼叫 Service.ExportToJson（）]
    D --> E{Repository 類型?}
    E -->|Dapper| F1[從資料庫撈出所有紀錄]
    E -->|JSON| F2[從 JSON 檔載入所有紀錄]
    F1 -->|成功| G1[呼叫 JsonFileHelper.ExportToJson（）]
    F1 -->|失敗| G2[寫入 Repo_Dapper_error_log.txt]
    F2 -->|成功| G3[呼叫 JsonFileHelper.ExportToJson（）]
    F2 -->|失敗| G4[寫入 Repo_Json_error_log.txt]
    G1 -->|成功| H1[寫入 Repo_Json_action_log.txt]
    G1 -->|失敗| H2[寫入 Repo_Json_error_log.txt]
    G3 -->|成功| H1
    G3 -->|失敗| H2
    G2 --> I2
    G4 --> I2
    H1 --> I1[顯示「匯出成功」]
    H2 --> I2[顯示錯誤訊息並返回主選單]
    I1 --> J[返回主選單]
```  

匯入 JSON 檔案置資料庫

```mermaid
graph TD  
    A[使用者選擇「匯入 JSON 檔案」] --> B[輸入 JSON 檔案路徑]
    B --> C[呼叫 Service.ImportFromJson（filePath）]
    C --> |檔案不存在或格式錯誤| D[顯示錯誤訊息並返回主選單]
    C --> |成功讀取| E[將檔案反序列化為 List<RFTestRecord>]
    E --> F{Repository 類型?}
    F -->|Dapper| G1[逐筆執行 SQL INSERT INTO RFTestRecord]
    F -->|JSON| G2[逐筆加入 JSON 檔紀錄清單]
    G1 -->|成功| H1[寫入 Repo_Dapper_action_log.txt]
    G1 -->|失敗| H2[寫入 Repo_Dapper_error_log.txt]
    G2 -->|成功| H3[寫入 Repo_Json_action_log.txt]
    G2 -->|失敗| H4[寫入 Repo_Json_error_log.txt]
    H1 --> I1[顯示「匯入成功，共 X 筆資料」]
    H2 --> I2[顯示錯誤訊息並返回主選單]
    H3 --> I1
    H4 --> I2
    I1 --> J[返回主選單]
    I2 --> J
```  

---

### 4. 程式架構設計 (分層式架構)

* **Presentation Layer (UI)** ：Console 負責輸入輸出、顯示介面

* **Business Logic Layer (Service)**：商業邏輯、檢查輸入、流程控制

* **Data Access Layer (Repository)**：封裝資料存取邏輯 (SQL_Server / Json)

* **Utilities（工具層）**：匯出/匯入 JSON 檔案、建立與管理 logs 資料夾

* **Infrastructure Layer (DatabaseConfig)** ：負責資料庫連線與測試

* **Domain Models (Entity)**：負責資料結構定義

* **UnitTest**：驗證 Service 層邏輯

---

### 5. 應用技術
* 使用的程式語言 (C#)
* 使用的工具 (VS Studio 2022, .Net Framework 4.8)
* 資料存放方式 (SQL Server + Dapper(ORM) & JSON)
* 練習的觀念 
  * 分層架構、資料存取分離、例外處理
  * 依賴注入與介面設計、物件導向（封裝、多型、泛型）
  * LINQ、基礎SQL語法、基礎Dapper（ORM）資料庫操作
  * Json檔案操作、日期時間操作、logs日誌
  * 匯出/匯入 Json檔案、單元測試（Moq + xUnit）

---

### 6. 程式碼結構

```mermaid
graph TD  
    A[Program.cs<br>主控台主流程] --> B[Service層<br>RFTestRecordService]
    B --> C1[DapperRFTestRecordRepository<br>（SQL Server + Dapper）]
    B --> C2[JsonRFTestRecordRepository<br>（JSON 檔案存取）]
    C1 --> D1[DatabaseConfigurement.cs<br>管理 DB 連線]
    C1 --> D2[JsonFileHelper.cs<br>匯出/匯入 JSON 檔案]
    C1 --> E[LogsHelper.cs<br>紀錄 Log 檔案]
    C1 --> F[RFTestRecord.cs<br>領域模型]
    C2 --> D2
    C2 --> E[LogsHelper.cs<br>紀錄 Log 檔案]
    C2 --> F
    B --> E[LogsHelper.cs<br>紀錄 Log 檔案]
```

---

### 7. 測試案例 (Test Cases)

|測試案例編號| 測試項目 | 測試輸入 | 預期結果 |
|--------------|---------|-----------|-----------|
|Test Case 01 | 新增紀錄 | 法規 = `null` / `""` /`" "` | 拋出`ArgumentException`|
|Test Case 02 | 新增紀錄 | 功率 > 50 or 功率 < -50  | 拋出`ArgumentOutOfRangeException`|
|Test Case 03 | 新增紀錄 | 功率未超出範圍時 | 不拋出`ArgumentOutOfRangeException` |
|Test Case 04 | 新增紀錄 | 測試日期超出範圍時 | 拋出`ArgumentOutOfRangeException` |
|Test Case 05 | 新增紀錄 | 測試日期等於預設值時 | 拋出`ArgumentOutOfRangeException` |
|Test Case 06 | 新增紀錄 | 當正確輸入時 | 應正確呼叫 `Repository.InsertRecord` 並正確設定欄位|
|Test Case 07 | 新增紀錄 | 在輸入驗證失敗時 | 不應呼叫 `Repository.InsertRecord`|

---

### 8. 學習心得

本週學習主題為：

* 「Dapper(ORM)」

* 「Json檔案匯入/匯出」

* 「Logs日誌」

* 「單元測試（Moq + xUnit）」
   
* 「Console_Mode顏色變更」

* 「Readme撰寫（使用Visual Code）」

過程中誤將 Repository 建立成 .Net 8.0版本，導致在撰寫 Service 層無法參考 Repository；

原本透過修改 Repository.csproj，將版本修正為.Net.FrameWork 4.8，但實際在UI層測試資料庫連線一直跑出 無法載入 DLL 'Microsoft.Data.SqlClient.SNI.x86.dll'；

最後發現是.Net.FrameWork 4.8 不支援 Microsoft.Data.SqlClient，需要使用 System.Data.SqlClient;


   
