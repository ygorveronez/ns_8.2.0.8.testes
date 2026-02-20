namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TEMPLATE_MENSAGEM_WHATSAPP", EntityName = "ConfiguracaoTemplateMensagemWhatsApp", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTemplateMensagemWhatsApp", NameType = typeof(ConfiguracaoTemplateMensagemWhatsApp))]
    public class ConfiguracaoTemplateMensagemWhatsApp : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CTM_NOME", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Idioma", Column = "CTM_IDIOMA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Idioma { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CTM_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTemplate", Column = "CTM_TIPO_TEMPLATE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTemplateWhatsApp), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTemplateWhatsApp TipoTemplate { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CTM_STATUS_TEMPLATE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusTemplateWhatsApp Status { get; set; }
    }
}
