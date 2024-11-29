using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsWear.Filters.AdminSessionFilter;
using SportsWear.Models;

namespace SportsWear.Controllers
{
    [ClassAdminSessionFilter]
    public class ManageProductsController : Controller
    {
        private readonly SportsWearContext _context;


        private readonly IWebHostEnvironment _hostEnvironment;

      
        public ManageProductsController(SportsWearContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: ManageProducts
        public async Task<IActionResult> Index()
        {
            var sportsWearContext = _context.Products.Include(p => p.FkCategory);
            return View(await sportsWearContext.ToListAsync());
        }

        // GET: ManageProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.FkCategory)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: ManageProducts/Create
        public IActionResult Create()
        {
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: ManageProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,FkCategoryId,ProductName,ProductPrice,ProductQty,ProductSize,ProductGender,ImageFile,ProductDesc")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    //save image to folder wwwroth
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                    string extention = Path.GetExtension(product.ImageFile.FileName);
                    product.ProductImage = fileName = Guid.NewGuid().ToString() + "_" + fileName + extention;
                    string path = Path.Combine(wwwRootPath + "/images/uploads/productImages/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.message = "Please select the image";
                }

            }

            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.FkCategoryId);
            return View(product);
        }

        // GET: ManageProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.FkCategoryId);
            return View(product);
        }

        // POST: ManageProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,FkCategoryId,ProductName,ProductPrice,ProductQty,ProductSize,ProductGender,ProductImage,ImageFile,ProductDesc")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (product.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    var deleteImagepath = Path.Combine(_hostEnvironment.ContentRootPath, wwwRootPath+"/images/", product.ProductImage);

                    if (System.IO.File.Exists(deleteImagepath))
                    {
                        System.IO.File.Delete(deleteImagepath);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                    string extention = Path.GetExtension(product.ImageFile.FileName);
                    product.ProductImage = fileName = Guid.NewGuid().ToString() + "_" +fileName + extention;
                    string path = Path.Combine(wwwRootPath + "/images/uploads/prouductImages/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                }
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.FkCategoryId);
            return View(product);
        }

        // GET: ManageProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.FkCategory)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: ManageProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var deleteImagepath = Path.Combine(_hostEnvironment.ContentRootPath, wwwRootPath + "/images/uploads/prouductImages/", product.ProductImage);

            if (System.IO.File.Exists(deleteImagepath))
            {
                System.IO.File.Delete(deleteImagepath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
