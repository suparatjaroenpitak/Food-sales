using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TestQuit.Models;

namespace TestQuit.Service
{
    public class FoodRepo : IFood
    {
        private readonly IWebHostEnvironment _env;

        public FoodRepo(IWebHostEnvironment env)
        {
            _env = env;
        }

        private string FilePath => Path.Combine(_env.WebRootPath, "Data", "Food sales.xlsx");
        private const string SheetName = "Foods";

        public List<Food> GetFood() => GetFoodFromExcel();

        public string Create(Food food)
        {
            AppendFoodToExcel(food);
            return "บันทึกสำเร็จ";
        }

        public string Delete(Food food)
        {
            DeleteFoodFromExcel(food);
            return "ลบสำเร็จ";
        }

        public string Edit(Food food)
        {
            UpdateFoodInExcel(food);
            return "บันทึกเรียบร้อย";
        }

        // Refactored Sorting Method
        public List<Food> Sort(string sortBy, string sortDir)
        {
            var allFoods = GetFoodFromExcel();
            var sortedFoods = allFoods.AsQueryable();

            if (string.IsNullOrEmpty(sortBy)) return sortedFoods.ToList();

            // Use reflection to get the property to sort by dynamically
            var property = typeof(Food).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return sortedFoods.ToList();

            if (sortDir?.ToLower() == "desc")
            {
                sortedFoods = sortedFoods.OrderByDescending(x => property.GetValue(x, null));
            }
            else
            {
                sortedFoods = sortedFoods.OrderBy(x => property.GetValue(x, null));
            }

            return sortedFoods.ToList();
        }

        // Refactored Search Method
        public List<Food> Search(string product)
        {
            var allFoods = GetFoodFromExcel();
            return allFoods.Where(x => x.Product?.Equals(product, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        // Refactored Filter Method
        public List<Food> fillter(string date)
        {
            var allFoods = GetFoodFromExcel();
            return allFoods.Where(x => x.OrderDate?.Equals(date, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        // ------------------- CORE EXCEL METHODS ------------------------

        private List<Food> GetFoodFromExcel()
        {
            List<Food> dataList = new List<Food>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"⚠️ Excel file not found at: {FilePath}");
                return dataList;
            }

            using var package = new ExcelPackage(new FileInfo(FilePath));
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet == null) return dataList;

            int rowCount = worksheet.Dimension.End.Row;

            for (int row = 2; row <= rowCount; row++)
            {
                var food = new Food
                {
                    OrderDate = ReadCell<string>(worksheet.Cells[row, 1]),
                    Region = ReadCell<string>(worksheet.Cells[row, 2]),
                    City = ReadCell<string>(worksheet.Cells[row, 3]),
                    Category = ReadCell<string>(worksheet.Cells[row, 4]),
                    Product = ReadCell<string>(worksheet.Cells[row, 5]),
                    Quantity = ReadCell<int>(worksheet.Cells[row, 6]),
                    UnitPrice = ReadCell<decimal>(worksheet.Cells[row, 7]),
                    TotalPrice = ReadCell<decimal>(worksheet.Cells[row, 8])
                };
                dataList.Add(food);
            }

            return dataList;
        }

        private void AppendFoodToExcel(Food food)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(FilePath));

            var worksheet = package.Workbook.Worksheets[0] ?? package.Workbook.Worksheets.Add(SheetName);

            if (worksheet.Dimension == null)
            {
                // Header
                worksheet.Cells[1, 1].Value = "OrderDate";
                worksheet.Cells[1, 2].Value = "Region";
                worksheet.Cells[1, 3].Value = "City";
                worksheet.Cells[1, 4].Value = "Category";
                worksheet.Cells[1, 5].Value = "Product";
                worksheet.Cells[1, 6].Value = "Quantity";
                worksheet.Cells[1, 7].Value = "UnitPrice";
                worksheet.Cells[1, 8].Value = "TotalPrice";
            }

            int newRow = worksheet.Dimension.End.Row + 1;


            worksheet.Cells[newRow, 1].Value = food.OrderDate ;
            worksheet.Cells[newRow, 2].Value = food.Region;
            worksheet.Cells[newRow, 3].Value = food.City;
            worksheet.Cells[newRow, 4].Value = food.Category;
            worksheet.Cells[newRow, 5].Value = food.Product;
            worksheet.Cells[newRow, 6].Value = food.Quantity;
            worksheet.Cells[newRow, 7].Value = food.UnitPrice;
            worksheet.Cells[newRow, 8].Value = food.TotalPrice;

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            package.Save();
        }

        private void UpdateFoodInExcel(Food food)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(FilePath));
            var worksheet = package.Workbook.Worksheets[0];
            if (worksheet == null) return;

            int rowCount = worksheet.Dimension.End.Row;

            for (int row = 2; row <= rowCount; row++)
            {
                // Assuming you use `OrderDate`, `Region`, and `Product` as unique identifiers for a record
                string region = ReadCell<string>(worksheet.Cells[row, 2]);
                string product = ReadCell<string>(worksheet.Cells[row, 5]);

                if (region.Equals(food.Region, StringComparison.OrdinalIgnoreCase) && product.Equals(food.Product, StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cells[row, 6].Value = food.Quantity;
                    worksheet.Cells[row, 7].Value = food.UnitPrice;
                    worksheet.Cells[row, 8].Value = food.TotalPrice;

                    Console.WriteLine($"🟡 แก้ไขข้อมูลแถว {row}");
                    break;
                }
            }

            package.Save();
        }

        private void DeleteFoodFromExcel(Food foodToDelete)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(FilePath));
            var worksheet = package.Workbook.Worksheets[0];
            if (worksheet == null) return;

            int rowCount = worksheet.Dimension.End.Row;
            int deletedCount = 0;

            for (int row = rowCount; row >= 2; row--)
            {
                string region = ReadCell<string>(worksheet.Cells[row, 2]);
                string product = ReadCell<string>(worksheet.Cells[row, 5]);

                if (
                    region != null && product != null &&
                    region.Equals(foodToDelete.Region, StringComparison.OrdinalIgnoreCase) &&
                    product.Equals(foodToDelete.Product, StringComparison.OrdinalIgnoreCase)
                )
                {
                    worksheet.DeleteRow(row);
                    Console.WriteLine($"🔴 ลบแถวที่ {row} ที่มี Product = '{foodToDelete.Product}'");
                    deletedCount++;
                }
            }

            if (deletedCount > 0)
            {
                package.Save();
                Console.WriteLine($"✅ ลบไป {deletedCount} แถวสำเร็จแล้ว");
            }
            else
            {
                Console.WriteLine($"ℹ️ ไม่พบแถวที่ตรงกับเงื่อนไข");
            }
        }

        // ------------------- HELPER ------------------------

        private T ReadCell<T>(ExcelRange cell)
        {
            try
            {
                if (cell?.Value == null) return default;
                var val = cell.Value.ToString();
                if (string.IsNullOrWhiteSpace(val)) return default;

                // Handle date conversion specifically if needed
                if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                {
                    if (DateTime.TryParse(val, out DateTime parsedDate))
                    {
                        return (T)(object)parsedDate;
                    }
                    return default;
                }

                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading cell value: {ex.Message}");
                return default;
            }
        }
    }
}