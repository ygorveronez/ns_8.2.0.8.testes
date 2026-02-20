namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONDICAO_PAGAMENTO_FILIAL", EntityName = "CondicaoPagamentoFilial", Name = "Dominio.Entidades.Embarcador.Filiais.CondicaoPagamentoFilial", NameType = typeof(CondicaoPagamentoFilial))]
    public class CondicaoPagamentoFilial : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_DIA_EMISSAO_LIMITE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiaEmissaoLimite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_DIA_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiaMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_DIA_SEMANA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? DiaSemana { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_DIAS_DE_PRAZO_PAGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasDePrazoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_TIPO_PRAZO_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoPagamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoPagamento? TipoPrazoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_VENCIMENTO_FORA_MES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VencimentoForaMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPF_CONSIDERAR_DIA_UTIL_VENCIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDiaUtilVencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual string Descricao
        {
            get { return this.Filial.Descricao + " - Condição Pagamento"; }
        }
    }
}
