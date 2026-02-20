namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_AGREGADO_ABASTECIMENTO", EntityName = "PagamentoAgregadoAbastecimento", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento", NameType = typeof(PagamentoAgregadoAbastecimento))]

    public class PagamentoAgregadoAbastecimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Abastecimento", Column = "ABA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Abastecimento Abastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PagamentoAgregado", Column = "PAA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado PagamentoAgregado { get; set; }
    }
}


