using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsWear.Filters.CustomerSessionFilter;
using SportsWear.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SportsWear.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SportsWearContext _context;
        public HomeController(ILogger<HomeController> logger, SportsWearContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            var customerId = HttpContext.Session.GetString("customerId");
            if (customerId != null)
            {
                var result = CustomersCartItems();
                int totalCartItems = result.Count;
                HttpContext.Session.SetString("totalCartItems", totalCartItems.ToString());
            }
            ViewBag.Categories = _context.Categories.OrderBy(x=>x.CategoryId).ToList();
            if (id != null)
            {
                var _categoryContext = _context.Categories.Find(id);
                if (_categoryContext != null)
                {
                    ViewBag.categoryName = _categoryContext.CategoryName;
                    var _contextProducts = _context.Products.Include(p => p.FkCategory).Where(x => x.FkCategoryId == id);
                    return View(_contextProducts.ToList());
                }
            }
            else
            {
                var _contextProducts = _context.Products.Include(p => p.FkCategory);
                return View(_contextProducts.ToList());
            }
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var sportsWearContext = _context.Products.Include(p => p.FkCategory);
            return View(await sportsWearContext.ToListAsync());
        }

        [ClassCustomerSessionFilter]
        public  IActionResult Cart()
        {
            var result = CustomersCartItems();
            int totalCartItems=result.Count;
            HttpContext.Session.SetString("totalCartItems", totalCartItems.ToString());
            return View(result);
        }
        [HttpPost]
        [ClassCustomerSessionFilter]
        public  IActionResult AddToCart(int productId, int productQty)
        {
            var customerId =HttpContext.Session.GetString("customerId");
            var _contextProduct = _context.Products.Find(productId);
            if (_contextProduct?.ProductQty < productQty)
            {
                TempData["itemOutOffStock"] = "Hi, item is out of stock";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var _contextCart = _context.Carts.Where(x => x.FkProductId == productId && x.FkCustomerId==Int32.Parse(customerId)).FirstOrDefault();
                if (_contextCart != null)
                {
                    _contextCart.TotalPrice = _contextCart.TotalPrice+( _contextProduct!.ProductPrice * productQty);
                    _contextCart.Qty = _contextCart.Qty + productQty;
                    _context.Carts.Update(_contextCart);
                    _contextProduct.ProductQty = _contextProduct.ProductQty - productQty;
                    _context.Products.Update(_contextProduct);
                    _context.SaveChanges();
                }
                else
                {
                    var data = new Cart();
                    data.FkCustomerId = Int32.Parse(customerId);
                    data.FkProductId = productId;
                    data.Qty = productQty;
                    data.TotalPrice = _contextProduct.ProductPrice * productQty;
                    _context.Carts.Add(data);
                    _contextProduct.ProductQty = _contextProduct.ProductQty - productQty;
                    _context.Products.Update(_contextProduct);
                    _context.SaveChanges();
                }
                TempData["itemAddedIntoCart"] = "Product is added into cart";
                return RedirectToAction("Index", "Home");
            }
        }
        [ClassCustomerSessionFilter]

        public IActionResult RemoveFromCart(int id)
        {
            var _contextCart = _context.Carts.Find(id);
            var _contextProduct = _context.Products.Find(_contextCart.FkProductId);
            _contextProduct.ProductQty = _contextProduct.ProductQty + _contextCart.Qty;
            _context.Products.Update(_contextProduct);
            _context.Carts.Remove(_contextCart);
            _context.SaveChanges();
            return RedirectToAction("Cart", "Home");
        }

        public IActionResult DecreaseQuantity(int id)
        {
            var _contextCart = _context.Carts.Find(id);
            var _contextProduct = _context.Products.Find(_contextCart.FkProductId);
            if (_contextCart.Qty<2)
            {
                TempData["cartProductQntyCount"] = "Hi, quantity can not be decrease, minimum should be 1 for product";
            }
            else
            {
                _contextProduct.ProductQty = _contextProduct.ProductQty + 1;
                _contextCart.Qty = _contextCart.Qty - 1;
                _contextCart.TotalPrice = _contextCart.TotalPrice - _contextProduct.ProductPrice;
                _context.Products.Update(_contextProduct);
                _context.Carts.Update(_contextCart);
                _context.SaveChanges();
            }
            
            return RedirectToAction("Cart", "Home");
        }
        public IActionResult IncreaseQuantity(int id)
        {
            var _contextCart = _context.Carts.Find(id);
            var _contextProduct = _context.Products.Find(_contextCart.FkProductId);
            if (_contextProduct.ProductQty>0)
            {
                _contextProduct.ProductQty = _contextProduct.ProductQty - 1;
                _contextCart.Qty = _contextCart.Qty + 1;
                _contextCart.TotalPrice = _contextCart.TotalPrice + _contextProduct.ProductPrice;
                _context.Products.Update(_contextProduct);
                _context.Carts.Update(_contextCart);
                _context.SaveChanges();
            }
            else
            {
                TempData["itemOutOffStock"] = "Hi, item is out of stock";
            }

            return RedirectToAction("Cart", "Home");
        }
        [HttpGet]
        [ClassCustomerSessionFilter]
        public IActionResult Checkout()
        {
            var result = CustomersCartItems();
            ViewBag.checkOutItems = result;
            return View();
        }

        public IActionResult Process(Order order, string chargeTotalAmount, string stripeToken, string stripeEmail)
        {
            var optionsCust = new Stripe.CustomerCreateOptions
            {
                Email = stripeEmail,
                Name = order.FullName,
                Phone = order.PhoneNumber

            };
            var serviceCust = new Stripe.CustomerService();
     
            Stripe.Customer customer = serviceCust.Create(optionsCust);
            var optionsCharge = new Stripe.ChargeCreateOptions
            {
                Amount = long.Parse(chargeTotalAmount),
                Currency = "CAD",
                Description = "Buying Sports wear products",
                Source = stripeToken,
                ReceiptEmail = stripeEmail,

            };
            var service = new Stripe.ChargeService();
            Stripe.Charge charge = service.Create(optionsCharge);
            if (charge.Status == "succeeded")
            {
                var customerId = Int32.Parse(HttpContext.Session.GetString("customerId"));
                double subTotals = _context.Carts.Where(c => c.FkCustomerId == customerId)
                .Select(i => Convert.ToDouble(i.TotalPrice)).Sum();
                order.FkCustomerId = customerId;
                order.TotalPrice = subTotals;
                order.OrderDate = DateTime.Now.ToString();
                order.OrderStatus = 0;
                _context.Orders.Update(order);
                _context.SaveChanges();
                var _contextCart = _context.Carts.Where(x => x.FkCustomerId == customerId).ToList();
                foreach (var items in _contextCart)
                {
                    var itemPrice = _context.Products.Find(items.FkProductId);
                    var orderDetail = new OrderDetail();
                    orderDetail.FkOrderId = order.OrderId;
                    orderDetail.FkProductId = items.FkProductId;
                    orderDetail.Qty = items.Qty;
                    orderDetail.Price = itemPrice.ProductPrice;
                    _context.OrderDetails.Add(orderDetail);
                    _context.SaveChanges();
                    _context.Carts.Remove(items);
                    _context.SaveChanges();
                }
                TempData["checkoutConfirm"] = "Your order has been confirmed successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["checkoutConfirm"] = "Something went wrong, while processing with payment";
                return RedirectToAction("Index", "Home");
            }

        }



        [HttpPost]
        [ClassCustomerSessionFilter]
        public IActionResult Checkout(Order order, string chargeTotalAmount, string stripeToken, string stripeEmail)
        {
            return Process(order, chargeTotalAmount,stripeToken, stripeEmail);
        }

        public List<CustomerCartItems> CustomersCartItems()
        {
            var customerId = HttpContext.Session.GetString("customerId");
            var result = (from products in _context.Products
                          join cart in _context.Carts
                          on products.ProductId equals cart.FkProductId
                          join customer in _context.Customers on cart.FkCustomerId equals customer.CustomerId
                          where customer.CustomerId == Int32.Parse(customerId)
                          select new CustomerCartItems
                          {
                              cartId = cart.CartId,
                              customerId = customer.CustomerId,
                              productId = products.ProductId,
                              productName = products.ProductName,
                              productPrice = products.ProductPrice,
                              cartQty = cart.Qty,
                              productTotalPrice = cart.TotalPrice

                          }).ToList();
            return result;
        }
        [ClassCustomerSessionFilter]
        public IActionResult MyOrders()
        {
            var customerId = Int32.Parse(HttpContext.Session.GetString("customerId"));
            var result = _context.Orders.Where(x => x.FkCustomerId == customerId).OrderByDescending(x=>x.OrderId).ToList();
            return View(result);
        }
        [ClassCustomerSessionFilter]
        public IActionResult CancelOrder(int id)
        {
            var _contextOrder = _context.Orders.Find(id);
            _contextOrder.OrderStatus = 3;
            _context.Orders.Update(_contextOrder);
            _context.SaveChanges();
            var result = (from orders in _context.Orders
                          join order_detail
                          in _context.OrderDetails on orders.OrderId equals
                          order_detail.FkOrderId
                          where orders.OrderId == id
                          select new
                          {
                              productId=order_detail.FkProductId,
                              productQty=order_detail.Qty
                          }).ToList();
            foreach(var item in result)
            {
                var _contextProductQty = _context.Products.Where(x => x.ProductId == item.productId).FirstOrDefault();
                _contextProductQty.ProductQty = _contextProductQty.ProductQty + item.productQty;
                _context.SaveChanges();
            }
            TempData["orderCancelled"] = "Your order has been cancelled";
            return RedirectToAction("MyOrders");
        }


        // GET: Home/OrderDetails/5
        public IActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order_details = (from orders in _context.Orders
                                 join order_detail
                                 in _context.OrderDetails on orders.OrderId equals
                                 order_detail.FkOrderId
                                 join product in _context.Products on order_detail.FkProductId equals product.ProductId
                                 where orders.OrderId == id
                                 select new Product
                                 {
                                     ProductId = product.ProductId,
                                     ProductName = product.ProductName,
                                     ProductPrice = product.ProductPrice,
                                     ProductQty = order_detail.Qty
                                 }).ToList();

            return View(order_details);
        }

        [HttpPost]
        public IActionResult Search(string searchQuery)
        {
            var result=_context.Products.Where(x => EF.Functions.Like(x.ProductName, $"%{searchQuery}%")).ToList();
            ViewBag.searchQuery = searchQuery;
            return View(result);
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
