namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ENVIO_EMAIL_COBRANCA", EntityName = "ConfiguracaoEnvioEmailCobranca", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca", NameType = typeof(ConfiguracaoEnvioEmailCobranca))]
    public class ConfiguracaoEnvioEmailCobranca : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EEC_AVISO_VENCIMETO_ENVAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoEnvarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvisoVencimetoQunatidadeDias", Column = "EEC_AVISO_VENCIMETO_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int AvisoVencimetoQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EEC_AVISO_VENCIMETO_ENVIAR_DIARIAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoEnviarDiariamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EEC_AVISO_VENCIMETO_ASSUNTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string? AvisoVencimetoAssunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EEC_AVISO_VENCIMETO_MENSAGEM", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string? AvisoVencimetoMensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EEC_COBRANCA_ENVAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaEnvarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaQunatidadeDias", Column = "EEC_COBRANCA_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int CobrancaQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EEC_COBRANCA_ASSUNTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string? CobrancaAssunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EEC_COBRANCA_MENSAGEM", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string? CobrancaMensagem { get; set; }
    }
}
