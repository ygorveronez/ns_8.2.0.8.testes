using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_EMISSAO_DOCUMENTO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoEmissaoDocumento", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento", NameType = typeof(ConfiguracaoTipoOperacaoEmissaoDocumento))]
    public class ConfiguracaoTipoOperacaoEmissaoDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_FINALIZAR_CARGA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FinalizarCargaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_UTILIZAR_EXPEDIDOR_RECEBEDOR_PEDIDO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarExpedidorRecebedorPedidoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_NAO_PERMITIR_ACESSAR_DOCUMENTOS_ANTES_CARGA_EM_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAcessarDocumentosAntesCargaEmTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_RATEAR_PESO_MODELO_VEICULAR_ENTRE_CTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearPesoModeloVeicularEntreCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DESCRICAO_UNIDADE_MEDIDA_PESO_MODELO_VEICULAR_RATEADO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DescricaoUnidadeMedidaPesoModeloVeicularRateado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoConhecimentoProceda", Column = "CTO_TIPO_CONHECIMENTO_PROCEDA", TypeType = typeof(string), NotNull = false, Length = 1)]
        public virtual string TipoConhecimentoProceda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioAprovarCtesImportados", Column = "CTO_OBRIGATORIO_APROVAR_CTES_IMPORTADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioAprovarCtesImportados { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirLiberarSemValePedagio", Column = "CTO_LIBERAR_SEM_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirLiberarSemValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_NAO_PERMITIR_EMISSAO_COM_MESMA_ORIGEM_E_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirEmissaoComMesmaOrigemEDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_TIPO_EMITENTE", TypeType = typeof(Dominio.Enumeradores.TipoEmitenteMDFe), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoEmitenteMDFe? TipoDeEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_VALIDAR_RELEVANCIA_NOTAS_PRECKIN", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarRelevanciaNotasPrechekin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_EMITIR_DOCUMENTO_SEMPRE_ORIGEM_DESTINO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirDocumentoSempreOrigemDestinoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_GERAR_CTE_SIMPLIFICADO_QUANDO_COMPATIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCTeSimplificadoQuandoCompativel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_CLASSIFICACAO_NFE_REMESSA_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ClassificacaoNFeRemessaVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_ENVIAR_PARA_OBSERVACAO_CTE_NFE_REMESSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarParaObservacaoCTeNFeRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_ENVIAR_PARA_OBSERVACAO_CTE_NFE_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarParaObservacaoCTeNFeVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_AVERBAR_CONTAINER_COM_AVERBACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarContainerComAverbacaoCarga { get; set; } 
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_UTILIZAR_OUTRO_ENDERECO_PEDIDO_MESMO_POSSUIR_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_VALOR_CONTAINER_AVERBACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorContainerAverbacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFR_CHARGE_CODE_VINCULADO", TypeType = typeof(Int32), NotNull = false)]
        public virtual int ChargeCodeVinculado { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configurações para emissão de documentos";
            }
        }
    }
}
