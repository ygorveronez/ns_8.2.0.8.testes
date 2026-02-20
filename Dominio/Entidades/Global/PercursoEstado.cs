using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERCURSO_ESTADO", EntityName = "PercursoEstado", Name = "Dominio.Entidades.PercursoEstado", NameType = typeof(PercursoEstado))]
    public class PercursoEstado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoDestino", Column = "PRC_DESCRICAO_DESTINO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DescricaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERCURSO_ESTADO_ESTADO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosDestino { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.EstadoOrigem?.Nome ?? string.Empty) + " - " + this.DescricaoDestino;
            }
        }
    }
}
