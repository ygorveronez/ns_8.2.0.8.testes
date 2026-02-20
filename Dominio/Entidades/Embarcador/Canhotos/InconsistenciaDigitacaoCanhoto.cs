using System;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INCONSISTENCIA_DIGITACAO_CANHOTO", EntityName = "InconsistenciaDigitacaoCanhoto", Name = "Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto", NameType = typeof(InconsistenciaDigitacaoCanhoto))]
    public class InconsistenciaDigitacaoCanhoto : EntidadeBase, IEquatable<InconsistenciaDigitacaoCanhoto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CID_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_OBSERVACOES", TypeType = typeof(string), NotNull = false, Length = 400)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Canhoto", Column = "CNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Canhoto Canhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoInconsistenciaDigitacao", Column = "CMI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoInconsistenciaDigitacao MotivoInconsistenciaDigitacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual bool Equals(InconsistenciaDigitacaoCanhoto other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
