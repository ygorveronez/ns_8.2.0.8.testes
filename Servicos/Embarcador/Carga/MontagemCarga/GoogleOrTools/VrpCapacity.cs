//using Google.OrTools.ConstraintSolver;
//using System.Collections.Generic;

//namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
//{
//    public class VrpCapacity
//    {
//        /// Locais que representam um local de pedido ou uma rota de veículo
//        /// start/end. locations_
//        private Position[] _locais_entrega;
//        /// Quantidade a receber para cada pedido. order_demands_
//        //private double[] _demanda_entregas;
//        private long[] _demanda_entregas;
//        /// Custo de penalidade "pago" por deixar um pedido. order_penalties_
//        private int[] _tempo_descarga;
//        /// Capacidade dos veículos. vehicle_capacity_
//        private long[] _capac_veiculos;

//        private string _servidorOSRM;

//        /// <summary>
//        /// Cria dados de pedidos. A localização da ordem é de acordo com o parâmetro dos pedidos, bem como a sua demanda (quantidade), janela de tempo e penalidade.
//        /// </summary>
//        /// <param name="local_deposito">Localização geográfica do depósito.</param>
//        /// <param name="pedidos">Lista de locais de entrega com suas respectivas quantidades..</param>
//        /// <param name="qtde_veiculos"></param>
//        public void Construir(List<Pedido> pedidos, long[] capac_veiculos, string servidorOSRM)
//        {
//            _locais_entrega = new Position[pedidos.Count];
//            _demanda_entregas = new long[pedidos.Count];
//            _tempo_descarga = new int[pedidos.Count];
//            for (int i = 0; i < pedidos.Count; ++i)
//            {
//                _locais_entrega[i] = new Position(pedidos[i].id, pedidos[i].latitude, pedidos[i].longitude);
//                _demanda_entregas[i] = pedidos[i].peso_total;
//                _tempo_descarga[i] = pedidos[i].tempo_desc_min;
//            }
//            _capac_veiculos = capac_veiculos;
//            _servidorOSRM = servidorOSRM;
//        }

//        public List<Resultado> Resolver(GoogleOrTools.EnumFirstSolutionStrategy fss, EnumCargaTpRota tp_rota, int index_deposito = 0)
//        {
//            DistanceCallback data = new DistanceCallback(_locais_entrega, tp_rota, _servidorOSRM);

//            // Create Routing Index Manager
//            RoutingIndexManager manager = new RoutingIndexManager(_locais_entrega.Length, _capac_veiculos.Length, index_deposito);

//            // Create Routing Model.
//            RoutingModel routing = new RoutingModel(manager);

//            // Create and register a transit callback.
//            int transitCallbackIndex = routing.RegisterTransitCallback(
//              (long fromIndex, long toIndex) =>
//              {
//                  // Convert from routing variable Index to distance matrix NodeIndex.
//                  var fromNode = manager.IndexToNode(fromIndex);
//                  var toNode = manager.IndexToNode(toIndex);
//                  return data.DistanceMatrix[fromNode][toNode];
//              }
//            );

//            // Define cost of each arc.
//            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

//            // Add Capacity constraint.
//            int demandCallbackIndex = routing.RegisterUnaryTransitCallback(
//              (long fromIndex) =>
//              {
//                  // Convert from routing variable Index to demand NodeIndex.
//                  var fromNode = manager.IndexToNode(fromIndex);
//                  return _demanda_entregas[fromNode];
//              }
//            );
//            routing.AddDimensionWithVehicleCapacity(demandCallbackIndex, 0,     // null capacity slack
//                                                    _capac_veiculos,            // vehicle maximum capacities
//                                                    true,                       // start cumul to zero
//                                                    "Capacity");

//            // Setting first solution heuristic.
//            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
//            searchParameters.FirstSolutionStrategy = (FirstSolutionStrategy.Types.Value)fss;
//            searchParameters.TimeLimit = new Google.Protobuf.WellKnownTypes.Duration();
//            searchParameters.TimeLimit.Seconds = 60;

//            // Solve the problem.
//            Assignment solution = routing.SolveWithParameters(searchParameters);
//            if (solution == null)
//                return null;

//            List<Resultado> resultado = new List<Resultado>();
//            long totalDistance = 0;
//            long totalLoad = 0;
//            for (int i = 0; i < _capac_veiculos.Length; ++i)
//            {
//                Resultado item = new Resultado(i);
//                //Console.WriteLine("Route for Vehicle {0}:", i);
//                long routeDistance = 0;
//                long routeLoad = 0;
//                var index = routing.Start(i);
//                while (routing.IsEnd(index) == false)
//                {
//                    long nodeIndex = manager.IndexToNode(index);
//                    var qtde = _demanda_entregas[nodeIndex];
//                    routeLoad += qtde;
//                    //Console.Write("{0} Load({1}) -> ", nodeIndex, routeLoad);
//                    var previousIndex = index;
//                    index = solution.Value(routing.NextVar(index));
//                    var dist = routing.GetArcCostForVehicle(previousIndex, index, i);
//                    routeDistance += dist;// routing.GetArcCostForVehicle(previousIndex, index, 0);
//                    if (index < _demanda_entregas.Length)
//                    {
//                        qtde = _demanda_entregas[index];
//                        item.itens.Add(new ResultadoItens() { item = index, qtde = qtde, distancia = dist, tempo = 0 });
//                    }
//                }
//                //Console.WriteLine("{0}", manager.IndexToNode((int)index));
//                //Console.WriteLine("Distance of the route: {0}m", routeDistance);
//                totalDistance += routeDistance;
//                totalLoad += routeLoad;

//                //Não carregou nada...
//                if (routeDistance == 0 && item.itens.Count == 1)
//                    item.itens.RemoveAt(0);

//                resultado.Add(item);
//            }
//            //Console.WriteLine("Total distance of all routes: {0}m", totalDistance);
//            //Console.WriteLine("Total load of all routes: {0}m", totalLoad);
//            return resultado;
//        }
//    }
//}
