namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDirecionamentoCustoExtra
    {
        Nenhum = 0,
        Faturar = 1,
        Abonar = 2,
        PassThrought = 3
    }

    public static class TipoDirecionamentoCustoExtraHelper
    {
        public static string ObterDescricao(this TipoDirecionamentoCustoExtra etapa)
        {
            switch (etapa)
            {
                case TipoDirecionamentoCustoExtra.Nenhum: return "Nenhum";
                case TipoDirecionamentoCustoExtra.Faturar: return "Faturar";
                case TipoDirecionamentoCustoExtra.Abonar: return "Abonar";
                case TipoDirecionamentoCustoExtra.PassThrought: return "Pass Throught";
                default: return string.Empty;
            }
        }
    }
}
