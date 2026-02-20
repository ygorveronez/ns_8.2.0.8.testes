namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemMovimentacaoContainer
    {
        UsuarioInterno = 1,
        Tracking = 2,
        PortalTransportador = 3,
        WebService = 5,
        Sistema = 6,
        AlteracaoManual = 7
    }

    public static class OrigemMovimentacaoContainerHelper
    {
        public static string ObterDescricao(this OrigemMovimentacaoContainer origem)
        {
            switch (origem)
            {
                case OrigemMovimentacaoContainer.UsuarioInterno: return "Usuário MultiEmbarcador";
                case OrigemMovimentacaoContainer.AlteracaoManual: return "Usuário MultiEmbarcador";
                case OrigemMovimentacaoContainer.Tracking: return "Tracking";
                case OrigemMovimentacaoContainer.PortalTransportador: return "Portal do Transportador";
                case OrigemMovimentacaoContainer.WebService: return "Web Service";
                case OrigemMovimentacaoContainer.Sistema: return "Sistema";
                default: return string.Empty;
            }
        }
    }
}