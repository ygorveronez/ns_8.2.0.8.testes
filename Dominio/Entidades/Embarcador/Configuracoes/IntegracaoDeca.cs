namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_DECA", EntityName = "IntegracaoDeca", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca", NameType = typeof(IntegracaoDeca))]
    public class IntegracaoDeca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoDeca", Column = "CID_POSSUI_INTEGRACAO_DECA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoDeca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacaoDeca", Column = "CID_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacaoDeca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioDeca", Column = "CID_USUARIO_DECA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioDeca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaDeca", Column = "CID_SENHA_DECA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaDeca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoBalanca", Column = "CID_POSSUI_INTEGRACAO_BALANCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLBalanca", Column = "CID_URL_BALANCA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenBalanca", Column = "CID_TOKEN_BALANCA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TokenBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLInicioViagemDeca", Column = "CID_URL_INICIO_VIAGEM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URLInicioViagemDeca { get; set; }
    }
}