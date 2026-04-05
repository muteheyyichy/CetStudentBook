using CetStudentBook.Data;
using CetStudentBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CetStudentBook.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        
        public IActionResult AddToCart(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                var cart = GetCart();
                var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity++; 
                }
                else
                {
                    
                    cart.Add(new CartItem { ProductId = product.Id, ProductName = product.Name, Price = product.Price, Quantity = 1 });
                }
                SaveCart(cart); 
            }
            return RedirectToAction("Index");
        }

       
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction("Index"); 

            
            var order = new Order
            {
                UserId = User.Identity.Name ?? "Bilinmiyor", 
                OrderDate = System.DateTime.Now,
                OrderItems = new List<OrderItem>()
            };

           
            foreach (var item in cart)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            
            _context.Orders.Add(order);
            _context.SaveChanges();

           
            HttpContext.Session.Remove("Cart");

            
            return RedirectToAction("MyOrders", "Order");
        }

       
        private List<CartItem> GetCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (sessionCart == null) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(sessionCart);
        }

        private void SaveCart(List<CartItem> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", json);
        }
    }
}