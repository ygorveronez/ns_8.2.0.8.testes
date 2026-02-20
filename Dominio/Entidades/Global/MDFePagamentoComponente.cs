using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_PAGAMENTO_COMPONENTE", EntityName = "MDFePagamentoComponente", Name = "Dominio.Entidades.MDFePagamentoComponente", NameType = typeof(MDFePagamentoComponente))]
    public class MDFePagamentoComponente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MDFeInformacoesBancarias", Column = "MIB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.MDFeInformacoesBancarias InformacoesBancarias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponente", Column = "MPC_TIPO_COMPONENTE", TypeType = typeof(TipoComponentePagamento), Length = 100, NotNull = false)]
        public virtual TipoComponentePagamento? TipoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "MPC_VALOR_COMPONENTE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? ValorComponente { get; set; }
    }
}
