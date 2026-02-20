using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class PosicaoFrota
    {

        #region Métodos públicos

        public string StatusMonitoramento { get; set; }
        public string SituacoesVeiculo { get; set; }
        public string CodigosVeiculo { get; set; }
        public string TiposModeloVeicular { get; set; }
        public int CodigoVeiculo { get; set; }
        public string PlacaVeiculo { get; set; }
        public long CodigoPosicao { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DataDaPosicao { get; set; }
        public DateTime DataFimStatusAtual { get; set; }
        public DateTime DataInicioStatusAtual { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPosicaoFrota Situacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador Rastreador { get; set; }
        public string Status { get; set; }
        public int GrupoStatusViagemCodigo { get; set; }
        public string GrupoStatusViagemDescricao { get; set; }
        public string GrupoStatusViagemCor { get; set; }
        public int GrupoTipoOperacaoCodigo { get; set; }
        public string GrupoTipoOperacaoDescricao { get; set; }
        public string GrupoTipoOperacaoCor { get; set; }
        public string Descricao { get; set; }
        public bool EmAlvo { get; set; }
        public string CodigosClientesAlvos { get; set; }
        public string ClientesAlvos { get; set; }
        public string EnderecoDaEntrega { get; set; }
        public decimal PercentualViagem { get; set; }
        public string CategoriasClientesAlvos { get; set; }
        public string TempoDaUltimaPosicaoFormatada { get { return getDataFormatadaExtenso(DataDaPosicao); } }
        public int CodigoCarga { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string CodigoUltimaCargaEmbarcador { get; set; }
        public int CodigoCargaVinculada { get; set; }
        public string DestinoCarga { get; set; }
        public string Filial { get; set; }
        public DateTime DataDaCarga { get; set; }
        public DateTime PrevisaoEntregaAtualizada { get; set; }
        public string PrevisaoEntregaAtualizadaFormatada { get { return getDataFormatada(PrevisaoEntregaAtualizada); } }
        public string DataDaCargaFormatada { get { return getDataFormatada(DataDaCarga); } }
        public string DataDaPosicaoFormatada { get { return getDataFormatada(DataDaPosicao); } }
        public string SM { get; set; }
        public string ModeloVeicular { get; set; }
        public string TiposModeloVeicularFormatado { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga> ListaTipoModeloVeicular
        {
            get => TiposModeloVeicular != null ? TiposModeloVeicular.Split(',').Select(x => (TipoModeloVeicularCarga)int.Parse(x)).ToList() : new List<TipoModeloVeicularCarga> { TipoModeloVeicularCarga.SemModeloVeicular };
        }

        public virtual List<ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo> ListaSituacaoVeiculo
        {
            get => SituacoesVeiculo.Split(',').Select(x => (SituacaoVeiculo)int.Parse(x)).ToList();
        }
        public int Permanencia
        {
            get
            {
                TimeSpan diff = DataDaPosicao - DataDaCarga;
                return (int)diff.TotalMinutes;
            }
        }

        public string TempoStatusDescricao
        {
            get
            {
                if (DataInicioStatusAtual != DateTime.MinValue)
                {
                    return FormatarTempo((DataFimStatusAtual == DateTime.MinValue ? DateTime.Now : DataFimStatusAtual) - DataInicioStatusAtual);
                }
                else
                    return "";

            }
        }

        public string TipoDeTransporte { get; set; }
        public string Transportador { get; set; }
        public string Reboque { get; set; }
        public string SituacaoVeiculo { get; set; }
        public string Motorista { get; set; }
        public string Tecnlogia { get; set; }

        //controles para verificar pre-viagem
        public bool CargaPreCarga { get; set; }
        public bool CargaFechada { get; set; }
        public List<ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus> ListaStatusMonitoramento
        {
            get => StatusMonitoramento.Split(',').Select(x => (ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus)int.Parse(x)).ToList();
        }
        public bool MonitoramentoCritico { get; set; }
        public int RaioFilial { get; set; }
        public bool VeiculosDentroDoRaioFilial { get; set; }
        public int? SituacaoCarga { get; set; }
        public string DescricaoSituacaoCarga
        {
            get
            {
                if (!SituacaoCarga.HasValue)
                    return string.Empty;

                var situacaoEnum = (SituacaoCarga)SituacaoCarga.Value;

                return situacaoEnum.ObterDescricao();
            }
        }
        public int RastreadorOnlineOffline { get; set; }

        #endregion

        #region Métodos privados

        private string getDataFormatada(DateTime data)
        {
            if (data != DateTime.MinValue)
                return data.ToString("dd/MM/yyyy HH:mm");
            return "";
        }

        private static string FormatarTempo(TimeSpan tempo)
        {
            string formato = String.Empty;
            if (tempo.Days > 0)
            {
                formato = $"{tempo.Days}d";
            }
            formato += tempo.ToString(@"hh\:mm");
            return formato;
        }


        private string getDataFormatadaExtenso(DateTime data)
        {
            if (data != DateTime.MinValue)
            {
                TimeSpan ts = DateTime.Now - data;

                const int second = 1;
                const int minute = 60 * second;
                const int hour = 60 * minute;
                const int day = 24 * hour;
                const int month = 30 * day;
                double delta = Math.Abs(ts.TotalSeconds);
                if (delta < 1 * minute) return "Há " + (ts.Seconds == 1 ? "um segundo" : ts.Seconds + " segundos");
                if (delta < 2 * minute) return "Há um minuto";
                if (delta < 45 * minute) return "Há " + ts.Minutes + " minutos";
                if (delta < 90 * minute) return "Há uma hora";
                if (delta < 24 * hour) return "Há " + ts.Hours + " horas";
                if (delta < 48 * hour) return "Ontem";
                if (delta < 30 * day) return "Há " + ts.Days + " dias";
                if (delta < 12 * month)
                {
                    var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    return "Há " + (months <= 1 ? "um mês" : months + " meses");
                }
                else
                {
                    var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    return "Há " + (years <= 1 ? "um ano" : years + " anos");
                }
            }
            else
                return "";
        }

        #endregion

    }
}

