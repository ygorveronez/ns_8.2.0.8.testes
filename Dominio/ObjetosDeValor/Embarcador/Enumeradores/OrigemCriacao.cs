namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemCriacao
    {
        WebService = 0,
        Operador = 1
    }

    public static class OrigemCriacaoHelper
    {
        public static string ObterDescricao(this OrigemCriacao origemCriacao)
        {
            switch (origemCriacao)
            {
                case OrigemCriacao.WebService: return "WebService";
                case OrigemCriacao.Operador: return "Operador";
                default: return string.Empty;
            }
        }
    }
}