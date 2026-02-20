namespace Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota
{
    public class Baseline
    {
        public string Codigo { get; }
        public int CodigoTipoBaseline { get; }
        public string TipoBaseline { get; }
        public string Valor { get; }

        public Baseline(string codigo, int codigoTipoBaseline, string tipoBaseline, string valor)
        {
            Codigo = codigo;
            CodigoTipoBaseline = codigoTipoBaseline;
            TipoBaseline = tipoBaseline;
            Valor = valor;
        }
    }
}
