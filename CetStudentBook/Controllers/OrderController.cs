using CetStudentBook.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CetStudentBook.Controllers
{
    [Authorize] 
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult MyOrders()
        {
            
            var userName = User.Identity.Name ?? "Bilinmiyor";

           
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userName)
                .OrderByDescending(o => o.OrderDate) 
                .ToList();

            return View(orders);
        }
    }
}