namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_VALE_PEDAGIO", EntityName = "ValePedagioCTe", Name = "Dominio.Entidades.ValePedagioCTe", NameType = typeof(ValePedagioCTe))]
    public class ValePedagioCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJFornecedor", Column = "VPC_CNPJ_FORNECEDOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJResponsavel", Column = "VPC_CNPJ_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroComprovante", Column = "VPC_NUMERO_COMPROVANTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagio", Column = "VPC_VALOR_VALE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorValePedagio { get; set; }
    }
}
