using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaRelatorioExtratoAcertoViagem
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int Motorista { get; set; }
        public int CentroResultado { get; set; }
        public int Veiculo { get; set; }
        public int SegmentoVeiculo { get; set; }
        public List<int> SituacaoAcerto { get; set; }
        public string TipoLancamento { get; set; }
        public List<int> Justificativas { get; set; }

        #region Propriedades com Regras

        public int Justificativa
        {
            set { if (value > 0) Justificativas = new List<int>() { value }; }
        }

        #endregion
    }
}