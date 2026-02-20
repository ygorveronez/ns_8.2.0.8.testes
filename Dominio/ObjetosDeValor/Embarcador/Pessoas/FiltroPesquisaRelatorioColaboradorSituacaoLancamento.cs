using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class FiltroPesquisaRelatorioColaboradorSituacaoLancamento
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int Colaborador { get; set; }
        public int Empresa { get; set; }
        public List<SituacaoLancamentoColaborador> Situacao { get; set; }
        public List<SituacaoColaborador> SituacaoColaborador { get; set; }
        public int Veiculo { get; set; }

    }
}
