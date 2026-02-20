namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CORREIOS", EntityName = "IntegracaoCorreios", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios", NameType = typeof(IntegracaoCorreios))]
    public class IntegracaoCorreios : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLToken", Column = "CIC_URL_TOKEN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLEventos", Column = "CIC_URL_EVENTOS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEventos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CartaoPostagem", Column = "CIC_CARTAO_POSTAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CartaoPostagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPLP", Column = "CIC_URL_PLP", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLPLP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioSIGEP", Column = "CIC_USUARIO_SIGEP", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UsuarioSIGEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSIGEP", Column = "CIC_SENHA_SIGEP", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SenhaSIGEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContrato", Column = "CIC_NUMERO_CONTRATO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDiretoria", Column = "CIC_NUMERO_DIRETORIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDiretoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAdministrativo", Column = "CIC_CODIGO_ADMINISTRATIVO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoAdministrativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoServicoAdicional", Column = "CIC_CODIGO_SERVICO_ADICIONAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoServicoAdicional { get; set; }
    }
}