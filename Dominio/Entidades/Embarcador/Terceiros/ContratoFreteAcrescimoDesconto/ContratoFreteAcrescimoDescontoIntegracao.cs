namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TERCEIRO_ACRESCIMO_DESCONTO_INTEGRACAO", EntityName = "ContratoFreteAcrescimoDescontoIntegracao", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao", NameType = typeof(ContratoFreteAcrescimoDescontoIntegracao))]
    public class ContratoFreteAcrescimoDescontoIntegracao : Integracao.Integracao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteAcrescimoDesconto", Column = "CAD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteAcrescimoDesconto ContratoFreteAcrescimoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.CIOT CIOT { get; set; }
    }
}
