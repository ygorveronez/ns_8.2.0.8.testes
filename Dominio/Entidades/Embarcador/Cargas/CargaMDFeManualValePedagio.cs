namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_VALE_PEGADIO", EntityName = "CargaMDFeManualValePedagio", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio", NameType = typeof(CargaMDFeManualValePedagio))]
    public class CargaMDFeManualValePedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJFornecedor", Column = "CMV_CNPJ_FORNECEDOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJResponsavel", Column = "CMV_CNPJ_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroComprovante", Column = "CMV_NUMERO_COMPROVANTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAgendamentoPorto", Column = "CMV_CODIGO_AGENDAMENTO_PORTO", TypeType = typeof(string), Length = 16, NotNull = false)]
        public virtual string CodigoAgendamentoPorto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagio", Column = "CMV_VALOR_VALE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorValePedagio { get; set; }

    }
}
