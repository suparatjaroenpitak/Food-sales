using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TestQuit.Models;
using TestQuit.Service;

public class FoodController : Controller
{
    private readonly IFood _foodService;

    public FoodController(IFood foodService)
    {
        _foodService = foodService;
    }

    public IActionResult Index()
    {
        var allFoods = _foodService.Sort("");
        return View(allFoods);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Food food)
    {
        if (food == null)
        {
            return Json(new { success = false, message = "The provided data is null or could not be deserialized." });
        }
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        try
        {
            var message = _foodService.Create(food);
            return Json(new { success = true, message });
        }
        catch (Exception ex)
        {
            // Log the full exception details for debugging.
            Console.WriteLine(ex.ToString());
            return Json(new { success = false, message = "An error occurred: " + ex.Message });
        }
    }
    [HttpPost]
    public IActionResult Edit(string orderDate, string region, string product, string quantity, decimal unitPrice)
    {
        // Validate the received data
        if (string.IsNullOrEmpty(product))
        {
            return Json(new { success = false, message = "ข้อมูลไม่ถูกต้อง" });
        }

        // Create a Food object from the received parameters
        var foodToEdit = new Food
        {
            Region = region,
            Product = product,
            Quantity = int.Parse( quantity),
            UnitPrice = unitPrice,
            TotalPrice = decimal.Parse(quantity) * unitPrice
        };

        try
        {
            var message = _foodService.Edit(foodToEdit);
            return Json(new { success = true, message });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    // ปรับปรุงเมธอด Delete เพื่อรับข้อมูลเป็น Food object
    [HttpPost]
    public IActionResult Delete([FromForm] Food food)
    {
        if (food == null || string.IsNullOrEmpty(food.Product))
            return Json(new { success = false, message = "ข้อมูลว่างหรือ Product ไม่ถูกต้อง" });

        try
        {
            // ส่ง Food object ที่มี OrderDate, Region, และ Product ไปยัง service
            var message = _foodService.Delete(food);
            return Json(new { success = true, message });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult Search(string product)
    {
        var searchResult = _foodService.Search(product);

        // Return the search result as a JSON object
        return Json(searchResult);
    }

    [HttpGet]
    public IActionResult Filter(string date)
    {
        var filterResult = _foodService.fillter(date);
        return PartialView("_FoodTable", filterResult);
    }
    [HttpGet]
    public IActionResult Sort(string sortBy, string sortDir)
    {
        var sortedFoods = _foodService.Sort(sortBy, sortDir);
        return Json(sortedFoods);
    }
}
