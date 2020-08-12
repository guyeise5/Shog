﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Gosh.Models;

namespace Gosh.Controllers
{
    public class UserController : Controller
    {
        private MyDB db = new MyDB();

        // GET: User
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // Get: User/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "ID,Username,FirstName,LastName,Mail,PhoneNumber")] User user, [Bind(Include ="Password")] string password, [Bind(Include ="ConfirmPassword")] string ConfirmPassword)
        {
            Password pwd = Password.Create(password);
            pwd.User = user;
            pwd.UserID = user.ID;

            if (!ValidatePassword(password))
            {
                ModelState.AddModelError("Password", "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character");
            }
            if(password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords does not match");
            }

            if(db.Users.Where<User>(u => u.Username ==  user.Username).ToList<User>().Count != 0)
            {
                ModelState.AddModelError("Username","Username is aleardy exists");
            }
            if(db.Users.Where<User>(u => u.Mail == user.Mail).ToList<User>().Count != 0)
            {
                ModelState.AddModelError("Mail", "This mail address is aleady in use");
            }

             user.Username = user.Username.ToUpper();
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.Passwords.Add(pwd);
                db.SaveChanges();
                return Login(user.Username, password);
            }

            return View(user);
        }

        private bool ValidatePassword(string password)
        {
            Regex regex = new Regex(@"^(?=.*?[A - Z])(?=.*?[a - z])(?=.*?[0 - 9])(?=.*?[#?!@$%^&*-]).{8,}$");
            return regex.IsMatch(password);

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username")] string username, [Bind(Include = "Password")] string password)
        {
            try
            {
                User usr = db.Users.Where<User>(u => u.Username == username).ToList<User>()[0];
                Password pwd = db.Passwords.Where<Password>(p => p.UserID == usr.ID).ToList<Password>()[0];
                if (Password.Check(password, pwd))
                {
                    Session.Add("Username", usr.Username);
                    Session.Add("Userid", usr.ID);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Password", "Password is incorrect, try again");
                    return Login();
                }
            }
            catch
            {
                ModelState.AddModelError("Username", "Username does not exist");
                return Login();
            }
        }
        // GET: User/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Username,Password,FirstName,LastName,Mail,CreditCard,PhoneNumber")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
