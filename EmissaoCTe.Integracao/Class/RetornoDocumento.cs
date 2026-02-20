namespace EmissaoCTe.Integracao
{
    public class RetornoDocumento<T>
    {
        public T Objeto { get; set; }

        public bool Status { get; set; }

        public string Mensagem { get; set; }

        public string Documento { get; set; }
    }
}