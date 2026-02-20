namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_COMISSAO", EntityName = "ClienteComissao", Name = "Dominio.Entidades.ClienteComissao", NameType = typeof(ClienteComissao))]
    public class ClienteComissao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Parceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "CLC_PERCENTUAL", TypeType = typeof(decimal), NotNull = true, Scale = 6, Precision = 18)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MinimoComissao", Column = "CLC_MINIMO", TypeType = typeof(decimal), NotNull = true, Scale = 6, Precision = 18)]
        public virtual decimal MinimoComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CLC_STATUS", TypeType = typeof(Enumeradores.StatusComissaoCliente), NotNull = false)]
        public virtual Enumeradores.StatusComissaoCliente Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTDA", Column = "CLC_VALOR_TDA", TypeType = typeof(decimal), NotNull = true, Scale = 2, Precision = 18)]
        public virtual decimal ValorTDA { get; set; }
    }
}
