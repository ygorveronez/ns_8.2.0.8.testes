namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento
{
    public sealed class RetornoIntegracaoRecebimento
    {
        #region Propriedades

        public string Error { get; set; }

        public string Mensagem { get; set; }

        #endregion Propriedades

        #region Métodos Públicos

        public string ObterMensagemRetorno()
        {
            if (!string.IsNullOrWhiteSpace(Error))
                return Error.Trim();

            return !string.IsNullOrWhiteSpace(Mensagem) ? Mensagem.Trim() : "";
        }

        #endregion Métodos Públicos
    }
}
