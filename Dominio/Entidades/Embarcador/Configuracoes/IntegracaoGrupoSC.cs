using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_GRUPOSC", EntityName = "IntegracaoGrupoSC", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoSC", NameType = typeof(IntegracaoGrupoSC))]
    public class IntegracaoGrupoSC : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIG_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [Obsolete("Migrado para URLIntegracao", true)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_INTEGRACAO_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [Obsolete("Utilizando o mesmo de URLIntegracao", true)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_URL_INTEGRACAO_NFSE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoNfse { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiKey", Column = "CIG_API_KEY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ApiKey { get; set; }
    }
}
