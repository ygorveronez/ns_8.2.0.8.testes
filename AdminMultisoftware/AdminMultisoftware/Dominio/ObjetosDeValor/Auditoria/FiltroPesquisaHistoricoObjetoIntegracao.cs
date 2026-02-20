using System;

namespace AdminMultisoftware.Dominio.ObjetosDeValor.Auditoria
{
    public sealed class FiltroPesquisaHistoricoObjetoIntegracao
    {
        public string CodigoIntegracao { get; set; }

        public int CodigoIntegradora { get; set; }

        public int CodigoUsuario { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public string NumeroCarga { get; set; }

        public int NumeroCte { get; set; }

        public Enumeradores.OrigemAuditadoAdmin? OrigemAuditado { get; set; }
    }
}