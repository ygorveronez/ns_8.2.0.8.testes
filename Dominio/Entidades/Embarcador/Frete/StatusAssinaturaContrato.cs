using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_STATUS_ASSINATURA_CONTRATO", EntityName = "StatusAssinaturaContrato", Name = "Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato", NameType = typeof(StatusAssinaturaContrato))]
    public class StatusAssinaturaContrato : EntidadeBase, IEquatable<StatusAssinaturaContrato>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "STC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "STC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "STC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        public virtual bool Equals(StatusAssinaturaContrato other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
