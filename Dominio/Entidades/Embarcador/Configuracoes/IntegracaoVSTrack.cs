using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Configuracoes
{

    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_VSTRACK", EntityName = "IntegracaoVSTrack", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVSTrack", NameType = typeof(IntegracaoVSTrack))]
    public class IntegracaoVSTrack : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLProducao", Column = "CVS_URL_PRODUCAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLProducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLHomologacao", Column = "CVS_URL_HOMOLOGACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrantType", Column = "CVS_GRANT_TYPE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string GrantType { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Username", Column = "CVS_USERNAME", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Username { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Password", Column = "CVS_PASSWORD", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Password { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoEtapa1Carga", Column = "CVS_INTEGRACAO_ETAPA_1_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoEtapa1Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoEtapa6Carga", Column = "CVS_INTEGRACAO_ETAPA_6_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoEtapa6Carga { get; set; }

    }
}

