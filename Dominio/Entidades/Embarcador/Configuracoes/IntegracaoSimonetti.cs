
namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SIMONETTI", EntityName = "IntegracaoSimonetti", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti", NameType = typeof(IntegracaoSimonetti))]
    public class IntegracaoSimonetti : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoSimonetti", Column = "CIS_POSSUI_INTEGRACAO_SIMONETTI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoSimonetti { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLEnviaOcorrenciaSimonetti", Column = "CIS_URL_ENVIO_OCORRENCIA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLEnviaOcorrenciaSimonetti { get; set; }
    }

}