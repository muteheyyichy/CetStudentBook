using Microsoft.AspNetCore.Mvc;
using CetStudentBook.Data;
using CetStudentBook.Models;
using System.Linq;

namespace CetStudentBook.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var books = _context.Books.ToList();
            return View(books);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // --- YENİ EKLENEN KISIMLAR ---

        // DÜZENLEME SAYFASINI AÇMA
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // DÜZENLENEN VERİYİ KAYDETME
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // SİLME ONAY SAYFASINI AÇMA
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // SİLME İŞLEMİNİ GERÇEKLEŞTİRME
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}