using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Pedido
{
    public sealed class FiltroPesquisaRelatorioPedido
    {
        public List<int> CodigosGruposPessoas { get; set; }

        public List<int> CodigosModelosVeiculos { get; set; }

        public int CodigoMotorista { get; set; }

        public List<int> CodigosRotaFrete { get; set; }

        public List<int> CodigosOrigem { get; set; }

        public List<string> SiglasOrigem { get; set; }

        public List<int> CodigosDestino { get; set; }

        public List<string> SiglasDestino { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<double> CodigosRecebedores { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public List<int> CodigosTransportadores { get; set; }

        public List<int> CodigosRestricoes { get; set; }

        public List<int> CodigosPedido { get; set; }

        public int CodigoVeiculo { get; set; }

        public List<double> CpfCnpjsExpedidor { get; set; }

        public List<double> CpfCnpjsDestinatario { get; set; }

        public List<double> CpfCnpjsRemetente { get; set; }

        public DateTime? PrevisaoDataInicial { get; set; }

        public DateTime? PrevisaoDataFinal { get; set; }

        public DateTime PrevisaoEntregaPedidoDataInicial { get; set; }

        public DateTime PrevisaoEntregaPedidoDataFinal { get; set; }

        public DateTime DataFinal { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataCarregamentoInicial { get; set; }

        public DateTime DataCarregamentoFinal { get; set; }
        public DateTime DataCriacaoPedidoInicial { get; set; }

        public DateTime DataCriacaoPedidoFinal { get; set; }

        public DateTime? DataInclusaoBookingInicial { get; set; }

        public DateTime? DataInclusaoBookingLimite { get; set; }

        public DateTime? DataInclusaoPCPInicial { get; set; }

        public DateTime? DataInclusaoPCPLimite { get; set; }

        public string DeliveryTerm { get; set; }

        public bool ExibirProdutos { get; set; }

        public string IdAutorizacao { get; set; }

        public bool PedidosSemCarga { get; set; }

        public bool UtilizarDadosDasCargasAgrupadas { get; set; }

        public bool UtilizarDadosDosPedidos { get; set; }

        public List<Enumeradores.SituacaoCarga> Situacoes { get; set; }
        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }

        public List<Enumeradores.SituacaoPedido> SituacoesPedido { get; set; }

        public List<Enumeradores.SituacaoEntrega> SituacoesEntrega { get; set; }

        public bool SomenteComReserva { get; set; }

        public bool SomentePedidosCanceladosAposVincularCarga { get; set; } 

        public Enumeradores.TipoLocalPrestacao TipoLocalPrestacao { get; set; }

        public string NumeroCarga { get; set; }

        public int CodigoGerente { get; set; }

        public int CodigoVendedor { get; set; }

        public int CodigoSupervisor { get; set; }

        public DateTime DataInicioViagemInicial { get; set; }

        public DateTime DataInicioViagemFinal { get; set; }

        public DateTime DataEntregaInicial { get; set; }

        public DateTime DataEntregaFinal { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public string NumeroPedido { get; set; }

        public bool? PossuiExpedidor { get; set; }

        public bool? PossuiRecebedor { get; set; }

        public bool ExibirCargasAgrupadas { get; set; }

        public List<TipoPropostaMultimodal> TipoPropostaMultiModal { get; set; }

        public int CodigoNumeroViagemNavio { get; set; }

        public int CodigoPortoOrigem { get; set; }

        public int CodigoPortoDestino { get; set; }

        public DateTime DataETAPortoOrigemInicial { get; set; }

        public DateTime DataETAPortoOrigemFinal { get; set; }

        public DateTime DataETSPortoOrigemInicial { get; set; }

        public DateTime DataETSPortoOrigemFinal { get; set; }

        public DateTime DataETAPortoDestinoInicial { get; set; }

        public DateTime DataETAPortoDestinoFinal { get; set; }

        public DateTime DataETSPortoDestinoInicial { get; set; }

        public DateTime DataETSPortoDestinoFinal { get; set; }

        public DateTime DataInclusaoPedidoInicial { get; set; }

        public DateTime DataInclusaoPedidoFinal { get; set; }
        public List<int> CodigoOperadorPedido { get; set; }

        public List<int> CodigoCentroResultado { get; set; }
        
        public DateTime DataInicioJanela { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public Enumeradores.AguardandoIntegracao? AguardandoIntegracao { get; set; }
        public bool SomentePedidosDeIntegracao { get; set; }

        public int CentroDeCustoViagemCodigo { get; set; }
        public string CentroDeCustoViagemDescricao { get; set; }

    }
}
