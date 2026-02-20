using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.TorreControle
{
    public class Permanencias
    {
        #region Propriedades
        private bool? _entregaForaDoRaio;
        private bool? _tipoParada;

        public string Carga { get; set; }
        public string Placa { get; set; }
        public string Transportador { get; set; }
        public string Motoristas { get; set; }
        public string Cliente { get; set; }
        public string GrupoPessoas { get; set; }
        public string TipoOperacao { get; set; }
        public string Filial { get; set; }
        public string OrigemCarga { get; set; }
        public string DestinoCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus SituacaoMonitoramento { get; set; }
        public string EtapaMonitoramento { get; set; }
        public int TempoTotalEtapaMonitoramento { get; set; }
        public bool EntregaForaDoRaio 
        {
            get => _entregaForaDoRaio ?? false;
            set => _entregaForaDoRaio = value;
        }
        public DateTime DataCarregamento { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public DateTime DataAgendamento { get; set; }
        public DateTime DataConfirmacao { get; set; }
        public DateTime DataEntregaAtualizada { get; set; }
        public int TempoAguardoNFE { get; set; }
        public DateTime DataPrimeiroEspelhamento { get; set; }
        public DateTime DataUltimoEspelhamento { get; set; }
        public bool TipoParada
        {
            get => _tipoParada ?? false;
            set => _tipoParada = value;
        }
        public DateTime DataEntradaArea { get; set; }
        public DateTime DataSaidaArea { get; set; }
        public int TempoArea { get; set; }
        public string SubArea { get; set; }
        public string TipoSubArea { get; set; }
        public DateTime DataEntradaSubArea { get; set; }
        public DateTime DataSaidaSubArea { get; set; }
        public int TempoSubArea { get; set; }
        #endregion

        #region Propriedades Formatadas

        public virtual string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataAgendamentoFormatada
        {
            get { return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataConfirmacaoFormatada
        {
            get { return DataConfirmacao != DateTime.MinValue ? DataConfirmacao.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataEntregaAtualizadaFormatada
        {
            get { return DataEntregaAtualizada != DateTime.MinValue ? DataEntregaAtualizada.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataPrimeiroEspelhamentoFormatada
        {
            get { return DataPrimeiroEspelhamento != DateTime.MinValue ? DataPrimeiroEspelhamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataUltimoEspelhamentoFormatada
        {
            get { return DataUltimoEspelhamento != DateTime.MinValue ? DataUltimoEspelhamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataEntradaAreaFormatada
        {
            get { return DataEntradaArea != DateTime.MinValue ? DataEntradaArea.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataSaidaAreaFormatada
        {
            get { return DataSaidaArea != DateTime.MinValue ? DataSaidaArea.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataEntradaSubAreaFormatada
        {
            get { return DataEntradaSubArea != DateTime.MinValue ? DataEntradaSubArea.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DataSaidaSubAreaFormatada
        {
            get { return DataSaidaSubArea != DateTime.MinValue ? DataSaidaSubArea.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public virtual string DescricaoSituacaoMonitoramento
        {
            get { return SituacaoMonitoramento.ObterDescricao(); }
        }

        public virtual string DescricaoEntregaForaDoRaio
        {
            get => _entregaForaDoRaio == null ? string.Empty : (_entregaForaDoRaio.Value ? SimNao.Sim.ObterDescricao() : SimNao.Nao.ObterDescricao());
        }

        public virtual string DescricaoTipoParada
        {
            get => _tipoParada == null ? string.Empty : (_tipoParada.Value ? "Coleta" : "Entrega");
        }

        public string DescricaoTempoTotalEtapaMonitoramento
        {
            get
            {
                if (TempoTotalEtapaMonitoramento > 0)
                    return FormatarTempo(TimeSpan.FromMinutes(TempoTotalEtapaMonitoramento));
                else
                    return string.Empty;
            }
        }

        public string DescricaoTempoAguardoNFE
        {
            get
            {
                if (TempoAguardoNFE > 0)
                    return FormatarTempo(TimeSpan.FromMinutes(TempoAguardoNFE));
                else
                    return string.Empty;
            }
        }

        public string DescricaoTempoArea
        {
            get
            {
                if (TempoArea > 0)
                    return FormatarTempo(TimeSpan.FromSeconds(TempoArea));
                else
                    return string.Empty;
            }
        }

        public string DescricaoTempoSubArea
        {
            get
            {
                if (TempoSubArea > 0)
                    return FormatarTempo(TimeSpan.FromSeconds(TempoSubArea));
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Métodos Privados

        private static string FormatarTempo(TimeSpan tempo)
        {
            string formato = string.Empty;
            if (tempo.Days > 0)
            {
                formato = $"{tempo.Days}d";
            }
            formato += tempo.ToString(@"hh\:mm");
            return formato;
        }

        #endregion
    }
}