using System.ComponentModel.DataAnnotations;

namespace SGT.WebAdmin.Models
{
    public class EsqueciSenha
    {
        [Required]
        public string Usuario { get; set; }
        public string ReturnUrl { get; set; }
    }
}