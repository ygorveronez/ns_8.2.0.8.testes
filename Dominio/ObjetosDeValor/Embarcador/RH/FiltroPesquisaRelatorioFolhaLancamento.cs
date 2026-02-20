using System;

namespace Dominio.ObjetosDeValor.Embarcador.RH
{
    public class FiltroPesquisaRelatorioFolhaLancamento
    {
        public int CodigoFuncionario { get; set; }
        public int CodigoInformacaoFolha { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataCompetenciaInicial { get; set; }
        public DateTime DataCompetenciaFinal { get; set; }
        public string SituacaoFuncionario { get; set; }

    }
}
