using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TestQuit.Models;
using TestQuit.Service;

public class FoodController : Controller
{
    private readonly IFood _foodService;

    public FoodController(IFood foodService)
    {
        _foodService = foodService;
    }

    // GET: Food/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: Food/Create
    [HttpPost]
    public ActionResult Create(Food food)
    {
        if (ModelState.IsValid)
        {
            var message = _foodService.Create(food);
            TempData["Message"] = message;
            return RedirectToAction("Index");
        }
        return View(food);
    }

    // GET: Food/Edit/{productId}
    public ActionResult Edit(string productId)
    {
        var foodList = _foodService.Search(productId);
        var food = foodList.Count > 0 ? foodList[0] : null;

        if (food == null)
            return View();

        return View(food);
    }

    // POST: Food/Edit
    [HttpPost]
    public ActionResult Edit(Food food)
    {
        if (ModelState.IsValid)
        {
            var message = _foodService.Edit(food);
            TempData["Message"] = message;
            return RedirectToAction("Index");
        }
        return View(food);
    }

    // POST: Food/Delete/{productId}
    [HttpPost]
    public ActionResult Delete(string productId)
    {
        var message = _foodService.Delete(productId);
        TempData["Message"] = message;
        return RedirectToAction("Index");
    }

    // GET: Food/Sort?product=xxx
    public ActionResult Sort(string product)
    {
        var sortedList = _foodService.Sort(product);
        return View("Index", sortedList);
    }

    // GET: Food/Search?product=xxx
    public ActionResult Search(string product)
    {
        var searchResult = _foodService.Search(product);
        return View("Index", searchResult);
    }

    // GET: Food/Filter?date=yyyy-MM-dd
    public ActionResult Filter(string date)
    {
        var filterResult = _foodService.fillter(date);
        return View("Index", filterResult);
    }
    public ActionResult Index()
    {
        var allFoods = _foodService.Sort(""); // หรือดึงข้อมูลทั้งหมด
        return View(allFoods);
    }
}
