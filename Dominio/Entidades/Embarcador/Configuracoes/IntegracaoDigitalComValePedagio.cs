namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_DIGITALCOM_VALE_PEDAGIO", EntityName = "IntegracaoDigitalComValePedagio", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalComValePedagio", NameType = typeof(IntegracaoDigitalComValePedagio))]

    public class IntegracaoDigitalComValePedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FORNECEDOR_VP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDV_NOTIFICAR_TRANSPORTADOR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarNumeroCargaNoCampoDocumentoTransporte", Column = "CDV_ENVIAR_NUMERO_CARGA_NO_CAMPO_DOCUMENTO_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNumeroCargaNoCampoDocumentoTransporte { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Integração DigitalCom"; }
        }
    }
}

