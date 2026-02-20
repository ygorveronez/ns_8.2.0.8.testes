namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PostedKey
    {
        SemTipo = 0,
        Desconto = 21,
        Debito = 29,
        Credito = 31,
    }

    public class PostedKeyHelper
    {
        public static PostedKey ObterTipoPost(string key)
        {
            if (key == "21")
                return PostedKey.Desconto;

            if (key == "29")
                return PostedKey.Debito;

            if (key == "31")
                return PostedKey.Credito;
            
            return PostedKey.SemTipo;
        }
    }
}
