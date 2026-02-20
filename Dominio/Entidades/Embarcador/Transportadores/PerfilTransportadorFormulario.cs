using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_TRANSPORTADOR_FORMULARIO", EntityName = "PerfilTransportadorFormulario", Name = "Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario", NameType = typeof(PerfilTransportadorFormulario))]
    public class PerfilTransportadorFormulario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PerfilAcessoTransportador", Column = "PAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador PerfilAcessoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(Column = "FOR_CODIGO_FORMULARIO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "SomenteLeitura", Column = "PTF_SOMENTE_LEITURA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SomenteLeitura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FormularioPermissaoPersonalizada", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERFIL_TRANSPORTADOR_FORMULARIO_PERMISSAO_PERSONALIZADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PTF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PerfilTransportadorFormularioPermissaoPersonalizada", Column = "FPP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada> FormularioPermissaoPersonalizada { get; set; }

        public virtual string Descricao
        {
            get
            {
                return PerfilAcessoTransportador?.Descricao ?? string.Empty;
            }
        }

    }
}



