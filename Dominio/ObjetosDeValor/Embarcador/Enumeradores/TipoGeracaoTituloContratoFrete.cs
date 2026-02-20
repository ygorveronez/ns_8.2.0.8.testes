namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGeracaoTituloContratoFrete
    {
        /// <summary>
        /// Gera o título do adiantamento e do saldo restante na aprovação do contrato de frete
        /// </summary>
        NaAprovacao = 0,
        /// <summary>
        /// Gera o título do adiantamento na aprovação do contrato e o saldo restante no encerramento do contrato
        /// </summary>
        NaAprovacaoEEncerramento = 1
    }
}
