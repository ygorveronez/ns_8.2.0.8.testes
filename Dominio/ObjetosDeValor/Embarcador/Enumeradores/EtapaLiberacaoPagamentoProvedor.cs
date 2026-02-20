namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaLiberacaoPagamentoProvedor
    {
        Todos = 0,
        DetalhesCarga = 1,
        DocumentoProvedor = 2,
        Aprovacao = 3,
        Liberacao = 4,
    }

    public static class EtapaLiberacaoPagamentoProvedoHelper
    {
        public static string ObterDescricao(this EtapaLiberacaoPagamentoProvedor etapa)
        {
            switch (etapa)
            {
                case EtapaLiberacaoPagamentoProvedor.Todos: return "Todos";
                case EtapaLiberacaoPagamentoProvedor.DetalhesCarga: return "Detalhes da Carga";
                case EtapaLiberacaoPagamentoProvedor.DocumentoProvedor: return "Documentos do Provedor";
                case EtapaLiberacaoPagamentoProvedor.Aprovacao: return "Aprovação";
                case EtapaLiberacaoPagamentoProvedor.Liberacao: return "Liberação";
                default: return string.Empty;
            }
        }
    }
}
