using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public sealed class FiltroPesquisaChamado
    {
        #region Propriedades

        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoPedido { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<double> CodigosRecebedor { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public List<int> CodigosMotivoChamado { get; set; }

        public int CodigoResponsavel { get; set; }
        public int CodigoResponsavelPorRegra { get; set; }

        public bool SomenteValoresPendentes { get; set; }

        public int CodigoVeiculo { get; set; }

        public int CodigoSetor { get; set; }

        public List<int> CodigosTransportador { get; set; }

        public double CpfCnpjCliente { get; set; }

        public double CpfCnpjTomador { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public int CodigoGrupoPessoasCliente { get; set; }

        public int CodigoGrupoPessoasTomador { get; set; }

        public int CodigoGrupoPessoasDestinatario { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public int NotaFiscal { get; set; }

        public int NumeroLote { get; set; }

        public int NumeroInicial { get; set; }

        public int NumeroFinal { get; set; }

        public SituacaoChamado SituacaoChamado { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public List<int> CodigosOcorrencia { get; set; }

        public OpcaoSimNaoPesquisa ComOcorrencia { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public bool? ComDevolucao { get; set; }

        public int CodigoMotorista { get; set; }

        public double CpfCnpjClienteResponsavel { get; set; }

        public int CodigoGrupoPessoasResponsavel { get; set; }

        public bool? ComNotaFiscalServico { get; set; }

        public bool AguardandoTratativaDoCliente { get; set; }

        public double FornecedorLogado { get; set; }

        public bool? ComResponsavel { get; set; }

        public bool? ComNovaMovimentacao { get; set; }

        public DateTime? DataInicialAgendamentoPedido { get; set; }

        public DateTime? DataFinalAgendamentoPedido { get; set; }

        public DateTime? DataInicialColetaPedido { get; set; }

        public DateTime? DataFinalColetaPedido { get; set; }

        public string NumeroPedidoCliente { get; set; }
        public bool SomenteCargasCriticas { get; set; }
        public bool SomenteAtendimentoComMsgNaoLida { get; set; }

        public List<double> ClienteComplementar { get; set; }

        public int CanalVenda { get; set; }

        public TipoCobrancaMultimodal ModalTransporte { get; set; }

        public List<int> SetorEscalationList { get; set; }
        public List<int> Vendedores { get; set; }
        public List<int> MesoRegiao { get; set; }
        public List<int> Regiao { get; set; }
        public List<string> UFDestino { get; set; }
        public string EscritorioVendas { get; set; }
        public string Matriz { get; set; }
        public bool? Parqueada { get; set; }
        public int CodigoTiposCausadoresOcorrencia { get; set; }
        public int CodigoCausasMotivoCamado { get; set; }

        #endregion

        #region Propriedades com Regras

        public int CodigoTransportador
        {
            set { if (value > 0) CodigosTransportador = new List<int>() { value }; }
        }

        #endregion
    }
}
