using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class PosicaoFrotaRastreamento
    {
        public Int64 Codigo { get; set; }
        public DateTime DataVeiculo { get; set; }
        public string CodigosClientesEmAlvo { get; set; }
        public double CodigoClienteEmAlvo { get; set; }
        public string Placa { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Rastreador { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public TimeSpan TempoEstadia { get; set; }
        public string TempoEstadiaFormatado { get { return Formatar(TempoEstadia); } }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime Entrada { get; set; }
        public string EntradaFormatado { get { return Formatar(Entrada); } }
        public DateTime Saida { get; set; }
        public string SaidaFormatado { get { return Formatar(Saida); } }
        public DateTime ChegadaDestino { get; set; }
        public string ChegadaDestinoFormatado { get { return Formatar(ChegadaDestino); } }
        public TimeSpan TempoTransito { get; set; }
        public string TempoTransitoFormatado { get { return Formatar(TempoTransito); } }
        private string Formatar(TimeSpan tempo)
        {
            if (tempo != TimeSpan.MinValue)
            {
                string formato = String.Empty;
                if (tempo.Days > 0) formato = $"{tempo.Days}d";
                formato += tempo.ToString(@"hh\:mm");
                return formato;
            }
            return string.Empty; 
        }
        private string Formatar(DateTime data)
        {
            return (data != DateTime.MinValue) ? data.ToString("dd/MM/yyyy HH:mm") : string.Empty;
        }
    }
}
