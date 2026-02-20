using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class CargaViagemEventos
    {
        #region Propriedades
        public int CodigoFilial { get; set; }

        public int CodigoClienteOrigem { get; set; }

        public int CodigoClienteDestino { get; set; }

        public int CodigoLocalidadeOrigem { get; set; }

        public int CodigoLocalidadeDestino { get; set; }

        public int CodigoCargaEmbarcador { get; set; }

        public int Codigo { get; set; }
        public string CNPJFilial { get; set; }
        public string NomeFilial { get; set; }
        public string CNPJClienteOrigem { get; set; }
        public string NomeClienteOrigem { get; set; }
        public string CNPJClienteDestino { get; set; }
        public string NomeClienteDestino { get; set; }
        public string CNPJTransportador { get; set; }
        public string NomeTransportador { get; set; }
        public string NumeroCarga { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }        
        public string OperadorMonitoramento { get; set; }
        public string NomeMotoristas { get; set; }
        public string PlacaVeiculo { get; set; }        
        public string TipoOperacao { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public EnumTecnologiaRastreador UltimaPosicaoRastreador { get; set; }
        public MonitoramentoStatus UltimaPosicaoStatus { get; set; }

        public DateTime PrevisaoEntrega { get; set; }
        public DateTime PrevisaoChegadaPlanta { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public DateTime DataInicioViagem { get; set; }
        public DateTime DataETA { get; set; }
        public DateTime DataChegadaCliente { get; set; }
        public DateTime DataEncerramentoViagem { get; set; }
        #endregion

        #region PropriedadesComRegras

        public string TipoTecnologiaDescricao { 
            get
            {
                return EnumTecnologiaRastreadorHelper.ObterDescricao(UltimaPosicaoRastreador);
            }
        }

        public string LatitudeFormatada
        {
            get
            {
                return Latitude != 0.0M ? Latitude.ToString() : "";
            }
        }

        public string LongitudeFormatada
        {
            get
            {
                return Longitude != 0.0M ? Longitude.ToString() : "";
            }
        }

        public string PreenchimentoManualMobile 
        { 
            get
            {
                return TipoTecnologiaDescricao != "" ? "Mobile" : "Manual";
            }
        }

        public string SituacaoMonitoramento 
        { 
            get 
            {
                return MonitoramentoStatusHelper.ObterDescricao(UltimaPosicaoStatus);
            } 
        }

        public string CNPJTransportadorFormatado
        {
            get { return CNPJTransportador.ObterCpfOuCnpjFormatado(); }
        }

        public string CNPJFilialFormatado
        {
            get { return CNPJFilial.ObterCpfOuCnpjFormatado(); }
        }

        public string CNPJClienteOrigemFormatado
        {
            get { return CNPJClienteOrigem.ObterCpfOuCnpjFormatado(); }
        }

        public string CNPJClienteDestinoFormatado
        {
            get { return CNPJClienteDestino.ObterCpfOuCnpjFormatado(); }
        }

        public string DataCriacaoCargaFormatada
        {
            get { return ObterDataFormatada(DataCriacaoCarga); }            
        }

        public string DataInicioViagemFormatada
        {
            get { return ObterDataFormatada(DataInicioViagem); }
        }

        public string DataETAFormatada
        {
            get { return ObterDataFormatada(DataETA); }
        }

        public string DataChegadaClienteFormatada
        {
            get { return ObterDataFormatada(DataChegadaCliente); }
        }

        public string DataEncerramentoViagemFormatada
        {
            get { return ObterDataFormatada(DataEncerramentoViagem); }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get { return ObterDataFormatada(PrevisaoEntrega); }
        }

        public string DataPrevisaoChegadaPlantaFormatada
        {
            get
            {
                return ObterDataFormatada(PrevisaoChegadaPlanta);
            }
        }

        private string ObterDataFormatada(DateTime d)
        {
            return d != DateTime.MinValue ? d.ToString("dd/MM/yyyy HH:mm") : string.Empty;
        }
        
        #endregion
    }
}
