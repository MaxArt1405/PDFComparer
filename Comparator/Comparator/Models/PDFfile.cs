using System.ComponentModel.DataAnnotations;

namespace Comparator.Models
{
    public class PDFfile
    {
        [Key]
        public int PDFID { get; set; }
        public byte[] Content { get; set; }
    }
}