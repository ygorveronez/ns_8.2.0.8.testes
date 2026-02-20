using Dominio.Entidades.Embarcador.Pedidos;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA", EntityName = "TipoDeOcorrenciaDeCTe", Name = "Dominio.Entidades.TipoDeOcorrenciaDeCTe", NameType = typeof(TipoDeOcorrenciaDeCTe))]
    public class TipoDeOcorrenciaDeCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OCO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_DESCRICAO_PORTAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoPortal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProceda", Column = "OCO_COD_PROCEDA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoProceda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoDocumentoOcorrencia", Column = "OCO_TIPO_EMISSAO_DOCUMENTO_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoDocumentoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoDocumentoOcorrencia TipoEmissaoDocumentoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "OCO_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmail), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BuscarCSTQuandoDocumentoOrigemIsento", Column = "OCO_BUSCAR_CST_QUANTO_DOCUMENTO_ORIGEM_ISENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool BuscarCSTQuandoDocumentoOrigemIsento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearVisualizacaoTipoOcorrenciaTransportador", Column = "OCO_BLOQUEAR_VISUALIZACAO_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearVisualizacaoTipoOcorrenciaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "OCO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoIntramunicipal", Column = "OCO_TIPO_EMISSAO_INTRAMUNICIPAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TomadorTipoOcorrencia", Column = "OCO_TOMADOR_TIPO_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia TomadorTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_OUTRO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente OutroTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitenteTipoOcorrencia", Column = "OCO_EMITENTE_TIPO_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EmitenteTipoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EmitenteTipoOcorrencia EmitenteTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalidadeTipoOcorrencia", Column = "OCO_FINALIDADE_TIPO_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoOcorrencia? FinalidadeTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_OUTRO_EMITENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa OutroEmitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoCFOPEstadual", Column = "OCO_CODIGO_INTEGRACAO_CFOP_ESTADUAL", TypeType = typeof(string), NotNull = false, Length = 6)]
        public virtual string CodigoIntegracaoCFOPEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoCFOPInterestadual", Column = "OCO_CODIGO_INTEGRACAO_CFOP_INTERESTADUAL", TypeType = typeof(string), NotNull = false, Length = 6)]
        public virtual string CodigoIntegracaoCFOPInterestadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoCFOPEstadualIsento", Column = "OCO_CODIGO_INTEGRACAO_CFOP_ESTADUAL_ISENTO", TypeType = typeof(string), NotNull = false, Length = 6)]
        public virtual string CodigoIntegracaoCFOPEstadualIsento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoCFOPInterestadualIsento", Column = "OCO_CODIGO_INTEGRACAO_CFOP_INTERESTADUAL_ISENTO", TypeType = typeof(string), NotNull = false, Length = 6)]
        public virtual string CodigoIntegracaoCFOPInterestadualIsento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemOcorrencia", Column = "OCO_ORIGEM_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia OrigemOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_PERIODO_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador PeriodoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRateio", Column = "OCO_TIPO_RATEIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula? TipoRateio { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ParametrosOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_PARAMETRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ParametroOcorrencia", Column = "POC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> ParametrosOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ClientesBloqueados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_CLIENTES_BLOQUEADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClientesBloqueados", Column = "OCB_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.ClientesBloqueados> ClientesBloqueados { get; set; }

        /// <summary>
        /// P - PENDENTE
        /// F - FINAL
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "OCO_TIPO", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteInformarValor", Column = "OCO_PERMITE_INFORMAR_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnexoObrigatorio", Column = "OCO_ANEXO_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AnexoObrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalculaValorPorTabelaFrete", Column = "OCO_CALCULA_VALOR_POR_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalculaValorPorTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiltrarCargasPeriodo", Column = "OCO_FILTRAR_CARGAS_PERIODOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarCargasPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloqueiaOcorrenciaDuplicada", Column = "OCO_BLOQUEIA_OCORRENCIA_DUPLICADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloqueiaOcorrenciaDuplicada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TodosCTesSelecionados", Column = "OCO_TODOS_CTES_SELECIONADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TodosCTesSelecionados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearOcorrenciaDuplicadaCargaMesmoMDFe", Column = "OCO_BLOQUEAR_OCORRENCIA_DUPLICADA_CARGA_MESMO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearOcorrenciaDuplicadaCargaMesmoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado", Column = "OCO_BLOQUEAR_OCORRENCIA_CARGA_CANHOTO_FINALIZADO_DIGITALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteSelecionarTomador", Column = "OCO_PERMINTE_SELECIONAR_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaPorPeriodo", Column = "OCO_OCORRENCIA_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaPorPeriodo { get; set; }

        [Obsolete("Atributo temporário para calcular valor da ocorrência no GPA")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_VALOR_BASE_OCORRENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_TIPO_CTE_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TipoCTeIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaComplementoValorFreteCarga", Column = "OCO_OCORRENCIA_COMPLEMENTO_VALOR_FRETE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaComplementoValorFreteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaComVeiculo", Column = "OCO_OCORRENCIA_COM_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaComVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarMobile", Column = "OCO_USAR_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "OCO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_PERCENTUAL_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_CARACTERISTICA_ADICIONAL_CTE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CaracteristicaAdicionalCTe { get; set; }

        /**
         * CAMPO MODIFICADO PARA SER GENÉRICO, NÃO UTILIZA-LO 
         * DROPAR EM 03/10/2018
         */
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_VALOR_AJUDANTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_PERCENTUAL_DO_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoCFOPNFSe", Column = "OCO_CODIGO_INTEGRACAO_CFOP_NFSE", TypeType = typeof(string), NotNull = false, Length = 6)]
        public virtual string CodigoIntegracaoCFOPNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_NAO_GERAR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_DESTINADA_FRANQUIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaDestinadaFranquias { get; set; }

        /**
        * CAMPO MODIFICADO PARA SER GENÉRICO, NÃO UTILIZA-LO 
        * DROPAR EM 03/10/2018
        */
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_POR_QUANTIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaPorQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_POR_PERCENTUAL_DO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaPorPercentualDoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_ENTREGA_REALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregaRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_PERMITE_INFORMAR_APROVADOR_RESPONSAVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarAprovadorResponsavel { get; set; }

        [Obsolete("Passou a ser uma lista de tipo de integrações - Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteSelecionarTodosCTes", Column = "OCO_NAO_PERMITE_SELECIONAR_TODOS_CTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteSelecionarTodosCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirInformarObservacao", Column = "OCO_EXIGIR_INFORMAR_OBSERVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirChamadoParaAbrirOcorrencia", Column = "OCO_EXIGIR_CHAMADO_ABRIR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirChamadoParaAbrirOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_ENVIAR_EMAIL_GERACAO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailGeracaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_EMAIL_GERACAO_OCORRENCIA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string EmailGeracaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOcorrenciaControleEntrega", Column = "OCO_TIPO_OCORRENCIA_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOcorrenciaControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoIndicarAoCliente", Column = "OCO_NAO_INDICAR_AO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIndicarAoCliente { get; set; }

        /// <summary>
        /// É marcada quando a ocorrência é só informativa. Quando ativa, a ocorrência criada não vai gerar um atendimento e nem vai mudar o status da coleta/entrega. Vai apenas
        /// ficar registrado. #38144 e #39209.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAlterarSituacaoColetaEntrega", Column = "OCO_NAO_ALTERAR_SITUACAO_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAlterarSituacaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAplicacaoColetaEntrega", Column = "OCO_TIPO_APLICACAO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega TipoAplicacaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsadoParaMotivoRejeicaoColetaEntrega", Column = "OCO_USADO_PARA_MOTIVO_REJEICAO_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsadoParaMotivoRejeicaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamado { get; set; }

        /// <summary>
        /// Configuração das ocorrências que exibe para o portal dos Terceiros (GPA)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_TERCEIROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_CODIGO_OBSERVACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_ADICIONAR_PIS_COFINS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarPISCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_ADICIONAR_PIS_COFINS_BASE_CALCULO_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarPISCOFINSBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_SOMENTE_CARGAS_FINALIZADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomenteCargasFinalizadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_EXIGIR_MOTIVO_DE_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirMotivoDeDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_POR_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaPorNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_DISPONIBILIZAR_DOCUMENTOS_PARA_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaNFsManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_PERMITE_ALTERAR_NUMERO_DOCUMENTO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarNumeroDocumentoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_QUANTIDADE_REENVIO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeReenvioIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_EXCLUSIVA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaExclusivaParaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_EXIBIR_PARCELAS_NA_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirParcelasNaAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_UTILIZAR_PARCELAMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarParcelamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_FILTRAR_OCORRENCIAS_PERIODO_POR_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarOcorrenciasPeriodoPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_NAO_GERAR_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_DATA_COMPLEMENTO_IGUAL_DATA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DataComplementoIgualDataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_GERAR_APENAS_UM_COMPLEMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarApenasUmComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_EXCLUSIVA_PARA_CANHOTOS_DIGITALIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaExclusivaParaCanhotosDigitalizados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTipoDeOcorrenciaDeCTe", Column = "GTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe GrupoTipoDeOcorrenciaDeCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRejeicaoOcorrencia", Column = "MRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia JustificativaPadraoAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_DATA_OCORRENCIA_IGUAL_DATA_CTE_COMPLEMENTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DataOcorrenciaIgualDataCTeComplementado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_BLOQUEAR_ABERTURA_ATENDIMENTO_PARA_VEICULO_EM_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearAberturaAtendimentoParaVeiculoEmContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_NAO_PERMITE_INFORMAR_VALOR_DA_OCORRENCIA_AO_SELECIONAR_ATENDIMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteInformarValorDaOcorrenciaAoSelecionarAtendimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_GERAR_NFSE_PARA_COMPLEMENTOS_TOMADOR_IGUAL_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNFSeParaComplementosTomadorIgualDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_DIAS_APROVACAO_AUTOMATICA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAprovacaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_HORAS_TOLERANCIA_ENTRADA_SAIDA_RAIO", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasToleranciaEntradaSaidaRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_INFORMAR_MOTIVO_NA_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarMotivoNaAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_PROVISIONADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaProvisionada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_UTILIZAR_ENTRADA_SAIDA_DO_RAIO_CARGA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEntradaSaidaDoRaioCargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_GERAR_OCORRENCIA_COM_MESMO_VALOR_CTES_ANTERIORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaComMesmoValorCTesAnteriores { get; set; }

        /// <summary>
        /// propriedade com nome alterado deve ser VALOR DAS NOTAS (Foi modificada na tarefa #38887)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_GERAR_OCORRENCIA_COM_VALOR_GROSS_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaComValorGrossPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_OCORRENCIA_DIFERENCA_VALOR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaDiferencaValorFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoClassificacaoOcorrencia", Column = "OCO_TIPO_CLASSIFICACAO_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoOcorrencia TipoClassificacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_GERAR_PEDIDO_DEVOLUCAO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoDevolucaoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_PEDIDO_DEVOLUCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_PRAZO_SOLICITACAO_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoSolicitacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrefixoFaturamentoOutrosModelos", Column = "OCO_PREFIXO_FATURAMENTO_OUTROS_MODELOS", TypeType = typeof(string), NotNull = false, Length = 3)]
        public virtual string PrefixoFaturamentoOutrosModelos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoConhecimentoProceda", Column = "OCO_TIPO_CONHECIMENTO_PROCEDA", TypeType = typeof(string), NotNull = false, Length = 1)]
        public virtual string TipoConhecimentoProceda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_DISPONIBILIZAR_PEDIDO_PARA_NOVA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarPedidoParaNovaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoInclusaoImpostoComplemento", Column = "OCO_TIPO_INCLUSAO_IMPOSTO_COMPLEMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoInclusaoImpostoComplemento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoInclusaoImpostoComplemento? TipoInclusaoImpostoComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarTransportadorPorEmail", Column = "OCO_NOTIFICAR_TRANSPORTADOR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarPorEmail", Column = "OCO_NOTIFICAR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmailNotificacao", Column = "OCO_ASSUNTO_EMAIL_NOTIFICACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string AssuntoEmailNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmailNotificacao", Column = "OCO_CORPO_EMAIL_NOTIFICACAO", Type = "StringClob", NotNull = false)]
        public virtual string CorpoEmailNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes", Column = "OCO_IMPEDIR_CRIAR_OCORRENCIA_SE_EXISTIR_CANHOTOS_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteInformarSobras", Column = "OCO_PERMITE_INFORMAR_SOBRAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarSobras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DebitaFreeTimeCalculoValorOcorrencia", Column = "OCO_DEBITE_FREE_TIME_CALCULO_VALOR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DebitaFreeTimeCalculoValorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovaOcorrenciaAguardandoInformacoes", Column = "OCO_NOVA_OCORRENCIA_AGUARDANDO_INFORMACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NovaOcorrenciaAguardandoInformacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApresentarValorPesoDaCarga", Column = "OCO_APRESENTAR_VALOR_PESO_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApresentarValorPesoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirCodigoParaAprovacao", Column = "OCO_EXIGIR_CODIGO_PARA_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirCodigoParaAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_HORAS_SEM_FRANQUIA", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasSemFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EmailsNotificacao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_EMAILS_NOTIFICACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "OCO_CODIGO_EMAIL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual ICollection<string> EmailsNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CanaisDeEntrega", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_CANAL_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CanalEntrega", Column = "CNE_CODIGO")]
        public virtual ICollection<CanalEntrega> CanaisDeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirQueTransportadorSelecioneTipoOcorrencia", Column = "OCO_NAO_PERMITIR_QUE_TRANSPORTADOR_SELECIONE_TIPO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirQueTransportadorSelecioneTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirTransportadorInformarDataInicioFimRaioCarga", Column = "OCO_PERMITIR_TRANSPORTADOR_INFORMAR_DATA_INICIO_FIM_RAIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirTransportadorInformarDataInicioFimRaioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirConsultarCTesComEsseTipoDeOcorrencia", Column = "OCO_PERMITIR_CONSULTAR_CTES_COM_ESSE_TIPO_DE_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirConsultarCTesComEsseTipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoAuxiliar", Column = "OCO_DESCRICAO_AUXILIAR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoAuxiliar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoAuxiliar", Column = "OCO_CODIGO_INTEGRACAO_AUXILIAR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoAuxiliar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FreeTime", Column = "OCO_FREE_TIME", TypeType = typeof(int), NotNull = false)]
        public virtual int FreeTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoProposta", Column = "OCO_TIPO_PROPOSTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaOcorrencia? TipoProposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoReceita", Column = "OCO_TIPO_RECEITA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoReceita), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoReceita? TipoReceita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarEventoIntegracaoCargaEntregaPorCarga", Column = "OCO_GERAR_EVENTO_INTEGRACAO_CARGA_ENTREGA_POR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarEventoIntegracaoCargaEntregaPorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEventoOcorrenciaPrimeiro", Column = "OCO_CODIGO_EVENTO_OCORRENCIA_PRIMEIRO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEventoOcorrenciaPrimeiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEventoOcorrenciaSegundo", Column = "OCO_CODIGO_EVENTO_OCORRENCIA_SEGUNDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEventoOcorrenciaSegundo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarClientePorEmail", Column = "OCO_NOTIFICAR_CLIENTE_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarClientePorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaPorAjudante", Column = "OCO_OCORRENCIA_POR_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaPorAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvio", Column = "OCO_TIPO_ENVIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracaoNeokohm), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracaoNeokohm? TipoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoComprovei", Column = "OCO_TIPO_INTEGRACAO_COMPROVEI", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoComprovei), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoComprovei? TipoIntegracaoComprovei { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarIntegracaoComprovei", Column = "OCO_ATIVAR_INTEGRACAO_COMPROVEI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarIntegracaoComprovei { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecalcularPrevisaoEntregaPendentes", Column = "OCO_RECALCULAR_PREVISAO_ENTREGAS_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecalcularPrevisaoEntregaPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaFinalizaViagem", Column = "OCO_OCORRENCIA_FINALIZA_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaFinalizaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoTipoPrevisao", Column = "OCO_DESCRICAO_TIPO_PREVISAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoTipoPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioAutomaticoTipoOcorrencia", Column = "OCO_ATIVAR_ENVIO_AUTOMATICO_TIPO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioAutomaticoTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EssaOcorrenciaGeraOutraOcorrenciaIntegracao", Column = "OCO_OCORRENCIA_GERA_OUTRA_OCORRENCIA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EssaOcorrenciaGeraOutraOcorrenciaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "TOP_CODIGO_TIPO_OCORRENCIA_INTEGRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularDistanciaPorCTe", Column = "OCO_CALCULAR_DISTANCIA_POR_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularDistanciaPorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar", Column = "OCO_COPIAR_OBSERVACOES_DO_CTE_DE_ORIGEM_AO_GERAR_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAtendimentoAutomaticamente", Column = "OCO_GERAR_ATENDIMENTO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAtendimentoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO_GERACAO_AUTOMATICA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamadoGeracaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirValorSuplementoMaiorQueDocumento", Column = "OCO_NAO_PERMITIR_VALOR_SUPLEMENTO_MAIOR_QUE_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirValorSuplementoMaiorQueDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarProdutoLancamentoOcorrencia", Column = "OCO_INFORMAR_PRODUTO_NO_LANCAMENTO_DA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarProdutoLancamentoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOcorrenciaAutomaticamenteRejeicaoMobile", Column = "OCO_GERAR_OCORRENCIA_AUTOMATICAMENTE_REJEICAO_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaAutomaticamenteRejeicaoMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTFilialEmissora", Column = "OCO_CST_FILIAL_EMISSORA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTSubContratada", Column = "OCO_CST_SUB_CONTRATADA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTSubContratada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAlterarDataPrevisaoEntrega", Column = "OCO_PERMITIR_ALTERAR_DATA_PREVISAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarDataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirDocumentoParaFilialEmissoraComPreCTe", Column = "OCO_EMITIR_DOCUMENTO_PARA_FILIAL_EMISSORA_COM_PRE_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirDocumentoParaFilialEmissoraComPreCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirMotivoUltimaAprovacaoPortalTransportador", Column = "OCO_EXIBIR_MOTIVO_ULTIMA_APROVACAO_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirMotivoUltimaAprovacaoPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAlterarOcorrenciaAposReprovacao", Column = "OCO_PERMITE_ALTERAR_OCORRENCIA_APOS_REPROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarOcorrenciaAposReprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirSelecionarEssaOcorrenciaNoPortalDoCliente", Column = "OCO_PERMITIR_SELECIONAR_ESSA_OCORRENCIA_NO_PORTAL_DO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarEssaOcorrenciaNoPortalDoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaParaCobrancaDePedagio", Column = "OCO_OCORRENCIA_PARA_COBRANCA_DE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaParaCobrancaDePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias", Column = "OCO_SALVAR_CAUSADOR_DA_OCORRENCIA_NA_GESTAO_DE_OCORRENCIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EfetuarOControleQuilometragem", Column = "OCO_EFETUAR_O_CONTROLE_QUILOMETRAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EfetuarOControleQuilometragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao", Column = "OCO_EFETUAR_CALCULO_VALOR_OCORRENCIA_BASEADO_NOTAS_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaParaQuebraRegraPallet", Column = "OCO_OCORRENCIA_PARA_QUEBRA_REGRA_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaParaQuebraRegraPallet { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteInformarCausadorOcorrencia", Column = "OCO_PERMITE_INFORMAR_CAUSADOR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarCausadorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarGrupoOcorrencia", Column = "OCO_PERMITE_INFORMAR_GRUPO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarGrupoOcorrencia { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirEnviarOcorrenciaSemAprovacaoPreCTe", Column = "OCO_PERMITIR_ENVIAR_OCORRENCIA_SEM_APROVACAO_PRE_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnviarOcorrenciaSemAprovacaoPreCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularValorCTEComplementarPeloValorCTESemImposto", Column = "OCO_CALCULAR_VALOR_CTE_COMPLEMENTAR_PELO_VALOR_CTE_SEM_IMPOSTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularValorCTEComplementarPeloValorCTESemImposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirReabrirOcorrenciaEmCasoDeRejeicao", Column = "OCO_PERMITE_REABRIR_OCORRENCIA_EM_CASO_DE_REJEICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirReabrirOcorrenciaEmCasoDeRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCO_REMARK_SPED", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RemarkSped), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RemarkSped? RemarkSped { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoCalcularValorOcorrenciaAutomaticamente", Column = "OCO_NAO_CALCULAR_VALOR_OCORRENCIA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCalcularValorOcorrenciaAutomaticamente { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistSuperApp", Column = "CSA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "IdSuperApp", Column = "OCO_ID_SUPER_APP", TypeType = typeof(string), NotNull = false, Length = 24)]
        public virtual string IdSuperApp { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case "P":
                        return "Pendente";
                    case "F":
                        return "Final";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoCompleta
        {
            get
            {
                return Descricao + (string.IsNullOrWhiteSpace(DescricaoPortal) ? "" : $" - ({DescricaoPortal})");
            }
        }

        public virtual string DescricaoVisualizacao
        {
            get
            {
                return !string.IsNullOrWhiteSpace(DescricaoPortal) ? DescricaoPortal : Descricao;
            }
        }

        #endregion
    }
}
