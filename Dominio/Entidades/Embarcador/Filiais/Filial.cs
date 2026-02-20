using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILIAL", EntityName = "Filial", Name = "Dominio.Entidades.Embarcador.Filiais.Filial", NameType = typeof(Filial))]
    public class Filial : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Filiais.Filial>
    {
        public Filial() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "FIL_CNPJ", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FIL_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFilialEmbarcador", Column = "FIL_CODIGO_FILIAL_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoFilialEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_REDESPACHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroUnidadeImpressao", Column = "FIL_NUMERO_UNIDADE_IMPRESSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroUnidadeImpressao { get; set; }

        [Obsolete("Descontinuado, utiluzado anteriormente para gerar o controle de expedição na geração de carga pela leitura de EDI")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlaExpedicao", Column = "FIL_CONTROLA_EXPEDICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarSolicitacaoSuprimentoDeGas", Column = "FIL_HABILITAR_SOLICITACAO_GAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarSolicitacaoSuprimentoDeGas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirDescricaoRemetente", Column = "FIL_EXIBIR_DESCRICAO_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDescricaoRemetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [Obsolete("Transformado em uma lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SuprimentoDeGas", Column = "SDG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SuprimentoDeGas SuprimentoDeGas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFilial", Column = "FIL_TIPO_FILIAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFilial), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFilial TipoFilial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIL_EMAIL", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIL_SIGLA_FILIAL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SiglaFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIL_ACCOUNTNAME_INTEGRACAO_VTEX", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string AccountNameVtex { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "FIL_EMPRESA_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor SetorAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmiteMDFeFilialEmissora", Column = "FIL_EMITIR_MDFE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteMDFeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirMDFeManualmente", Column = "FIL_EMITIR_MDFE_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirMDFeManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailDiariaContainer", Column = "FIL_EMAIL_DIARIA_CONTAINER", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string EmailDiariaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirPreCargaMontagemCarga", Column = "FIL_EXIGIR_PRE_CARGA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirPreCargaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirConfirmacaoTransporte", Column = "FIL_EXIGIR_CONFIRMACAO_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirConfirmacaoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlaFluxoColetaEntrega", Column = "FIL_CONTROLA_FLUXO_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaFluxoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FIL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIL_VALOR_MEDIO_MERCADORIA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ValorMedioMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAdicionarValorDescarga", Column = "FIL_NAO_ADICIONA_VALOR_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAdicionarValorDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarCondicao", Column = "FIL_ATIVAR_CONDICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarCondicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarVeiculoIntegracao", Column = "FIL_NAO_VALIDAR_VEICULO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarVeiculoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarAutomaticamentePagamento", Column = "FIL_LIBERAR_AUTOMATICAMENTE_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarAutomaticamentePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CompraValePedagio", Column = "FIL_COMPRA_VALE_PEDAGIO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao CompraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoraValePedagio", Column = "FIL_INTEGRADORA_VALE_PEDAGIO", TypeType = typeof(Dominio.Enumeradores.IntegradoraValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraValePedagio IntegradoraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FornecedorValePedagio", Column = "FIL_FORNECEDOR_VALE_PEDAGIO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioValePedagio", Column = "FIL_USUARIO_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaValePedagio", Column = "FIL_SENHA_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenValePedagio", Column = "FIL_TOKEN_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoSemParar", Column = "CIA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoSemParar IntegracaoSemParar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoBuonny", Column = "CIB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoBuonny IntegracaoBuonny { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaCertificado", Column = "FIL_SENHA_CERTIFICADO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieCertificado", Column = "FIL_SERIECERTIFICADO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SerieCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeCertificado", Column = "FIL_NOME_CERTIFICADO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialCertificado", Column = "FIL_DATA_CERTIFICADOINI", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalCertificado", Column = "FIL_DATA_CERTIFICADOFINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalCertificado { get; set; }

        [Obsolete("Mudado para ser por tipo de carga do suprimento de gás")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaNotificacaoAbastecimentoGas", Column = "FIL_DATA_ULTIMA_NOTIFICACAO_ABASTECIMENTO_GAS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaNotificacaoAbastecimentoGas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoTarget", Column = "CIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Configuracoes.IntegracaoTarget IntegracaoTarget { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIL_GERAR_CIOT_PARA_TODAS_AS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOTParaTodasAsCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirAgruparCargaMesmaFilial", Column = "FIL_NAO_PERMITIR_AGRUPAR_CARGA_MESMA_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAgruparCargaMesmaFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIL_EXIGE_CONFIRMACAO_FRETE_ANTES_EMITIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeConfirmacaoFreteAntesEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoOperacoesIsencaoValorDescargaCliente", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FILIAL_TIPO_OPERACAO_ISENCAO_VALOR_DESCARGA_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TipoOperacoesIsencaoValorDescargaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "OutrosCodigosIntegracao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FILIAL_OUTROS_CODIGOS_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "FIL_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual ICollection<string> OutrosCodigosIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Setores", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FILIAL_SETORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SetorFilial", Column = "SEF_CODIGO")]
        public virtual ICollection<SetorFilial> Setores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CondicoesPagamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONDICAO_PAGAMENTO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CondicaoPagamentoFilial", Column = "CPF_CODIGO")]
        public virtual ICollection<CondicaoPagamentoFilial> CondicoesPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Descontos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FILIAL_DESCONTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FilialDesconto", Column = "FID_CODIGO")]
        public virtual ICollection<FilialDesconto> Descontos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DescontosExcecao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FILIAL_DESCONTO_EXCECAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FilialDescontoExcecao", Column = "FDE_CODIGO")]
        public virtual ICollection<FilialDescontoExcecao> DescontosExcecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasDeCortePlanejamentoDiario", Column = "FIL_DIAS_CORTE_PLANEJAMENTO_DIARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasDeCortePlanejamentoDiario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicialPlanejamentoDiario", Column = "FIL_HORA_INICIAL_PLANEJAMENTO_DIARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HoraInicialPlanejamentoDiario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFinalPlanejamentoDiario", Column = "FIL_HORA_FINAL_PLANEJAMENTO_DIARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HoraFinalPlanejamentoDiario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "FIL_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioCorteParaCalculoLeadTime", Column = "FIL_HORARIO_CORTE_PARA_CALCULO_LEAD_TIME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioCorteParaCalculoLeadTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarPreViagemTrizy", Column = "FIL_HABILITAR_PRE_VIAGEM_TRIZY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarPreViagemTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos", Column = "FIL_HORA_CORTE_RECALCULAR_PRAZO_ENTREGA_APOS_EMISSAO_DOCUMENTOS", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarParaFilaCarregamento", Column = "FIL_LIBERAR_PARA_FILA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarParaFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarIntegracaoP44", Column = "FIL_GERAR_INTEGRACAO_P44", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoP44 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarIntegracaoKlios", Column = "FIL_GERAR_INTEGRACAO_KLIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoKlios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCtesAnterioresComoCteFilialEmissora", Column = "FIL_UTILIZAR_CTES_ANTERIORES_COMO_CTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCtesAnterioresComoCteFilialEmissora { get; set; }

        [Obsolete("Esse campo não está sendo usado. Novo EmailCargaContainerCancelado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailCargaCanceladaContainer", Column = "FIL_EMAIL_CARGA_CANCELADA_CONTAINER", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string EmailCargaCanceladaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarContainerCargaCancelada", Column = "FIL_NOTIFICAR_CONTAINER_CARGA_CANCELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarContainerCargaCancelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarEquipamentoFluxoPatio", Column = "FIL_INFORMAR_EQUIPAMENTO_FLUXO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarEquipamentoFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoOperacoesTrizy", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FILIAL_TIPO_OPERACAO_TRIZY")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TipoOperacoesTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmiteMDFeFilialEmissoraPorEstadoDestino", Column = "FIL_EMITIR_MDFE_FILIAL_EMISSORA_POR_ESTADO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteMDFeFilialEmissoraPorEstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoRest", Column = "FIL_URL_INTEGRACAO_REST", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLIntegracaoRest { get; set; }

        #region Balança

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "MarcaBalanca", Column = "FIL_MARCA_BALANCA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string MarcaBalanca { get; set; }

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloBalanca", Column = "FIL_MODELO_BALANCA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ModeloBalanca { get; set; }

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "HostConsultaBalanca", Column = "FIL_HOST_CONSULTA_BALANCA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string HostConsultaBalanca { get; set; }

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "PortaBalanca", Column = "FIL_PORTA_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int PortaBalanca { get; set; }

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "TamanhoRetornoBalanca", Column = "FIL_TAMANHO_RETORNO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int TamanhoRetornoBalanca { get; set; }

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "PosicaoInicioPesoBalanca", Column = "FIL_POSICAO_INICIO_PESO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoInicioPesoBalanca { get; set; }

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "PosicaoFimPesoBalanca", Column = "FIL_POSICAO_FIM_PESO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoFimPesoBalanca { get; set; }

        [Obsolete("Migrado para a lista FilialBalanca")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "CasasDecimaisPesoBalanca", Column = "FIL_CASAS_DECIMAIS_PESO_BALANCA", TypeType = typeof(int), NotNull = false)]
        public virtual int CasasDecimaisPesoBalanca { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        #region Propriedades Virtuais

        public virtual string RetornaTipoFilial
        {
            get
            {
                return TipoFilial.ObterDescricao();
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return Localization.Resources.Gerais.Geral.Ativo;
                else
                    return Localization.Resources.Gerais.Geral.Inativo;
            }
        }

        public virtual string CNPJ_Formatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ));
            }
        }
        public virtual string CNPJ_SemFormato
        {
            get
            {
                return string.Format(@"{0:00000000000000}", long.Parse(this.CNPJ));
            }
        }

        public virtual bool Equals(Filial other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Clonar()
        {
            return (Dominio.Entidades.Embarcador.Filiais.Filial)this.MemberwiseClone();
        }

        #endregion
    }
}
