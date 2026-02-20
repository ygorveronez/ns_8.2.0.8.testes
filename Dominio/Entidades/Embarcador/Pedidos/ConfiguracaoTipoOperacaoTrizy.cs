using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_TRIZY", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoTrizy", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy", NameType = typeof(ConfiguracaoTipoOperacaoTrizy))]
    public class ConfiguracaoTipoOperacaoTrizy : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_ESTOU_INDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEstouIndoColeta { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_ESTOU_INDO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEstouIndoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_COMPROVANTE_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarComprovanteColetaEntrega { get; set; }

        [Obsolete("Não será utilizada. Transferido funcionamento para propriedade: InformacoesAdicionaisEntrega")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_QUANTIDADE_DE_FARDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarQuantidadeDeFardos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_PRIMEIRA_COLETA_COMO_ORIGEM_NO_LUGAR_DO_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_HABILITAR_CHAT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarChat { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_INCIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarInicioViagemColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_INCIO_VIAGEM_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarInicioViagemEntrega { get; set; }

        [Obsolete("Flag não será mais utilizada. Foi solicitada de forma errada e feita de forma errada.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_EVENTOS_CHEGADA_E_CONFIRMACAO_ENTREGA_OPCIONAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEventosChegadaEConfirmacaoEntregaOpcionais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_MENSAGEM_ALERTA_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarMensagemAlertaPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_PRE_TRIP_JUNTO_AO_NUMERO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPreTripJuntoAoNumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_INICIAR_VIAGEM_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarIniciarViagemColetaPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_INICIAR_VIAGEM_ENTREGA_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarIniciarViagemEntregaPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_ESTOU_INDO_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEstouIndoColetaPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_ESTOU_INDO_ENTREGA_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEstouIndoEntregaPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_PRIMEIRA_COLETA_COMO_ORIGEM_NO_LUGAR_DO_REMETENTE_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_CHEGUEI_PARA_CARREGAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarChegueiParaCarregar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_CHEGUEI_PARA_DESCARREGAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarChegueiParaDescarregar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_CHEGUEI_PARA_CARREGAR_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarChegueiParaCarregarPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_CHEGUEI_PARA_DESCARREGAR_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarChegueiParaDescarregarPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_FINALIZAR_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoFinalizarPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_EXIGIR_ENVIO_FOTOS_DAS_NOTAS_NA_ORIGEM_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirEnvioFotosDasNotasNaOrigemPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_COMPROVANTE_ENTREGA_SEM_OCR_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarComprovanteEntregaSemOCRPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_DATA_E_HORA_DO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarDataeHoraDoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_VINCULAR_DATA_E_HORA_SOLICITADA_NO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularDataEHoraSolicitadaNoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_ENVIAR_DOCUMENTOS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarDocumentosFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_ENVIAR_TAG_VALIDACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarTagValidacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_ASSINATURA_NA_CONFIRMACAO_DE_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarAssinaturaNaConfirmacaoDeColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TituloInformacaoAdicional", Column = "CTT_TITULO_INFORMACAO_ADICIONAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TituloInformacaoAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_FOTO_COMO_EVIDENCIA_OBRIGATORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarFotoComoEvidenciaObrigatoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_FOTO_COMO_EVIDENCIA_OPCIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarFotoComoEvidenciaOpcional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_EVENTO_INICIAR_VIAGEM_COMO_OPCIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEventoIniciarViagemComoOpcional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_EVENTO_INICIAR_VIAGEM_COMO_OPCIONAL_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEventoIniciarViagemComoOpcionalPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEsperadaParaColetas", Column = "CTT_DATA_ESPERADA_PARA_COLETAS", TypeType = typeof(DataEsperadaColetaEntregaTrizy), NotNull = false)]
        public virtual DataEsperadaColetaEntregaTrizy DataEsperadaParaColetas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEsperadaParaEntregas", Column = "CTT_DATA_ESPERADA_PARA_ENTREGAS", TypeType = typeof(DataEsperadaColetaEntregaTrizy), NotNull = false)]
        public virtual DataEsperadaColetaEntregaTrizy DataEsperadaParaEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_INFORMACOES_ADICIONAIS_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarInformacoesAdicionaisEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_QUANTIDADE_DE_CAIXAS_NO_LUGAR_DO_PESO_DOS_PRODUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_DADOS_EMPRESA_GR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDadosEmpresaGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_CNPJ_EMPRESA_GR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJEmpresaGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_DESCRICAO_EMPRESA_GR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoEmpresaGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_NOME_RECEBEDOR_NA_CONFIRMACAO_DE_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_SOLICITAR_DOCUMENTO_NA_CONFIRMACAO_DE_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarDocumentoNaConfirmacaoDeColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_IDENTIFICAR_NOTA_DE_MERCADORIA_E_NOTA_DE_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificarNotaDeMercadoriaENotaDePallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_CONTATO_INFORMACOES_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarContatoInformacoesEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_CONVERTER_CANHOTO_PARA_PRETO_E_BRANCO_E_ROTACIONAR_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_MASCARA_FIXA_PARA_O_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarMascaraFixaParaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_ENVIAR_MASCARA_DINAMICA_PARA_O_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarMascaraDinamicaParaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_PERMITIR_VINCULAR_FOTOS_DA_GALERIA_PARA_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirVincularFotosDaGaleriaParaCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_ENVIAR_EVENTOS_VIAGEM_E_COLETA_ENTREGA_SOLICITAR_APENAS_EVIDENCIAS_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_ENVIAR_EVENTOS_NA_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEventosNaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_HABILITAR_ENVIO_RELATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarEnvioRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_TITULO_RELATORIO_VIAGEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TituloRelatorioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_TITULO_RECIBO_VIAGEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TituloReciboViagem { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NECESSARIO_FINALIZAR_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessarioFinalizarOrigem { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_VERSAO_INTEGRACAO", TypeType = typeof(VersaoIntegracaoTrizy), NotNull = false)]
        public virtual VersaoIntegracaoTrizy VersaoIntegracao { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_HABILITAR_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarDevolucao { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_HABILITAR_DEVOLUCAO_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarDevolucaoParcial { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "CTT_NAO_ENVIAR_POLILINHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarPolilinha { get; set; }

        #endregion

        #region Collections
        [NHibernate.Mapping.Attributes.Set(0, Name = "InformacoesAdicionaisEntrega", Table = "T_CONFIGURACAO_TIPO_OPERACAO_TRIZY_INFORMACOES_ADICIONAIS_ENTREGA", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True)]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTT_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "IAE_INFORMACAO_ADICIONAL_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.InformacoesAdicionaisEntregaTrizy), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.InformacoesAdicionaisEntregaTrizy> InformacoesAdicionaisEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ContatosInformacoesEntrega", Table = "T_CONFIGURACAO_TIPO_OPERACAO_TRIZY_INFORMACOES_CONTATOS_ENTREGA", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True)]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTT_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "ICE_INFORMACAO_ADICIONAL_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.InformacoesAdicionaisEntregaTrizy), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.EnumContatosInformacoesEntregaTrizy> ContatosInformacoesEntrega { get; set; }

        #endregion

        public virtual string Descricao
        {
            get { return "Configurações Trizy"; }
        }
    }
}