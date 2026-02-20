using MongoDB.Driver;
using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MONGO", EntityName = "ConfiguracaoMongo", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo", NameType = typeof(ConfiguracaoMongo))]
    public class ConfiguracaoMongo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Servidor", Column = "COM_SERVIDOR", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Servidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Banco", Column = "COM_BANCO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "COM_PORTA", TypeType = typeof(int), NotNull = true)]
        public virtual int Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Timeout", Column = "COM_TIMEOUT", TypeType = typeof(int), NotNull = false)]
        public virtual int Timeout { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsaTls", Column = "COM_USA_TLS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsaTls { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioHangfire", Column = "COM_USUARIO_HANGFIRE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioHangfire { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaHangfire", Column = "COM_SENHA_HANGFIRE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaHangfire { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "COM_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "COM_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaCosmosDb", Column = "COM_UTILIZA_COSMOS_DB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaCosmosDb { get; set; }

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                return Servidor;
            }
        }

        public virtual MongoUrlBuilder MongoUrl
        {
            get
            {
                var builder = new MongoUrlBuilder
                {
                    Server = new MongoServerAddress(this.Servidor, this.Porta),
                    DatabaseName = this.Banco,
                    UseTls = this.UsaTls,
                    ConnectTimeout = TimeSpan.FromSeconds(this.Timeout > 0 ? this.Timeout : 60),
                    MaxConnectionPoolSize = 1000,
                    WaitQueueTimeout = TimeSpan.FromMilliseconds(10000),
                };

                if (!string.IsNullOrEmpty(this.Usuario) && !string.IsNullOrEmpty(this.Senha))
                {
                    builder.Username = this.Usuario;
                    builder.Password = this.Senha;
                }


                if (this.UtilizaCosmosDb)
                    builder.RetryWrites = false;

                return builder;
            }
        }

        #endregion Propriedades com Regras
    }
}