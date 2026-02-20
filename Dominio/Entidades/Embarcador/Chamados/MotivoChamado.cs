using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_CHAMADA", EntityName = "MotivoChamado", Name = "Dominio.Entidades.Embarcador.Chamados.MotivoChamado", NameType = typeof(MotivoChamado))]
    public class MotivoChamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_STATUS", TypeType = typeof(bool))]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_GERAR_CARGA_DEVOLUCAO_SE_APROVADO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool GerarCargaDevolucaoSeAprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_GERAR_VALE_PALLET_SE_APROVADO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool GerarValePalletSeAprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_CHAMADO_ABERTO_PELO_EMBARCADOR", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ChamadoDeveSerAbertoPeloEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_ASSUNTO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_CONTEUDO_EMAIL", TypeType = typeof(string), Length = 2000, NotNull = true)]
        public virtual string ConteudoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_EXIGE_VALOR", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ExigeValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_EXIGE_VALOR_NA_LIBERACAO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ExigeValorNaLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "MCH_DEVOLUCAO", NotNull = false, TypeType = typeof(bool))]
        //public virtual bool Devolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_DISPONIBILIZA_PARA_REENTREGA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool DisponibilizaParaReeentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_TIPO_MOTIVO_ATENDIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento TipoMotivoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_EXIGE_FOTO_ABERTURA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ExigeFotoAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_EXIGE_QRCODE_ABERTURA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ExigeQRCodeAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_GERAR_OCORRENCIA_AUTOMATICAMENTE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool GerarOcorrenciaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_VALIDAR_DUPLICIDADE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ValidarDuplicidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITIR_ABRIR_MAIS_ATENDIMENTO_COM_MESMO_MOTIVO_PARA_MESMA_CARGA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_VALIDAR_DUPLICIDADE_POR_DESTINATARIO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ValidarDuplicidadePorDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_REFERENTE_PAGAMENTO_DESCARGA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ReferentePagamentoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_GERAR_CTE_COM_VALOR_IGUAL_CTE_ANTERIOR", NotNull = false, TypeType = typeof(bool))]
        public virtual bool GerarCTeComValorIgualCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_CALCULAR_OCORRENCIA_POR_TABELA_FRETE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool CalcularOcorrenciaPorTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_DESCONTO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_ESTORNAR_ATENDIMENTO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteEstornarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_EXIGE_ANALISE_PARA_OPERACAO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ExigeAnaliseParaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_ADICIONAR_VALOR_COMO_ADIANTAMENTO_MOTORISTA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteAdicionarValorComoAdiantamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_ADICIONAR_VALOR_COMO_DESPESA_MOTORISTA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteAdicionarValorComoDespesaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_ADICIONAR_VALOR_COMO_DESCONTO_MOTORISTA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteAdicionarValorComoDescontoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_INFORMAR_CODIGO_SIF", NotNull = false, TypeType = typeof(bool))]
        public virtual bool InformarCodigoSIF { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotorista.PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorDespesa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Datas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTIVO_CHAMADO_DATA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MCH_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotivoChamadoData", Column = "MCD_CODIGO")]
        public virtual IList<MotivoChamadoData> Datas { get; set; }

        #region Atributos relativos à Diária Automática

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGATORIO_TER_DIARIA_AUTOMATICA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigatorioTerDiariaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_BLOQUEAR_APROVACAO_VALORES_SUPERIORES_A_DIARIA_AUTOMATICA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool BloquearAprovacaoValoresSuperioresADiariaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasLimiteAberturaAposDiariaAutomatica", Column = "MCH_DIAS_LIMITE_ABERTURA_APOS_DIARIA_AUTOMATICA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasLimiteAberturaAposDiariaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalFreeTime", Column = "MCH_LOCAL_FREE_TIME", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime LocalFreeTime { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_HABILITAR_PERFIL_ACESSO_ENVIO_EMAIL", NotNull = false, TypeType = typeof(bool))]
        public virtual bool HabilitarPerfilAcessoEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_RETORNAR_PARA_AJUSTE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteRetornarParaAjuste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGAR_ANEXO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigarAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_ATENDIMENTO_SEM_CARGA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteAtendimentoSemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_ALTERAR_DATAS_CARGA_ENTREGA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteAlterarDatasCargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_BUSCA_CONTA_BANCARIA_DESTINATARIO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool BuscaContaBancariaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGAR_INFORMAR_REPONSAVEL_ATENDIMENTO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigarInformarResponsavelAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_NAO_PERMITIR_LANCAR_ATENDIMENTO_SEM_ACERTO_ABERTO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool NaoPermitirLancarAtendimentoSemAcertoAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITIR_LANCAR_ATENDIMENTO_EM_CARGAS_COM_DOCUMENTO_EMITIDO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermitirLancarAtendimentoEmCargasComDocumentoEmitido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INSERIR_JUSTIFICATIVA_OCORRENCIA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInserirJustificativaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_INFORMAR_QUANTIDADE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool InformarQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGAR_INFORMACAO_LOTE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigarInformacaoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGAR_DATA_CRITICA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigarDataCritica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGAR_REAL_MOTIVO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigarRealMotivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GeneroMotivoChamado", Column = "GMC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GeneroMotivoChamado Genero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AreaEnvolvidaMotivoChamado", Column = "AEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AreaEnvolvidaMotivoChamado AreaEnvolvida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_EXIGE_INFORMAR_MODELO_VEICULAR_ABERTURA_CHAMADO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ExigeInformarModeloVeicularAberturaChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_TRATATIVA_DEVE_SER_CONFIRMADA_PELO_CLIENTE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool TratativaDeveSerConfirmadaPeloCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGAR_MOTORISTA_INFORMAR_MULTIMOBILE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigarMotoristaInformarMultiMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_NAO_EXIGIR_JUSTIFICATIVA_OCORRENCIA_CHAMADO_AO_SALVAR_OBSERVACAO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool NaoExigirJustificativaOcorrenciaChamadoAoSalvarObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_INTEGRAR_COM_DANSALES", NotNull = false, TypeType = typeof(bool))]
        public virtual bool IntegrarComDansales { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_TROCAR_TRANSPORTADORA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteTrocarTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_ATENDIMENTO_POR_LOTE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool AtendimentoPorLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_NFD", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarNFD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_QUANTIDADE_PARA_CALCULO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarQuantidadeParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_QUANTIDADE_VOLUMES_PARA_CALCULO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarQuantidadeVolumesParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_PESO_PARA_CALCULO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarPesoParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_DATA_RETORNO_APOS_ESTADIA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarDataRetornoAposEstadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_ORDEM_INTERNA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarOrdemInterna { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoMotivoChamado", Column = "GRUPO_MOTIVO_CHAMADO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoMotivoChamado GrupoMotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoIntegracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTIVO_CHAMADO_TIPOS_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MCH_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoIntegracao", Column = "TPI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITE_INFORMAR_MOTORISTA_NO_ATENDIMENTO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermiteInformarMotoristaNoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_GERAR_ACRESCIMO_DESCONTO_AGREGADO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool GerarAcrescimoDescontoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_ACRESCIMO_DESCONTO_AGREGADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa JustificativaAcrescimoDescontoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportadorAcrescimoDesconto", Column = "MCH_TIPO_TRANSPORTADOR_ACRESCIMO_DESCONTO", TypeType = typeof(TipoProprietarioVeiculo), NotNull = false)]
        public virtual TipoProprietarioVeiculo? TipoTransportadorAcrescimoDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_POSSIBILITAR_INCLUSAO_ANEXO_AO_ESCALAR_ATENDIMENTO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PossibilitarInclusaoAnexoAoEscalarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITIR_FRETE_RETORNO_DEVOLUCAO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermitirFreteRetornoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_VALIDAR_ESCALADA_ATENDIMENTO_USUARIO_RESPONSAVEL", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ValidarEscaladaAtendimentoUsuarioResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITIR_ACESSAR_ETAPA_OCORRENCIA_SEM_FINALIZAR_ETAPA_ANALISE", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermitirAcessarEtapaOcorrenciaSemFinalizarEtapaAnalise { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_VALIDA_VALOR_CARGA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ValidaValorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_VALIDA_VALOR_DESCARGA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ValidaValorDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_GERAR_GESTAO_DE_DEVOLUCAO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ListarNotasParaGeracaoGestaoDeDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_OBRIGAR_PREENCHIMENTO_NFD", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ObrigarPreenchimentoNFD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_CONSIDERAR_HORAS_DIAS_UTEIS", NotNull = false, TypeType = typeof(bool))]
        public virtual bool ConsiderarHorasDiasUteis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_TIPO_QUEBRA_REGRA_PALLET", NotNull = false, TypeType = typeof(TipoQuebraRegra))]
        public virtual TipoQuebraRegra TipoQuebraRegraPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_ENVIAR_EMAIL_PARA_TRANSPORTADOR_AO_CANCELAR_CHAMADO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool EnviarEmailParaTransportadorAoCancelarChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_ENVIAR_EMAIL_PARA_TRANSPORTADOR_AO_ALTERAR_CHAMADO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool EnviarEmailParaTransportadorAoAlterarChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_ENVIAR_EMAIL_PARA_TRANSPORTADOR_AO_FINALIZAR_CHAMADO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool EnviarEmailParaTransportadorAoFinalizarChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_NUMERO_CRITICIDADE_ATENDIMENTO", NotNull = false, TypeType = typeof(int))]
        public virtual int NumeroCriticidadeAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITIR_INFORMAR_CAUSADOR_OCORRENCIA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermitirInformarCausadorOcorrencia { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_BLOQUEAR_PARADA_APP_TRIZY", NotNull = false, TypeType = typeof(bool))]
        public virtual bool BloquearParadaAppTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_HABILITAR_SENHA_DEVOLUCAO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool HabilitarSenhaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_HABILITAR_ESTADIA", NotNull = false, TypeType = typeof(bool))]
        public virtual bool HabilitarEstadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_BLOQUEAR_ESTORNO_ATENDIMENTOS_FINALIZADOS_PORTAL_TRANSPORTADOR", NotNull = false, TypeType = typeof(bool))]
        public virtual bool BloquearEstornoAtendimentosFinalizadosPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_PERMITIR_ATUALIZAR_INFORMACOES_PEDIDO", NotNull = false, TypeType = typeof(bool))]
        public virtual bool PermitirAtualizarInformacoesPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "MCH_HABILITAR_CLASSIFICACAO_CRITICOS", NotNull = false, TypeType = typeof(bool))]
        public virtual bool HabilitarClassificacaoCriticos { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoStatus
        {
            get { return Status ? "Ativo" : "Inativo"; }
        }

        public virtual string DescricaoComTipo
        {
            get { return Descricao + (TipoMotivoAtendimento != TipoMotivoAtendimento.Atendimento ? $" ({TipoMotivoAtendimento.ObterDescricao()})" : string.Empty); }
        }

        #endregion
    }
}
