namespace Google.OrTools.Api.Models
{
    public enum EnumTipoRota { ComRetorno = 0, SemRetorno = 1, SemIda = 2 }
    public enum EnumFirstSolutionStrategy
    {
        Automatic = 0,
        GlobalCheapestArc = 1,
        LocalCheapestArc = 2,
        PathCheapestArc = 3,
        PathMostConstrainedArc = 4,
        EvaluatorStrategy = 5,
        AllUnperformed = 6,
        BestInsertion = 7,
        ParallelCheapestInsertion = 8,
        LocalCheapestInsertion = 9,
        Savings = 10,
        Sweep = 11,
        FirstUnboundMinValue = 12,
        Christofides = 13
    }
    public enum EnumTipoPonto
    {
        Coleta = 1,
        Entrega = 2,
        Pedagio = 3,
        Passagem = 4,
        Retorno = 5,
        Apoio = 6,
        Balanca = 7
    }
}