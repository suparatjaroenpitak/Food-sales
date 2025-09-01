using System.Collections.Generic;
using TestQuit.Models;

namespace TestQuit.Service
{
    public interface IFood
    {
        public List<Food> GetFood();
        public string Create(Food food);
        public string Edit(Food food);
        public string Delete(string productId);
        public List<Food> Sort(string Product);
        public  List<Food> Search(string Product);
        public  List<Food> fillter(string date);
    }
}
