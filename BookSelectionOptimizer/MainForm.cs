using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using Microsoft.Z3;
using Context = Microsoft.Z3.Context;
using Status = Microsoft.Z3.Status;

namespace BookSelectionOptimizer
{
    public partial class MainForm : Form
    {
        private DateTime _startTime;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = @"Excel Files|*.xlsx;*.xls";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                }
            }
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            var filePath = txtFilePath.Text;
            if (!File.Exists(filePath))
            {
                MessageBox.Show(@"请选择有效的 Excel 文件！", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var booksData = LoadBooksFromExcel(filePath);
            if (booksData == null || booksData.Count == 0)
            {
                MessageBox.Show(@"未找到有效的书籍数据！", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 提取书名和价格列表
            var books = booksData.Select(b => b.book).ToList();
            var prices = booksData.Select(b => b.price).ToList();

            if (!int.TryParse(txtTargetAmount.Text, out var targetAmount) ||
                !int.TryParse(txtMinQty.Text, out var minQty) ||
                !int.TryParse(txtMaxQty.Text, out var maxQty))
            {
                MessageBox.Show(@"请填写有效的数字！", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var allowZero = allowZeroCheck.Checked;

            btnRun.Enabled = false;
            _startTime = DateTime.Now;
            timer1.Start();
            var results = await Task.Run(() => SolveBookSelectionWithZ3(books, prices, targetAmount, minQty, maxQty, allowZero));
            btnRun.Enabled = true;
            timer1.Stop();
            btnRun.Text = @"开始凑单";
            MessageBox.Show($@"计算完成！耗时 {DateTime.Now - _startTime:hh\:mm\:ss}");

            if (results == null || results.Count == 0)
            {
                MessageBox.Show(@"未找到符合条件的组合！", @"结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dgvResults.Rows.Clear();
                foreach (var (book, qty, cost) in results)
                {
                    dgvResults.Rows.Add(book, qty, cost / (decimal)100.0);
                }

                var totalSum = results.Sum(item => item.cost);
                SaveResultsToExcel(filePath, results, totalSum);
                if (totalSum < targetAmount)
                {
                    MessageBox.Show($@"书单价格仍小于指定价格，可能为目标无解，或给定的数量上限不足，当前金额：{totalSum / 100.0:0.##}，目标金额：{targetAmount / 100.0:0.##}", @"警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($@"计算完成！目标金额：{totalSum / 100.0:0.##}", @"成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private static List<(string book, int qty, int cost)> SolveBookSelectionWithZ3(List<string> books, List<int> prices, int totalPrice, int n, int m, bool allowZero)
        {
            // 使用 Z3 定义优化问题
            using (var ctx = new Context())
            {
                var x = new IntExpr[prices.Count];
                for (var i = 0; i < prices.Count; i++)
                {
                    x[i] = ctx.MkIntConst($"x_{i}");
                }

                var opt = ctx.MkOptimize();

                // 添加约束 n <= x[i] <= m 或 x[i] == 0
                for (var i = 0; i < prices.Count; i++)
                {
                    if (allowZero)
                    {
                        opt.Add(ctx.MkOr(
                            ctx.MkAnd(ctx.MkGe(x[i], ctx.MkInt(n)), ctx.MkLe(x[i], ctx.MkInt(m))),
                            ctx.MkEq(x[i], ctx.MkInt(0))
                        ));
                    }
                    else
                    {
                        opt.Add(ctx.MkAnd(
                            ctx.MkGe(x[i], ctx.MkInt(n)),
                            ctx.MkLe(x[i], ctx.MkInt(m))
                        ));
                    }
                    
                }

                // 添加总价格约束
                ArithExpr totalCost = ctx.MkInt(0);
                for (var i = 0; i < prices.Count; i++)
                {
                    totalCost = ctx.MkAdd(totalCost, ctx.MkMul(x[i], ctx.MkInt(prices[i])));
                }
                opt.Add(ctx.MkLe(totalCost, ctx.MkInt(totalPrice)));

                // 设置目标函数
                opt.MkMaximize(totalCost);

                // 求解
                if (opt.Check() != Status.SATISFIABLE) return null;
                {
                    var model = opt.Model;
                    var selectedBooks = new List<(string book, int qty, int cost)>();

                    for (var i = 0; i < prices.Count; i++)
                    {
                        var qty = int.Parse(model.Evaluate(x[i]).ToString());
                        if (qty < 0) continue;
                        var cost = qty * prices[i];
                        selectedBooks.Add((books[i], qty, cost));
                    }
                    return selectedBooks;
                }

            }
        }

        private static List<(string book, int price)> LoadBooksFromExcel(string filePath)
        {
            var books = new List<(string book, int price)>();

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var sheet = workbook.Worksheet(1);
                    var headers = sheet.FirstRowUsed()?.Cells().Select(cell => cell.GetValue<string>()).ToList();

                    // 根据列名获取索引
                    if (headers != null)
                    {
                        var bookNameIndex = headers.IndexOf("书名") + 1; // ClosedXML 索引从 1 开始
                        var priceIndex = headers.IndexOf("定价") + 1;

                        if (bookNameIndex == 0 || priceIndex == 0)
                        {
                            MessageBox.Show(@"Excel 文件中未找到列名 '书名' 或 '定价'！", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }

                        // 读取数据
                        books.AddRange(from row in sheet.RowsUsed().Skip(1) let bookName = row.Cell(bookNameIndex).GetValue<string>() let price = (int)(row.Cell(priceIndex).GetValue<decimal>() * 100) select (bookName, price));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"无法加载 Excel 文件: {ex.Message}", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return books;
        }

        private static void SaveResultsToExcel(string originalFilePath, List<(string book, int qty, int cost)> results, int totalCost)
        {
            var directory = Path.GetDirectoryName(originalFilePath);
            if (directory == null) return;
            var outputFilePath = Path.Combine(directory, "凑单结果.xlsx");

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var sheet = workbook.Worksheets.Add("结果");
                    sheet.Cell(1, 1).Value = "书名";
                    sheet.Cell(1, 2).Value = "数量";
                    sheet.Cell(1, 3).Value = "小计（元）";

                    var row = 2;
                    foreach (var result in results)
                    {
                        sheet.Cell(row, 1).Value = result.book;
                        sheet.Cell(row, 2).Value = result.qty;
                        sheet.Cell(row, 3).Value = result.cost / 100.0; // 转回元
                        row++;
                    }

                    sheet.Cell(row, 2).Value = "总计（元）";
                    sheet.Cell(row, 3).Value = totalCost / 100.0;

                    workbook.SaveAs(outputFilePath);
                }

                MessageBox.Show($@"结果已保存到 {outputFilePath}", @"成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"无法保存结果文件: {ex.Message}", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _startTime;
            btnRun.Text = $@"计算中({elapsed:hh\:mm\:ss})";
        }
    }
}
