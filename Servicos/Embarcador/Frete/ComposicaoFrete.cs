namespace Servicos.Embarcador.Frete

{
    public class ComposicaoFrete
    {
        public static void InformarComposicaoFrete(ref Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, string formula, string valoresFormula, decimal valor)
        {
            composicao.Formula += formula;
            composicao.ValoresFormula += valoresFormula;
            composicao.Valor = valor;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete ObterComposicaoFrete(string formula, string valoresFormula, decimal valor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoParametro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor, int codigoComponente)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
            composicao.Formula = formula;
            composicao.ValoresFormula = valoresFormula;
            composicao.Valor = valor;
            composicao.TipoParametro = tipoParametro;
            composicao.TipoValor = tipoValor;
            composicao.CodigoComponente = codigoComponente;
            return composicao;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete ObterComposicaoFrete(string formula, string valoresFormula, decimal valor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoParametro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor, string descricaoComponente, int codigoComponente, decimal valorCalculado)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = new Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete();
            composicao.Formula = formula;
            composicao.ValoresFormula = valoresFormula;
            composicao.Valor = valor;
            composicao.TipoParametro = tipoParametro;
            composicao.ValorCalculado = valorCalculado;
            composicao.TipoValor = tipoValor;
            composicao.DescricaoComponente = descricaoComponente;
            composicao.CodigoComponente = codigoComponente;

            return composicao;
        }
    }
}
