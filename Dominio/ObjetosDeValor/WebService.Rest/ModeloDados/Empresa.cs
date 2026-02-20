using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Empresa
    {
        public string CodigoIntegracao { get; set; }

        public string RazaoSocial { get; set; }

        public string Cnpj { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public DateTime? DataValidadeCertificadoDigital { get; set; }
    }
}
