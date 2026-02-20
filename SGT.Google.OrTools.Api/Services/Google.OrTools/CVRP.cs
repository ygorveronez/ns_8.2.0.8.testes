using Google.OrTools.Api.Models;
using Google.OrTools.ConstraintSolver;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Google.OrTools.Api.Services.GoogleOrTools
{
    /// <summary>
    ///   Roteamento de veículo 
    ///   Amostra mostrando como modelar e resolver um roteamento capacitado de veículos
    ///   Problema com capacidades de carregamento por veículos.
    ///   biblioteca de roteamento em src/constraint_solver.
    /// </summary>
    public class CVRP
    {

        /// <summary>
        ///     Resolver o problema de roteirização atual.
        /// </summary>
        /// <param name="problema">Parâmetros para resolver</param>
        /// <returns>Retorna a lista de viagens.</returns>
        public List<Resultado> Resolver(Models.VrpCapacity problema)
        {
            // Ordenando por prioridade para tentar priorizar pedidos...
            problema.Locais = problema.Locais.OrderByDescending(x => x.Deposito).ThenBy(x => x.Prioridade).ToList();

            //Mínimo 5 segundos
            if (problema.TimeLimitMs < 5000 || problema.TimeLimitMs == 10000)
                problema.TimeLimitMs = 5000;

            int indiceDeposito = problema.Locais.FindIndex(x => x.Deposito);

            //Vamos guardar a quantidade original de veiculos disponíveis, pois ao montar o VrpCapacityData podemos aumentar
            // a quantidade de veiculos para conseguir atender toda a demanda...
            int totalVeiculos = (from veiculo in problema.Veiculos select veiculo.Quantidade).Sum();

            VrpCapacityData data = new VrpCapacityData(problema);

            // Create Routing Index Manager
            RoutingIndexManager manager = new RoutingIndexManager(data.DistanceMatrix.GetLength(0),
                                                                  data.VehicleCapacities.Length,
                                                                  data.Depot);

            // Create Routing Model.
            RoutingModel routing = new RoutingModel(manager);

            // Create and register a transit callback.
            int transitCallbackIndex = routing.RegisterTransitCallback(
              (long fromIndex, long toIndex) =>
              {
                  // Convert from routing variable Index to distance matrix NodeIndex.
                  var fromNode = manager.IndexToNode(fromIndex);
                  var toNode = manager.IndexToNode(toIndex);
                  return data.DistanceMatrix[fromNode][toNode];
              }
            );

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            int demandCallbackIndex = routing.RegisterUnaryTransitCallback(
             (long fromIndex) =>
             {
                 // Convert from routing variable Index to demand NodeIndex.
                 var fromNode = manager.IndexToNode(fromIndex);
                 return (int)System.Math.Ceiling(data.Demands[fromNode]);
             }
           );

            routing.AddDimensionWithVehicleCapacity(
              demandCallbackIndex,
              0,                        // null capacity slack
              data.VehicleCapacities,   // vehicle maximum capacities
              true,                     // start cumul to zero
              "Capacity");

            if (problema.QtdeMaximaEntregas == 0)
                problema.QtdeMaximaEntregas = (from veiculo in problema.Veiculos select veiculo.QtdeMaximaEntregas).Max();

            if (problema.QtdeMaximaEntregas > 0)
            {
                //// Add Counter constraint.
                //int counterCallbackIndex = routing.RegisterUnaryTransitCallback(
                //  (long fromIndex) =>
                //  {
                //      // Returns 1 for any locations except depot.
                //      // Convert from routing variable Index to demand NodeIndex.
                //      var fromNode = manager.IndexToNode(fromIndex);
                //      return (indiceDeposito != fromNode ? 1 : 0);
                //  }
                //);

                int counterCallbackIndex = routing.RegisterTransitCallback(
                  (long fromIndex, long toIndex) =>
                  {
                      // Convert from routing variable Index to distance matrix NodeIndex.
                      var fromNode = manager.IndexToNode(fromIndex);
                      var toNode = manager.IndexToNode(toIndex);

                      if (fromNode == toNode)
                          return 0;

                      //Se for mesma localização...
                      decimal distancia = (long)Services.Geo.DistanciaRaio(problema.Locais[fromNode].Latitude, problema.Locais[fromNode].Longitude, problema.Locais[toNode].Latitude, problema.Locais[toNode].Longitude);
                      if (distancia <= 20) // Metros...
                          return 0;

                      return (indiceDeposito != fromNode ? 1 : 0);
                  }
                );

                routing.AddDimensionWithVehicleCapacity(counterCallbackIndex,
                                                        0,                              // null counter slack
                                                        data.VehicleMaxStopeds,         // vehicle maximum stopds
                                                        true,                           // start cumul to zero
                                                        "Counter");
            }

            if (problema.Locais.Any(x => x.PedidosConfig.Any(p => p.LimiteCanalEntrega > 0)))
            {
                int counterCanalEntregaCallbackIndex = routing.RegisterTransitCallback(
                                  (long fromIndex, long toIndex) =>
                                  {
                                      // Convert from routing variable Index to distance matrix NodeIndex.
                                      var fromNode = manager.IndexToNode(fromIndex);
                                      var toNode = manager.IndexToNode(toIndex);

                                      if (fromNode == toNode)
                                          return 0;

                                      if (fromNode == indiceDeposito)
                                          return 0;

                                      if ((problema.Locais[fromNode].PedidosConfig?.Count ?? 0) == 0)
                                          return 0;

                                      int limiteCanalDestino = problema.Locais[fromNode].PedidosConfig?.Max(x => x.LimiteCanalEntrega) ?? 0;

                                      return limiteCanalDestino > 0 ? 1 : 0;
                                  }
                                );

                routing.AddDimensionWithVehicleCapacity(counterCanalEntregaCallbackIndex,
                                                        0,                              // null counter slack
                                                        data.VehicleMaxCanalEntrega,    // vehicle maximum stopds
                                                        true,                           // start cumul to zero
                                                        "canalentrega");
            }

            // 2023-03-29 - Adicionado Atributo Prioridade.. para efetuar testes de priorização dos pedidos...
            // Allow to drop nodes.// Precisamos adicionar penalização pois em caso de não ter veiculos suficiente para atender todos os pedidos, não gera nenhum carregamento.
            // https://developers.google.com/optimization/routing/penalties
            long penalty = Int64.MaxValue;
            for (int i = 1; i < data.DistanceMatrix.GetLength(0); ++i)
                routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, penalty);// (10000 * (problema.Locais[i].Prioridade * 1000)));
            //routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, (4800 * 10000 - 99) + problema.Locais[i].Prioridade);

            ////Alterado .. pois na Embare.. mais entregas com menos demanda de veiculos.. não gerar nenhum carregamento.
            //long penalty = (from maior in problema.Veiculos select maior.Capacidade).Max() * 100;
            //if (penalty < 1000000) penalty = 1000000;

            //for (int i = 1; i < data.DistanceMatrix.GetLength(0); ++i)
            //    routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, penalty);

            //Aki vamos validar as restrições de veiculos por local...
            for (int i = 0; i < problema.Locais.Count; i++)
            {
                //https://github.com/google/or-tools/issues/334
                //For API v7.0
                // Supposing you want to allow vehicle 2,3,4 to node 7
                //var index = manager.NodeToIndex(7);
                //routing.VehicleVar(index).SetValues(new int[] { -1, 2, 3, 4 });

                // Possui os códigos dos modelos veiculares restritos...
                if (problema.Locais[i].VeiculosRestritos?.Count > 0)
                {
                    //Localiza  indice do pedido com restrição de veiculo..
                    // Deve-se setar -1 no indice do veiculo que nao pode visitar...
                    List<long> indices = new List<long>();
                    for (int j = 0; j < data.VehicleModelo.Length; j++)
                    {
                        if (problema.Locais[i].VeiculosRestritos.Contains(data.VehicleModelo[j]))
                            indices.Add(-1);
                        else
                            indices.Add(j);// + 1); Problema EMBARE.. quando apenas 1 veiculo.. não gerava carregamento mesmo não tendo restrição do modelo.
                    }
                    var index = manager.NodeToIndex(i);
                    routing.VehicleVar(index).SetValues(indices.ToArray());// new long[] { -1, 2, 3, 4 });
                }
            }

            // Setting first solution heuristic.
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = problema.Strategy;                                                         //  FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Protobuf.WellKnownTypes.Duration { Seconds = problema.TimeLimitMs / 1000 };
            //searchParameters.SolutionLimit = 100000;

            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);

            if (solution != null)
            {
                List<Resultado> resultado = new List<Resultado>();

                // Inspect solution.
                for (int i = 0; i < data.VehicleCapacities.Length; ++i)
                {
                    Models.Veiculo veiculo = problema.Veiculos.Find(x => x.Codigo == data.Vehicle[i]);
                    Resultado item = new Resultado(veiculo);

                    long routeDistance = 0;
                    double routeLoad = 0;
                    var index = routing.Start(i);
                    while (routing.IsEnd(index) == false)
                    {
                        long nodeIndex = manager.IndexToNode(index);
                        double qtde = data.Demands[nodeIndex];
                        routeLoad += qtde;
                        var previousIndex = index;
                        index = solution.Value(routing.NextVar(index));
                        long dist = routing.GetArcCostForVehicle(previousIndex, index, 0);
                        routeDistance += dist;

                        if (indiceDeposito != (int)nodeIndex)
                            item.itens.Add(new ResultadoItens() { item = problema.Locais[(int)nodeIndex], distancia = dist, tempo = 0 });

                    }

                    //Validando a quantidade máxima de viagens do modelo.... ou se permite.. gerar carregamemtos extras...
                    int qtdeCargasModelo = (from viagem in resultado
                                            where viagem.veiculo.CodigoModelo == item.veiculo.CodigoModelo
                                            select viagem).Count();

                    if (item.itens.Count > 0 && (qtdeCargasModelo < veiculo.Quantidade || problema.GerarCarregamentosExtras))
                        resultado.Add(item);

                    //if (resultado.Count >= totalVeiculos && !problema.GerarCarregamentosExtras)
                    //    break;
                }

                return resultado;

            }
            else
                return null;

        }
    }
}