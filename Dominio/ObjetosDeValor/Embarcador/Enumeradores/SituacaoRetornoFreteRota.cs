namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRetornoFreteRota
    {
        Todos = 0,
        FreteValido = 1,
        TabelaNaoEncontrada = 2,
        NaoEncontrouTipoCarga = 3,
        NaoEncontrouModeloVeicularCarga = 4,
        MaisQueUmaTabelaParaRota = 5
    }
}
