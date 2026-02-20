using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PedidoDadosTransporteMaritimo
    {

        #region Propriedades Sem Regra

        public int Codigo { get; set; }
        public string NumeroPedido { get; set; }
        public string CodigoIdentificacaoCarga { get; set; }
        public string DescricaoIdentificacaoCarga { get; set; }
        public string CodigoNCM { get; set; }
        public string MetragemCarga { get; set; }
        public string Incoterm { get; set; }
        public string Transbordo { get; set; }
        public string MensagemTransbordo { get; set; }
        public string CodigoArmador { get; set; }
        public string CodigoRota { get; set; }
        private DateTime DataBooking { get; set; }
        private DateTime DataDeadLineCarga { get; set; }
        private DateTime DataDeadLineDraf { get; set; }
        private DateTime DataDepositoContainer { get; set; }
        private DateTime DataETADestino { get; set; }
        private DateTime DataETADestinoFinal { get; set; }
        private DateTime DataETASegundaOrigem { get; set; }
        private DateTime DataETASegundoDestino { get; set; }
        private DateTime DataETAOrigem { get; set; }
        private DateTime DataETAOrigemFinal { get; set; }
        private DateTime DataETATransbordo { get; set; }
        private DateTime DataETS { get; set; }
        private DateTime DataETSTransbordo { get; set; }
        private DateTime DataRetiradaContainer { get; set; }
        private DateTime DataRetiradaContainerDestino { get; set; }
        private DateTime DataRetiradaVazio { get; set; }
        private DateTime DataRetornoVazio { get; set; }
        public string CodigoPortoCarregamento { get; set; }
        public string DescricaoPortoOrigem { get; set; }
        public string PaisPortoOrigem { get; set; }
        public string SiglaPaisPortoOrigem { get; set; }
        public string CodigoPortoCarregamentoTransbordo { get; set; }
        public string DescricaoPortoCarregamentoTransbordo { get; set; }
        public string CodigoPortoDestinoTransbordo { get; set; }
        public string DescricaoPortoDestinoTransbordo { get; set; }
        public string PaisPortoDestinoTransbordo { get; set; }
        public string SiglaPaisPortoDestinoTransbordo { get; set; }
        public string ModoTransporte { get; set; }
        public string NomeNavio { get; set; }
        public string NomeNavioTransbordo { get; set; }
        private string NumeroBL { get; set; }
        private string NumeroBLPedido { get; set; }
        public string NumeroContainer { get; set; }
        public string NumeroLacre { get; set; }
        public string NumeroViagem { get; set; }
        public string NumeroViagemTransbordo { get; set; }
        public string TerminalContainer { get; set; }
        public string TerminalOrigem { get; set; }
        public string TipoTransporte { get; set; }
        private TipoEnvioTransporteMaritimo TipoEnvio { get; set; }
        private StatusControleMaritimo Status { get; set; }
        public string DescricaoFilial { get; set; }
        private string NumeroBooking { get; set; }
        private string NumeroBookingPedido { get; set; }
        private string NumeroEXP { get; set; }
        private string NumeroEXPPedido { get; set; }
        private string CodigoCargaEmbarcador { get; set; }
        private string PedidoCodigoCargaEmbarcador { get; set; }
        public string DescricaoTipoCarga { get; set; }
        private bool PossuiGenset { get; set; }
        public string CodigoDespachante { get; set; }
        public string DescricaoDespachante { get; set; }
        private bool Halal { get; set; }
        private DateTime DataETA { get; set; }
        public string NomeImportador { get; set; }
        public string DescricaoNavio { get; set; }
        public string DescricaoTipoContainer { get; set; }
        private TipoProbe TipoProbe { get; set; }
        public string DescricaoViaTransporte { get; set; }
        public string NomeArmador { get; set; }
        public string CodigoEspecie { get; set; }
        public string DescricaoEspecie { get; set; }
        private StatusEXP StatusEXP { get; set; }
        private FretePrepaid FretePrepaid { get; set; }
        private bool CargaPaletizada { get; set; }
        public string Temperatura { get; set; }
        private DateTime DataCarregamentoPedido { get; set; }
        public string NomeRemetente { get; set; }
        public string CodigoInLand { get; set; }
        public string DescricaoInLand { get; set; }
        private DateTime DataDeadLinePedido { get; set; }
        private DateTime DataReserva { get; set; }
        private DateTime SegundaDataDeadLineCarga { get; set; }
        private DateTime SegundaDataDeadLineDraf { get; set; }
        public decimal ValorCapatazia { get; set; }
        public string MoedaCapatazia { get; set; }
        public decimal ValorFrete { get; set; }
        public string CodigoContratoFOB { get; set; }
        public string Observacao { get; set; }
        public string JustificativaCancelamento { get; set; }
        private DateTime DataPrevisaoEntrega { get; set; }
        public string ProtocoloCarga { get; set; }
        private DateTime DataPrevisaoEstufagem { get; set; }
        private DateTime DataConhecimento { get; set; }
        public int CodigoOriginal { get; set; }
        private bool BookingTemporario { get; set; }

        #endregion

        #region Propriedades Com Regra

        public string DataBookingFormatada 
        { 
            get { return DataBooking != DateTime.MinValue ? DataBooking.ToString("dd/MM/yyyy HH:mm") : string.Empty; } 
        }

        public string DataDeadLineCargaFormatada
        {
            get { return DataDeadLineCarga != DateTime.MinValue ? DataDeadLineCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataDeadLineDrafFormatada
        {
            get { return DataDeadLineDraf != DateTime.MinValue ? DataDeadLineDraf.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataDepositoContainerFormatada
        {
            get { return DataDepositoContainer != DateTime.MinValue ? DataDepositoContainer.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETADestinoFormatada
        {
            get { return DataETADestino != DateTime.MinValue ? DataETADestino.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETADestinoFinalFormatada
        {
            get { return DataETADestinoFinal != DateTime.MinValue ? DataETADestinoFinal.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETASegundaOrigemFormatada
        {
            get { return DataETASegundaOrigem != DateTime.MinValue ? DataETASegundaOrigem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETASegundoDestinoFormatada
        {
            get { return DataETASegundoDestino != DateTime.MinValue ? DataETASegundoDestino.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETAOrigemFormatada
        {
            get { return DataETAOrigem != DateTime.MinValue ? DataETAOrigem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETAOrigemFinalFormatada
        {
            get { return DataETAOrigemFinal != DateTime.MinValue ? DataETAOrigemFinal.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETATransbordoFormatada
        {
            get { return DataETATransbordo != DateTime.MinValue ? DataETATransbordo.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETSFormatada
        {
            get { return DataETS != DateTime.MinValue ? DataETS.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataETSTransbordoFormatada
        {
            get { return DataETSTransbordo != DateTime.MinValue ? DataETSTransbordo.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataRetiradaContainerFormatada
        {
            get { return DataRetiradaContainer != DateTime.MinValue ? DataRetiradaContainer.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataRetiradaContainerDestinoFormatada
        {
            get { return DataRetiradaContainerDestino != DateTime.MinValue ? DataRetiradaContainerDestino.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataRetiradaVazioFormatada
        {
            get { return DataRetiradaVazio != DateTime.MinValue ? DataRetiradaVazio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataRetornoVazioFormatada
        {
            get { return DataRetornoVazio != DateTime.MinValue ? DataRetornoVazio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string NumeroBLFormatado 
        {
            get { return !string.IsNullOrWhiteSpace(NumeroBL) ? NumeroBL : NumeroBLPedido; }
        }

        public string TipoEnvioDescricao
        {
            get { return TipoEnvio.ObterDescricao(); }
        }

        public string StatusDescricao
        {
            get 
            {
                switch (Status)
                {
                    case StatusControleMaritimo.Ativo:
                        return "Ativo";
                    case StatusControleMaritimo.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

        public string NumeroBookingFormatado 
        { 
            get { return !string.IsNullOrWhiteSpace(NumeroBooking) ? NumeroBooking : NumeroBookingPedido; }
        }

        public string NumeroEXPFormatado
        {
            get { return !string.IsNullOrWhiteSpace(NumeroEXP) ? NumeroEXP : NumeroEXPPedido; }
        }

        public string CodigoCargaEmbarcadorFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CodigoCargaEmbarcador) ? CodigoCargaEmbarcador : PedidoCodigoCargaEmbarcador; }
        }

        public string PossuiGensetFormatado 
        { 
            get { return PossuiGenset ? "Sim" : "Não"; }
        }

        public string HalalFormatado 
        {
            get { return Halal ? "Sim" : "Não"; }
        }

        public string DataETAFormatada
        {
            get { return DataETA != DateTime.MinValue ? DataETA.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TipoProbeDescricao
        {
            get { return TipoProbe.ObterDescricao(); }
        }

        public string StatusEXPDescricao
        {
            get { return StatusEXP.ObterDescricao(); } 
        }

        public string FretePrepaidDescricao
        {
            get { return FretePrepaid.ObterDescricao(); }
        }

        public string CargaPaletizadaFormatado 
        { 
            get { return CargaPaletizada ? "Sim" : "Não"; }
        }

        public string DataCarregamentoPedidoFormatada
        {
            get { return DataCarregamentoPedido != DateTime.MinValue ? DataCarregamentoPedido.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataDeadLinePedidoFormatada
        {
            get { return DataDeadLinePedido != DateTime.MinValue ? DataDeadLinePedido.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataReservaFormatada
        {
            get { return DataReserva != DateTime.MinValue ? DataReserva.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SegundaDataDeadLineCargaFormatada
        {
            get { return SegundaDataDeadLineCarga != DateTime.MinValue ? SegundaDataDeadLineCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SegundaDataDeadLineDrafFormatada
        {
            get { return SegundaDataDeadLineDraf != DateTime.MinValue ? SegundaDataDeadLineDraf.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get { return DataPrevisaoEntrega != DateTime.MinValue ? DataPrevisaoEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoEstufagemFormatada
        {
            get { return DataPrevisaoEstufagem != DateTime.MinValue ? DataPrevisaoEstufagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataConhecimentoFormatada
        {
            get { return DataConhecimento != DateTime.MinValue ? DataConhecimento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string BookingTemporarioFormatado 
        { 
            get { return BookingTemporario ? "Sim" : "Não"; }
        }

        #endregion
    }
}
