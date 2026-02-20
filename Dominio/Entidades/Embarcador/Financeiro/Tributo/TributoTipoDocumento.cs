using System;

namespace Dominio.Entidades.Embarcador.Financeiro.Tributo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRIBUTO_TIPO_DOCUMENTO", EntityName = "TributoTipoDocumento", Name = "Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoTipoDocumento", NameType = typeof(TributoTipoDocumento))]
    public class TributoTipoDocumento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoTipoDocumento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TTD_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TTD_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TTD_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TributoTipoImposto", Column = "TTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TributoTipoImposto TributoTipoImposto { get; set; }

        public virtual bool Equals(TributoTipoDocumento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoCompleta
        {
            get
            {
                return this.Descricao + " (" + this.CodigoIntegracao + ")";
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Situacao)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
