using Google.OrTools.Api.Models;
using Google.OrTools.ConstraintSolver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Google.OrTools.Api.Services.GoogleOrTools
{
    /// <summary>
    ///   Roteamento de veículo com janelas de tempo....
    ///   Amostra mostrando como modelar e resolver um roteamento capacitado de veículos
    ///   Problema com as janelas de tempo usando a versão envolvida do veículo
    ///   biblioteca de roteamento em src/constraint_solver.
    ///   https://github.com/google/or-tools/blob/master/examples/dotnet/csharp-netfx/cscvrptw.cs
    /// </summary>
    public class CVRPTW
    {
        public List<Resultado> Resolver(Models.VrpCapacity problema)//, int TimeLimitMs = 60000) // 1 min
        {
            //https://developers.google.com/optimization/routing/vrptw
            // https://github.com/google/or-tools/blob/39f44709bba203f5ff3bc18fab8098739f189a6d/ortools/constraint_solver/samples/cvrptw.py#L138-L171

            //Mínimo 30 segundos
            if (problema.TimeLimitMs < 30000)
                problema.TimeLimitMs = 30000;

            int indiceDeposito = problema.Locais.FindIndex(x => x.Deposito);

            //Vamos guardar a quantidade original de veiculos disponíveis, pois ao montar o VrpCapacityData podemos aumentar
            // a quantidade de veiculos para conseguir atender toda a demanda...
            int totalVeiculos = (from veiculo in problema.Veiculos select veiculo.Quantidade).Sum();

            VrpCapacityData data = new VrpCapacityData(problema, true);

            // Create Routing Index Manager
            // ( Criar gerenciador de índice de roteamento ) 
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

                  if (fromNode == toNode)
                      return 0;

                  return data.DistanceMatrix[fromNode][toNode];
              }
            );

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Add Capacity constraint.
            int demandCallbackIndex = routing.RegisterUnaryTransitCallback(
              (long fromIndex) =>
              {
                  // Convert from routing variable Index to demand NodeIndex.
                  var fromNode = manager.IndexToNode(fromIndex);
                  //Arredondando para cima...
                  return (int)System.Math.Ceiling(data.Demands[fromNode]);
              }
            );

            routing.AddDimensionWithVehicleCapacity(demandCallbackIndex,
                                                    0,                        // null capacity slack
                                                    data.VehicleCapacities,   // vehicle maximum capacities
                                                    true,                     // start cumul to zero
                                                    "Capacity");

            if (problema.QtdeMaximaEntregas == 0)
                problema.QtdeMaximaEntregas = (from veiculo in problema.Veiculos select veiculo.QtdeMaximaEntregas).Max();

            if (problema.QtdeMaximaEntregas > 0)
            {
                // Add Counter constraint.
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

            //# Add Time Window constraint
            // Create and register a transit callback.
            // ( Crie e registre um retorno de chamada de trânsito. ) 
            int transitTimeCallbackIndex = routing.RegisterTransitCallback(
                (long fromIndex, long toIndex) =>
                {
                    // Convert from routing variable Index to distance matrix NodeIndex.
                    // ( Converta de índice de variável de roteamento para matriz de distância NodeIndex. ) 
                    var fromNode = manager.IndexToNode(fromIndex);
                    var toNode = manager.IndexToNode(toIndex);
                    int serv_time = problema.Locais[fromNode].Janela.time;
                    
                    //#66718 - Removido.. Joel.. PPC
                    serv_time = 0;
                    if (fromNode == toNode)
                        return 0;

                    if (problema.DesconsiderarTempoDeslocamentoDeposito && (indiceDeposito == fromNode || indiceDeposito == toNode))
                        return (indiceDeposito == toNode ? serv_time : 0);

                    return data.DurationMatrix[fromNode][toNode] + serv_time;
                }
                );

            //Adding time dimension constraints.
            string time = "Time";
            long horizon = 24 * 60;// 3600;
            if (problema.TempoMaxRota > 0)
                horizon = problema.TempoMaxRota;

            //Note: In this case fix_start_cumul_to_zero is set to False,
            // because some vehicles start their routes after time 0, due to resource constraints.
            // Add a dimension for time and a limit on the total time_horizon
            routing.AddDimension(transitTimeCallbackIndex, // total time function callback
                                 30,       //# allow waiting time
                                 horizon,  //# maximum time per vehicle
                                 false,    //# don't force start cumul to zero since we are giving TW to start nodes
                                 time);

            RoutingDimension timeDimension = routing.GetMutableDimension(time);

            // TODO: Problema PCP pedido 25162, não carrega de jeito nenhum...
            //// Allow to drop nodes.// Tempo * Distãncia.. custo total... (minumo 6 horas)
            //// https://developers.google.com/optimization/routing/penalties
            //for (int i = 1; i < data.DistanceMatrix.GetLength(0); ++i)
            //    routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, (horizon >= 480 ? horizon : 480) * 1000);

            ////Alterado .. pois na Embare.. mais entregas com menos demanda de veiculos.. não gerar nenhum carregamento.
            //long penalty = (from maior in problema.Veiculos select maior.Capacidade).Max() * 100;
            //if (penalty < 1000000) penalty = 1000000;

            //for (int i = 1; i < data.DistanceMatrix.GetLength(0); ++i)
            //    routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, penalty);

            //////#70354 PPC, Querem gerar carregamento e deixar pedidos de fora... quando atinge o limite de tempo de entrega...
            ///// https://stackoverflow.com/questions/63275072/how-do-i-use-google-or-tools-to-add-disjunctions-set-penalties-and-prevent-cer
            /// Para tornar o local "obrigatório", você deve usar o valor int64 máximo ( ), uma vez que o solver não pode estourar, ele o proibirá de descartar esse local.0x7FFFFFFFFFFFFFF
            long penalty = Int64.MaxValue;
            for (int i = 1; i < data.DistanceMatrix.GetLength(0); i++)
                routing.AddDisjunction(new long[] { manager.NodeToIndex(i) }, penalty);

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
                    var index = manager.NodeToIndex(i);
                    List<long> indices = new List<long>();
                    for (int j = 0; j < data.VehicleModelo.Length; j++)
                    {
                        if (problema.Locais[i].VeiculosRestritos.Contains(data.VehicleModelo[j]))
                            indices.Add(-1);
                        else
                            indices.Add(j);// + 1); Problema EMBARE.. quando apenas 1 veiculo.. não gerava carregamento mesmo não tendo restrição do modelo.
                    }
                    routing.VehicleVar(index).SetValues(indices.ToArray());
                }
            }

            // Add time window constraints for each location except depot and 'copy' the slack var in the solution object (aka Assignment) to print it
            // ( Adicione restrições de janela de tempo para cada local, exceto depósito e 'copie' a var do slack no objeto de solução (também conhecido como Atribuição) para imprimi-lo)
            for (int i = 1; i < data.TimeWindows.GetLength(0); ++i)
            {
                int[] timeWindow = data.TimeWindows[i];
                long index = manager.NodeToIndex(i);
                timeDimension.CumulVar(index).SetRange(timeWindow[0], timeWindow[1]);
                routing.AddToAssignment(timeDimension.SlackVar(index));
            }

            // Add time window constraints for each vehicle start node and 'copy' the slack var in the solution object (aka Assignment) to print it
            // ( Adicione restrições de janela de tempo para cada nó inicial do veículo e 'copie' a var do slack no objeto de solução (também conhecido como Atribuição) para imprimi-lo )
            for (int i = 0; i < data.Vehicle.Length; ++i)
            {
                long index = routing.Start(i);
                int[] janelaDeposito = data.TimeWindows[data.Depot];
                timeDimension.CumulVar(index).SetRange(
                    janelaDeposito[0],
                    janelaDeposito[1]
                );
                routing.AddToAssignment(timeDimension.SlackVar(index));
            }

            // Setting first solution heuristic.
            // Configurando a primeira solução heurística.
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = problema.Strategy; // FirstSolutionStrategy.Types.Value.PathCheapestArc;
            //searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Protobuf.WellKnownTypes.Duration { Seconds = problema.TimeLimitMs / 1000 };

            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);

            if (solution != null)
            {
                List<Resultado> resultado = new List<Resultado>();

                //RoutingDimension timeDimensionResult = routing.GetMutableDimension("Time");

                RoutingDimension capacity_dimension = routing.GetDimensionOrDie("Capacity");
                RoutingDimension time_dimension = routing.GetDimensionOrDie("Time");

                // Inspect solution.
                //long totalTime = 0;
                for (int i = 0; i < data.Vehicle.Length; ++i)
                {
                    Models.Veiculo veiculo = problema.Veiculos.Find(x => x.Codigo == data.Vehicle[i]);
                    Resultado item = new Resultado(veiculo);
#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("Route for Vehicle {0}:", i);
#endif
                    var index = routing.Start(i);
                    int timeRota = 0;
                    double distance = 0;
                    while (routing.IsEnd(index) == false)
                    {
                        var time_var = time_dimension.CumulVar(index);
                        var load_var = capacity_dimension.CumulVar(index);
                        var slack_var = time_dimension.SlackVar(index);

                        int local = manager.IndexToNode(index);
                        long qtde = solution.Value(load_var);
                        if (index < problema.Locais.Count)
                            qtde = (long)problema.Locais[(int)index].PesoTotal;

                        long min = solution.Min(time_var);
                        long max = solution.Max(time_var);
                        long slack_min = solution.Min(slack_var);
                        long slack_maxx = solution.Max(slack_var);

                        TimeWindow janela = new TimeWindow(0, 0, 0);
#if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("{0} Load({1}) Time({2},{3}) Slack({4},{5}) -> ", local, qtde, min, max, slack_min, slack_maxx);
#endif

                        //index = manager.IndexToNode(index);
                        int previous_index = (int)index;
                        index = solution.Value(routing.NextVar(index));
                        long metros = routing.GetArcCostForVehicle(previous_index, index, i);

                        if (indiceDeposito != (int)previous_index && (int)previous_index < problema.Locais.Count)
                            item.itens.Add(new ResultadoItens() { item = problema.Locais[(int)previous_index], tempo = (min - timeRota), distancia = metros });

                        timeRota = (int)min;
                        distance += metros;
                    }

                    var load_varr = capacity_dimension.CumulVar(index);
                    var time_varr = time_dimension.CumulVar(index);
                    var slack_varr = time_dimension.SlackVar(index);
#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("{0} Load({1}) Time({2},{3})", manager.IndexToNode(index), solution.Value(load_varr), solution.Min(time_varr), solution.Max(time_varr));
                    System.Diagnostics.Debug.WriteLine("Distance of the route: {0}m", distance);
                    System.Diagnostics.Debug.WriteLine("Load of the route: {0}", solution.Value(load_varr));
                    System.Diagnostics.Debug.WriteLine("Time of the route: {0}", solution.Value(time_varr));
#endif

                    //total_distance += distance
                    //total_load += assignment.Value(load_var)
                    //total_time += assignment.Value(time_var)

                    var endTimeVar = time_dimension.CumulVar(index);

#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("{0} Time({1},{2})", manager.IndexToNode(index), solution.Min(endTimeVar), solution.Max(endTimeVar));
                    System.Diagnostics.Debug.WriteLine("Time of the route: {0}min", solution.Min(endTimeVar));
#endif
                    //totalTime += solution.Min(endTimeVar);

                    if (item.itens.Count > 0)
                        resultado.Add(item);

                    if (resultado.Count >= totalVeiculos && !problema.GerarCarregamentosExtras)
                        break;

                }

                return resultado;

            }
            else
                return null;
        }
    }
}