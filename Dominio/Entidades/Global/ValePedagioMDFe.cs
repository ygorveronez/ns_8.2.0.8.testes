namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_VALE_PEDAGIO", EntityName = "ValePedagioMDFe", Name = "Dominio.Entidades.ValePedagioMDFe", NameType = typeof(ValePedagioMDFe))]
    public class ValePedagioMDFe: EntidadeBase 
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJFornecedor", Column = "MDV_CNPJ_FORNECEDOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJResponsavel", Column = "MDV_CNPJ_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroComprovante", Column = "MDV_NUMERO_COMPROVANTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAgendamentoPorto", Column = "MDV_CODIGO_AGENDAMENTO_PORTO", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string CodigoAgendamentoPorto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagio", Column = "MDV_VALOR_VALE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCompra", Column = "MDV_TIPO_COMPRA", TypeType = typeof(Dominio.Enumeradores.TipoCompraValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixos", Column = "MDV_QUANTIDADE_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixos { get; set; }

    }
}
