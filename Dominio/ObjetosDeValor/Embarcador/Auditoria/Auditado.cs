namespace Dominio.ObjetosDeValor.Embarcador.Auditoria
{
    public class Auditado
    {
        public Entidades.Empresa Empresa { get; set; }

        public Entidades.WebService.Integradora Integradora { get; set; }

        public string IP { get; set; }

        public string Texto { get; set; }

        public Enumerador.TipoAuditado TipoAuditado { get; set; }

        public Enumerador.OrigemAuditado OrigemAuditado { get; set; }

        public Entidades.Usuario Usuario { get; set; }
    }
    public class AuditadoAssincrono
    {
        public int Empresa { get; set; }

        public int Integradora { get; set; }

        public string IP { get; set; }

        public string Texto { get; set; }

        public Enumerador.TipoAuditado TipoAuditado { get; set; }

        public Enumerador.OrigemAuditado OrigemAuditado { get; set; }

        public int Usuario { get; set; }
    }
}
