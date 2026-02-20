//using Google.OrTools.ConstraintSolver;

//namespace Google.OrTools.Api.Services.GoogleOrTools
//{
//    /// <summary>
//    /// https://developers.google.com/optimization/routing/cvrptw_resources
//    /// </summary>
//    public class ServiceTimeCallback : NodeEvaluator2
//    {
//        public ServiceTimeCallback(int [] tempos_atendimento)
//        {
//            _tempos_atend = tempos_atendimento;
//        }

//        private int[] _tempos_atend;

//        public override long Run(int i, int j)
//        {
//            if (i < 0 || i >= _tempos_atend.Length)
//                return 0;
//            return _tempos_atend[i];
//        }
//    }
//}