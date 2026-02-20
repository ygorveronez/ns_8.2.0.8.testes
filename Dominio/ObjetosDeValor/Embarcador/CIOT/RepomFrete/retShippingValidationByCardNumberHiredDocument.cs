namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retShippingValidationByCardNumberHiredDocument : retPadrao
    {
        public retShippingValidationByCardNumberHiredDocumentResult Result { get; set; }
    }

    public class retShippingValidationByCardNumberHiredDocumentResult
    {
        public bool CardValidate { get; set; }

        public bool PayCardActivationFee { get; set; }

        public decimal? ValueCardActivationFee { get; set; }
    }
}