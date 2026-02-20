namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.DadosTransporteMaritimo
{
    public sealed class RetornoDadosTransporteMaritimoRecebimento
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
