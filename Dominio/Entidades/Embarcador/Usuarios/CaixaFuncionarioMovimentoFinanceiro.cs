namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CAIXA_FUNCIONARIO_MOVIMENTO_FINANCEIRO", EntityName = "CaixaFuncionarioMovimentoFinanceiro", Name = "Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro", NameType = typeof(CaixaFuncionarioMovimentoFinanceiro))]
    public class CaixaFuncionarioMovimentoFinanceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentoFinanceiro", Column = "MOV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.MovimentoFinanceiro MovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentoFinanceiroDebitoCredito", Column = "MDC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.MovimentoFinanceiroDebitoCredito MovimentoFinanceiroDebitoCredito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CaixaFuncionario", Column = "CAF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CaixaFuncionario CaixaFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEntradaSaida", Column = "CFM_ENTRADA_SAIDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida TipoEntradaSaida { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
