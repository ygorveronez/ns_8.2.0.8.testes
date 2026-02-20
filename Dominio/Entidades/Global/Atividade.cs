using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATIVIDADES", EntityName = "Atividade", Name = "Dominio.Entidades.Atividade", NameType = typeof(Atividade))]
    public class Atividade: EntidadeBase 
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ATI_DESCRICAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContribuinte", Column = "ATI_CONTRIBUINTE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOPDentroEstado", Column = "ATI_CFOPDENTRO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CFOPDentroEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOPForaEstado", Column = "ATI_CFOPFORA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CFOPForaEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "ATI_DATAATU", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacao { get; set; }
    }
}
