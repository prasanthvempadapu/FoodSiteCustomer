using Microsoft.AspNetCore.Mvc;
using Practice_CoreApp_04_08.Filter;
using Practice_CoreApp_04_08.Models;
using Serilog;
using System.Data.SqlClient;

namespace Practice_CoreApp_04_08.Controllers
{
    public class FoodSiteController : Controller
    {
        static List<Login> users = new List<Login>();
        static string login = "done";
        static string order="item";
        static string log = "login";
        static string name = "";
        
        //private readonly ILogger<FoodSiteController> _logger;

        
        public FoodSiteController()
        {
            //_logger = logger;
            SqlConnection conn = new SqlConnection("Data Source=PSL-FL527L3;Initial Catalog=DEMO;Integrated Security=True;");
            SqlCommand cmd = new SqlCommand("select * from users", conn);
            conn.Open();
            SqlDataReader sr = cmd.ExecuteReader();
            while (sr.Read())
            {
                Login user = new Login(sr["UserName"].ToString(), sr["Password"].ToString());
                users.Add(user);
            }
            conn.Close();
            Console.WriteLine("");
            Console.WriteLine("Constructor Implemented");

            ViewBag.Message = "";
        }
       
        public IActionResult CreateAccount()
        {
            return View(new NewUser());
        }
        [CustomExceptionFilter]
        [HttpPost]
        public IActionResult CreateAccount(NewUser n)
        {
            if (ModelState.IsValid)
            {
                var u = users.Find(e=>e.UserName == n.UserName);
                
                if (n.Password == n.ConfirmPassword && u==null)
                {
                    SqlConnection conn2 = new SqlConnection("Data Source=PSL-FL527L3;Initial Catalog=DEMO;Integrated Security=True;");
                    SqlCommand cmd2 = new SqlCommand(String.Format("insert into users values('{0}','{1}')", n.UserName, n.Password), conn2);
                    conn2.Open();
                    cmd2.ExecuteNonQuery();
                    conn2.Close();
                    return RedirectToAction("Created");
                }
                return RedirectToAction("NotCreated");
            }
            return RedirectToAction("NotCreated");
        }
        public IActionResult Created()
        {
            return View();
        }
        public IActionResult NotCreated()
        {
            return View();
        }
        public IActionResult Index()
        {
            ViewBag.login = login;
            log = "login";
            if(name!="")
            {
                Log.Information(String.Format("{0} Logged Out", name));
            }
            return View(new Login());
        }
        
        [HttpPost]
        public IActionResult Display(Login l)
        {
            

            if (ModelState.IsValid)
            {
                login = "done";
                foreach (var i in users)
                {

                    if (i.UserName == l.UserName && i.Password == l.Password)
                    {
                        ViewBag.Message = i.UserName.ToString();
                        if (log == "login")
                        {
                            Log.Information(String.Format("{0} Logged In", l.UserName.ToString()));
                        }
                        name= i.UserName;
                        log = "no";
                       
                       
                        
                        return View(new Order());
                        


                    }
                }
                name = "";
                Log.Error("Invalid User Details Entered ");
            }
            login = "";
            return RedirectToAction("Index");
        }
        
        public IActionResult ConfirmOrder(Order o)
        {
            Login lg = users.Find(e => e.UserName == o.UserName);
            if (ModelState.IsValid)
            {
                
                ViewBag.order = "item";
                order = "item";
                return View(o);
            }



            ViewBag.order = "";
            order = "";
            
        
            return View(o);
        }
        [CustomExceptionFilter]
        public IActionResult Order(Order o,string cancel)
        {
            //Console.WriteLine(cancel);
           
            Login lg = users.Find(e => e.UserName == o.UserName);
            if (order=="item" && cancel!="Cancel")
            {
                SqlConnection conn3 = new SqlConnection("Data Source=PSL-FL527L3;Initial Catalog=DEMO;Integrated Security=True;");
                SqlCommand cmd3 = new SqlCommand(String.Format("insert into orders values('{0}','{1}','{2}',{3},'{4}')", o.UserName, o.OrderName, o.Quantity,o.Price ,o.Address), conn3);
                conn3.Open();
                cmd3.ExecuteNonQuery();
                conn3.Close();
                ViewBag.order = "item";
                Order ol = new Order();

                SqlConnection conn4 = new SqlConnection("Data Source=PSL-FL527L3;Initial Catalog=DEMO;Integrated Security=True;");
                SqlCommand cmd4 = new SqlCommand(String.Format("select * from orders where userName='{0}'",o.UserName ), conn4);
                conn4.Open();
                SqlDataReader sr4 = cmd4.ExecuteReader();
                
                while (sr4.Read())
                {
                    ol = new Order(Convert.ToInt32(sr4["Id"]), sr4["userName"].ToString(), sr4["OrderName"].ToString(), sr4["Quantity"].ToString(), Convert.ToInt32(sr4["Price"]), sr4["Address"].ToString());
                    
                }


                Log.Debug(String.Format("Order placed by {0} with order Id: {1} ",o.UserName, ol.Id));
                return View(lg);
                
            }



            ViewBag.order = "";
            if(cancel=="Cancel")
            {
                ViewBag.order = cancel;
            }
            return View(lg);
        }
        public IActionResult History(string user)
        {
            ViewBag.user = user;
            return View(new Login());
        }
        public IActionResult Cancel(int id)
        {
            Orders o =new Orders();
            SqlConnection con = new SqlConnection("Data Source=PSL-FL527L3;Initial Catalog=DEMO;Integrated Security=True;");
            SqlCommand cm = new SqlCommand(String.Format("select * from orders where Id={0}", id), con);
            con.Open();
            SqlDataReader s = cm.ExecuteReader();

            while (s.Read())
            {
                o = new Orders(Convert.ToInt32(s["Id"]), s["userName"].ToString(), s["OrderName"].ToString(), s["Quantity"].ToString(), Convert.ToInt32(s["Price"]), s["Address"].ToString(),"Cancelled");

            }

            SqlConnection con2 = new SqlConnection("Data Source=PSL-FL527L3;Initial Catalog=DEMO;Integrated Security=True;");
            SqlCommand cm2 = new SqlCommand(String.Format("Delete from orders where Id={0}",id), con2);
            con2.Open();
            cm2.ExecuteNonQuery();
            con2.Close();

            SqlConnection con3 = new SqlConnection("Data Source=PSL-FL527L3;Initial Catalog=DEMO;Integrated Security=True;");
             SqlCommand cm3 = new SqlCommand(String.Format("insert into CompletedOrders values({0},'{1}','{2}','{3}',{4},'{5}','{6}')",o.Id, o.UserName, o.OrderName, o.Quantity, o.Price, o.Address,o.Status), con3);
             con3.Open();
             cm3.ExecuteNonQuery();
             con3.Close();


            return View(o);
        }
    }
}
