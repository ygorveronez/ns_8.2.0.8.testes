using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG", EntityName = "ConfiguracaoEmpresa", Name = "Dominio.Entidades.ConfiguracaoEmpresa", NameType = typeof(ConfiguracaoEmpresa))]
    public class ConfiguracaoEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceDeSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ApoliceDeSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoNFSe", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ServicoNFSe ServicoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoNFSe", Column = "SER_CODIGO_FORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ServicoNFSe ServicoNFSeFora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaNFSe", Column = "NAN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NaturezaNFSe NaturezaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaNFSe", Column = "NAN_CODIGO_FORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NaturezaNFSe NaturezaNFSeFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTeAvancadaTerceiros", Column = "COF_OBS_CTE_AVANCADA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeAvancadaTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTeAvancadaProprio", Column = "COF_OBS_CTE_AVANCADA_PROPRIO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeAvancadaProprio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTeNormal", Column = "COF_OBS_NORMAL", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeNormal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTeComplementar", Column = "COF_OBS_COMPLEMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTeAnulacao", Column = "COF_OBS_ANULACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTeSubstituicao", Column = "COF_OBS_SUBSTITUICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaEntrega", Column = "COF_DIAS_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaEmissaoDeCTeComplementar", Column = "COF_DIAS_COMP", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaEmissaoDeCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaEmissaoDeCTeSubstituicao", Column = "COF_DIAS_SUB", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaEmissaoDeCTeSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaEmissaoDeCTeAnulacao", Column = "COF_DIAS_ANULACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaEmissaoDeCTeAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearDuplicidadeCTeAcerto", Column = "COF_BLOQUEAR_DUPLICIDADE_CTE_ACERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearDuplicidadeCTeAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorDeLotacao", Column = "COF_INDICADOR_LOTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorDeLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirSemValorDaCarga", Column = "COF_EMITIR_SEM_VALOR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirSemValorDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CadastrarItemDocumentoEntrada", Column = "COF_CADASTRAR_ITEM_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarItemDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteSelecionarCTeOutroTomador", Column = "COF_PERMITE_SELECIONAR_CTE_OUTRO_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarCTeOutroTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DicasEmissaoCTe", Column = "COF_DICAS", Type = "StringClob", NotNull = false)]
        public virtual string DicasEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominante", Column = "COF_PRODUTO_PREDOMINANTE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string ProdutoPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasCaracteristicas", Column = "COF_OUTRAS_CARACTERISTICAS", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string OutrasCaracteristicas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressao", Column = "COF_TIPO_IMPRESSAO", TypeType = typeof(Dominio.Enumeradores.TipoImpressao), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoImpressao TipoImpressao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_CONTA_ABASTECIMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta PlanoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_CONTA_PAGAMENTO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta PlanoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_CONTA_CTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta PlanoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaTabelaDeFrete", Column = "COF_UTILIZA_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaTabelaDeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteVincularMesmaPlacaOutrosVeiculos", Column = "COF_PERMITE_VINCULAR_MESMA_PLACA_OUTROS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteVincularMesmaPlacaOutrosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_COPIAR_IMPOSTOS_CTE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCopiarImpostosCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeraDuplicatasAutomaticamente", Column = "COF_GERA_DUPLICATAS_AUTO", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao GeraDuplicatasAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaVencimentoDasDuplicatas", Column = "COF_DIAS_VENCTO_DUPLICATAS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaVencimentoDasDuplicatas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDeParcelasDasDuplicatas", Column = "COF_NUM_PARCELAS_DUPLICATAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDeParcelasDasDuplicatas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "COF_SERIE_INTERESTADUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie SerieInterestadual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "COF_SERIE_INTRAESTADUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie SerieIntraestadual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "COF_SERIE_MDFE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie SerieMDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "COF_SERIE_NFSE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie SerieNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ICMSIsento", Column = "COF_ICMS_ISENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ICMSIsento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGeracaoCTeWS", Column = "COF_TIPO_GERACAO_CTE_WS", TypeType = typeof(Enumeradores.TipoGeracaoCTeWS), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoGeracaoCTeWS TipoGeracaoCTeWS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSNoFrete", Column = "COF_INCLUIR_ICMS", TypeType = typeof(Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirICMSNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Perfil", Column = "COF_PERFIL", TypeType = typeof(Enumeradores.PerfilEmpresa), NotNull = false)]
        public virtual Dominio.Enumeradores.PerfilEmpresa Perfil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CriterioEscrituracaoEApuracao", Column = "COF_CRITERIO_ESCRITURACAO_APURACAO", TypeType = typeof(Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado), NotNull = false)]
        public virtual Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado CriterioEscrituracaoEApuracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncidenciaTributariaNoPeriodo", Column = "COF_INCIDENCIA_TRIBUTARIA_PERIODO", TypeType = typeof(Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo), NotNull = false)]
        public virtual Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo IncidenciaTributariaNoPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPISCOFINS", Column = "COF_CST_PISCOFINS", Type = "AnsiString", NotNull = false)]
        public virtual string CSTPISCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPIS", Column = "COF_CST_PIS", TypeType = typeof(Enumeradores.TipoPIS), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPIS? CSTPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "COF_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 5, NotNull = false)]
        public virtual decimal? AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINS", Column = "COF_CST_COFINS", TypeType = typeof(Enumeradores.TipoCOFINS), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCOFINS? CSTCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "COF_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 5, NotNull = false)]
        public virtual decimal? AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaIntegracaoAvon", Column = "COF_INTEGRACAO_AVON", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaIntegracaoAvon { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenIntegracaoAvon", Column = "COF_TOKEN_INTEGRACAO_AVON", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenIntegracaoAvon { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenIntegracaoCTe", Column = "COF_TOKEN_INTEGRACAO_CTE", TypeType = typeof(string), Length = 36, NotNull = false)]
        public virtual string TokenIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoContratanteSigaFacil", Column = "COF_COD_CONTRATANTE_SIGA_FACIL", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string CodigoContratanteSigaFacil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCriptograficaSigaFacil", Column = "COF_CHAVE_CRIPTO_SIGA_FACIL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ChaveCriptograficaSigaFacil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NSUSigaFacil", Column = "COF_NSU_SIGA_FACIL", TypeType = typeof(int), NotNull = false)]
        public virtual int NSUSigaFacil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FraseSecretaNFSe", Column = "COF_NFSE_FRASE_SECRETA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FraseSecretaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaNFSe", Column = "COF_NFSE_SENHA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieRPSNFSe", Column = "COF_NFSE_SERIE_RPS", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SerieRPSNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoSeguroATM", Column = "COF_CODIGO_SEGURO_ATM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoSeguroATM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioSeguroATM", Column = "COF_USUARIO_SEGURO_ATM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioSeguroATM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSeguroATM", Column = "COF_SENHA_SEGURO_ATM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaSeguroATM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbaAutomaticoATM", Column = "COF_AVERBA_AUTOMATICO_ATM", TypeType = typeof(int), NotNull = false)]
        public virtual int AverbaAutomaticoATM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualImpostoSimplesNacional", Column = "COF_PERCENTUAL_IMPOSTO_SIMPLES_NACIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal? PercentualImpostoSimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegradoraCIOT", Column = "COF_TIPO_INTEGRADORA_CIOT", TypeType = typeof(ObjetosDeValor.Enumerador.TipoIntegradoraCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.TipoIntegradoraCIOT? TipoIntegradoraCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatoriedadeCIOTEmissaoMDFe", Column = "COF_OBRIGATORIEDADE_CIOT_EMISSAO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ObrigatoriedadeCIOTEmissaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamentoCIOT", Column = "COF_TIPO_PAGAMENTO_CIOT", TypeType = typeof(ObjetosDeValor.Enumerador.TipoPagamentoCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.TipoPagamentoCIOT? TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegradorEFrete", Column = "COF_CODIGO_INTEGRADOR_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegradorEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioEFrete", Column = "COF_USUARIO_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaEFrete", Column = "COF_SENHA_EFRETE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ENCERRAR_CIOT_POR_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarCIOTPorViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoGratuitaEFrete", Column = "COF_EMISSAO_GRATUITA_EFRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoGratuitaEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearNaoEquiparadoEFrete", Column = "COF_BLOQUEAR_NAO_EQUIPARADO_EFRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearNaoEquiparadoEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMatrizNatura", Column = "COF_COD_MATRIZ_NATURA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoMatrizNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFilialNatura", Column = "COF_COD_FILIAL_NATURA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoFilialNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioNatura", Column = "COF_USUARIO_NATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaNatura", Column = "COF_SENHA_NATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPNaturaHost", Column = "COF_HOST_FTP_NATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FTPNaturaHost { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPNaturaPorta", Column = "COF_PORTA_FTP_NATURA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string FTPNaturaPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPNaturaUsuario", Column = "COF_USUARIO_FTP_NATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FTPNaturaUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPNaturaSenha", Column = "COF_SENHA_FTP_NATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FTPNaturaSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPNaturaDiretorio", Column = "COF_DIRETORIO_FTP_NATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FTPNaturaDiretorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPNaturaPassivo", Column = "COF_PASSIVO_FTP_NATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FTPNaturaPassivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FTPNaturaSeguro", Column = "COF_SEGURO_FTP_NATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FTPNaturaSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaturaEnviaOcorrenciaEntreguePadrao", Column = "COF_ENVIA_OCORRENCIA_ENTREGUE_PADRAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaturaEnviaOcorrenciaEntreguePadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "COF_LAY_OCOREN", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDIOcoren { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDINatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirNaturaAutomatico", Column = "COF_EMITIR_NATURA_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirNaturaAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailsNotificacaoNatura", Column = "COF_EMAILS_NOTIFICACAO_NATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailsNotificacaoNatura { get; set; }

        [Obsolete("Era utilizado para saber se era para usar a obs somente em veículos terceiros")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAvancadaVeiculosTerceiros", Column = "COF_OBS_AVANCADA_VEIC_TERCEIROS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObservacaoAvancadaVeiculosTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomePDFCTe", Column = "COF_NOME_PDF_CTE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NomePDFCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrincipalFilialEmissoraTMS", Column = "COF_PRINCIPAL_FILIAL_EMISSORA_TMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PrincipalFilialEmissoraTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmpresaPadraoLancamentoGuarita", Column = "COF_EMPRESA_PADRAO_LANCAMENTO_GUARITA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmpresaPadraoLancamentoGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmpresaCIOT", Column = "COF_TIPO_EMPRESA_CIOT", TypeType = typeof(Enumeradores.TipoEmpresaCIOT), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoEmpresaCIOT TipoEmpresaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTeSimplesNacional", Column = "COF_OBS_CTE_SIMPLES_NACIONAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ObservacaoCTeSimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtualizaVeiculoImpXMLCTe", Column = "COF_ATUALIZA_VEICULO_IMP_XML_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizaVeiculoImpXMLCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPrefeituraNFSe", Column = "COF_NFSE_URL_PREFEITURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLPrefeituraNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LoginSitePrefeituraNFSe", Column = "COF_NFSE_LOGIN_PREFEITURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string LoginSitePrefeituraNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSitePrefeituraNFSe", Column = "COF_NFSE_SENHA_PREFEITURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SenhaSitePrefeituraNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoIntegracaoNFSe", Column = "COF_NFSE_OBSERVACAO_INTEGRACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoIntegracaoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoPadraoNFSe", Column = "COF_NFSE_OBSERVACAO_PADRAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoPadraoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WsIntegracaoEnvioCTe", Column = "COF_WS_INTEGRACAO_ENVIO_CTE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string WsIntegracaoEnvioCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenIntegracaoEnvioCTe", Column = "COF_TOKEN_INTEGRACAO_ENVIO_CTE", TypeType = typeof(string), Length = 36, NotNull = false)]
        public virtual string TokenIntegracaoEnvioCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoCTe", Column = "COF_VERSAO_CTE", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string VersaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoMDFe", Column = "COF_VERSAO_MDFE", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string VersaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrazoCancelamentoCTe", Column = "COF_PRAZO_CANCELAMENTO_CTE", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoCancelamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrazoCancelamentoMDFe", Column = "COF_PRAZO_CANCELAMENTO_MDFE", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoCancelamentoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLimiteFrete", Column = "COF_VALOR_LIMITE_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLimiteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssinaturaEmail", Column = "COF_ASSINATURA_EMAIL", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AssinaturaEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLoteEmissaoCTe", Column = "COF_NUMERO_LOTE_EMISSAO_CTE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string NumeroLoteEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJTransportadorComoCNPJSeguradora", Column = "COF_CNPJT_CNPJS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao? CNPJTransportadorComoCNPJSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApoliceComoNumeroAverbacao", Column = "COF_NAPOL_NAVERB", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao? NumeroApoliceComoNumeroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasParaAvisoVencimentos", Column = "COF_DIAS_AVISO_VENCIMENTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasParaAvisoVencimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaNovaImportacaoEDI", Column = "COF_UTILIZA_NOVA_IMPORTACAO_EDI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaNovaImportacaoEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloPadrao", Column = "COF_MODELO_PADRAO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string ModeloPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFSeIntegracaoENotas", Column = "COF_NFSE_INTEGRACAO_ENOTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NFSeIntegracaoENotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFSeKeyENotas", Column = "COF_NFSE_KEY_ENOTAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NFSeKeyENotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFSeURLENotas", Column = "COF_NFSE_URL_ENOTAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NFSeURLENotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFSeCPF", Column = "COF_NFSE_CPF", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string NFSeCPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailSemTexto", Column = "COF_EMAIL_SEM_TEXTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailSemTexto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SeguradoraAverbacao", Column = "COF_SEGURADORA_AVERBACAO", TypeType = typeof(Dominio.Enumeradores.IntegradoraAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraAverbacao? SeguradoraAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenAverbacaoBradesco", Column = "COF_TOKEN_AVERBACAO_BRADESCO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenAverbacaoBradesco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WsdlAverbacaoQuorum", Column = "COF_WSDL_AVERBACAO_QUORUM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string WsdlAverbacaoQuorum { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EstadosDeEmissao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_CONFIG_ESTADOS_DE_EMISSAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Dominio.Entidades.Estado> EstadosDeEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_CNPJ_SEGURO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NOME_SEGURO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NomeSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NUMERO_APOLICE_SEGURO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NumeroApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_AVERBACAO_SEGURO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string AverbacaoSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_RESPONSAVEL_SEGURO", TypeType = typeof(Dominio.Enumeradores.TipoSeguro), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoSeguro? ResponsavelSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUtilizarDadosSeguroEmpresaPai", Column = "COF_NAO_UTILIZAR_DADOS_SEGURO_EMPRESA_PAI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarDadosSeguroEmpresaPai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardarAverbacaoCTeParaEmitirMDFe", Column = "COF_AGUARDAR_AVERBACAO_CTE_EMISSAO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardarAverbacaoCTeParaEmitirMDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "COF_SERIE_CTE_COMPLEMENTAR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie SerieCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbarComoEmbarcador", Column = "COF_AVERBAR_COMO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarComoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValePedagioIntegradora", Column = "COF_VALE_PEDAGIO_INTEGRADORA", TypeType = typeof(Dominio.Enumeradores.IntegradoraValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraValePedagio ValePedagioIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValePedagioIntegraAutomatico", Column = "COF_VALE_PEDAGIO_INTEGRA_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int ValePedagioIntegraAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValePedagioUsuario", Column = "COF_VALE_PEDAGIO_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ValePedagioUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValePedagioSenha", Column = "COF_VALE_PEDAGIO_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ValePedagioSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValePedagioToken", Column = "COF_VALE_PEDAGIO_TOKEN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ValePedagioToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValePedagioFornecedor", Column = "COF_VALE_PEDAGIO_FORNECEDOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string ValePedagioFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValePedagioResponsavel", Column = "COF_VALE_PEDAGIO_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string ValePedagioResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EncerramentoMDFeAutomatico", Column = "COF_ENCERRAMENTO_MDFE_AUTOMATICO", TypeType = typeof(Dominio.Enumeradores.EncerramentoMDFeAutomatico), NotNull = false)]
        public virtual Dominio.Enumeradores.EncerramentoMDFeAutomatico EncerramentoMDFeAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoCTeComUFDestinosDiferentes", Column = "COF_BLOQUARE_MISSAO_CTE_COM_UF_DESTINOS_DIFERENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoCTeComUFDestinosDiferentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPesoNFe", Column = "COF_TIPO_PESO_NFE", TypeType = typeof(Dominio.Enumeradores.TipoPesoNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoPesoNFe TipoPesoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_BLOQUEAR_EMISSAO_MDFE_WS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoMDFeWS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarNFSeImportacoes", Column = "COF_GERAR_NFSE_IMPORTACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNFSeImportacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTeAverbacao", Column = "COF_AVERBA_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTEAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTEAverbacao TipoCTeAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbarMDFe", Column = "COF_AVERBAR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormatarPlacaComHifenNaObservacao", Column = "COF_PLACA_COM_HIFEM_OBS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FormatarPlacaComHifenNaObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportacaoNaoRateiaPedagio", Column = "COF_IMPORTACAO_NAO_RATEIA_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportacaoNaoRateiaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TagParaImportarObservacaoNFe", Column = "COF_TAG_PARA_IMPORTAR_OBSERVACAO_NFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TagParaImportarObservacaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TamanhoTagObservacaoNFe", Column = "COF_TAG_TAMANHO_INFORMACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TamanhoTagObservacaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCTeIntegracaoDocumentosMunicipais", Column = "COF_GERAR_CTE_INTEGRACAO_DOCUMENTOS_MUNICIPAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCTeIntegracaoDocumentosMunicipais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoMedidaKgCTe", Column = "COF_DESCRICAO_UNIDADE_KG_CTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string DescricaoMedidaKgCTe { get; set; }

        public virtual int ProximoNSUSigaFacil
        {
            get
            {
                this.NSUSigaFacil++;

                return this.NSUSigaFacil;
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LoginCTF", Column = "COF_LOGIN_CTF", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LoginCTF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaCTF", Column = "COF_SENHA_CTF", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaCTF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PonteiroCTF", Column = "COF_PONTEIRO_CTF", TypeType = typeof(int), NotNull = false)]
        public virtual int PonteiroCTF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodTemplateCTF", Column = "COF_COD_TEMPLATE_CTF", TypeType = typeof(int), NotNull = false)]
        public virtual int CodTemplateCTF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdRegistroCTF", Column = "COF_QTD_REGUSTRO_CTF", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdRegistroCTF { get; set; }

        //public virtual bool NFSeIntegracaoENotas
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        //public virtual string NFSeKeyENotas
        //{
        //    get
        //    {
        //        return "MjE0ZjRlMTUtZGU1OS00NTM4LTg2ZDQtZWI2Y2E3ZDYwMjAw";
        //    }
        //}

        //public virtual string NFSeURLENotas
        //{
        //    get
        //    {
        //        return "https://api.enotasgw.com.br";
        //    }
        //}

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_ACERTO_VIAGEM_CONTA_RECEITAS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta AcertoViagemContaReceitas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ACERTO_VIAGEM_MOVIMENTO_RECEITAS", TypeType = typeof(Dominio.Enumeradores.TipoMovimentoAcerto), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoMovimentoAcerto AcertoViagemMovimentoReceitas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_ACERTO_VIAGEM_CONTA_DESPESAS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta AcertoViagemContaDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ACERTO_VIAGEM_MOVIMENTO_DESPESAS", TypeType = typeof(Dominio.Enumeradores.TipoMovimentoAcerto), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoMovimentoAcerto AcertoViagemMovimentoDespesas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_ACERTO_VIAGEM_CONTA_DESPESAS_ABASTECIMENTOS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta AcertoViagemContaDespesasAbastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ACERTO_VIAGEM_MOVIMENTO_DESPESASA_BASTECIMENTOS", TypeType = typeof(Dominio.Enumeradores.TipoMovimentoAcerto), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoMovimentoAcerto AcertoViagemMovimentoDespesasAbastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_ACERTO_VIAGEM_CONTA_DESPESAS_PAGAMENTOS_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta AcertoViagemContaDespesasPagamentosMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_ACERTO_VIAGEM_CONTA_DESPESAS_ADIANTAMENTOS_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta AcertoViagemContaDespesasAdiantamentosMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ACERTO_VIAGEM_MOVIMENTO_DESPESASA_DIANTAMENTOSMOTORISTA", TypeType = typeof(Dominio.Enumeradores.TipoMovimentoAcerto), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoMovimentoAcerto AcertoViagemMovimentoDespesasAdiantamentosMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "COF_ACERTO_VIAGEM_CONTA_RECEITAS_DEVOLUCOES_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoDeConta AcertoViagemContaReceitasDevolucoesMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ACERTO_VIAGEM_MOVIMENTO_RECEITAS_DEVOLUCOES_MOTORISTA", TypeType = typeof(Dominio.Enumeradores.TipoMovimentoAcerto), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoMovimentoAcerto AcertoViagemMovimentoReceitasDevolucoesMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_EMITE_NFSE_FORA_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteNFSeForaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoCalcularDIFALCTeOS", Column = "COF_NAO_CALCULAR_DIFAL_CTEOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCalcularDIFALCTeOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoCalcularDIFALTomadorExterior", Column = "COF_NAO_CALCULAR_DIFAL_TOMADOR_EXTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCalcularDIFALTomadorExterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirDuplciataMesmoDocumento", Column = "COF_NAO_PERMITIR_DUPLICATA_MESMO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirDuplciataMesmoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomeVencimentoCertificado", Column = "COF_EXIBIR_HOME_VENCIMENTO_CERTIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomeVencimentoCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomePendenciasEntrega", Column = "COF_EXIBIR_HOME_PENDENCIAS_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomePendenciasEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomeGraficosEmissoes", Column = "COF_EXIBIR_HOME_GRAFICOS_EMISSOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomeGraficosEmissoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomeServicosVeiculos", Column = "COF_EXIBIR_HOME_SERVICOS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomeServicosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomeParcelaDuplicatas", Column = "COF_EXIBIR_HOME_PARCELA_DUPLICATAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomeParcelaDuplicatas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomePagamentosMotoristas", Column = "COF_EXIBIR_HOME_PAGAMENTOS_MOTORISTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomePagamentosMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomeAcertoViagem", Column = "COF_EXIBIR_HOME_ACERTO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomeAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoCopiarSeguroCTeAnterior", Column = "COF_NAO_COPIAR_SEGURO_CTE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCopiarSeguroCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca", Column = "COF_BLOQUEAR_EMISSAO_MDFE_COM_MDFE_AUTORIZADO_PARA_MESMA_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoCTeParaCargaMunicipal", Column = "COF_BLOQUEAR_EMISSAO_CTE_CARGA_MUNICIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEmissaoCTeParaCargaMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirCobrancaCancelamento", Column = "COF_EXIBIR_COBRANCA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCobrancaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirHomeMDFesPendenteEncerramento", Column = "COF_EXIBIR_HOME_MDFES_PENDENTE_ENCERRAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirHomeMDFesPendenteEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeEDIFiscalMT", Column = "COF_EXIGE_EDI_FISCAL_MT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeEDIFiscalMT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbarNFSe", Column = "COF_AVERBAR_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoCarregarTomadorCTes", Column = "COF_NAO_CARREGAR_TOMADOR_CTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCarregarTomadorCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIR", Column = "COF_ALIQUOTA_IR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualBaseINSS", Column = "COF_PERCENTUAL_BC_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualBaseINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaINSS", Column = "COF_ALIQUOTA_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCSLL", Column = "COF_ALIQUOTA_CSLL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarINSSValorReceber", Column = "COF_DESCONTAR_INSS_VALOR_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarINSSValorReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CopiarObservacaoFiscoContribuinteCTeAnterior", Column = "COF_COPIAR_OBS_FISCO_CONTRIBUINTE_CTE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CopiarObservacaoFiscoContribuinteCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_COPIAR_VALORES_CTE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCopiarValoresCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_BLOQUEAR_CONSULTA_NFE_SEFAZ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearConsultaNFeSefaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_UTILIZA_RESUMO_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaResumoEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_UTILIZA_EMPRESA_VEICULO_COMO_PROPRIETARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaEmpresaVeiculoComoProprietario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_INCLUIR_ICMS_FRETE_DESPESA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirICMSFreteDespesaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ADICIONAR_RESPONSAVEL_SEGURO_OBS_CONTRIBUINTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarResponsavelSeguroObsContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "COF_EMPRESA_MATRIZ_CIOT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaMatrizCIOT { get; set; }

        /// <summary>
        /// Observacao padrão para CT-es de subcontratação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_OBSERVACAO_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string ObservacaoCTeSubcontratacao { get; set; }

        /// <summary>
        /// CST padrão para CT-es de subcontratação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_CST_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTCTeSubcontratacao { get; set; }

        /// <summary>
        /// CFOP padrão para CT-es de subcontratação
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO_SUBCONTRATACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOPCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ENCERRAR_MDFE_MESMA_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarMDFeComMesmaData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_UTILIZAR_DESTINO_PARA_CALCULO_DIFAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDestinoParaCalculoDifal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_CODIGO_FILIAL_REPOM", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CodigoFilialRepom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_IDENTIFICADOR_TRANSPORTADOR_ELECTROLUX", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IdentificadorTransportadorElectrolux { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_EXIBIR_EDI_CATERPILLAR_DUPLICATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirOpcaoEDICaterpillarDuplicata { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ARMAZENA_NOTAS_PARA_GERAR_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenaNotasParaGerarPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITE_IMPORTAR_XML_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteImportarXMLNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_DEFINIR_MULTIMODAL_OBSERVACAO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DefinirMultimodalObservacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_MERCADO_URL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string URLMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_MERCADO_LIVRE_ID_CLIENT", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IDMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_MERCADO_LIVRE_SECRET_KEY", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SecretKeyMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_TRAFEGUS_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TrafegusURL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_TRAFEGUS_USUARIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TrafegusUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_TRAFEGUS_SENHA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TrafegusSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_BUONNY_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string BuonnyURL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_BUONNY_TOKEN", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string BuonnyToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_BUONNY_CNPJ_GERENCIADORA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string BuonnyGerenciadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_CODIGO_TIPO_PRODUTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string BuonnyCodigoTipoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoraSM", Column = "COF_INTEGRADORA_SM", TypeType = typeof(Dominio.Enumeradores.IntegradoraSM), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraSM? IntegradoraSM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_IMPORTAR_VALORES_IMPORTACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImportarValoresImportacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WsIntegracaoEnvioCTeEmbarcadorTMS", Column = "COF_WS_INTEGRACAO_ENVIO_CTE_EMBARCADOR_TMS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string WsIntegracaoEnvioCTeEmbarcadorTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WsIntegracaoEnvioNFSeEmbarcadorTMS", Column = "COF_WS_INTEGRACAO_ENVIO_NFSE_EMBARCADOR_TMS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string WsIntegracaoEnvioNFSeEmbarcadorTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenIntegracaoEmbarcadorTMS", Column = "COF_TOKEN_INTEGRACAO_CTE_NFSE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenIntegracaoEmbarcadorTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "WsIntegracaoEnvioMDFeEmbarcadorTMS", Column = "COF_WS_INTEGRACAO_ENVIO_MDFE_EMBARCADOR_TMS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string WsIntegracaoEnvioMDFeEmbarcadorTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TruckPadURL", Column = "COF_TRUCKPAD_URL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TruckPadURL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TruckPadUser", Column = "COF_TRUCKPAD_USER", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TruckPadUser { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TruckPadPassword", Column = "COF_TRUCKPAD_PASSWORD", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TruckPadPassword { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_EXIGIR_OBS_CONT_VALOR_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirObservacaoContribuinteValorContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_USAR_REGRA_ICMS_PARA_CTE_DE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarRegraICMSParaCteDeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_IMPORTAR_NOTA_DUPLICADA_EDI_NOVA_IMPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImportarNotaDuplicadaEDINovaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_SOMAR_CREDITO_ICMS_NO_VALOR_DA_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSmarCreditoICMSNoValorDaPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_GERAR_CTE_COM_ICMS_ZERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCteComICMSZerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoComprarValePedagioCargaTransbordo", Column = "COF_NAO_COMPRAR_VALE_PEDAGIO_CARGA_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagioCargaTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoMigrate", Column = "COF_POSSUI_INTEGRACAO_MIGRATE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoMigrate { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenMigrate", Column = "COF_TOKEN_MIGRATE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenMigrate { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoMigrateRegimeTributario", Column = "MRT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario IntegracaoMigrateRegimeTributario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarObservacaoNaDiscriminacaoServicoMigrate", Column = "COF_ENVIAR_OBSERVACAO_NA_DISCRIMINACAO_SERVICO_MIGRATE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarObservacaoNaDiscriminacaoServicoMigrate { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoRest", Column = "EMP_URL_INTEGRACAO_REST", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLIntegracaoRest { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarNovoImposto", Column = "COF_ENVIAR_NOVO_IMPOSTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarNovoImposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReduzirPISCOFINSBaseCalculoIBSCBS", Column = "COF_REDUZIR_PISCOFINS_BASE_CALCULO_IBSCBS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ReduzirPISCOFINSBaseCalculoIBSCBS { get; set; } 

        #region Propriedades Virtuais
        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
        #endregion
    }
}
