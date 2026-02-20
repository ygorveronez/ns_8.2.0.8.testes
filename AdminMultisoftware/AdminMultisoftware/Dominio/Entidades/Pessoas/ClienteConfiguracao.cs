using System;

namespace AdminMultisoftware.Dominio.Entidades.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_CONFIGURACAO", EntityName = "ClienteConfiguracao", Name = "AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao", NameType = typeof(ClienteConfiguracao))]
    public class ClienteConfiguracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultisoftware", Column = "CCF_TIPO_SERVICO_MULTISOFTWARE", TypeType = typeof(Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        [Obsolete("Migrado para InstanciaBase")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DBServidor", Column = "CCF_DB_SERVIDOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DBServidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DBBase", Column = "CCF_DB_BASE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string DBBase { get; set; }

        [Obsolete("Migrado para InstanciaBase")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DBUsuario", Column = "CCF_DB_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DBUsuario { get; set; }

        [Obsolete("Migrado para InstanciaBase")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DBSenha", Column = "CCF_DB_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DBSenha { get; set; }

        [Obsolete("Migrado para InstanciaBase")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DBPorta", Column = "CCF_DB_PORTA", TypeType = typeof(int), NotNull = false)]
        public virtual int DBPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseHomologacao", Column = "CCF_BASE_HOMOLOGACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool BaseHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_CAMINHO_DOCUMENTOS_FISCAIS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CaminhoDocumentosFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LoginPorAD", Column = "CCF_LOGIN_POR_AD", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LoginPorAD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_EXIGE_CONTRA_SENHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeContraSenha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "InstanciaBase", Column = "INB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Configuracoes.InstanciaBase InstanciaBase { get; set; }
    }
}
