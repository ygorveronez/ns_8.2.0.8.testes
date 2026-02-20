using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Mobile
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_MOBILE", EntityName = "ConfiguracaoMobile", Name = "AdminMultisoftware.Dominio.Entidades.Mobile.ConfiguracaoMobile", NameType = typeof(ConfiguracaoMobile))]
    public class ConfiguracaoMobile : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OneSignalAppId", Column = "CMO_ONE_SIGNAL_APP_ID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string OneSignalAppId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OneSignalApiKey", Column = "CMO_ONE_SIGNAL_API_KEY", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string OneSignalApiKey { get; set; }

        /// <summary>
        /// Versão atual do aplicativo que está na loja.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoAppLoja", Column = "CMO_VERSAO_APP_LOJA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string VersaoAppLoja { get; set; }

    }
}
