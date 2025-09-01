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
    public ActionResult Index()
    {
        var allFoods = _foodService.Sort("");
        return View(allFoods);
    }
    public ActionResult Create()
    {
        return View();
    }
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
    public ActionResult Edit(string productId)
    {
        var foodList = _foodService.Search(productId);
        var food = foodList.Count > 0 ? foodList[0] : null;

        if (food == null)
            return View();

        return View(food);
    }
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

    [HttpPost]
    public ActionResult Delete(string productId)
    {
        var message = _foodService.Delete(productId);
        TempData["Message"] = message;
        return RedirectToAction("Index");
    }

    public ActionResult Sort(string product)
    {
        var sortedList = _foodService.Sort(product);
        return View("Index", sortedList);
    }

    public ActionResult Search(string product)
    {
        var searchResult = _foodService.Search(product);
        return View("Index", searchResult);
    }

    public ActionResult Filter(string date)
    {
        var filterResult = _foodService.fillter(date);
        return View("Index", filterResult);
    }

}
