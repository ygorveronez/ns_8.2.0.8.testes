using Google.OrTools.Api.Models;
using Google.OrTools.ConstraintSolver;

namespace Google.OrTools.Api.Services.GoogleOrTools
{
    public class TSP
    {
        /// <summary>
        /// https://developers.google.com/optimization/routing/tsp
        /// </summary>
        /// <param name="problema"></param>
        /// <returns></returns>
        public Resultado Resolver(Tsp problema)
        {
            int indiceDeposito = problema.Locais.FindIndex(x => x.Deposito);

            // Instantiate the data problem.
            TspData data = new TspData(problema);

            // Create Routing Index Manager
            RoutingIndexManager manager = new RoutingIndexManager(problema.Locais.Count, data.VehicleNumber, data.Depot); //data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);

            // Create Routing Model.
            RoutingModel routing = new RoutingModel(manager);

            int transitCallbackIndex = routing.RegisterTransitCallback(
              (long fromIndex, long toIndex) =>
              {
                  // Convert from routing variable Index to distance matrix NodeIndex.
                  var fromNode = manager.IndexToNode(fromIndex);
                  var toNode = manager.IndexToNode(toIndex);
                  return data.DistanceMatrix[fromNode][toNode]; //data.DistanceMatrix[fromNode, toNode];
              }
            );

            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            // Setting first solution heuristic.
            RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = problema.Strategy;                      // FirstSolutionStrategy.Types.Value.PathCheapestArc;

            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);

            if (solution != null)
            {
                Resultado resultado = new Resultado(null);
                long routeDistance = 0;
                var index = routing.Start(0);
                while (routing.IsEnd(index) == false)
                {
                    int proximo = manager.IndexToNode((int)index);
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    long distancia = routing.GetArcCostForVehicle(previousIndex, index, 0);
                    routeDistance += distancia;

                    if (indiceDeposito != proximo)
                        resultado.itens.Add(new ResultadoItens() { item = problema.Locais[proximo], distancia = distancia });
                }
                return resultado;
            }
            else
                return null;

        }
    }
}