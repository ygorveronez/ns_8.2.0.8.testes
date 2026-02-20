using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class MonitoramentoAlerta
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string DataFormatada
        {
            get
            {
                return Data.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        public DateTime DataTratativa { get; set; }
        public DateTime DataFim { get; set; }
        public string DataTratativaFormatada
        {
            get
            {
                return (DataTratativa != DateTime.MinValue) ? DataTratativa.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            }
        }
        public string DataFimTratativaFormatada
        {
            get
            {
                return (DataFim != DateTime.MinValue) ? DataFim.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            }
        }

        private decimal Latitude { get; set; }
        private decimal Longitude { get; set; }
        public string Descricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }
        public string StatusDescricao
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatusHelper.ObterDescricao(Status);
            }
        }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta Tipo { get; set; }
        public string TipoDescricao
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(Tipo);
            }
        }
        public string Observacao { get; set; }
        public string Acao { get; set; }
        public string Usuario { get; set; }
        public string Placa { get; set; }
        public string Motorista { get; set; }
        public string Transportador { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string TipoOperacao { get; set; }
        public bool AlertaPossuiPosicaoRetroativa { get; set; }
        public string NomeAlerta { get; set; }
        public string Responsavel { get; set; }
        public string Causa { get; set; }

        public string AlertaPossuiPosicaoRetroativaDescricao
        {
            get
            {
                return AlertaPossuiPosicaoRetroativa ? "Sim" : "Não";
            }
        }

        public bool FinalizadoSemRetornoSinal { get; set; }
        public string FinalizadoSemRetornoSinalDescricao
        {
            get
            {
                return FinalizadoSemRetornoSinal ? "Sim" : "Não";
            }
        }
        public string CPFMotorista { get; set; }
        public string CentroResultadoCarga { get; set; }
        public string LatitudeFormatada
        {
            get
            {
                return Latitude.ToString("n6");
            }
        }

        public string LongitudeFormatada
        {
            get
            {
                return Longitude.ToString("n6");
            }
        }

        public DateTime DataCriacao { get; set; }
        public string DataCriacaoFormatada
        {
            get
            {
                return DataCriacao.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
    }
}
