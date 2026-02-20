namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    public enum EnumTipoTSP { MANHATTAN = 0, RAIO = 1, DISTANCIA = 2 }
    public enum EnumCargaTpRota { ComRetorno = 0, SemRetorno = 1, SemIda = 2 }
    public enum EnumOcupacaoVeiculo { EQUALIZAR = 0, CAPACIDADE_MAX = 1 }
    public enum EnumFirstSolutionStrategy
    {
        Unset = 0,
        //
        // Summary:
        //     --- Variable-based heuristics --- Iteratively connect two nodes which produce
        //     the cheapest route segment.
        GlobalCheapestArc = 1,
        //
        // Summary:
        //     Select the first node with an unbound successor and connect it to the node which
        //     produces the cheapest route segment.
        LocalCheapestArc = 2,
        //
        // Summary:
        //     --- Path addition heuristics --- Starting from a route "start" node, connect
        //     it to the node which produces the cheapest route segment, then extend the route
        //     by iterating on the last node added to the route.
        PathCheapestArc = 3,
        //
        // Summary:
        //     Same as PATH_CHEAPEST_ARC, but arcs are evaluated with a comparison-based selector
        //     which will favor the most constrained arc first. To assign a selector to the
        //     routing model, see RoutingModel::ArcIsMoreConstrainedThanArc() in routing.h for
        //     details.
        PathMostConstrainedArc = 4,
        //
        // Summary:
        //     Same as PATH_CHEAPEST_ARC, except that arc costs are evaluated using the function
        //     passed to RoutingModel::SetFirstSolutionEvaluator() (cf. routing.h).
        EvaluatorStrategy = 5,
        //
        // Summary:
        //     --- Path insertion heuristics --- Make all nodes inactive. Only finds a solution
        //     if nodes are optional (are element of a disjunction constraint with a finite
        //     penalty cost).
        AllUnperformed = 6,
        //
        // Summary:
        //     Iteratively build a solution by inserting the cheapest node at its cheapest position;
        //     the cost of insertion is based on the global cost function of the routing model.
        //     As of 2/2012, only works on models with optional nodes (with finite penalty costs).
        BestInsertion = 7,
        //
        // Summary:
        //     Iteratively build a solution by inserting the cheapest node at its cheapest position;
        //     the cost of insertion is based on the arc cost function. Is faster than BEST_INSERTION.
        ParallelCheapestInsertion = 8,
        //
        // Summary:
        //     Iteratively build a solution by inserting each node at its cheapest position;
        //     the cost of insertion is based on the arc cost function. Differs from PARALLEL_CHEAPEST_INSERTION
        //     by the node selected for insertion; here nodes are considered in decreasing order
        //     of distance to the start/ends of the routes, i.e. farthest nodes are inserted
        //     first. Is faster than SEQUENTIAL_CHEAPEST_INSERTION.
        LocalCheapestInsertion = 9,
        //
        // Summary:
        //     Savings algorithm (Clarke & Wright). Reference: Clarke, G. & Wright, J.W.: "Scheduling
        //     of Vehicles from a Central Depot to a Number of Delivery Points", Operations
        //     Research, Vol. 12, 1964, pp. 568-581
        Savings = 10,
        //
        // Summary:
        //     Sweep algorithm (Wren & Holliday). Reference: Anthony Wren & Alan Holliday: Computer
        //     Scheduling of Vehicles from One or More Depots to a Number of Delivery Points
        //     Operational Research Quarterly (1970-1977), Vol. 23, No. 3 (Sep., 1972), pp.
        //     333-344
        Sweep = 11,
        //
        // Summary:
        //     Select the first node with an unbound successor and connect it to the first available
        //     node. This is equivalent to the CHOOSE_FIRST_UNBOUND strategy combined with ASSIGN_MIN_VALUE
        //     (cf. constraint_solver.h).
        FirstUnboundMinValue = 12,
        //
        // Summary:
        //     Christofides algorithm (actually a variant of the Christofides algorithm using
        //     a maximal matching instead of a maximum matching, which does not guarantee the
        //     3/2 factor of the approximation on a metric travelling salesman). Works on generic
        //     vehicle routing models by extending a route until no nodes can be inserted on
        //     it. Reference: Nicos Christofides, Worst-case analysis of a new heuristic for
        //     the travelling salesman problem, Report 388, Graduate School of Industrial Administration,
        //     CMU, 1976.
        Christofides = 13,
        //
        // Summary:
        //     Iteratively build a solution by constructing routes sequentially, for each route
        //     inserting the cheapest node at its cheapest position until the route is completed;
        //     the cost of insertion is based on the arc cost function. Is faster than PARALLEL_CHEAPEST_INSERTION.
        SequentialCheapestInsertion = 14,
        //
        // Summary:
        //     Lets the solver detect which strategy to use according to the model being solved.
        Automatic = 15
    }
}
