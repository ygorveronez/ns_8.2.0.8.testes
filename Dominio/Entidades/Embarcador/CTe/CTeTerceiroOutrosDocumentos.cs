using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_OUTROS_DOCUMENTOS", EntityName = "CTeTerceiroOutrosDocumentos", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos", NameType = typeof(CTeTerceiroOutrosDocumentos))]

    public class CTeTerceiroOutrosDocumentos : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]

        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CSO_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CSO_NUMERO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CSO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CSO_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "CSO_NCM", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NCM { get; set; }

        public virtual bool Equals(CTeTerceiroOutrosDocumentos other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual CTeTerceiroOutrosDocumentos Clonar()
        {
            return (CTeTerceiroOutrosDocumentos)this.MemberwiseClone();
        }
    }
}
