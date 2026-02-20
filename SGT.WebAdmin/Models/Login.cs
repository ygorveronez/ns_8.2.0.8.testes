using System.ComponentModel.DataAnnotations;

namespace SGT.WebAdmin.Models
{
    public class Login
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public string ReturnUrl { get; set; }

        public bool AzureAD { get; set; }

        public bool Saml { get; set; }

        public string AzureAdDisplay { get; set; }

        public bool LoginSSO { get; set; }

        public int StatusSSO { get; set; }

        public bool AutenticacaoForms { get; set; }

        public bool HabilitarRegistro { get; set; }
    }
}