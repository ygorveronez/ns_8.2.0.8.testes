//using Google.OrTools.ConstraintSolver;
//using System;

//namespace Google.OrTools.Api.Services.GoogleOrTools
//{
//    /// <summary>
//    /// Manhattan distance implemented as a callback. It uses an array of
//    ///   positions and computes the Manhattan distance between the two
//    ///   positions of two different indices.
//    ///   https://github.com/google/or-tools/blob/master/examples/csharp/cscvrptw.cs
//    /// </summary>
//    public class Manhattan : NodeEvaluator2
//    {
//        public Manhattan(Position[] locations, int coefficient, bool time = false)
//        {
//            this.locations_ = locations;
//            this.coefficient_ = coefficient;
//            this.time_ = time;
//        }

//        public override long Run(int first_index, int second_index)
//        {
//            if (first_index >= locations_.Length || second_index >= locations_.Length)
//            {
//                return 0;
//            }
//            var valor =  (long)(Math.Abs(Math.Abs(locations_[first_index].Longitude * 1000000) - Math.Abs(locations_[second_index].Longitude * 1000000)) + 
//                          Math.Abs(Math.Abs(locations_[first_index].Latitude * 1000000) - Math.Abs(locations_[second_index].Latitude * 1000000))) * coefficient_;
//            if (time_) // em metros... 40km / hora
//                valor = (long)(((60.0 / 40000) * (valor / 10)));// * 60);
//            return valor;
//        }

//        private Position[] locations_;
//        private int coefficient_;
//        private bool time_;
//    };
//}