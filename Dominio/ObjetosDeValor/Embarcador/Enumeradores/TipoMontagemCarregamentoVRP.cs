namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMontagemCarregamentoVRP
    {
        Nenhum = 0,
        VrpCapacity = 1,
        VrpTimeWindows = 2,
        SimuladorFrete = 3,
        Prioridades = 4
    }

    public enum SimuladorFreteCriterioSelecaoTransportador
    {
        Nenhum = 0,
        MenorValor = 1,
        MenorPrazoEntrega = 2
    }
}
