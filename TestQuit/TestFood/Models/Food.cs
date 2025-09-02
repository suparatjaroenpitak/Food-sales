using System;

namespace TestQuit.Models
{
    public class Food
    {
        // OrderDate เป็น string เพราะคุณส่งมาในรูปแบบสตริง
        public DateTime OrderDate { get; set; }

        // ข้อมูลเหล่านี้เป็น string ทั้งหมด
        public string Region { get; set; }
        public string City { get; set; }
        public string Category { get; set; }
        public string Product { get; set; }

        // Quantity เป็นตัวเลข
        public int Quantity { get; set; }

        // UnitPrice และ TotalPrice เป็นตัวเลขทศนิยม
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
