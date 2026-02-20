//using Google.OrTools.ConstraintSolver;

//namespace Google.OrTools.Api.Services.GoogleOrTools
//{
//    /// <summary>
//    /// Um retorno de chamada que calcula o volume de uma demanda armazenada em uma matriz de números inteiros.
//    /// </summary>
//    public class DemandaCallback : NodeEvaluator2
//    {
//        public DemandaCallback(double[] order_demands)
//        {
//            this.order_demands = order_demands;
//        }

//        private double[] order_demands;

//        public override long Run(int first_index, int second_index)
//        {
//            if (first_index < order_demands.Length)
//                return (long)order_demands[first_index];
//            return 0;
//        }
//    }
//}