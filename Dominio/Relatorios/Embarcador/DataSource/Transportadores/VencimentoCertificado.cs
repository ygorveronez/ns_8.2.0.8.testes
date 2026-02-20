using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Transportadores
{
    public class VencimentoCertificado
    {
        public string CNPJEmpresa { get; set; }
        public string Empresa { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string EmailContador { get; set; }
        public string EmailAdministrativo { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}
