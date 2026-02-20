using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaTipoOperacao
    {
        public Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public int ContratoFrete { get; set; }

        public string Descricao { get; set; }

        public List<int> ListaCodigoTipoOperacaoPermitidos { get; set; }

        public Entidades.Cliente Pessoa { get; set; }

        public string CodigoIntegracao { get; set; }

        public bool SomenteTipoOperacaoPermiteGerarRedespacho { get; set; }

        public bool TipoOperacaoPorTransportador { get; set; }

        public int CodigoTransportadorLogado { get; set; }

        public List<int> CodigosTiposOperacao { get; set; }
        public int CodigoTipoCargaEmissao { get; set; }
        public bool FiltrarTipoOperacaoOcultas { get; set; }
        public bool FiltrarPorTipoDevolucao { get; set; }
    }
}
