namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_DOCUMENTO_EMISSAO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoDocumentoEmissao", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoDocumentoEmissao", NameType = typeof(ConfiguracaoTipoOperacaoDocumentoEmissao))]
    public class ConfiguracaoTipoOperacaoDocumentoEmissao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_NAO_ALTERAR_TOMADOR_CARGA_PEDIDO_IMPORTACAO_CTE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao { get; set; }

        /// <summary>
        /// Esta opção serve para importar os CT-es (mesmo que emitidos dentro do sistema) sempre como CT-es para subcontratação.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_IMPORTAR_CTE_SEMPRE_COMO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarCTeSempreComoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_POSSUI_NOTA_ORDEM_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNotaOrdemVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_UTILIZA_NOTA_ORDEM_OBJETO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaNotaVendaObjetoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_MINUTOS_AVANCAR_EMISSAO_INFORADO_DADOS_TRANSPORTE", TypeType = typeof(int), NotNull = false)]
        public virtual int MinutosAvancarParaEmissaoseInformadosDadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_NAO_UTILIZA_NOTA_ORDEM_OBJETO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizaNotaVendaObjetoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_EMITIR_CTE_NOTA_REMESSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirCTENotaRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDE_DESCONSIDERAR_NOTA_PALLET_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesconsiderarNotaPalletEmissaoCTE { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações de Documentos para Emissão"; }
        }
    }
}
