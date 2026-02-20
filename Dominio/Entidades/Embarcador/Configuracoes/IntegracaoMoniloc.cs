namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MONILOC", EntityName = "IntegracaoMoniloc", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm", NameType = typeof(IntegracaoMoniloc))]
    public class IntegracaoMoniloc : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoMoniloc", Column = "CIM_POSSUI_INTEGRACAO_MONILOC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMoniloc { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioFTP", Column = "CIM_USUARIO_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaFTP", Column = "CIM_SENHA_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortaFTP", Column = "CIM_PORTA_FTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PortaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioConsumoCargasDiarias", Column = "CIM_DIRETORIO_CONSUMO_CARGAS_DIARIAS", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DiretorioConsumoCargasDiarias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioConsumo", Column = "CIM_DIRETORIO_CONSUMO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DiretorioConsumo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioEnvioCVA", Column = "CIM_DIRETORIO_ENVIO_CVA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DiretorioEnvioCVA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioRetornoCVA", Column = "CIM_DIRETORIO_RETORNO_CVA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DiretorioRetornoCVA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPPassivo", Column = "CIM_FTP_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FTPPassivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SFTP", Column = "CIM_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SSL", Column = "CIM_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HostFTP", Column = "CIM_HOST_FTP", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string HostFTP { get; set; }
    }
}
