using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FOLHA_INFORMACAO", EntityName = "FolhaInformacao", Name = "Dominio.Entidades.Embarcador.RH.FolhaInformacao", NameType = typeof(FolhaInformacao))]
    public class FolhaInformacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.FolhaInformacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FOI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FOI_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "FOI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        public virtual bool Equals(FolhaInformacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
