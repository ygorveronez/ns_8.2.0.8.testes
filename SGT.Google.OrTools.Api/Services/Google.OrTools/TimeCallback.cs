//using Google.OrTools.ConstraintSolver;
//using System;

//namespace Google.OrTools.Api.Services.GoogleOrTools
//{
//    public class TimeCallback : NodeEvaluator2
//    {
//        public TimeCallback(NodeEvaluator2 tempo_atendimento, Position[] locations)
//        {
//            this._tempos_atend = tempo_atendimento;
//            this.locations = locations;
//            var cnn = new Repository.DBConnection.ConexaoORACLE();
//            int i = 0;
//            int j = 0;
//            try
//            {
//                cnn.Connect();
//                custos = new long[locations.Length][];
//                for (i = 0; i < locations.Length; i++)
//                {
//                    custos[i] = new long[locations.Length];
//                    for (j = 0; j < locations.Length; j++)
//                    {
//                        if (i == j)
//                            custos[i][j] = 0;
//                        else
//                        {
//                            var tmp = cnn.ExecuteScalar("select d.tempo_min from distancia_ender d where d.ender_id_orig = :orig and d.ender_id_dest = :dest", locations[i].codigo, locations[j].codigo);
//                            if (tmp != null)
//                                custos[i][j] = long.Parse(tmp.ToString());
//                            else
//                            {
//                                var metros = (long)Geo.DistanciaRaio(locations[i].Latitude, locations[i].Longitude, locations[j].Latitude, locations[j].Longitude);
//                                custos[i][j] = (long)((metros / 1000) * 40) / 60;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex) { throw ex; }
//            finally { cnn.Disconnect(); }
//        }

//        NodeEvaluator2 _tempos_atend;
//        private Position[] locations;
//        private long[][] custos;

//        public override long Run(int i, int j)
//        {
//            if (i >= locations.Length || j >= locations.Length)
//                return 0;
//            else if (i == j)
//                return 0;
//            else
//                return this._tempos_atend.Run(i, j) + custos[i][j];
//        }
//    }
//}