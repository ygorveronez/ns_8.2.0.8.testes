namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAjusteConfiguracaoDescargaCliente
    {
        AgAprovacao = 0,
        SemRegraAprovacao = 1,
        RejeitadaAutorizacao = 2,
        Aprovada = 3,
    }

    public static class SituacaoAjusteConfiguracaoDescargaClienteHelper
    {
        public static string ObterCor(this SituacaoAjusteConfiguracaoDescargaCliente situacao)
        {
            switch (situacao)
            {
                case SituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao: return CorGrid.Amarelo;
                case SituacaoAjusteConfiguracaoDescargaCliente.RejeitadaAutorizacao: return CorGrid.Vermelho;
                case SituacaoAjusteConfiguracaoDescargaCliente.Aprovada: return CorGrid.Success;
                default: return CorGrid.Branco;
            }
        }

        public static string ObterDescricao(this SituacaoAjusteConfiguracaoDescargaCliente situacao)
        {
            switch (situacao)
            {
                case SituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao: return "Aguardando Aprovação";
                case SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao: return "Sem Regra Aprovação";
                case SituacaoAjusteConfiguracaoDescargaCliente.RejeitadaAutorizacao: return "Rejeitada Autorização";
                case SituacaoAjusteConfiguracaoDescargaCliente.Aprovada: return "Aprovada";
                default: return string.Empty;
            }
        }
    }
}
