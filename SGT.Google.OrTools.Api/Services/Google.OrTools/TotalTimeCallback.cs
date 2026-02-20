//using Google.OrTools.ConstraintSolver;

//namespace Google.OrTools.Api.Services.GoogleOrTools
//{
//    /// <summary>
//    /// https://developers.google.com/optimization/routing/cvrptw_resources
//    /// </summary>
//    public class TotalTimeCallback : NodeEvaluator2
//    {
//        public TotalTimeCallback(NodeEvaluator2 tempo_atendimento, NodeEvaluator2 distancias, int speed)
//        {
//            _tempos_atend = tempo_atendimento;
//            _distancias = distancias;
//            _speed = speed;
//        }

//        NodeEvaluator2 _tempos_atend;
//        NodeEvaluator2 _distancias;
//        int _speed;

//        public override long Run(int i, int j)
//        {
//            //return this._tempos_atend.Run(i, j) + (((this._distancias.Run(i, j) / this._speed) * 60) / 10);
//            return this._tempos_atend.Run(i, j) + (long)((((double)this._distancias.Run(i, j) / 1000) / this._speed) * 60);
//        }
//    }
//}