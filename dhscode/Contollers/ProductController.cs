using DHSOnlineStore.Data;
using DHSOnlineStore.ImageService;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _repository;
        private readonly IFileService _fileService;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, IProductRepository repository, IFileService fileService) : base(context, userManager, contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<IActionResult> Products(string sTerm = "", string category = "")
        {
            var products = await _repository.GetProducts(sTerm, category);

            return View(products);
        }

        public async Task<IActionResult> Index()
        {
            var products = await _repository.GetProducts();

            return View(products);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return View(product);
        }

        public IActionResult AddProduct()
        {
            Product product = new Product();
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
                return View(product);

            try
            {
                if (product.ImageFile != null)
                {
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png", ".jfif"];
                    string imageName = await _fileService.SaveFile(product.ImageFile, allowedExtensions);
                    product.Image = imageName;
                }
                else
                {
                    // Handle the case where the image is not provided (e.g., keep the current image)
                    ModelState.AddModelError("ImageFile", "Please upload an image.");
                    return View(); // Return to the edit view with the error message
                }
                Product prod = new Product()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Brand = product.Brand,
                    Image = product.Image,
                    Price = product.Price,
                    Category = product.Category,
                    Description = product.Description,
                };



                await _context.Products.AddAsync(prod);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = "Product added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Product not added succesfully!";
                Console.WriteLine(ex.ToString());
                return View(product);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if (ModelState.IsValid)
                return View(product);

            try
            {


                if (product.ImageFile != null)
                {
                    if (string.IsNullOrWhiteSpace(product.Image) == false)
                    {
                        _fileService.DeleteFile(product.Image);
                    }

                    string[] allowedExtensions = [".jpeg", ".jpg", ".png", ".jfif"];

                    string imageName = await _fileService.SaveFile(product.ImageFile, allowedExtensions);

                    product.Image = imageName;
                }
                //else
                //{
                //    ModelState.AddModelError("ImageFile", "Please upload an image.");
                //    return View(); // Return to the edit view with the error message
                //}
                Product prod = new Product()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Brand = product.Brand,
                    Image = product.Image,
                    Price = product.Price,
                    Category = product.Category,
                    Description = product.Description,
                };

                _context.Products.Update(prod);

                await _context.SaveChangesAsync();



                TempData["successMessage"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Product not updated succesfully!";
                Console.WriteLine(ex.ToString());
                return View(product);
            }
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(Product product)
        {
            try
            {

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                if (!string.IsNullOrWhiteSpace(product.Image))
                {
                    _fileService.DeleteFile(product.Image);
                }
                TempData["successMessage"] = "Product successfully deleted";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = "Product not deleted succesfully!";
                Console.WriteLine(ex.ToString());
                return View("Index");
            }
        }
    }
}
