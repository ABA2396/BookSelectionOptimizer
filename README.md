# BookSelectionOptimizer

`BookSelectionOptimizer` 是一个用于优化书籍选择的 Windows 窗体应用程序。该应用程序允许用户从 Excel 文件中加载书籍数据，并根据用户指定的目标金额和单种书的数量范围，使用动态规划算法找到最优的书籍组合。

## 使用场景

在某些情况下，系统中可能没有某些书籍，或者某些书籍被锁定，无法购买。为了录入书单，用户可以使用相同的价格进账。通过 `BookSelectionOptimizer`，用户可以：

- 从现有的书籍列表中选择最优的书籍组合，以满足特定的预算和数量要求。
- 在系统中缺少某些书籍时，使用相同的价格进账，确保书单的总金额和数量符合要求。
- 在书籍被锁定或不可用时，找到替代书籍，以便继续进行采购和录入。

## 使用

1. 启动应用程序。
2. 点击“选择文件”按钮，选择包含书籍数据的 Excel 文件，推荐使用 xslx 格式，不支持 xsl 格式。Excel 文件应包含以下列：
   - `书名`
   - `定价`（以元为单位）
3. 输入目标金额（以分为单位）、单种书本的最小数量和最大数量。
4. 选择是否允许数量为 0（不购买某些书，结果会使备选中的书的数量偏向单种书本的最大数量，速度会比较慢）。
5. 点击“开始凑单”按钮，程序将计算可行的书籍组合。
6. 计算完成后，结果将显示在表格中，并保存到 `原文件名 + 凑单结果.xslx` Excel 文件中。

## 示例 Excel 文件格式

| 书名   | 定价  |
| ------ | ----- |
| 书籍1  | 50 |
| 书籍2  | 30 |
| 书籍3  | 20.2 |

## 代码结构

- `MainForm.cs`：主窗体代码，包含 UI 事件处理和主要逻辑。
- `Book`：书籍类，包含书籍的名称和价格。
- `ParentInfo`：用于动态规划算法中的父节点信息。
- `BitDP_Optimized`：使用位动态规划算法计算可行的书籍组合。
- `LoadBooksFromExcel`：从 Excel 文件加载书籍数据。
- `SaveResultsToExcel`：将计算结果保存到 Excel 文件。

## 界面截图

![image](https://github.com/user-attachments/assets/f8f12173-3d37-402d-a72a-6a4a32888a84)
![image](https://github.com/user-attachments/assets/be984d0c-9e1b-4dfb-b361-23c839052f21)
![image](https://github.com/user-attachments/assets/5d4aa21d-a0bd-447e-93c8-f667343e909f)
![image](https://github.com/user-attachments/assets/352b9000-97c3-4dc8-a1ae-5acfce9bcea4)

## 贡献

欢迎贡献代码！请 fork 此仓库并提交 pull request。

## 许可证

此项目使用 AGPL 3.0 许可证。详情请参阅 LICENSE 文件。
