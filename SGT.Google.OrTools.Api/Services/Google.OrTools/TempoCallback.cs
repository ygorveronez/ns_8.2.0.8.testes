//using Google.OrTools.ConstraintSolver;
//using System;

//namespace Google.OrTools.Api.Services.GoogleOrTools
//{
//    /// <summary>
//    /// Distância de Manhattan implementada como retorno de chamada. Ele usa uma série de
//    /// posiciona e calcula a distância de Manhattan entre os dois
//    /// posições de dois índices diferentes.
//    /// </summary>
//    public class TempoCallback : NodeEvaluator2
//    {
//        public TempoCallback(Position[] locations, int coefficient)
//        {
//            this.locations = locations;
//            this.coefficient = coefficient;
//        }

//        private Position[] locations;
//        private int coefficient;

//        public override long Run(int first_index, int second_index)
//        {
//            if (first_index >= locations.Length || second_index >= locations.Length)
//                return 0;
//            ////Utilizado para calcular na função callback
//            return (long)(Math.Abs(locations[first_index].Longitude - locations[second_index].Longitude) + Math.Abs(locations[first_index].Latitude - locations[second_index].Latitude)) * coefficient;          
//        }
//    }
//}