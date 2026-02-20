namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCalculoPercentualViagem
    {
        EntregasRealizadas = 1,
        ProximidadeEntrePosicaoVeiculoRotaPrevista = 2,
        DistanciaRotaPrevistaVersusDistanciaRotaRealizada = 3,
        DistanciaRotaRestanteAteDestino = 4
    }

    public static class TipoCalculoPercentualViagemHelper
    {
        public static string ObterDescricao(this TipoCalculoPercentualViagem tipo)
        {
            switch (tipo)
            {
                case TipoCalculoPercentualViagem.EntregasRealizadas: return "Entregas realizadas";
                case TipoCalculoPercentualViagem.ProximidadeEntrePosicaoVeiculoRotaPrevista: return "Proximidade entre posição do veículo e rota prevista";
                case TipoCalculoPercentualViagem.DistanciaRotaPrevistaVersusDistanciaRotaRealizada: return "Distância da rota prevista versus distância da rota realizada";
                case TipoCalculoPercentualViagem.DistanciaRotaRestanteAteDestino: return "Distância da rota restante até destino";
                default: return string.Empty;
            }
        }
    }
}
