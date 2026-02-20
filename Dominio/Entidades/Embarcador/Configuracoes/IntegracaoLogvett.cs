namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_LOGVETT", EntityName = "IntegracaoLogvett", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett", NameType = typeof(IntegracaoLogvett))]
    public class IntegracaoLogvett : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIL_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIL_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLTituloPagar", Column = "CIL_URL_TITULO_PAGAR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLTituloPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLBaixarTitulo", Column = "CIL_URL_BAIXAR_TITULO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLBaixarTitulo { get; set; }
    }
}