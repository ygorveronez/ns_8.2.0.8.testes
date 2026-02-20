using Google.OrTools.ConstraintSolver;
using System.Collections.Generic;

namespace Google.OrTools.Api.Models
{
    public class Problem
    {
        public Problem()
        {
            this.Strategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            this.TimeLimitMs = 10000;
            this.GerarCarregamentosExtras = true;
            //this.ConsiderarTempoDeslocamentoPrimeiraEntrega = true;
        }

        /// <summary>
        /// Gera carregamentos extras alem da disponibilidade da frota....
        /// </summary>
        public bool GerarCarregamentosExtras { get; set; }

        ///// <summary>
        ///// Propriedade utilizanda na Opção Janela de tempo se é para considerar 
        ///// o tempo de deslocamento do depósito até a primeira entrega.
        ///// </summary>
        //public bool DesconsiderarTempoDeslocamentoPrimeiraEntrega { get; set; }
        public bool DesconsiderarTempoDeslocamentoDeposito { get; set; }

        public string ServerOsrm { get; set; }
        
        public List<Services.GoogleOrTools.Position> Locais { get; set; }
        
        public FirstSolutionStrategy.Types.Value Strategy { get; set; }
        
        public EnumTipoRota TipoRota { get; set; }
        
        public int TimeLimitMs { get; set; }

        /// <summary>
        /// Tempo máximo em rota em minutos, utilizado para o algoritmo CVRPTW.
        /// Quando não possuir, será considerado como padrão 1440 (24 horas)  
        /// </summary>
        public int TempoMaxRota { get; set; }

        /// <summary>
        /// Quantidade máxima de paradas de entregas na mesma carga.
        /// </summary>
        public int QtdeMaximaEntregas { get; set; }

    }
}