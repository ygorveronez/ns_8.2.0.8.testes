namespace EmissaoCTe.API.Models
{
    public class Retorno<T>
    {
        public Retorno(T objeto)
            : base()
        {
            this.Objeto = objeto;
        }

        public Retorno()
        {
            this.SessaoExpirada = false;
        }

        public T Objeto;
        public bool Sucesso;
        public string[] Campos;
        public string Erro;
        public long TotalRegistros;
        public bool SessaoExpirada;
    }
}