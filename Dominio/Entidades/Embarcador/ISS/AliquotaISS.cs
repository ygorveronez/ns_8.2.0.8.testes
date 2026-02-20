using System;

namespace Dominio.Entidades.Embarcador.ISS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALIQUOTA_ISS", EntityName = "AliquotaISS", Name = "Dominio.Entidades.Embarcador.ISS.AliquotaISS", NameType = typeof(AliquotaISS))]
    public class AliquotaISS : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ALI_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ALI_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aliquota", Column = "ALI_ALIQUOTA", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal Aliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALI_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALI_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetemISS", Column = "ALI_RETEM_ISS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetemISS { get; set; }
    }
}