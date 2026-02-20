using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
	public sealed class FiltroPesquisaCarregamento
    {
        public int CodigoFuncionarioVendedor { get; set; }

        public int Transportador { get; set; }

        public bool CarregamentosDeColeta { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoDestino { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public int CodigoOrigem { get; set; }

        public int CodigoPaisDestino { get; set; }

        public int CodigoPedidoViagemNavio { get; set; }

        public List<int> CodigosFilial { get; set; }

		public List<int> CodigosFilialVenda { get; set; }

		public List<int> CodigosPedido { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public double CpfCnpjExpedidor { get; set; }

        public double CpfCnpjRecebedor { get; set; }
        public List<int> NotaFiscal { get; set; }

        public double CpfCnpjRemetente { get; set; }

        public DateTime? DataFim { get; set; }

        public DateTime? DataInclusaoPCPInicial { get; set; }

        public DateTime? DataInclusaoPCPLimite { get; set; }

        public DateTime? DataInclusaoBookingInicial { get; set; }

        public DateTime? DataInclusaoBookingLimite { get; set; }

        public DateTime? DataInicio { get; set; }

        public string DeliveryTerm { get; set; }

        public bool FiltrarPorParteDoNumero { get; set; }

        public string IdAutorizacao { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroCarregamento { get; set; }

        public string NumeroOS { get; set; }

        public int NumeroPedido { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public string Ordem { get; set; }

        public string PortoSaida { get; set; }

        public string Reserva { get; set; }

        public List<Enumeradores.SituacaoCarregamento> SituacoesCarregamento { get; set; }

        public bool SomenteComReserva { get; set; }

        public string TipoEmbarque { get; set; }

        public Enumeradores.TipoMontagemCarga TipoMontagemCarga { get; set; }

        public bool ProgramaComSessaoRoteirizador { get; set; }

        public Enumeradores.OpcaoSessaoRoteirizador OpcaoSessaoRoteirizador { get; set; }

        public int CodigoSessaoRoteirizador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador SituacaoSessaoRoteirizador { get; set; }

        public int CodigoProdutoEmbarcador { get; set; }

        public List<int> CodigosCanalEntrega { get; set; }

        public List<int> CodigosLinhaSeparacao { get; set; }

        public List<int> CodigosProdutos { get; set; }

        public List<int> CodigosGrupoProdutos { get; set; }

        public List<int> CodigosCategoriaClientes { get; set; }

        public int TransportadoraMatriz { get; set; }

        public int GrupoPessoaDestinatario { get; set; }

        public bool ExigeAgendamento { get; set; }

        public List<int> CodigosRegiaoDestino { get; set; }

        public string NomeTransportadora { get; set; }

        public int SituacaoAgendamento { get; set; }
    }
}
