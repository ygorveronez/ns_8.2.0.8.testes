using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioDescontoAcrescimoFatura
    {
        public int ConhecimentoDeTransporte { get; set; }
        public double Pessoa { get; set; }
        public int Fatura { get; set; }
        public int GrupoPessoa { get; set; }
        public List<int> GruposPessoas { get; set; }
        public DateTime? DataInicialEmissao { get; set; }
        public DateTime? DataFinalEmissao { get; set; }
        public DateTime? DataInicialQuitacao { get; set; }
        public DateTime? DataFinalQuitacao { get; set; }
    }
}
