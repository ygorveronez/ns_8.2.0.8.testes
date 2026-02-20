using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ATS_SMART_WEB", EntityName = "IntegracaoATSSmartWeb", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSSmartWeb", NameType = typeof(IntegracaoATSSmartWeb))]
    public class IntegracaoATSSmartWeb : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAW_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CAW_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CAW_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SecretKEY", Column = "CAW_SECRET_KEY", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SecretKEY { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CAW_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CAW_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAW_CNPJ_COMPANY", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJCompany { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAW_NOME_COMPANY", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeCompany { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

    }
}

