using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MIC_DTA", EntityName = "IntegracaoMicDta", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta", NameType = typeof(IntegracaoMicDta))]
    public class IntegracaoMicDta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CMD_URL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetodoManifestacaoEmbarca", Column = "CMD_METODO_MANIFESTACAO_EMBARCA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MetodoManifestacaoEmbarca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LicencaTNTI", Column = "CMD_LICENCA_TNTI", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LicencaTNTI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VencimentoLicencaTNTI", Column = "CMD_VENCIMENTO_LICENCA_TNTI", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VencimentoLicencaTNTI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_GERAR_INTEGRACAO_NA_ETAPA_DO_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarIntegracaNaEtapaDoFrete { get; set; }
    }
}
