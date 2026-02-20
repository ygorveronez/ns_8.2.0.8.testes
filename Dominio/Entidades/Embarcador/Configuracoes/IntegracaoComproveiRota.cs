using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_COMPROVEI_ROTA", EntityName = "IntegracaoComproveiRota", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota", NameType = typeof(IntegracaoComproveiRota))]
    public class IntegracaoComproveiRota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CCR_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CCR_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLBaseRest", Column = "CCR_URL_BASE_REST", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLBaseRest { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CCR_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CCR_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }
    }
}
