namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_TICKETLOG", EntityName = "IntegracaoTicketLog", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog", NameType = typeof(IntegracaoTicketLog))]
    public class IntegracaoTicketLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoTicketLog", Column = "CIT_POSSUI_INTEGRACAO_TICKETLOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLTicketLog", Column = "CIT_URL_TICKETLOG", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URLTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioTicketLog", Column = "CIT_USUARIO_TICKETLOG", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaTicketLog", Column = "CIT_SENHA_TICKETLOG", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoClienteTicketLog", Column = "CIT_CODIGO_CLIENTE_TICKETLOG", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoClienteTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveAutorizacaoTicketLog", Column = "CIT_CHAVE_AUTORIZACAO_TICKETLOG", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChaveAutorizacaoTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasConsultaTicketLog", Column = "CIT_HORAS_CONSULTA_TICKETLOG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string HorasConsultaTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoAbastecimento", Column = "ABC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frotas.ConfiguracaoAbastecimento ConfiguracaoAbastecimentoTicketLog { get; set; }
    }
}
