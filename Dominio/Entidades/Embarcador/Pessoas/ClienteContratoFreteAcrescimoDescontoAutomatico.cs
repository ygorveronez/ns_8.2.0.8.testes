namespace Dominio.Entidades.Embarcador.Pessoas
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_CONTRATO_FRETE_ACRESCIMO_DESCONTO_AUTOMATICO", EntityName = "ClienteContratoFreteAcrescimoDescontoAutomatico", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico", NameType = typeof(ClienteContratoFreteAcrescimoDescontoAutomatico))]
    public class ClienteContratoFreteAcrescimoDescontoAutomatico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteAcrescimoDescontoAutomatico", Column = "CFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico AcrescimoDescontoAutomatico { get; set; }
    }
}
