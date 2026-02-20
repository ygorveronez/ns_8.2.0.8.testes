namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTransbordo
    {
        Todas = 0,
        AgInformacoes = 1,
        EmTransporte = 2,
        Finalizado = 3,
        Cancelado = 4,
        AgIntegracao = 5,
        FalhaIntegracao = 6
    }

    public static class SituacaoTransbordoHelper
    {
        public static string ObterDescricao(this SituacaoTransbordo situacaoTransbordo)
        {
            switch (situacaoTransbordo)
            {
                case SituacaoTransbordo.AgInformacoes: return "Ag. Informações da Carga";
                case SituacaoTransbordo.EmTransporte: return "Em Transporte";
                case SituacaoTransbordo.Finalizado: return "Finalizado";
                case SituacaoTransbordo.Cancelado: return "Cancelado";
                case SituacaoTransbordo.AgIntegracao: return "Ag. Integração";
                case SituacaoTransbordo.FalhaIntegracao: return "Falha na Integração";
                default: return string.Empty;
            }
        }
    }
}
