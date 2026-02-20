namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MINUTA_DEVOLUCAO_CONTAINER", EntityName = "MinutaDevolucaoContainer", Name = "Dominio.Entidades.MinutaDevolucaoContainer", NameType = typeof(MinutaDevolucaoContainer))]
    public class MinutaDevolucaoContainer : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MDC_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Container", Column = "MDC_CONTAINER", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Importador", Column = "MDC_IMPORTADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Importador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Armador", Column = "MDC_ARMADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Armador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEquipamento", Column = "MDC_TIPO_EQUIPAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Navio", Column = "MDC_NAVIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Navio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MDC_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "MDC_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "MDC_PESO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Terminal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MDC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "PMO_VEI_TRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "PMO_VEI_REBOQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Reboque { get; set; }        

    }
}

