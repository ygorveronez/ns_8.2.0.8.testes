namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotivoChamadoGatilhoNaCarga
    {
        AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto = 1,
    }

    public static class TipoMotivoChamadoGatilhoNaCargaHelper
    {
        public static string ObterDescricao(this TipoMotivoChamadoGatilhoNaCarga TipoMotivoChamadoGatilhoNaCarga)
        {
            switch (TipoMotivoChamadoGatilhoNaCarga)
            {
                case TipoMotivoChamadoGatilhoNaCarga.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto: return Localization.Resources.Chamado.MotivoChamado.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto;
                default: return string.Empty;
            }
        }
        public static string ObterDescricaoAuditoria(this TipoMotivoChamadoGatilhoNaCarga TipoMotivoChamadoGatilhoNaCarga)
        {
            switch (TipoMotivoChamadoGatilhoNaCarga)
            {
                case TipoMotivoChamadoGatilhoNaCarga.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto: return "Alteração de Modelo Veícular previsto na carga.";
                default: return string.Empty;
            }
        }
    }
}
