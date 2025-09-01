using OfficeOpenXml;
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

        public List<Food> GetFood()
        {
            return GetFoodFromExcel();
        }
        public string Create(Food food)
        {
            AppendFoodToExcel(food);
            return "บันทึกสำเร็จ";
        }

        public string Delete(string productId)
        {
            DeleteFoodByProduct(productId);
            return "ลบสำเร็จ";
        }

        public string Edit(Food food)
        {
            UpdateFoodInExcel(food);
            return "บันทึกเรียบร้อย";
        }

        public List<Food> Sort(string product)
        {
            return GetFoodFromExcel()
                .OrderByDescending(x => x.Product)
                .ToList();
        }

        public List<Food> Search(string product)
        {
            return GetFoodFromExcel()
                .Where(x => x.Product == product)
                .ToList();
        }

        public List<Food> fillter(string date)
        {
            var targetDate = DateTime.Parse(date);
            return GetFoodFromExcel()
                .Where(x => x.OrderDate.Date == targetDate.Date)
                .ToList();
        }

        // ------------------- CORE EXCEL METHODS ------------------------

        private List<Food> GetFoodFromExcel()
        {
            List<Food> dataList = new List<Food>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(FilePath));
            var worksheet = package.Workbook.Worksheets[SheetName];

            if (worksheet == null)
                return dataList;

            int rowCount = worksheet.Dimension.End.Row;

            for (int row = 2; row <= rowCount; row++)
            {
                var food = new Food
                {
                    OrderDate = ReadCell<DateTime>(worksheet.Cells[row, 1]),
                    Region = ReadCell<string>(worksheet.Cells[row, 2]),
                    City = ReadCell<string>(worksheet.Cells[row, 3]),
                    Category = ReadCell<string>(worksheet.Cells[row, 4]),
                    Product = ReadCell<string>(worksheet.Cells[row, 5]),
                    Quantity = ReadCell<string>(worksheet.Cells[row, 6]),
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

            var worksheet = package.Workbook.Worksheets[SheetName] ?? package.Workbook.Worksheets.Add(SheetName);

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

            worksheet.Cells[newRow, 1].Value = food.OrderDate;
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
            var worksheet = package.Workbook.Worksheets[SheetName];
            if (worksheet == null) return;

            int rowCount = worksheet.Dimension.End.Row;

            for (int row = 2; row <= rowCount; row++)
            {
                DateTime? date = ReadCell<DateTime?>(worksheet.Cells[row, 1]);
                string region = ReadCell<string>(worksheet.Cells[row, 2]);
                string product = ReadCell<string>(worksheet.Cells[row, 5]);

                if (date.HasValue && date.Value.Date == food.OrderDate.Date &&
                    region == food.Region && product == food.Product)
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

        private void DeleteFoodByProduct(string productToDelete)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(FilePath));
            var worksheet = package.Workbook.Worksheets[SheetName];
            if (worksheet == null) return;

            int rowCount = worksheet.Dimension.End.Row;

            for (int row = rowCount; row >= 2; row--)
            {
                string product = ReadCell<string>(worksheet.Cells[row, 5]);

                if (product == productToDelete)
                {
                    worksheet.DeleteRow(row);
                    Console.WriteLine($"🔴 ลบแถวที่ {row} ที่มี Product = '{productToDelete}'");
                }
            }

            package.Save();
        }

        // ------------------- HELPER ------------------------

        private T ReadCell<T>(ExcelRange cell)
        {
            try
            {
                var val = cell?.Value?.ToString();
                if (string.IsNullOrWhiteSpace(val)) return default;

                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
