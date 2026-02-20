using Dominio.Entidades.Embarcador.Usuarios;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OCORRENCIA_CTE_PERFIL_ACESSO", EntityName = "TipoDeOcorrenciaDeCTePerfilAcesso", Name = "Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso", NameType = typeof(TipoDeOcorrenciaDeCTePerfilAcesso))]
    public class TipoDeOcorrenciaDeCTePerfilAcesso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PerfilAcesso", Column = "PAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PerfilAcesso PerfilAcesso { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoDeOcorrenciaDeCTe { get; set; }
    }
}
