namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_DIAGEO", EntityName = "IntegracaoDiageo", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo", NameType = typeof(IntegracaoDiageo))]
    public class IntegracaoDiageo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "CID_ENDERECO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CID_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CID_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Porta", Column = "CID_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Porta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Outbound", Column = "CID_OUTBOUND", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Outbound { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiretorioInbound", Column = "CID_DIRETORIO_INBOUND", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DiretorioInbound { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Passivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_UTILIZAR_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_UTILIZAR_SSL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SSL { get; set; }

    }
}
