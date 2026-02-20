namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AutorizacaoOcorrenciaPagamento
    {
        Remetente = 1,
        Destinatario = 2,
        ConfirmarPagador = 3
    }

    public static class AutorizacaoOcorrenciaPagamentoHelper
    {
        public static string ObterDescricao(this AutorizacaoOcorrenciaPagamento opcao)
        {
            switch (opcao)
            {
                case AutorizacaoOcorrenciaPagamento.Remetente: return "Remetente";
                case AutorizacaoOcorrenciaPagamento.Destinatario: return "Destinatário";
                case AutorizacaoOcorrenciaPagamento.ConfirmarPagador: return "Confirmar Pagador";
                default: return string.Empty;
            }
        }
    }
}
