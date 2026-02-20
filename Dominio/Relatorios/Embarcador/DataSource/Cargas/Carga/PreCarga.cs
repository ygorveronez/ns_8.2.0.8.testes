using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class PreCarga
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string NumeroPreCarga { get; set; }
        public string DocaCarregamento { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroPedidos { get; set; }
        private string CNPJTransportador { get; set; }
        public string Transportador { get; set; }
        private DateTime DataPrevisaoInicioViagem { get; set; }
        private DateTime PrevisaoChegadaDoca { get; set; }
        private DateTime DataCriacaoPreCarga { get; set; }
        public string ModeloVeiculo { get; set; }
        public string CNPJRemetente { get; set; }
        public string Remetente { get; set; }
        public string CNPJDestinatario { get; set; }
        public string Destinatario { get; set; }
        public decimal Peso { get; set; }
        public string Veiculos { get; set; }
        public string Motoristas { get; set; }
        public string Filial { get; set; }
        public string TipoOperacao { get; set; }
        public string FaixaTemperatura { get; set; }
        public string TipoCarga { get; set; }
        public string Operador { get; set; }
        private DateTime PrevisaoChegadaDestinatario { get; set; }
        private DateTime PrevisaoSaidaDestinatario { get; set; }
        public string RotaProgramada { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CNPJTransportadorFormatado
        {
            get { return CNPJTransportador.ObterCnpjFormatado(); }
        }

        public string DataPrevisaoInicioViagemFormatada
        {
            get { return DataPrevisaoInicioViagem != DateTime.MinValue ? DataPrevisaoInicioViagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PrevisaoChegadaDocaFormatada
        {
            get { return PrevisaoChegadaDoca != DateTime.MinValue ? PrevisaoChegadaDoca.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataCriacaoPreCargaFormatada
        {
            get { return DataCriacaoPreCarga != DateTime.MinValue ? DataCriacaoPreCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PrevisaoChegadaDestinatarioFormatada
        {
            get { return PrevisaoChegadaDestinatario != DateTime.MinValue ? PrevisaoChegadaDestinatario.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string PrevisaoSaidaDestinatarioFormatada
        {
            get { return PrevisaoSaidaDestinatario != DateTime.MinValue ? PrevisaoSaidaDestinatario.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}
