using Microsoft.AspNetCore.Mvc;
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
        var allFoods = _foodService.Sort("OrderDate", "asc");
        return View(allFoods);
    }
    [HttpPost]
    public IActionResult Create([FromBody] Food food)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, message = "Validation failed.", errors });
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
    public IActionResult Edit([FromForm] Food food)
    {
        // The model binder automatically populates the food object
        if (food == null || string.IsNullOrEmpty(food.Product) || !ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data provided." });
        }
        try
        {
            var message = _foodService.Edit(food);
            return Json(new { success = true, message });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpPost]
    public IActionResult Delete([FromForm] Food food)
    {
        if (food == null || string.IsNullOrEmpty(food.Product))
        {
            return Json(new { success = false, message = "Invalid data provided." });
        }
        try
        {
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
        return Json(searchResult);
    }
    [HttpGet]
    public IActionResult Filter(string date)
    {
        DateTime dates = DateTime.Parse(date);
        var filterResult = _foodService.fillter(dates);
        return Json(filterResult);
    }

    [HttpGet]
    public IActionResult Sort(string sortBy, string sortDir)
    {
        var sortedFoods = _foodService.Sort(sortBy, sortDir);
        return Json(sortedFoods);
    }
}