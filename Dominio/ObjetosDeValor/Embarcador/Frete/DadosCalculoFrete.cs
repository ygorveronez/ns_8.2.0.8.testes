using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class DadosCalculoFrete
    {
        public DadosCalculoFrete()
        {
            Componentes = new List<DadosCalculoFreteComponente>();
            ComposicaoFrete = new List<ComposicaoFrete>();
            ComposicaoValorBaseFrete = new List<ComposicaoFrete>();
        }

        public bool FreteCalculado { get; set; }
        public bool FreteCalculadoComProblemas { get; set; }
        public string MensagemRetorno { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorFreteResidual { get; set; }
        public decimal PercentualSobreNF { get; set; }
        public decimal ValorFixo { get; set; }
        public decimal PercentualPagamentoAgregado { get; set; }
        public decimal QuantidadeHoras { get; set; }
        public decimal QuantidadeHorasExcedentes { get; set; }
        public string Observacao { get; set; }

        public List<ComposicaoFrete> ComposicaoFrete { get; set; }
        public List<ComposicaoFrete> ComposicaoValorBaseFrete { get; set; }

        public string ObservacaoTerceiro { get; set; }
        public string ObservacaoContratoFrete { get; set; }
        public string TextoAdicionalContratoFrete { get; set; }
        public bool ReterImpostosContratoFrete { get; set; }
        public int DiasVencimentoAdiantamentoContratoFrete { get; set; }
        public int DiasVencimentoSaldoContratoFrete { get; set; }
        public List<DadosCalculoFreteComponente> Componentes { get; set; }
        public int CodigoCargaPedido { get; set; }
        public bool UtilizaMoedaEstrangeira { get; set; }
        public decimal ValorCotacaoMoeda { get; set; }
        public Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }
        public Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }
        public decimal ValorFreteMoeda { get; set; }

        public decimal ValorBase { get; set; }

        public decimal ValorTotalComponentes
        {
            get { return Componentes?.Sum(o => o.ValorComponente) ?? 0m; }
        }
        public Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        public virtual DadosCalculoFrete Clonar()
        {
            return (DadosCalculoFrete)this.MemberwiseClone();
        }

        public decimal ValorTotal
        {
            get { return (Componentes?.Sum(o => o.ValorComponente) ?? 0m) + this.ValorFrete; }
        }

        public decimal BaseCalculoICMS
        {
            get { return (Componentes?.Where(o => o.IncluirBaseCalculoICMS).Sum(o => o.ValorComponente) ?? 0m) + this.ValorFrete; }
        }

        public decimal ValorComponentesNaoIncluirBaseCalculoICMS
        {
            get { return (Componentes?.Where(o => !o.IncluirBaseCalculoICMS).Sum(o => o.ValorComponente) ?? 0m); }
        }

        public decimal ValorComponentesIncluirBaseCalculoICMS
        {
            get { return (Componentes?.Where(o => o.IncluirBaseCalculoICMS).Sum(o => o.ValorComponente) ?? 0m); }
        }

        public int LeadTime { get; set; }

        public List<DadosCalculoFrete> Variacoes { get; set; }

        public Dictionary<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete, List<ComposicaoFrete>> ComposicoesVariacao { get; set; }
    }
}
