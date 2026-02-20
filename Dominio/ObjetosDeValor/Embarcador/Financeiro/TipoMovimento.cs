using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class TipoMovimento
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public PlanoDeConta PlanoEntrada { get; set; }
        public PlanoDeConta PlanoSaida { get; set; }
        public SituacaoAtivoPesquisa Situacao { get; set; }
        public List<CentroResultado> CentrosResultados { get; set; }
    }
}
