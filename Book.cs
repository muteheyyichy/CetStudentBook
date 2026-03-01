using System;
using System.ComponentModel.DataAnnotations;

namespace CetStudentBook.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap adı zorunludur.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Kitap adı 2 ile 200 karakter arasında olmalıdır.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Yazar adı 2 ile 200 karakter arasında olmalıdır.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Basım tarihi zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "Sayfa sayısı zorunludur.")]
        [Range(1, 10000, ErrorMessage = "Sayfa sayısı 1 ile 10000 arasında olmalıdır.")]
        public int PageCount { get; set; }

        [Required]
        public bool IsSecondHand { get; set; }
    }
}