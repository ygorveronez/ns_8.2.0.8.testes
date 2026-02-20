namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_DOCUMENTO_ENTRADA", EntityName = "ConfiguracaoDocumentoEntrada", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada", NameType = typeof(ConfiguracaoDocumentoEntrada))]
    public class ConfiguracaoDocumentoEntrada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_BLOQUEAR_FINALIZACAO_COM_FLUXO_COMPRA_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearFinalizacaoComFluxoCompraAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_PERMITIR_SELECIONAR_OS_FINALIZADA_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarOSFinalizadaDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_FORMULA_CUSTO_PADRAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string FormulaCustoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_BLOQUEAR_CADASTRO_PRODUTO_COM_MESMO_CODIGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearCadastroProdutoComMesmoCodigo { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para documentos de entrada"; }
        }
    }
}
