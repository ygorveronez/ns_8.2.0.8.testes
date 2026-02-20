using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class ProgramacaoLogistica
    {
        public int Codigo { get; set; }
        public string Motorista { get; set; }
        public string SituacaoColaborador { get; set; }
        public string PeriodoSituacaoColaborador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.Cores CorSituacaoColaborador { get; set; }
        public string Veiculos { get; set; }
        public string Manutencao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
        public string TipoCarroceria { get; set; }
        public string TipoVeiculo { get; set; }
        public string TipoPlotagem { get; set; }
        public DateTime DataUltimaEntragaGuarita { get; set; }
        public string ObservacaoUltimaEntragaGuarita { get; set; }
        public int Ociosidade { get; set; }

        public string DescricaoSituacaoCarga
        {
            get { return this.SituacaoCarga.ObterDescricao(); }
        }

        public string DescricaDataUltimaEntragaGuarita
        {
            get { return this.DataUltimaEntragaGuarita > DateTime.MinValue ? this.DataUltimaEntragaGuarita.ToString("dd/MM/yyyy HH:mm") : ""; }
        }
    }
}
