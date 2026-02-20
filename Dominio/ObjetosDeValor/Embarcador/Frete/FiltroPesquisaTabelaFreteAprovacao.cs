using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaTabelaFreteAprovacao
    {
        public int CodigoTabelaFrete { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoUsuario { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.SituacaoAlteracaoTabelaFrete? SituacaoAlteracao { get; set; }

        public Enumeradores.TipoAprovadorRegra? TipoAprovadorRegra { get; set; }
    }
}
