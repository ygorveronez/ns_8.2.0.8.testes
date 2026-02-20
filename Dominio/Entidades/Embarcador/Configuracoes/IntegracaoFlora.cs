namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_FLORA", EntityName = "IntegracaoFlora", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFlora", NameType = typeof(IntegracaoFlora))]
    public class IntegracaoFlora : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIF_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioCarga", Column = "CIF_ENVIO_CARGA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string EnvioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIF_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIF_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIF_URL", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFretePrevisto", Column = "CIF_CODIGO_FRETE_PREVISTO", TypeType = typeof(string), Length = 80, NotNull = false)] 
        public virtual string CodigoFretePrevisto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFreteConfirmado", Column = "CIF_CODIGO_FRETE_CONFIRMADO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string CodigoFreteConfirmado { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Flora";
            }
        }
    }
}
