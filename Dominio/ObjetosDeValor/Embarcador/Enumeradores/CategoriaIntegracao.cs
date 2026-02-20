namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CategoriaIntegracao
    {
        Sistema = 1,
        GerenciadoraRisco = 2,
        Rastreamento = 3,
        Pedagio = 4,
        CIOT = 5,
        Embarcador = 6,
        Ocorrencia = 7,
        Comunicacao = 8,
        Outros = 9
    }

    public static class CategoriaIntegracaoHelper
    {
        public static string ObterDescricao(this CategoriaIntegracao categoria)
        {
            switch (categoria)
            {
                case CategoriaIntegracao.Sistema: return "Sistemas";
                case CategoriaIntegracao.GerenciadoraRisco: return "Gerenciadoras de Risco";
                case CategoriaIntegracao.Rastreamento: return "Rastreamento";
                case CategoriaIntegracao.Pedagio: return "Pedágio";
                case CategoriaIntegracao.CIOT: return "CIOT";
                case CategoriaIntegracao.Embarcador: return "Embarcador";
                case CategoriaIntegracao.Ocorrencia: return "Ocorrências";
                case CategoriaIntegracao.Comunicacao: return "Comunicação";
                case CategoriaIntegracao.Outros: return "Outros";
                default: return string.Empty;
            }
        }

        public static string ObterIcone(this CategoriaIntegracao categoria)
        {
            switch (categoria)
            {
                case CategoriaIntegracao.Sistema: return "fa fa-desktop";
                case CategoriaIntegracao.GerenciadoraRisco: return "fa-exclamation-triangle";
                case CategoriaIntegracao.Rastreamento: return "fa-map-marker";
                case CategoriaIntegracao.Pedagio: return "fa-credit-card";
                case CategoriaIntegracao.CIOT: return "fa fa-ticket";
                case CategoriaIntegracao.Embarcador: return "fa fa-ship";
                case CategoriaIntegracao.Ocorrencia: return "fa fa-calendar";
                case CategoriaIntegracao.Comunicacao: return "fa fa-envelope";
                case CategoriaIntegracao.Outros: return "fa-list-ul";

                default: return string.Empty;
            }
        }
    }
}