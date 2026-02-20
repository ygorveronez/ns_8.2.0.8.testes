using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.GestaoPedido
{
    public class FiltroPesquisaGestaoPedido
    {
        public List<int> CodigosFilial { get; set; }

        public List<double> CodigosRemetente { get; set; }

        public List<int> CodigosDestino { get; set; }

        public List<double> CodigosDestinatario { get; set; }

        public List<int> CodigosTransportador { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> ListaNumeroPedido { get; set; }

        public List<int> CodigosRegiaoDestino { get; set; }

        public List<int> CodigosVendedor { get; set; }

        public List<int> CodigosGerente { get; set; }

        public List<int> CodigosSupervisor { get; set; }

        public List<string> EstadosDestino { get; set; }

        public List<string> EstadosOrigem { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public string NumeroCarregamento { get; set; }

        public string CodigoAgrupamentoCarregamento { get; set; }

        public Enumeradores.SituacaoPedido? Situacao { get; set; }

        public Enumeradores.SituacaoEstoqueProdutoArmazem SituacaoEstoqueProdutoArmazem { get; set; }
        public Enumeradores.SituacaoRoteirizadorIntegracao? SituacaoRoteirizadorIntegracao { get; set; }
        public Enumeradores.SituacaoPedidoGestaoPedido SituacaoPedido { get; set; }

        public bool? Reentrega { get; set; }
        public int SituacaoComercialPedido { get; set; }

        public int CodigoCanalEntrega { get; set; }

        public int CodigoSessaoRoteirizador { get; set; }

        public int GrupoPessoaDestinatario { get; set; }

        public int TipoOperacao { get; set; }
    }
}
