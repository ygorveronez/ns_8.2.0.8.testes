namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCIOT
    {
        Aberto = 0,
        Encerrado = 1,
        Cancelado = 2,
        AgIntegracao = 3,
        Pendencia = 4,
        /// <summary>
        /// Após o encerramento/quitação do CIOT, para algumas operadoras é necessário autorizar o pagamento
        /// </summary>
        PagamentoAutorizado = 5,
        AgLiberarViagem = 6
    }

    public static class SituacaoCIOTHelper
    {
        public static string ObterDescricao(this SituacaoCIOT? situacao)
        {
            if (!situacao.HasValue)
                return string.Empty;

            return situacao.Value.ObterDescricao();
        }

        public static string ObterDescricao(this SituacaoCIOT situacao)
        {
            switch (situacao)
            {
                case SituacaoCIOT.Aberto: return "Aberto";
                case SituacaoCIOT.Encerrado: return "Encerrado";
                case SituacaoCIOT.Cancelado: return "Cancelado";
                case SituacaoCIOT.AgIntegracao: return "Ag. Integração";
                case SituacaoCIOT.Pendencia: return "Pendência";
                case SituacaoCIOT.PagamentoAutorizado: return "Pagamento Autorizado";
                case SituacaoCIOT.AgLiberarViagem: return "Ag. Liberar Viagem";
                default: return string.Empty;
            }
        }
    }
}
