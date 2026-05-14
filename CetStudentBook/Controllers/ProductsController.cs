using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CetStudentBook.Data;
using CetStudentBook.Models;

// YENİ EKLENEN KÜTÜPHANELER (Resim işlemleri ve dosya yönetimi için)
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CetStudentBook.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,CategoryId,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
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
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // DİKKAT 1: Bind kısmına "ImageUrl" eklendi.
        // DİKKAT 2: Parametrelerin sonuna bilgisayardan gelecek dosyayı tutması için "IFormFile? ImageFile" eklendi.
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CategoryId,ImageUrl")] Product product, IFormFile? ImageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // EĞER KULLANICI YENİ BİR RESİM SEÇTİYSE BU BLOK ÇALIŞIR
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // 1. Yeni resim için benzersiz bir isim oluştur (aynı isimli dosyalar çakışmasın diye)
                        string yeniDosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                        // 2. Resmin kaydedileceği klasör yolunu belirle (wwwroot/images)
                        string resimKlasoru = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                        // Eğer images adında bir klasör yoksa otomatik oluştur
                        if (!Directory.Exists(resimKlasoru))
                        {
                            Directory.CreateDirectory(resimKlasoru);
                        }

                        string tamKayitYolu = Path.Combine(resimKlasoru, yeniDosyaAdi);

                        // 3. Resmi ImageSharp ile aç ve hocanın istediği gibi 1024px kontrolü yap
                        using (var image = Image.Load(ImageFile.OpenReadStream()))
                        {
                            if (image.Width > 1024)
                            {
                                // Genişliği 1024 yap, yüksekliği resmin oranını bozmadan otomatik ayarla
                                image.Mutate(x => x.Resize(1024, 0));
                            }
                            // Resmi yeni haliyle klasöre kaydet
                            image.Save(tamKayitYolu);
                        }

                        // 4. Temizlik Vakti: Eğer ürünün önceden kayıtlı bir resmi varsa, o eski resmi diskten sil
                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            string eskiResimYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", product.ImageUrl);
                            if (System.IO.File.Exists(eskiResimYolu))
                            {
                                System.IO.File.Delete(eskiResimYolu);
                            }
                        }

                        // 5. Ürünün resim yolunu yeni dosyanın adıyla güncelle ki veritabanına bu isim yazılsın
                        product.ImageUrl = yeniDosyaAdi;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}