namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retShippingValidationByHiredDocument : retPadrao
    {
        public retShippingValidationByHiredDocumentResult Result { get; set; }
    }

    public class retShippingValidationByHiredDocumentResult
    {
        public bool HiredValidate { get; set; }

        public string DateValidateRNTRC { get; set; }

        public string Message { get; set; }
    }
}