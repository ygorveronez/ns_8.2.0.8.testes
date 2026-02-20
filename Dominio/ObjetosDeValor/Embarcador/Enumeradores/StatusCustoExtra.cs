namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusCustoExtra
    {
        Abonado = 0,
        Faturado = 1,
        PassThrough = 2,
        EmAberto = 3,
        Nenhum = 4,
    }

    public static class EnumStatusCustoExtraeHelper
    {
        public static string ObterDescricao(this StatusCustoExtra situacao)
        {
            switch (situacao)
            {
                case StatusCustoExtra.Abonado: return "Abonado";
                case StatusCustoExtra.Faturado: return "Faturado";
                case StatusCustoExtra.PassThrough: return "Pass Through";
                case StatusCustoExtra.EmAberto: return "Em Aberto";
                case StatusCustoExtra.Nenhum: return "Nenhum";
                default: return string.Empty;
            }
        }
    }
}
