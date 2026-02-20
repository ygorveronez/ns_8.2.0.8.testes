using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaProdutoTransportador
    {
        public int CodigoCarga { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public int CodigoPedido { get; set; }

        public int CodigoProduto { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<double> CodigosRecebedores { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public System.DateTime? DataInicial { get; set; }

        public System.DateTime? DataLimite { get; set; }

        public List<SituacaoCargaJanelaCarregamento> Situacao { get; set; }

        public bool SituacaoFaturada { get; set; }

        public bool SituacaoNaoFaturada { get; set; }

        public bool? ExibirCodigoBarras { get; set; }
    }
}
