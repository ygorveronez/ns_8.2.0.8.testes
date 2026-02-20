namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRegistroIntegracaoCTeCanhoto
    {
        SemTipo = 0,
        Imagem = 1,
        Confirmacao = 2,
    }

    public static class TipoRegistroIntegracaoCTeCanhotoHelper
    {
        public static string ObterDescricao(this TipoRegistroIntegracaoCTeCanhoto tipo)
        {
            switch (tipo)
            {
                case TipoRegistroIntegracaoCTeCanhoto.SemTipo : return "Sem Tipo";
                case TipoRegistroIntegracaoCTeCanhoto.Imagem : return "Imagem";
                case TipoRegistroIntegracaoCTeCanhoto.Confirmacao : return "Confirmação";
                default: return string.Empty;
            }
        }
    }
}
