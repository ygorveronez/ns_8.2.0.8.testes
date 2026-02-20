namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_GERACAO_CTE_ANULACAO_COMPONENTE_FRETE", EntityName = "ControleGeracaoCTeAnulacaoComponenteFrete", Name = "Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete", NameType = typeof(ControleGeracaoCTeAnulacaoComponenteFrete))]
    public class ControleGeracaoCTeAnulacaoComponenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]

        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleGeracaoCTeAnulacao", Column = "CGA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleGeracaoCTeAnulacao ControleGeracaoCTeAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGC_INCLUIR_BASE_CALCULO_ICMS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGC_INCLUIR_TOTAL_RECEBER", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncluirTotalReceber { get; set; }
    }
}
