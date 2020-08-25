﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gosh.Models;

namespace Gosh.Controllers
{
    class QueryResult
    {
        public string CategoryName { get; set; }
        public int NumOfRecipes { get; set; }
        public int year { get; set; }
        public int recipes { get; set; }
    }
    public class GraphController : Controller
    {
        private MyDB db = new MyDB();

        // GET: Graph
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult GetAllCategories()
        {

            var query = db.Database.SqlQuery<QueryResult>(
                "select CategoryName, (select count(RecipeId) from Recipes r where r.CategoryId = c.ID) as NumOfRecipes from Categories c order by c.CategoryName").ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecipesByYears()
        {
            var yearsQuery = db.Database.SqlQuery<QueryResult>(
                "SELECT YEAR(DateCreated) as year, COUNT(*) as recipes FROM Recipes GROUP BY YEAR(DateCreated)").ToList();
            return Json(yearsQuery, JsonRequestBehavior.AllowGet);
        }
    }
}