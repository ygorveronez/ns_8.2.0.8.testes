using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_CARGA_OPERADOR", EntityName = "LogCargaOperador", Name = "Dominio.Entidades.Embarcador.Cargas.LogCargaOperador", NameType = typeof(LogCargaOperador))]
    public class LogCargaOperador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "lCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRegistroLog", Column = "lCO_DATA_REGISTRO_LOG", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRegistroLog { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ATUAL", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario OperadorLogisticaAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ANTERIOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario OperadorLogisticaAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "lCO_JUSTIFICATIVA", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Justificativa { get; set; }

    }
}
