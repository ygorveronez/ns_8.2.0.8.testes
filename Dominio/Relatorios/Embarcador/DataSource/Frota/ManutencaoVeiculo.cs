using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class ManutencaoVeiculo
    {
        #region Propriedades

        public int CodigoServico { get; set; }
        public string DescricaoServico { get; set; }
        public MotivoServicoVeiculo MotivoServico { get; set; }
        public string ObservacaoServico { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoEquipamento { get; set; }
        public int KmAtualVeiculo { get; set; }
        public string PlacaVeiculo { get; set; }
        public int ValidadeKM { get; set; }
        public int ToleranciaKM { get; set; }
        public int ValidadeDias { get; set; }
        public int ToleranciaDias { get; set; }
        public TipoServicoVeiculo TipoServico { get; set; }
        public int NumeroOS { get; set; }
        public string ObservacaoOS { get; set; }
        private DateTime DataUltimaExecucao { get; set; }
        public int QuilometragemUltimaExecucao { get; set; }
        private int ExecucaoUnica { get; set; }
        public string Equipamento { get; set; }
        public int HorimetroAtual { get; set; }
        public int HorimetroUltimaExecucao { get; set; }
        private int ToleranciaHorimetro { get; set; }
        public int ValidadeHorimetro { get; set; }
        private int SituacaoUltimaOS { get; set; }
        public string MarcaVeiculo { get; set; }
        public string ModeloVeiculo { get; set; }
        public string SegmentoVeiculo { get; set; }
        public string ResponsavelVeiculo { get; set; }
        public string CentroResultado { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DT_RowColor
        {
            get
            {
                if ((TipoServico == TipoServicoVeiculo.PorDia && DiaRestante < 0) || (TipoServico == TipoServicoVeiculo.PorHorimetro && HorimetroRestante < 0) || (TipoServico == TipoServicoVeiculo.PorKM && KMRestante < 0))
                    return CorGrid.Salmon;
                else if ((TipoServico == TipoServicoVeiculo.Ambos && DiaRestante < 0 && KMRestante < 0) || (TipoServico == TipoServicoVeiculo.PorHorimetroDia && DiaRestante < 0 && HorimetroRestante < 0))
                    return CorGrid.Salmon;
                else if (TipoServico == TipoServicoVeiculo.Todos && DiaRestante < 0 && HorimetroRestante < 0 && KMRestante < 0)
                    return CorGrid.Salmon;

                else if ((TipoServico == TipoServicoVeiculo.PorDia && DataProximaTroca != DateTime.MinValue && DiaRestante < ToleranciaDias) ||
                        (TipoServico == TipoServicoVeiculo.PorHorimetro && HorimetroRestante < ToleranciaHorimetro) || (TipoServico == TipoServicoVeiculo.PorKM && KMRestante < ToleranciaKM))
                    return CorGrid.Amarelo;
                else if ((TipoServico == TipoServicoVeiculo.Ambos && ((DataProximaTroca != DateTime.MinValue && DiaRestante < ToleranciaDias) || KMRestante < ToleranciaKM)) ||
                        (TipoServico == TipoServicoVeiculo.PorHorimetroDia && ((DataProximaTroca != DateTime.MinValue && DiaRestante < ToleranciaDias) || HorimetroRestante < ToleranciaHorimetro)))
                    return CorGrid.Amarelo;
                else if (TipoServico == TipoServicoVeiculo.Todos && ((DataProximaTroca != DateTime.MinValue && DiaRestante < ToleranciaDias) || HorimetroRestante < ToleranciaHorimetro || KMRestante < ToleranciaKM))
                    return CorGrid.Amarelo;

                else
                    return CorGrid.Verde;
            }
        }

        public int KMProximaTroca
        {
            get
            {
                return this.QuilometragemUltimaExecucao + this.ValidadeKM;
            }
        }

        public int KMRestante
        {
            get
            {
                return this.KMProximaTroca - this.KmAtualVeiculo;
            }
        }

        private DateTime DataProximaTroca
        {
            get
            {
                return this.DataUltimaExecucao != null && this.DataUltimaExecucao > DateTime.MinValue ? this.DataUltimaExecucao.AddDays(this.ValidadeDias) : DateTime.MinValue;
            }
        }

        public string DescricaoDataProximaTroca
        {
            get
            {
                return this.DataProximaTroca != null && this.DataProximaTroca > DateTime.MinValue ? this.DataProximaTroca.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public double DiaRestante
        {
            get
            {
                return this.DataProximaTroca != null && this.DataProximaTroca > DateTime.MinValue ? (this.DataProximaTroca - DateTime.Now.Date).TotalDays : 0;
            }
        }

        public int HorimetroProximaTroca
        {
            get
            {
                return this.HorimetroUltimaExecucao + this.ValidadeHorimetro;
            }
        }

        public int HorimetroRestante
        {
            get
            {
                return this.HorimetroProximaTroca - this.HorimetroAtual;
            }
        }

        public string DescricaoDataUltimaExecucao
        {
            get
            {
                if (this.DataUltimaExecucao > DateTime.MinValue)
                    return this.DataUltimaExecucao.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }

        public string DescricaoTipoServico
        {
            get { return TipoServico.ObterDescricao(); }
        }

        public string DescricaoMotivoServico
        {
            get { return MotivoServico.ObterDescricao(); }
        }

        public string TipoManutencao
        {
            get
            {
                if (this.ExecucaoUnica == 1)
                {
                    if (this.ValidadeKM <= this.KmAtualVeiculo)
                        return "Corretiva, execução única";
                    else
                        return "Preventiva, execução única";
                }
                else
                {
                    if ((this.TipoServico == TipoServicoVeiculo.Ambos || this.TipoServico == TipoServicoVeiculo.PorDia ||
                        this.TipoServico == TipoServicoVeiculo.Todos || this.TipoServico == TipoServicoVeiculo.PorHorimetroDia) &&
                    this.DataUltimaExecucao.AddDays(this.ValidadeDias) <= DateTime.Now.Date)
                        return "Corretiva";
                    else if ((this.TipoServico == TipoServicoVeiculo.Ambos || this.TipoServico == TipoServicoVeiculo.PorKM || this.TipoServico == TipoServicoVeiculo.Todos) &&
                    (this.QuilometragemUltimaExecucao + this.ValidadeKM) <= this.KmAtualVeiculo)
                        return "Corretiva";
                    else
                        return "Preventiva";
                }
            }
        }

        public string DescricaoSituacaoUltimaOS
        {
            get
            {
                if (SituacaoUltimaOS >= 0)
                    return SituacaoUltimaOS.ToString().ToEnum<SituacaoOrdemServicoFrota>().ObterDescricao();
                else
                    return string.Empty;
            }
        }

        #endregion
    }
}
