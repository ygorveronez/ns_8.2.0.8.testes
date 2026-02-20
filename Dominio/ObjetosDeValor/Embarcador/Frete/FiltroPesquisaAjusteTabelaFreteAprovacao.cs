using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaAjusteTabelaFreteAprovacao
    {
        public int CodigoTabelaFrete { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoUsuario { get; set; }
        
        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }
        
        public Enumeradores.EtapaAjusteTabelaFrete EtapaAjuste { get; set; }

        public Enumeradores.EtapaAutorizacaoTabelaFrete EtapaAutorizacao { get; set; }

        public Enumeradores.SituacaoAjusteTabelaFrete Situacao { get; set; }

        public Enumeradores.TipoAprovadorRegra? TipoAprovadorRegra { get; set; }
    }
}
