namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_MODELO_EMAIL", EntityName = "ConfiguracaoModeloEmail", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail", NameType = typeof(ConfiguracaoModeloEmail))]
    public class ConfiguracaoModeloEmail : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CME_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CME_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Assunto", Column = "CME_ASSUNTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Corpo", Column = "CME_CORPO", Type = "StringClob", NotNull = false)]
        public virtual string Corpo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RodaPe", Column = "CME_RODA_PE", Type = "StringClob", NotNull = false)]
        public virtual string RodaPe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModeloEmail", Column = "CME_TIPO_MODELO_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloEmail), Length = 1, NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloEmail TipoModeloEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnviarPara", Column = "CME_TIPO_ENVIAR_PARA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnviarPara), Length = 1, NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnviarPara TipoEnviarPara { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGatilhoNotificacao", Column = "CME_TIPO_GATILHO_NOTIFICACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoNotificacao), Length = 1, NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoNotificacao TipoGatilhoNotificacao { get; set; }
    }
}
