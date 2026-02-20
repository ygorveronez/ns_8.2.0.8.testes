using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ComposicaoFrete
    {
        public ComposicaoFrete()
        {
        }

        public ComposicaoFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoParametro)
        {
            TipoParametro = tipoParametro;
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete TipoParametro { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }
        public decimal Valor { get; set; }
        public string Formula { get; set; }
        public string DescricaoComponente { get; set; }
        public int CodigoComponente { get; set; }
        public string ValoresFormula { get; set; }
        public decimal ValorCalculado { get; set; }

        public string Descricao
        {
            get { return (TipoParametro == Enumeradores.TipoParametroBaseTabelaFrete.ComponenteFrete || TipoParametro == Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido) ? DescricaoComponente : TipoParametro.ObterDescricao(); }
        }
    }
}
