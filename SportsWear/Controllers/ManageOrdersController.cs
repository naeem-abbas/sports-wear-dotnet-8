using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsWear.Filters.AdminSessionFilter;
using SportsWear.Models;

namespace SportsWear.Controllers
{
    [ClassAdminSessionFilter]
    public class ManageOrdersController : Controller
    {
        private readonly SportsWearContext _context;

        public ManageOrdersController(SportsWearContext context)
        {
            _context = context;
        }

        // GET: ManageOrders
        public async Task<IActionResult> Index()
        {
            var sportsWearContext = _context.Orders.Include(o => o.FkCustomer);
            return View(await sportsWearContext.ToListAsync());
        }

        // GET: ManageOrders/Details/5
        public IActionResult Details(int? id)
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
                                     ProductId=product.ProductId,
                                     ProductName = product.ProductName,
                                     ProductPrice = product.ProductPrice,
                                     ProductQty = order_detail.Qty
                                 }).ToList();

            return View(order_details);
        }


        // GET: ManageOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: ManageOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,FullName,PhoneNumber,AddressDetail,OrderStatus")] Order order)
        {
            var _contextOrder = _context.Orders.Find(id);
            _contextOrder.FullName = order.FullName;
            _contextOrder.PhoneNumber = order.PhoneNumber;
            _contextOrder.AddressDetail = order.AddressDetail;
            _contextOrder.OrderStatus = order.OrderStatus;
            if (order.OrderStatus == 3)
            {
                var result = (from orders in _context.Orders
                              join order_detail
                              in _context.OrderDetails on orders.OrderId equals
                              order_detail.FkOrderId
                              where orders.OrderId == id
                              select new
                              {
                                  productId = order_detail.FkProductId,
                                  productQty = order_detail.Qty
                              }).ToList();
                foreach (var item in result)
                {
                    var _contextProductQty = _context.Products.Where(x => x.ProductId == item.productId).FirstOrDefault();
                    _contextProductQty.ProductQty = _contextProductQty.ProductQty + item.productQty;
                    _context.SaveChanges();
                }
            }
            _context.Orders.Update(_contextOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ManageOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.FkCustomer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: ManageOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
