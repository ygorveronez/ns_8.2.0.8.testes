using Dominio.Entidades.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEQUENCIA_GESTAO_PATIO", EntityName = "SequenciaGestaoPatio", Name = "Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio", NameType = typeof(SequenciaGestaoPatio))]
    public class SequenciaGestaoPatio : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SGP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TIPO", TypeType = typeof(TipoFluxoGestaoPatio), NotNull = false)]
        public virtual TipoFluxoGestaoPatio Tipo { get; set; }

        public virtual string Descricao
        {
            get { return this.Filial?.Descricao ?? string.Empty; }
        }

        #endregion Propriedades

        #region Propriedades da Etapa Montagem Carga

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_MONTAGEM_CARGA", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool MontagemCargaHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_MONTAGEM_CARGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MontagemCargaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool MontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_MONTAGEM_CARGA", TypeType = typeof(int), NotNull = true)]
        public virtual int MontagemCargaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_MONTAGEM_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int MontagemCargaTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_MONTAGEM_CARGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MontagemCargaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_MONTAGEM_CARGA_INFORMAR_DOCA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool MontagemCargaInformarDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_MONTAGEM_CARGA_PERMITE_INFORMAR_QUANTIDADE_CAIXAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaPermiteInformarQuantidadeCaixas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_MONTAGEM_CARGA_PERMITE_INFORMAR_QUANTIDADE_ITENS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaPermiteInformarQuantidadeItens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_MONTAGEM_CARGA_PERMITE_INFORMAR_QUANTIDADE_PALLETS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaPermiteInformarQuantidadePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Montagem Carga

        #region Propriedades da Etapa Doca de Carregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemInformarDocaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool InformarDocaCarregamentoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InformarDocaCarregamentoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGT_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int InformarDocaCarregamentoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int InformarDocaCarregamentoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InformarDocaCarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_TRANSPORTADOR_LANCAR_HORARIOS_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamentoPermiteTransportadorLancarHorarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_INFORMAR_DADOS_LAUDO_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamentoPermiteInformarDadosLaudo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_DOCA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamentoGerarIntegracaoP44 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarDocaCarregamentoTipoIntegracao", Column = "SGP_INFORMAR_DOCA_CARREGAMENTO_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao InformarDocaCarregamentoTipoIntegracao { get; set; }

        #endregion Propriedades da Etapa Doca de Carregamento

        #region Propriedades da Etapa Chegada de Veículo

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_CHEGADA_VEICULO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemChegadaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_CHEGADA_VEICULO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ChegadaVeiculoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHEGADA_VEICULO_PERMITE_IMPRIMIR_RELACAO_DE_PRODUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPermiteImprimirRelacaoDeProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHEGADA_VEICULO_PREENCHER_DATA_SAIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPreencherDataSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_CHEGADA_VEICULO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChegadaVeiculoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_INFORMAR_CHEGADA_VEICULO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ChegadaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_GUARITA_ENTRADA_INFORMAR_CHEGADA", TypeType = typeof(int), NotNull = true)]
        public virtual int ChegadaVeiculoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_GUARITA_ENTRADA_INFORMAR_CHEGADA", TypeType = typeof(int), NotNull = true)]
        public virtual int ChegadaVeiculoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_GUARITA_ENTRADA_INFORMAR_CHEGADA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChegadaVeiculoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_TRANSPORTADOR_LANCAR_HORARIOS_INFORMAR_INFORMAR_CHEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPermiteTransportadorLancarHorarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_IMPRIMIR_COMPROVANTE_MODELO_COLETA_OUTBOUND_CHEGADA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoImprimirComprovanteModeloColetaOutbound { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_CHEGADA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Chegada de Veículo

        #region Propriedades da Etapa Guarita de Entrada

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_GUARITA_ENTRADA", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemGuaritaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_GUARITA_ENTRADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GuaritaEntradaHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_GUARITA_ENTRADA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string GuaritaEntradaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GuaritaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_INFORMAR_DOCA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GuaritaEntradaInformarDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_GUARITA_ENTRADA", TypeType = typeof(int), NotNull = true)]
        public virtual int GuaritaEntradaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_GUARITA_ENTRADA", TypeType = typeof(int), NotNull = true)]
        public virtual int GuaritaEntradaTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_GUARITA_ENTRADA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string GuaritaEntradaDescricao { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "", TypeType = typeof(bool), NotNull = true)]
        //public virtual bool ChegadaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_EXIBIR_HORARIO_EXATO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GuaritaEntradaExibirHorarioExato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_PERMITE_INFORMACOES_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteInformacoesPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_PERMITE_INFORMACOES_PRODUTOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteInformacoesProdutor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_PERMITE_INFORMAR_ANEXO_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteInformarAnexoPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_PERMITE_INFORMAR_PRESSAO_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteInformarPressaoPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_PERMITE_INFORMAR_QUANTIDADE_CAIXAS_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_PERMITE_DENEGAR_CHEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteDenegarChegada { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_GUARITA_ENTRADA_INFORMAR_CHEGADA", TypeType = typeof(int), NotNull = true)]
        //public virtual int ChegadaVeiculoTempo { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEM_PERMANENCIAPO_GUARITA_ENTRADA_INFORMAR_CHEGADA", TypeType = typeof(int), NotNull = true)]
        //public virtual int ChegadaVeicuTempoPermanenciapo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_TRANSPORTADOR_LANCAR_HORARIOS_INFORMAR_GUARITA_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteTransportadorLancarHorarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_ENTRADA_PERMITE_INFORMAR_DADOS_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteInformarDadosDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilialBalanca", Column = "FBA_CODIGO_BALANCA_GUARITA_ENTRADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilialBalanca BalancaGuaritaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuaritaEntradaTipoIntegracaoBalanca", Column = "SGP_GUARITA_ENTRADA_TIPO_INTEGRACAO_BALANCA", TypeType = typeof(TipoIntegracao), NotNull = false)]
        public virtual TipoIntegracao? GuaritaEntradaTipoIntegracaoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_GUARITA_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaGerarIntegracaoP44 { get; set; }


        #endregion Propriedades da Etapa Guarita de Entrada

        #region Propriedades da Etapa Checklist

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_CHECK_LIST", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemCheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_CHECK_LIST", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CheckListHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITIR_IMPRESSAO_APENAS_COM_CHECKLIST_FINALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListPermiteImpressaoApenasComCheckListFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITIR_PREENCHER_CHECKLIST_ANTES_DE_CHEGAR_NA_ETAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListPermitePreencherCheckListAntesDeChegarNaEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_CHECK_LIST", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CheckListCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_INFORMAR_DOCA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CheckListInformarDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_CHECK_LIST", TypeType = typeof(int), NotNull = true)]
        public virtual int CheckListTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_CHECK_LIST", TypeType = typeof(int), NotNull = true)]
        public virtual int CheckListTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_CHECK_LIST", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CheckListDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_PERMITE_SALVAR_PREENCHER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListPermiteSalvarSemPreencher { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_CANCELAR_PATIO_AO_REPROVAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListCancelarPatioAoReprovar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_NAO_EXIGE_OBSERVACAO_AO_REPROVAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListNaoExigeObservacaoAoReprovar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_NOTIFICAR_POR_EMAIL_REPROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListNotificarPorEmailReprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_EMAILS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CheckListEmails { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoChecklistImpressao", Column = "TCI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoChecklistImpressao TipoChecklistImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_TRANSPORTADOR_LANCAR_HORARIOS_INFORMAR_CHECK_LIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListPermiteTransportadorLancarHorarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_ASSINATURA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListAssinaturaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_ASSINATURA_CARREGADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListAssinaturaCarregador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_ASSINATURA_RESPONSAVEL_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListAssinaturaResponsavelAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_CHECK_LIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListGerarIntegracaoP44 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_GERAR_NOVO_PEDIDO_AO_TERMINO_DO_FLUXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListGerarNovoPedidoAoTerminoFluxo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_EXIGIR_ANEXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListExigirAnexo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_CHECKLIST", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao CheckListTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente CheckListDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHECK_LIST_UTILIZAR_VIGENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListUtilizarVigencia { get; set; }

        #endregion Propriedades da Etapa Checklist

        #region Propriedades da Etapa Travamento de Chave

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_TRAVA_CHAVE", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemTravaChave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_TRAVA_CHAVE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool TravaChaveHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_TRAVA_CHAVE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TravaChaveCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TRAVA_CHAVE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool TravaChave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TRAVA_CHAVE_INFORMAR_DOCA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool TravaChaveInformarDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_TRAVA_CHAVE", TypeType = typeof(int), NotNull = true)]
        public virtual int TravaChaveTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_TRAVA_CHAVE", TypeType = typeof(int), NotNull = true)]
        public virtual int TravaChaveTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_TRAVA_CHAVE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TravaChaveDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_TRANSPORTADOR_LANCAR_HORARIOS_INFORMAR_TRAVA_CHAVE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TravaChavePermiteTransportadorLancarHorarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_TRAVA_CHAVE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TravaChaveGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Travamento de Chave

        #region Propriedades da Etapa Expedição

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_EXPEDICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_EXPEDICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExpedicaoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_EXPEDICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ExpedicaoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_EXPEDICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Expedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_EXPEDICAO_INFORMAR_DOCA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExpedicaoInformarDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_EXPEDICAO_INFORMAR_INICIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExpedicaoInformarInicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_EXPEDICAO_INFORMAR_TERMINO_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExpedicaoInformarTerminoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_EXPEDICAO_CONFIRMAR_PLACA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExpedicaoConfirmarPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_EXPEDICAO_INICIO_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int ExpedicaoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_EXPEDICAO_INICIO_CARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ExpedicaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_EXPEDICAO_FIM_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int ExpedicaoTempoFimCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_EXPEDICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExpedicaoGerarIntegracaoP44 { get; set; }


        #endregion Propriedades da Etapa Expedição

        #region Propriedades da Etapa Liberação de Chave

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_LIBERA_CHAVE", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemLiberaChave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_LIBERA_CHAVE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberaChaveHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_LIBERA_CHAVE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LiberaChaveCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_LIBERA_CHAVE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LiberaChave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_LIBERA_CHAVE", TypeType = typeof(int), NotNull = true)]
        public virtual int LiberaChaveTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_LIBERA_CHAVE", TypeType = typeof(int), NotNull = true)]
        public virtual int LiberaChaveTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_LIBERA_CHAVE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string LiberaChaveDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_LIBERA_CHAVE_BLOQUEAR_LIBERACAO_ETAPA_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChaveBloquearLiberacaoEtapaAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_INFORMAR_NUMERO_PALETES_LIBERA_CHAVE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChaveInformarNumeroDePaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_LIBERAR_CHAVE_SOLICITAR_ASSINATURA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChaveSolicitarAssinaturaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_LIBERAR_CHAVE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChaveGerarIntegracaoP44 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_EXIGIR_ANEXO_TRAVA_CHAVE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChaveExigirAnexo { get; set; }

        #endregion Propriedades da Etapa Liberação de Chave

        #region Propriedades da Etapa Faturamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_CONTROLAR_FATURAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_CONTROLAR_FATURAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FaturamentoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_CONTROLAR_FATURAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FaturamentoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CONTROLAR_FATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Faturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_CONTROLAR_FATURAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int FaturamentoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_CONTROLAR_FATURAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int FaturamentoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_CONTROLAR_FATURAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FaturamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_FATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Faturamento

        #region Propriedades da Etapa Início da Viagem

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_GUARITA_SAIDA", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemGuaritaSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_GUARITA_SAIDA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GuaritaSaidaHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_GUARITA_SAIDA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string GuaritaSaidaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_SAIDA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GuaritaSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_GUARITA_SAIDA", TypeType = typeof(int), NotNull = true)]
        public virtual int GuaritaSaidaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_GUARITA_SAIDA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string GuaritaSaidaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_SAIDA_PERMITE_INFORMACOES_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaPermiteInformacoesPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_SAIDA_PERMITE_INFORMAR_LACRE_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaPermiteInformarLacrePesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_SAIDA_PERMITE_INFORMAR_PERCENTUAL_REFUGO_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaPermiteInformarPercentualRefugoPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_SAIDA_PERMITE_ANEXOS_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaPermiteAnexosPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GUARITA_SAIDA_INICIAR_EMISSAO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaIniciarEmissaoDocumentosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_IMPRIMIR_TICKET_BALANCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirTicketBalanca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_GUARITA_SAIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaGerarIntegracaoP44 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_EXIGIR_ANEXO_GUARITA_SAIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaExigirAnexo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilialBalanca", Column = "FBA_CODIGO_BALANCA_GUARITA_SAIDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilialBalanca BalancaGuaritaSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuaritaSaidaTipoIntegracaoBalanca", Column = "SGP_GUARITA_SAIDA_TIPO_INTEGRACAO_BALANCA", TypeType = typeof(TipoIntegracao), NotNull = false)]
        public virtual TipoIntegracao? GuaritaSaidaTipoIntegracaoBalanca { get; set; }

        #endregion Propriedades da Etapa Início da Viagem

        #region Propriedades da Etapa Posicao

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_POSICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PosicaoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_POSICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PosicaoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_POSICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int PosicaoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int PosicaoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_POSICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PosicaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_POSICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PosicaoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Posicao

        #region Propriedades da Etapa Chegada na Loja

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_CHEGADA_LOJA", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemChegadaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_CHEGADA_LOJA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ChegadaLojaHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_CHEGADA_LOJA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChegadaLojaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CHEGADA_LOJA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ChegadaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_CHEGADA_LOJA", TypeType = typeof(int), NotNull = true)]
        public virtual int ChegadaLojaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_CHEGADA_LOJA", TypeType = typeof(int), NotNull = true)]
        public virtual int ChegadaLojaTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_CHEGADA_LOJA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChegadaLojaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_CHEGADA_LOJA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaLojaGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Chegada na Loja

        #region Propriedades da Etapa Deslocamento Pátio

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_DESLOCAMENTO_PATIO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemDeslocamentoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_DESLOCAMENTO_PATIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool DeslocamentoPatioHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_DESLOCAMENTO_PATIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DeslocamentoPatioCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESLOCAMENTO_PATIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool DeslocamentoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_DESLOCAMENTO_PATIO", TypeType = typeof(int), NotNull = true)]
        public virtual int DeslocamentoPatioTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_DESLOCAMENTO_PATIO", TypeType = typeof(int), NotNull = true)]
        public virtual int DeslocamentoPatioTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_DESLOCAMENTO_PATIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DeslocamentoPatioDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESLOCAMENTO_PATIO_PERMITE_INFORMACOES_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoPatioPermiteInformacoesPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESLOCAMENTO_PATIO_PERMITE_INFORMACOES_LOTE_INTERNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoPatioPermiteInformacoesLoteInterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_DESLOCAMENTO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoPatioGerarIntegracaoP44 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESLOCAMENTO_PATIO_PERMITE_INFORMAR_QUANTIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoPatioPermiteInformarQuantidade { get; set; }

        #endregion Propriedades da Etapa Deslocamento Pátio

        #region Propriedades da Etapa Saída da Loja

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_SAIDA_LOJA", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemSaidaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_SAIDA_LOJA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SaidaLojaHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_SAIDA_LOJA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SaidaLojaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_SAIDA_LOJA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SaidaLoja { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_SAIDA_LOJA", TypeType = typeof(int), NotNull = true)]
        public virtual int SaidaLojaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_SAIDA_LOJA", TypeType = typeof(int), NotNull = true)]
        public virtual int SaidaLojaTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_SAIDA_LOJA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SaidaLojaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_SAIDA_LOJA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SaidaLojaGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Saída da Loja

        #region Propriedades da Etapa Fim da Viagem

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_FIM_VIAGEM", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_FIM_VIAGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimViagemHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_FIM_VIAGEM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimViagemCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_FIM_VIAGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_FIM_VIAGEM", TypeType = typeof(int), NotNull = true)]
        public virtual int FimViagemTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_FIM_VIAGEM", TypeType = typeof(int), NotNull = true)]
        public virtual int FimViagemTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_FIM_VIAGEM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimViagemDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_FIM_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimViagemGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Fim da Viagem

        #region Propriedades da Etapa Início do Carregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_INICIO_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemInicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_INICIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool InicioCarregamentoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_INICIO_CARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InicioCarregamentoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_INICIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool InicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_INICIO_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int InicioCarregamentoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_INICIO_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int InicioCarregamentoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_INICIO_CARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InicioCarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_INFORMAR_PESAGEM_INICIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioCarregamentoPermiteInformarPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_INICIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioCarregamentoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Início do Carregamento

        #region Propriedades da Etapa Fim do Carregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_FIM_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemFimCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_FIM_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimCarregamentoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_FIM_CARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimCarregamentoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_FIM_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_FIM_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int FimCarregamentoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_FIM_CARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int FimCarregamentoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_FIM_CARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimCarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_INFORMAR_PESAGEM_FIM_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimCarregamentoPermiteInformarPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_FIM_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimCarregamentoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Fim do Carregamento

        #region Propriedades da Etapa Início de Higienização

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_INICIO_HIGIENIZACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemInicioHigienizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_INICIO_HIGIENIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool InicioHigienizacaoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_INICIO_HIGIENIZACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InicioHigienizacaoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_INICIO_HIGIENIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool InicioHigienizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_INICIO_HIGIENIZACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int InicioHigienizacaoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_INICIO_HIGIENIZACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int InicioHigienizacaoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_INICIO_HIGIENIZACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InicioHigienizacaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_INICIO_HIGIENIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioHigienizacaoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Início de Higienização

        #region Propriedades da Etapa Fim da Higienização

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_FIM_HIGIENIZACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemFimHigienizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_FIM_HIGIENIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimHigienizacaoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_FIM_HIGIENIZACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimHigienizacaoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_FIM_HIGIENIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimHigienizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_FIM_HIGIENIZACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int FimHigienizacaoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_FIM_HIGIENIZACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int FimHigienizacaoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_FIM_HIGIENIZACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimHigienizacaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_FIM_HIGIENIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimHigienizacaoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Fim da Higienização

        #region Propriedades da Etapa Solicitação de Veículo

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_SOLICITACAO_VEICULO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemSolicitacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_SOLICITACAO_VEICULO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SolicitacaoVeiculoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_NAO_PERMITIR_ENVIAR_SMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirEnviarSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_SOLICITACAO_VEICULO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SolicitacaoVeiculoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_SOLICITACAO_VEICULO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SolicitacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_SOLICITACAO_VEICULO", TypeType = typeof(int), NotNull = true)]
        public virtual int SolicitacaoVeiculoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_SOLICITACAO_VEICULO", TypeType = typeof(int), NotNull = true)]
        public virtual int SolicitacaoVeiculoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_SOLICITACAO_VEICULO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SolicitacaoVeiculoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_SOLICITACAO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitacaoVeiculoGerarIntegracaoP44 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITIR_INFORMAR_DADOS_TRANSPORTE_CARGA_SOLICITACAO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitacaoVeiculoPermitirInformarDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_SOLICITACAO_VEICULO_HABILITAR_INTEGRACAO_PAGER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitacaoVeiculoHabilitarIntegracaoPager { get; set; }

        #endregion Propriedades da Etapa Solicitação de Veículo

        #region Propriedades da Etapa Início do Descarregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_INICIO_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemInicioDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_INICIO_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool InicioDescarregamentoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_INICIO_DESCARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InicioDescarregamentoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_INICIO_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool InicioDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_INICIO_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int InicioDescarregamentoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_INICIO_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int InicioDescarregamentoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_INICIO_DESCARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string InicioDescarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_INFORMAR_PESAGEM_INICIO_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioDescarregamentoPermiteInformarPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_INICIO_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioDescarregamentoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Início do Carregamento

        #region Propriedades da Etapa Fim do Descarregamento

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_FIM_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemFimDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_FIM_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimDescarregamentoHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_FIM_DESCARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimDescarregamentoCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_FIM_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FimDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_FIM_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int FimDescarregamentoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_FIM_DESCARREGAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int FimDescarregamentoTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_FIM_DESCARREGAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string FimDescarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_INFORMAR_PESAGEM_FIM_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimDescarregamentoPermiteInformarPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_FIM_DESCARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimDescarregamentoGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Fim do Carregamento

        #region Propriedades da Etapa Documento Fiscal

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_DOCUMENTO_FISCAL", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_DOCUMENTO_FISCAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool DocumentoFiscalHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_DOCUMENTO_FISCAL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DocumentoFiscalCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DOCUMENTO_FISCAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool DocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_DOCUMENTO_FISCAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DocumentoFiscalTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_DOCUMENTO_FISCAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DocumentoFiscalTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_DOCUMENTO_FISCAL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DocumentoFiscalDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DOCUMENTO_FISCAL_VINCULAR_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFiscalVincularNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_DOCUMENTO_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFiscalGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Documento Fiscal

        #region Propriedades da Etapa Documentos de Transporte

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_DOCUMENTOS_TRANSPORTE", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemDocumentosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool DocumentosTransporteHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DocumentosTransporteCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DOCUMENTOS_TRANSPORTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool DocumentosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(int), NotNull = true)]
        public virtual int DocumentosTransporteTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_DOCUMENTOS_TRANSPORTE", TypeType = typeof(int), NotNull = true)]
        public virtual int DocumentosTransporteTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DocumentosTransporteDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_DOCUMENTOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentosTransporteGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Documento Fiscal

        #region Propriedades da Etapa Separação de Mercadoria

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_SEPARACAO_MERCADORIA", TypeType = typeof(int), NotNull = true)]
        public virtual int OrdemSeparacaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_SEPARACAO_MERCADORIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SeparacaoMercadoriaHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_SEPARACAO_MERCADORIA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SeparacaoMercadoriaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_SEPARACAO_MERCADORIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SeparacaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_SEPARACAO_MERCADORIA", TypeType = typeof(int), NotNull = true)]
        public virtual int SeparacaoMercadoriaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_SEPARACAO_MERCADORIA", TypeType = typeof(int), NotNull = true)]
        public virtual int SeparacaoMercadoriaTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_SEPARACAO_MERCADORIA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SeparacaoMercadoriaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_INFORMAR_DADOS_CARREGADORES_SEPARACAO_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoMercadoriaPermiteInformarDadosCarregadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_INFORMAR_DADOS_SEPARADORES_SEPARACAO_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoMercadoriaPermiteInformarDadosSeparadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_GERAR_INTEGRACAO_P44_SEPARACAO_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoMercadoriaGerarIntegracaoP44 { get; set; }

        #endregion Propriedades da Etapa Documento Fiscal

        #region Propriedades da Etapa Avalição de Descarga

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_ORDEM_AVALIACAO_DESCARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemAvaliacaoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_HABILITAR_INTEGRACAO_AVALIACAO_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaHabilitarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITIR_IMPRESSAO_APENAS_COM_AVALIACAO_DESCARGA_FINALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaPermiteImpressaoApenasComAvaliacaoDescargaFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITIR_PREENCHER_AVALIACAO_DESCARGA_ANTES_DE_CHEGAR_NA_ETAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaPermitePreencherAvaliacaoDescargaAntesDeChegarNaEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_CODIGO_INTEGRACAO_AVALIACAO_DESCARGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string AvaliacaoDescargaCodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_INFORMAR_DOCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaInformarDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_AVALIACAO_DESCARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int AvaliacaoDescargaTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_TEMPO_PERMANENCIA_AVALIACAO_DESCARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int AvaliacaoDescargaTempoPermanencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_DESCRICAO_AVALIACAO_DESCARGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string AvaliacaoDescargaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_PERMITE_SALVAR_PREENCHER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaPermiteSalvarSemPreencher { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_CANCELAR_PATIO_AO_REPROVAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaCancelarPatioAoReprovar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_NAO_EXIGE_OBSERVACAO_AO_REPROVAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaNaoExigeObservacaoAoReprovar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_NOTIFICAR_POR_EMAIL_REPROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaNotificarPorEmailReprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_EMAILS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string AvaliacaoDescargaEmails { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoAvaliacaoDescargaImpressao", Column = "TAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual TipoAvaliacaoDescargaImpressao TipoAvaliacaoDescargaImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_PERMITE_TRANSPORTADOR_LANCAR_HORARIOS_INFORMAR_AVALIACAO_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaPermiteTransportadorLancarHorarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_ASSINATURA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaAssinaturaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_ASSINATURA_CARREGADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaAssinaturaCarregador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SGP_AVALIACAO_DESCARGA_ASSINATURA_RESPONSAVEL_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaAssinaturaResponsavelAprovacao { get; set; }

        #endregion Propriedades da Etapa Avalição de Descarga

        #region Métodos Públicos

        public virtual bool PermitirGerarIntegracaoP44(EtapaFluxoGestaoPatio etapa)
        {
            switch (etapa)
            {
                case EtapaFluxoGestaoPatio.CheckList: return CheckListGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.ChegadaLoja: return ChegadaLojaGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.ChegadaVeiculo: return ChegadaVeiculoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.DeslocamentoPatio: return DeslocamentoPatioGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.DocumentoFiscal: return DocumentoFiscalGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.DocumentosTransporte: return DocumentosTransporteGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.Expedicao: return ExpedicaoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.Faturamento: return FaturamentoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.FimCarregamento: return FimCarregamentoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.FimDescarregamento: return FimDescarregamentoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.FimHigienizacao: return FimHigienizacaoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.FimViagem: return FimViagemGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.Guarita: return GuaritaEntradaGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.InformarDoca: return InformarDocaCarregamentoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.InicioCarregamento: return InicioCarregamentoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.InicioDescarregamento: return InicioDescarregamentoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.InicioHigienizacao: return InicioHigienizacaoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.LiberacaoChave: return LiberaChaveGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.MontagemCarga: return MontagemCargaGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.Posicao: return PosicaoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.SaidaLoja: return SaidaLojaGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.SeparacaoMercadoria: return SeparacaoMercadoriaGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.SolicitacaoVeiculo: return SolicitacaoVeiculoGerarIntegracaoP44;
                case EtapaFluxoGestaoPatio.TravamentoChave: return TravaChaveGerarIntegracaoP44;
            }

            return false;
        }

        #endregion Métodos Públicos
    }
}
