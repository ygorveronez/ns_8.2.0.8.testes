using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CREDITO_RESPONSAVEL_TEMPORARIO", EntityName = "ResponsavelTemporario", Name = "Dominio.Entidades.Embarcador.Creditos.ResponsavelTemporario", NameType = typeof(ResponsavelTemporario))]
    public class ResponsavelTemporario : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.ResponsavelTemporario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CREDITOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Creditor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CREDITOR_TEMPORARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario CreditorTemporario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "CRT_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "CRT_DATA_FIM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFim { get; set; }

        public virtual bool Equals(ResponsavelTemporario other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
