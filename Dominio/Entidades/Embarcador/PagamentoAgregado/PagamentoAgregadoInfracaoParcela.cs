namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_AGREGADO_INFRACAO_PARCELA", EntityName = "PagamentoAgregadoInfracaoParcela", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela", NameType = typeof(PagamentoAgregadoInfracaoParcela))]

    public class PagamentoAgregadoInfracaoParcela : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "InfracaoParcela", Column = "IFP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.InfracaoParcela InfracaoParcela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PagamentoAgregado", Column = "PAA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado PagamentoAgregado { get; set; }
    }
}

