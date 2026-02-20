using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CONECTTEC", EntityName = "IntegracaoConecttec", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec", NameType = typeof(IntegracaoConecttec))]
    public class IntegracaoConecttec : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIC_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIC_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProviderID", Column = "CIC_PROVIDER_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ProviderID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SecretKEY", Column = "CIC_SECRET_KEY", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SecretKEY { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BrokerPort", Column = "CIC_BROKER_PORT", TypeType = typeof(int), Length = 500, NotNull = false)]
        public virtual int BrokerPort { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StationID", Column = "CIC_STATION_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string StationID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenURLRecebimento", Column = "CIC_TOKEN_URL_RECEBIMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenURLRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLRecebimentoCallback", Column = "CIC_URL_RECEBIMENTO_CALLBACK", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLRecebimentoCallback { get; set; }
    }
}
