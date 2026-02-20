using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class TemposGestaoPatio
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Filial { get; set; }
        private DateTime DataCarregamento { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroDoca { get; set; }
        private int SituacaoCarga { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Transportador { get; set; }
        public string Motorista { get; set; }
        public string ModeloVeiculo { get; set; }
        public string Veiculo { get; set; }
        public string VeiculosVinculados { get; set; }
        public string TipoCarga { get; set; }
        public string TipoOperacao { get; set; }
        private DateTime DataDocaInformada { get; set; }
        public decimal TempoAgInformarDoca { get; set; }
        private DateTime DataChegadaVeiculo { get; set; }
        private DateTime DataChegadaVeiculoPrevista { get; set; }
        public decimal TempoAgChegadaVeiculo { get; set; }
        private DateTime DataEntregaGuarita { get; set; }
        public decimal TempoAgEntradaGuarita { get; set; }
        private DateTime DataFimCheckList { get; set; }
        public decimal TempoAgChecklist { get; set; }
        private DateTime DataTravaChave { get; set; }
        public decimal TempoAgTravaChave { get; set; }
        private DateTime DataInicioCarregamento { get; set; }
        public decimal TempoAgInicioCarregamento { get; set; }
        private DateTime DataFimCarregamento { get; set; }
        public decimal TempoAgFimCarregamento { get; set; }
        private DateTime DataLiberacaoChave { get; set; }
        public decimal TempoAgLiberacaoChave { get; set; }
        private DateTime DataFaturamento { get; set; }
        public decimal TempoAgFaturamento { get; set; }
        private DateTime DataInicioViagem { get; set; }
        public decimal TempoAgInicioViagem { get; set; }
        private DateTime DataPosicao { get; set; }
        public decimal TempoAgPosicao { get; set; }
        private DateTime DataChegadaLoja { get; set; }
        public decimal TempoAgChegadaLoja { get; set; }
        private DateTime DataDeslocamentoPatio { get; set; }
        public decimal TempoAgDeslocamentoPatio { get; set; }
        private DateTime DataSaidaLoja { get; set; }
        public decimal TempoAgSaidaLoja { get; set; }
        private DateTime DataFimViagem { get; set; }
        public decimal TempoAgFimViagem { get; set; }
        private DateTime DataInicioHigienizacao { get; set; }
        public decimal TempoAgInicioHigienizacao { get; set; }
        private DateTime DataFimHigienizacao { get; set; }
        public decimal TempoAgFimHigienizacao { get; set; }
        private DateTime DataSolicitacaoVeiculo { get; set; }
        public decimal TempoAgSolicitacaoVeiculo { get; set; }
        private DateTime DataInicioDescarregamento { get; set; }
        public decimal TempoAgInicioDescarregamento { get; set; }
        private DateTime DataFimDescarregamento { get; set; }
        public decimal TempoAgFimDescarregamento { get; set; }
        private DateTime DataDocumentoFiscal { get; set; }
        public decimal TempoAgDocumentoFiscal { get; set; }
        private DateTime DataDocumentosTransporte { get; set; }
        public decimal TempoAgDocumentosTransporte { get; set; }
        private DateTime DataMontagemCarga { get; set; }
        public decimal TempoAgMontagemCarga { get; set; }
        private DateTime DataSeparacaoMercadoria { get; set; }
        public decimal TempoAgSeparacaoMercadoria { get; set; }
        public string Rota { get; set; }
        public string ObservacaoFluxoPatio { get; set; }
        public decimal Peso { get; set; }
        public string AreaVeiculo { get; set; }
        public decimal PesoChegadaVeiculo { get; set; }
        public decimal PesoSaidaVeiculo { get; set; }
        public string CodigoTransportador { get; set; }
        public decimal ValorTotalNotaFiscal { get; set; }


        #endregion

        #region Propriedades com Regras

        public string SituacaoCargaFormatada
        {
            get { return SituacaoCarga >= 0 ? SituacaoCarga.ToString().ToEnum<SituacaoCarga>().ObterDescricao() : string.Empty; }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SomaTotalDosTempos
        {
            get
            {
                return TimeFromMinutes(
                          (double)TempoAgInformarDoca +
                          (double)TempoAgChegadaVeiculo +
                          (double)TempoAgEntradaGuarita +
                          (double)TempoAgChecklist +
                          (double)TempoAgTravaChave +
                          (double)TempoAgInicioCarregamento +
                          (double)TempoAgFimCarregamento +
                          (double)TempoAgLiberacaoChave +
                          (double)TempoAgFaturamento +
                          (double)TempoAgInicioViagem +
                          (double)TempoAgPosicao +
                          (double)TempoAgChegadaLoja +
                          (double)TempoAgDeslocamentoPatio +
                          (double)TempoAgSaidaLoja +
                          (double)TempoAgFimViagem +
                          (double)TempoAgInicioHigienizacao +
                          (double)TempoAgFimHigienizacao +
                          (double)TempoAgSolicitacaoVeiculo +
                          (double)TempoAgInicioDescarregamento +
                          (double)TempoAgFimDescarregamento +
                          (double)TempoAgDocumentoFiscal +
                          (double)TempoAgDocumentosTransporte +
                          (double)TempoAgMontagemCarga +
                          (double)TempoAgSeparacaoMercadoria
                          );
            }
        }

        public string DataDocaInformadaFormatada
        {
            get { return DataDocaInformada != DateTime.MinValue ? DataDocaInformada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgInformarDocaDescricao
        {
            get { return TimeFromMinutes((double)TempoAgInformarDoca); }
        }

        public string DataChegadaVeiculoFormatada
        {
            get { return DataChegadaVeiculo != DateTime.MinValue ? DataChegadaVeiculo.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgChegadaVeiculoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgChegadaVeiculo); }
        }

        public string PrevistoRealizadoChegadaVeiculoDescricao
        {
            get { return DecimalToTime(DiferencaChegadaVeiculo); }
        }

        public int PrevistoRealizadoChegadaVeiculo
        {
            get { return DiferencaChegadaVeiculo ?? 0; }
        }

        private int? DiferencaChegadaVeiculo
        {
            get { return DiffTimeMinutes(DataChegadaVeiculoPrevista, DataChegadaVeiculo); }
        }

        public string DataEntregaGuaritaFormatada
        {
            get { return DataEntregaGuarita != DateTime.MinValue ? DataEntregaGuarita.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgEntradaGuaritaDescricao
        {
            get { return TimeFromMinutes((double)TempoAgEntradaGuarita); }
        }

        public string DataFimCheckListFormatada
        {
            get { return DataFimCheckList != DateTime.MinValue ? DataFimCheckList.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgChecklistDescricao
        {
            get { return TimeFromMinutes((double)TempoAgChecklist); }
        }

        public string DataTravaChaveFormatada
        {
            get { return DataTravaChave != DateTime.MinValue ? DataTravaChave.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgTravaChaveDescricao
        {
            get { return TimeFromMinutes((double)TempoAgTravaChave); }
        }

        public string DataInicioCarregamentoFormatada
        {
            get { return DataInicioCarregamento != DateTime.MinValue ? DataInicioCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgInicioCarregamentoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgInicioCarregamento); }
        }

        public string DataFimCarregamentoFormatada
        {
            get { return DataFimCarregamento != DateTime.MinValue ? DataFimCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgFimCarregamentoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgFimCarregamento); }
        }

        public string DataLiberacaoChaveFormatada
        {
            get { return DataLiberacaoChave != DateTime.MinValue ? DataLiberacaoChave.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgLiberacaoChaveDescricao
        {
            get { return TimeFromMinutes((double)TempoAgLiberacaoChave); }
        }

        public string DataFaturamentoFormatada
        {
            get { return DataFaturamento != DateTime.MinValue ? DataFaturamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgFaturamentoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgFaturamento); }
        }

        public string DataInicioViagemFormatada
        {
            get { return DataInicioViagem != DateTime.MinValue ? DataInicioViagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgInicioViagemDescricao
        {
            get { return TimeFromMinutes((double)TempoAgInicioViagem); }
        }

        public string DataPosicaoFormatada
        {
            get { return DataPosicao != DateTime.MinValue ? DataPosicao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgPosicaoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgPosicao); }
        }

        public string DataChegadaLojaFormatada
        {
            get { return DataChegadaLoja != DateTime.MinValue ? DataChegadaLoja.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgChegadaLojaDescricao
        {
            get { return TimeFromMinutes((double)TempoAgChegadaLoja); }
        }

        public string DataDeslocamentoPatioFormatada
        {
            get { return DataDeslocamentoPatio != DateTime.MinValue ? DataDeslocamentoPatio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgDeslocamentoPatioDescricao
        {
            get { return TimeFromMinutes((double)TempoAgDeslocamentoPatio); }
        }

        public string DataSaidaLojaFormatada
        {
            get { return DataSaidaLoja != DateTime.MinValue ? DataSaidaLoja.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgSaidaLojaDescricao
        {
            get { return TimeFromMinutes((double)TempoAgSaidaLoja); }
        }

        public string DataFimViagemFormatada
        {
            get { return DataFimViagem != DateTime.MinValue ? DataFimViagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgFimViagemDescricao
        {
            get { return TimeFromMinutes((double)TempoAgFimViagem); }
        }

        public string DataInicioHigienizacaoFormatada
        {
            get { return DataInicioHigienizacao != DateTime.MinValue ? DataInicioHigienizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgInicioHigienizacaoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgInicioHigienizacao); }
        }

        public string DataFimHigienizacaoFormatada
        {
            get { return DataFimHigienizacao != DateTime.MinValue ? DataFimHigienizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgFimHigienizacaoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgFimHigienizacao); }
        }

        public string DataSolicitacaoVeiculoFormatada
        {
            get { return DataSolicitacaoVeiculo != DateTime.MinValue ? DataSolicitacaoVeiculo.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgSolicitacaoVeiculoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgSolicitacaoVeiculo); }
        }

        public string DataInicioDescarregamentoFormatada
        {
            get { return DataInicioDescarregamento != DateTime.MinValue ? DataInicioDescarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgInicioDescarregamentoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgInicioDescarregamento); }
        }

        public string DataFimDescarregamentoFormatada
        {
            get { return DataFimDescarregamento != DateTime.MinValue ? DataFimDescarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgFimDescarregamentoDescricao
        {
            get { return TimeFromMinutes((double)TempoAgFimDescarregamento); }
        }

        public string DataDocumentoFiscalFormatada
        {
            get { return DataDocumentoFiscal != DateTime.MinValue ? DataDocumentoFiscal.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgDocumentoFiscalDescricao
        {
            get { return TimeFromMinutes((double)TempoAgDocumentoFiscal); }
        }

        public string DataDocumentosTransporteFormatada
        {
            get { return DataDocumentosTransporte != DateTime.MinValue ? DataDocumentosTransporte.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgDocumentosTransporteDescricao
        {
            get { return TimeFromMinutes((double)TempoAgDocumentosTransporte); }
        }

        public string DataMontagemCargaFormatada
        {
            get { return DataMontagemCarga != DateTime.MinValue ? DataMontagemCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgMontagemCargaDescricao
        {
            get { return TimeFromMinutes((double)TempoAgMontagemCarga); }
        }

        public string DataSeparacaoMercadoriaFormatada
        {
            get { return DataSeparacaoMercadoria != DateTime.MinValue ? DataSeparacaoMercadoria.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string TempoAgSeparacaoMercadoriaDescricao
        {
            get { return TimeFromMinutes((double)TempoAgSeparacaoMercadoria); }
        }

        #endregion

        #region Métodos Privados

        private string TimeFromMinutes(double tempo)
        {
            return TimeSpan.FromMinutes((double)tempo).RoundUpMinute().ToString(@"hh\:mm");
        }

        private string DecimalToTime(int? val)
        {
            if (!val.HasValue)
                return string.Empty;

            decimal num = Math.Abs(val.Value);
            decimal horas = Math.Floor(num / 60);
            decimal minutos = (num % 60);

            return (val.Value < 0 ? "-" : "") + horas.ToString("00") + ":" + minutos.ToString("00");
        }

        private int? DiffTimeMinutes(DateTime previsto, DateTime realizado)
        {
            if (previsto == DateTime.MinValue || realizado == DateTime.MinValue)
                return null;

            return (int)(previsto - realizado).TotalMinutes;
        }

        #endregion
    }
}
