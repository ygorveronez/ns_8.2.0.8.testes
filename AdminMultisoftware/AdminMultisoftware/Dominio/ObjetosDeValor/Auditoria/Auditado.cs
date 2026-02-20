namespace AdminMultisoftware.Dominio.ObjetosDeValor.Auditoria
{
    public class Auditado
    {
        public string IP { get; set; }

        public string Texto { get; set; }

        public Enumeradores.TipoAuditadoAdmin TipoAuditado { get; set; }

        public Enumeradores.OrigemAuditadoAdmin OrigemAuditado { get; set; }

        public Entidades.Pessoas.Usuario Usuario { get; set; }
    }
}