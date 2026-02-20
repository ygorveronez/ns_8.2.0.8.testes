using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DUPLICATA_PARCELAS", EntityName = "DuplicataParcelas", Name = "Dominio.Entidades.DuplicataParcelas", NameType = typeof(DuplicataParcelas))]
    public class DuplicataParcelas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Duplicata", Column = "DUP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Duplicata Duplicata { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Parcela", Column = "DPA_PARCELA", TypeType = typeof(int), NotNull = true)]
        public virtual int Parcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "DPA_VALOR", TypeType = typeof(decimal), NotNull = true, Scale = 6, Precision = 18)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVcto", Column = "DPA_DATA_VCTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVcto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoDeConta PlanoDeConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPgto", Column = "DPA_VALOR_PGTO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorPgto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPgto", Column = "DPA_DATA_PGTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPgto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "DPA_STATUS", TypeType = typeof(Enumeradores.StatusDuplicata), NotNull = true)]
        public virtual Enumeradores.StatusDuplicata Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoBaixa", Column = "DPA_OBS_BAIXA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoBaixa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "DPA_FUNCIONARIO_BAIXA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }
    }
}
