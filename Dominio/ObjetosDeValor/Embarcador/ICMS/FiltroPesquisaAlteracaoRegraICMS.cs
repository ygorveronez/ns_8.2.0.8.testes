using System;

namespace Dominio.ObjetosDeValor.Embarcador.ICMS
{
    public sealed class FiltroPesquisaAlteracaoRegraICMS
    {
        public int CodigoUsuario { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SituacaoAlteracaoRegraICMS? SituacaoAlteracao { get; set; }
    }
}
