namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusCarregamentoIntegracao
    {
        Inserir = 1,
        Atualizar = 2,
        Remover = 3
    }

    public static class StatusCarregamentoIntegracaoHelper
    {
        public static string ObterDescricao(this StatusCarregamentoIntegracao situacao)
        {
            switch (situacao)
            {
                case StatusCarregamentoIntegracao.Inserir: return "Inserir";
                case StatusCarregamentoIntegracao.Atualizar: return "Atualizar";
                case StatusCarregamentoIntegracao.Remover: return "Remover";
                default: return string.Empty;
            }
        }

        public static string ObterValorIntegracao(this StatusCarregamentoIntegracao situacao)
        {
            switch (situacao)
            {
                case StatusCarregamentoIntegracao.Inserir: return "I";
                case StatusCarregamentoIntegracao.Atualizar: return "U";
                case StatusCarregamentoIntegracao.Remover: return "D";
                default: return string.Empty;
            }
        }
    }
}
