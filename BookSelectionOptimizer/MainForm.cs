// <copyright file="MainForm.cs" company="uye">
// MainForm - A part of the BookSelectionOptimizer project
// Copyright (C) 2024 uye and Contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License v3.0 only as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

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

        private void Start()
        {
            btnRun.Enabled = false;
            _startTime = DateTime.Now;
            timer1.Start();
        }

        private void Stop()
        {
            btnRun.Enabled = true;
            timer1.Stop();
            btnRun.Text = @"开始凑单";
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            var filePath = txtFilePath.Text;
            if (!File.Exists(filePath))
            {
                MessageBox.Show(@"请选择有效的 Excel 文件！", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Start();

            var booksData = await Task.Run(() => LoadBooksFromExcel(filePath));
            if (booksData == null || booksData.Count == 0)
            {
                MessageBox.Show(@"未找到有效的书籍数据！", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Stop();
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
                Stop();
                return;
            }

            var allowZero = allowZeroCheck.Checked;

            // var results = await Task.Run(() => SolveBookSelectionWithZ3(books, prices, targetAmount, minQty, maxQty, allowZero));
            var bookList = books.Select((t, i) => new Book { Name = t, Price = prices[i] }).ToList();
            var results = await Task.Run(() => DP_Optimized(bookList, minQty, maxQty, targetAmount, allowZero));
            Stop();
            var timeSpan = DateTime.Now - _startTime;

            if (results == null || results.Count == 0)
            {
                MessageBox.Show(@"未找到符合条件的组合！", @"结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dgvResults.Rows.Clear();
                foreach (var (book, price, qty, cost) in results)
                {
                    dgvResults.Rows.Add(book, (decimal)price / 100, qty, (decimal)cost / 100);
                }

                var totalSum = results.Sum(item => item.cost);
                SaveResultsToExcel(filePath, results);
                if (totalSum != targetAmount)
                {
                    MessageBox.Show($@"书单价格不等于指定价格，可能为目标无解，或给定的范围不足，当前金额：{totalSum / 100.0:0.0#}，目标金额：{targetAmount / 100.0:0.0#}", @"警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($@"计算完成！目标金额：{totalSum / 100.0:0.0#}，耗时 {timeSpan:hh\:mm\:ss}", @"成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /*
        private static List<(string book, int qty, int cost)> SolveBookSelectionWithZ3(List<string> books, List<int> prices, int totalPrice, int n, int m, bool allowZero)
        {
            // 使用 Z3 定义优化问题
            using (var ctx = new Context())
            {
                var count = books.Count;
                var x = new IntExpr[count];
                for (var i = 0; i < count; i++)
                {
                    x[i] = ctx.MkIntConst($"x_{i}");
                }

                var opt = ctx.MkOptimize();

                // 定义 n <= x[i] <= m 或 x[i] == 0 的约束
                for (var i = 0; i < count; i++)
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
                for (var i = 0; i < count; i++)
                {
                    totalCost = ctx.MkAdd(totalCost, ctx.MkMul(x[i], ctx.MkInt(prices[i])));
                }
                opt.Add(ctx.MkLe(totalCost, ctx.MkInt(totalPrice)));

                // 设置目标函数为最大化总成本
                opt.MkMaximize(totalCost);

                // 求解
                if (opt.Check() != Status.SATISFIABLE) return null;

                var model = opt.Model;
                var selectedBooks = new List<(string book, int qty, int cost)>();

                for (var i = 0; i < count; i++)
                {
                    var qty = int.Parse(model.Evaluate(x[i]).ToString());
                    if (qty < 0) continue;
                    var cost = qty * prices[i];
                    selectedBooks.Add((books[i], qty, cost));
                }

                return selectedBooks;
            }
        }
        */

        public class Book
        {
            public string Name { get; set; }
            public int Price { get; set; } // 已乘以 100 的整数价格
        }

        private static List<(string book, int price, int qty, int cost)> DP_Optimized(List<Book> books, int n, int m, int targetAmount, bool allowZero)
        {
            var rawN = n;

            var minPossibleAmount = 0;
            var maxPossibleAmount = 0;

            foreach (var price in books.Select(book => book.Price))
            {
                minPossibleAmount += price * n;
                maxPossibleAmount += price * m;
            }

            if (targetAmount <= minPossibleAmount)
            {
                return books.Select(book => (book.Name, book.Price, n, n * book.Price)).ToList(); ;
            }

            if (targetAmount >= maxPossibleAmount)
            {
                return books.Select(book => (book.Name, book.Price, m, m * book.Price)).ToList();
            }

            // 如果不允许数量为 0，先将总价减去所有书籍的最小数量的总价
            if (!allowZero)
            {
                targetAmount -= minPossibleAmount;
                m -= n;
                n = 0;
            }

            // 方案重构
            var dp = new bool[targetAmount + 1];
            dp[0] = true;
            var parent = new ParentInfo[targetAmount + 1];
            for (var i = 0; i <= targetAmount; i++)
            {
                parent[i] = new ParentInfo { BookIndex = -1, Count = 0 };
            }

            var booksCount = books.Count;

            for (var i = 0; i < booksCount; ++i)
            {
                var p = books[i].Price;
                if (p == 0) continue;

                // 遍历选择数量
                for (var cnt = n; cnt <= m; ++cnt)
                {
                    var contribution = p * cnt;
                    if (contribution > targetAmount) break;

                    for (var current = targetAmount; current >= contribution; --current)
                    {
                        var prev = current - contribution;
                        if (prev < 0) continue;

                        if (!dp[prev] || parent[prev].BookIndex == i) continue;

                        if (dp[current]) continue;

                        dp[current] = true;
                        parent[current].BookIndex = i;
                        parent[current].Count = cnt;
                    }
                }
            }

            // 回溯获取购买方案
            var selectedCounts = new int[booksCount];
            var sum = targetAmount;
            while (sum > 0)
            {
                var info = parent[sum];
                if (info.BookIndex == -1)
                {
                    return null;
                }
                selectedCounts[info.BookIndex] += info.Count;
                sum -= info.Count * books[info.BookIndex].Price;
            }

            return allowZero
                ? books.Select((t, i) => (t.Name, t.Price, selectedCounts[i], selectedCounts[i] * t.Price)).ToList()
                : books.Select((t, i) => (t.Name, t.Price, selectedCounts[i] + rawN, (selectedCounts[i] + rawN) * t.Price)).ToList();
        }

        private struct ParentInfo
        {
            public int BookIndex;
            public int Count;
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

        private static void SaveResultsToExcel(string originalFilePath, List<(string book, int price, int qty, int cost)> results)
        {
            var directory = Path.GetDirectoryName(originalFilePath);
            if (directory == null) return;
            var outputFilePath = $"{originalFilePath}凑单结果.xlsx";

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var sheet = workbook.Worksheets.Add("结果");
                    sheet.Cell(1, 1).Value = "书名";
                    sheet.Cell(1, 2).Value = "单价";
                    sheet.Cell(1, 3).Value = "数量";
                    sheet.Cell(1, 4).Value = "小计（元）";

                    var row = 2;
                    foreach (var result in results)
                    {
                        sheet.Cell(row, 1).Value = result.book;
                        sheet.Cell(row, 2).Value = result.price / 100.0;
                        sheet.Cell(row, 3).Value = result.qty;
                        sheet.Cell(row, 4).Value = result.cost / 100.0; // 转回元
                        row++;
                    }

                    // sheet.Cell(row, 3).Value = "总计（元）";
                    // sheet.Cell(row, 4).Value = totalCost / 100.0;

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
