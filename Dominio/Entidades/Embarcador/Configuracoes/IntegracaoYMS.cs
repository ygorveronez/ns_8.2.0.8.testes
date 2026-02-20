using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_YMS", EntityName = "IntegracaoYMS", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYMS", NameType = typeof(IntegracaoYMS))]
    public class IntegracaoYMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CYM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CYM_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CYM_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CYM_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CYM_URL_Integracao", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLCancelamento", Column = "CYM_URL_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CYM_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CYM_TOKEN", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAutenticacaoYMS", Column = "CYM_TIPO_AUTENTICACAO", TypeType = typeof(TipoAutenticacaoYMS), NotNull = false)]
        public virtual TipoAutenticacaoYMS TipoAutenticacaoYMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CYM_PARAMETROS_ADICIONAIS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ParametrosAdicionais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoAtualizacao", Column = "CYM_URL_INTEGRACAO_ATUALIZACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoAtualizacao { get; set; }
    }
}
