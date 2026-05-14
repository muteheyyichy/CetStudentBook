namespace CetStudentBook.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        // YENİ EKLENEN SATIR BURASI (Soru işareti boş bırakılabilir demek)
        public string? ImageUrl { get; set; }
    }
}