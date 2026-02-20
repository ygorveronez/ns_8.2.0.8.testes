namespace EmissaoCTe.Integracao
{
    public class Retorno<T>
    {
        public T Objeto { get; set; }

        public bool Status { get; set; }

        public string Mensagem { get; set; }
    }
}