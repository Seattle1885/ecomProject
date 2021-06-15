using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Client.Models;

namespace Client.Controllers
{
    public class HomeController : Controller
    {

        private MyContext db_context;

        public HomeController (MyContext context)
        {
            db_context = context;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("Session has beeen cleared");
            return View();
        }

        [HttpGet("deleteSession")]
        public IActionResult Logout()
        
        {
            HttpContext.Session.Clear();
            Console.WriteLine("Session has beeen cleared");
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                var userEmailDB = db_context.Users.FirstOrDefault(userdb => userdb.Email == newUser.Email);
                if (userEmailDB != null) // checks email in the database
                {
                    ModelState.AddModelError("Email", "Email is taken");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                db_context.Add(newUser);
                db_context.SaveChanges();
                
                var userInDB = db_context.Users.SingleOrDefault(u => u.UserId == newUser.UserId);
                
                HttpContext.Session.SetInt32("LogInUser", userInDB.UserId);

                Console.WriteLine($"New User {newUser.FirstName} Created and session was started");
                return RedirectToAction("Products");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("login")]
        public IActionResult LoginProcess(LUser loginAttemp)
        {
            if (ModelState.IsValid)
            {   
                    //user in DB email to comapare to login attemp email
                var userInDB = db_context.Users.FirstOrDefault(userdb => userdb.Email == loginAttemp.LEmail);
                
                if (userInDB == null)
                {
                    ModelState.AddModelError("LEmail","Invalid Email/Password");
                    
                    return View("Index");
                }
    
                var hasher = new PasswordHasher<LUser>();
                var result = hasher.VerifyHashedPassword(loginAttemp, userInDB.Password, loginAttemp.LPassword);
                // verify provided password against hash stored in db if 0 fail
                if (result == 0)
                {
                    ModelState.AddModelError("LPassword","Invalid Email/Password");
                    
                    return View("Index");
                }

                HttpContext.Session.SetInt32("LogInUser",userInDB.UserId);
                Console.WriteLine("Session Created***************************");
                
                return RedirectToAction("Products");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("welcome")]
        public IActionResult Welcome()
        {
            return View();
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser"); 
                Console.WriteLine($"logInUser Id number is {LogInUser} and you are in Products");
                // LogInUser ID number is displayed ex. 1 

            if (LogInUser == null)
            {
                Console.WriteLine("You are not in session");
                return RedirectToAction("Index");
            }

            ViewBag.LoginUser = db_context.Users
                .SingleOrDefault(User => User.UserId == LogInUser);
            ViewBag.AllProducts = db_context.Products
                .Include(user => user.UserBuyers)
                .Include(u => u.Creator)
                .OrderByDescending(u => u.CreatedAt)// display recent to oldest
                .ToList();

            return View();
        }

        [HttpPost("productProcess")]
        public IActionResult ProductProcess(Product newProduct)
        {
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser");

            if (LogInUser == null)
                {
                    Console.WriteLine("You are not in session");
                    return RedirectToAction("Index");
                }

            if (ModelState.IsValid)
            {
                newProduct.UserId =(int)LogInUser; //string "1"  into a 1
               // string stringFileName = UploadFile(newProduct);
            
                db_context.Add(newProduct);
                db_context.SaveChanges();

                return RedirectToAction("Products",newProduct);
            }
            else
            {
                ModelState.AddModelError("ProductName","2 Characters min");
                Console.WriteLine("Model is inValid");
                
                return RedirectToAction("Products");
            }
        }

        [HttpGet("delete/{SelectedProductId}")]
        public IActionResult DeleteProduct(int SelectedProductId)
        {
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser");
            
                Console.WriteLine("******************************************************");
                Console.WriteLine("Line 149 You have reached the DeleteProduct function");
                Console.WriteLine($"Line 150 selected Product #{SelectedProductId} to delete  and LogInUser is {LogInUser}");
            
            var productInDB = db_context.Products
                .SingleOrDefault(product => product.ProductId == SelectedProductId);
                Console.WriteLine($"This is the product in the database {productInDB}");
            
            db_context.Remove(productInDB);
            db_context.SaveChanges();

            return RedirectToAction("Products");
        }


        [HttpGet("editPage/{ProductId}")]
        public IActionResult Edit(int ProductId)
        {   
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser");
            
            if (LogInUser == null)
            {
                Console.WriteLine("You are not in session");
                return RedirectToAction("Index");
            }
            else
            {            
            ViewBag.LoginUser = db_context.Users
                .SingleOrDefault(User => User.UserId == LogInUser);

            ViewBag.productDescription = db_context.Products
                .SingleOrDefault(product => product.ProductId == ProductId)
                .Description;
            
            Console.WriteLine($" product description : {ViewBag.productDescription}");

            var productInDB = db_context.Products
                .SingleOrDefault(product => product.ProductId == ProductId);
        
            ViewBag.Product = productInDB;
            Console.WriteLine("ViewBag.Product.Description is :",ViewBag.Product.Description);
            return View("Edit");
            }
        }

        [HttpPost("UpdateProduct/{productId}")]
        public IActionResult UpdateProduct(Product editProduct,int productId)
        {
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser");
            ViewBag.LoginUser = db_context.Users
                .SingleOrDefault(User => User.UserId == LogInUser);
            
            var productInDB = db_context.Products
                .SingleOrDefault(product => product.ProductId == productId);
        
            ViewBag.Product = productInDB;

            if (ModelState.IsValid)
            {
            var oldProduct = db_context.Products.FirstOrDefault(product => product.ProductId == productId);

            oldProduct.ProductName = editProduct.ProductName;
            oldProduct.Description = editProduct.Description;
            oldProduct.Quantity = editProduct.Quantity;
            oldProduct.UpdatedAt = DateTime.Now;
            
            db_context.SaveChanges();

            return RedirectToAction ("Products");
            }
            else
            {
                return View("Edit");
            }
        }

        [HttpGet("purchase/{SelectedProductId}")]
        public IActionResult PurchaseProduct(int SelectedProductId)
        {

            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser");

            if (LogInUser == null)
                {
                    Console.WriteLine("You are not in session");
                    return RedirectToAction("Index");
                }

            // Query single Product from Context object to track changes.
            var productInDB = db_context.Products
                .SingleOrDefault(product => product.ProductId == SelectedProductId);
                Console.WriteLine($"Line 199 This is the product in the database {productInDB}");

            // Then modify properties of tracked model object
                Console.WriteLine($"Line 202 productInDb before value change is {productInDB}");

            if ( productInDB.Quantity == 0)
            {
                
                return RedirectToAction("Products");
            }
            else
            {
            productInDB.Quantity -= 1; // removes 1 from the Quanitity in the DB
                Console.WriteLine($"Line 203 above is the results of productInBd.Quantity -= 1 {productInDB.Quantity}");
            productInDB.UpdatedAt = DateTime.Now;
                Console.WriteLine($"Line 206 this is the UpdatedAt time {productInDB.UpdatedAt}");

            // Finally, .SaveChanges() will update the DB with these new values
            db_context.SaveChanges();

            Buyer addBuyer = new Buyer(); //instance of a buyer, Now we need ProductId & UserId

            addBuyer.UserId =(int)LogInUser; //adding LoginUser Id to fullfill requirements
            addBuyer.ProductId = SelectedProductId; //adding ProductID to fullfill requirements    
                Console.WriteLine($"addBuyer is :{addBuyer}");// we now created a buyer now add it to table db 

            db_context.Add(addBuyer);
            db_context.SaveChanges();      
                Console.WriteLine($" addBuyer Id is : #{addBuyer.BuyerId}");
            
            return RedirectToAction("Orders");
            }
        }

       [HttpGet("/orders")]
        public async Task<IActionResult>Orders(string sortOrder)
        {
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser"); 
                Console.WriteLine($"logInUser Id number is {LogInUser} and you are in Orders");
                // LogInUser ID number is displayed ex. 1 
            if (LogInUser == null)
            {
                Console.WriteLine("You are not in session");
                return RedirectToAction("Index");
            }

                ViewBag.buyer = db_context.Buyers
                    .ToList();

                ViewBag.AllOrders = db_context.Buyers
                    .Include(u => u.Users)
                    .Include(p => p.Products)
                    .Include(s => s.Products.Creator)
                    .ToList();

                Console.WriteLine($"ViewBag.buyer is : {ViewBag.buyer}");

                Console.WriteLine($"ViewBag.AllOrders is : {ViewBag.AllOrders}");

                ViewBag.LoginUser = db_context.Users
                    .SingleOrDefault(User => User.UserId == LogInUser);
            
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            var products = from s in db_context.Products select s;

            switch(sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "Date":
                    products = products.OrderBy(p => p.UpdatedAt);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(p => p.UpdatedAt);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            return View(await products.AsNoTracking().ToListAsync());
        }

        [HttpGet("seeMore")]
        public IActionResult SeeMore()
        {

            return RedirectToAction("Orders");
        }


       [HttpGet("/customers")]
        public IActionResult Customers()
        {
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser"); 
            Console.WriteLine($"logInUser Id number is {LogInUser} and you are in Customers");
            // LogInUser ID number is displayed ex. 1 

            if (LogInUser == null)
            {
                Console.WriteLine("You are not in session line 142");
                return RedirectToAction("Index");
            }

            ViewBag.LoginUser = db_context.Users.SingleOrDefault(User => User.UserId == LogInUser);
            Console.WriteLine($"line 147{ViewBag.LoginInUser}"); // e_commerce.Models.User
            return View();
        }

        
       [HttpGet("settings")]
        public IActionResult Settings()
        {
            int ? LogInUser = HttpContext.Session.GetInt32("LogInUser");
            Console.WriteLine($"logInUser Id number is {LogInUser} and you are in Settings");

            if (LogInUser == null)
            {
                Console.WriteLine("You are not in session");
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
