using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_RESPONSAVEL", EntityName = "CargaResponsavel", Name = "Dominio.Entidades.Embarcador.Logistica.CargaResponsavel", NameType = typeof(CargaResponsavel))]
    public class CargaResponsavel : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        public virtual string Descricao { get { return Codigo.ToString(); } }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CategoriaResponsavel", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CategoriaResponsavel CategoriaResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VigenciaInicio", Column = "CRS_VIGENCIA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VigenciaFinal", Column = "CRS_VIGENCIA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CRS_FUNCIONARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_RESPONSAVEL_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Filiais.Filial> Filiais { get; set; }
    }
}
