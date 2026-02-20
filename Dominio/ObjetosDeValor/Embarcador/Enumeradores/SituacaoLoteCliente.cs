namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLoteCliente
    {
        EmCriacao = 1,
        AgIntegracao = 2,
        FalhaIntegracao = 3,
        Finalizado = 4
    }

    public static class SituacaoLoteClienteHelper
    {
        public static string ObterDescricao(this SituacaoLoteCliente situacao)
        {
            switch (situacao)
            {
                case SituacaoLoteCliente.EmCriacao: return "Em Criação";
                case SituacaoLoteCliente.AgIntegracao: return "Ag. Integração";
                case SituacaoLoteCliente.FalhaIntegracao: return "Falha na Integração";
                case SituacaoLoteCliente.Finalizado: return "Finalizado";
                default: return "";
            }
        }
    }
}
