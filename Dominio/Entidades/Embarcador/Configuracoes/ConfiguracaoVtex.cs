using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_VTEX", EntityName = "ConfiguracaoVtex", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex", NameType = typeof(ConfiguracaoVtex))]
    public class ConfiguracaoVtex : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVT_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AccountName", Column = "CVT_ACCOUNT_NAME", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string AccountName { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Environment", Column = "CVT_ENVIRONMENT", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Environment { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XVtexApiAppToken", Column = "CVT_X_VTEX_API_APP_TOKEN", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string XVtexApiAppToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XVtexApiAppKey", Column = "CVT_X_VTEX_API_APP_KEY", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string XVtexApiAppKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaIntegracao", Column = "CVT_DATA_ULTIMA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsNotificacao", Column = "CVT_EMAILS_NOTIFICACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailsNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNotificacao", Column = "CVT_QUANTIDADE_NOTIFICACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeNotificacao { get; set; }

        #region Propriedades Virtuais

        public virtual string Descricao => Codigo.ToString();

        public virtual string DescricaoSituacao => Situacao ? "Ativo" : "Inativo";

        public virtual bool Equals(ConfiguracaoVtex other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}