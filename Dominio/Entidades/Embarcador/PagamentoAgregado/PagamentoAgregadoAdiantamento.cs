namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_AGREGADO_ADIANTAMENTO", EntityName = "PagamentoAgregadoAdiantamento", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento", NameType = typeof(PagamentoAgregadoAdiantamento))]
    public class PagamentoAgregadoAdiantamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }        

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS PagamentoMotoristaTMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PagamentoAgregado", Column = "PAA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado PagamentoAgregado { get; set; }
    }
}
