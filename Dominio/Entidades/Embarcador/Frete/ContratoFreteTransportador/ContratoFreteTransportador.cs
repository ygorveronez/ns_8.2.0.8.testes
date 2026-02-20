using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR", EntityName = "ContratoFreteTransportador", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador", NameType = typeof(ContratoFreteTransportador))]
    public class ContratoFreteTransportador : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CFT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_NUMERO", TypeType = typeof(int))]
        public virtual int NumeroSequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_NUMERO_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO_ORIGINARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoOriginario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoContratoFrete", Column = "TCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoContratoFrete TipoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "CFT_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "CFT_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_NUMERO_ADITIVO", TypeType = typeof(int))]
        public virtual int NumeroAditivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CFT_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeTabelaFreteComValor", Column = "CFT_EXIGE_TABELA_FRETE_COM_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeTabelaFreteComValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CFT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DESCONTAR_VALORES_OUTRAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValoresOutrasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DEDUZIR_VALOR_POR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeduzirValorPorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_UTILIZAR_VALOR_FIXO_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorFixoModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TAMBEM_UTILIZAR_CONTRATO_PARA_FILIAIS_DO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TambemUtilizarContratoParaFiliaisDoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_DIARIA_POR_VEICULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiariaPorVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_QUINZENA_POR_VEICULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuinzenaPorVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_DIARIA_POR_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiariaPorMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_QUINZENA_POR_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuinzenaPorMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_TOTAL_OCORRENCIA_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalOcorrenciaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_QUANTIDADE_MOTORISTAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_OUTROS_VALORES_VALOR_KM_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal OutrosValoresValorKmExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_FRANQUIA_TOTAL_POR_CAVALO", TypeType = typeof(int), NotNull = false)]
        public virtual int FranquiaTotalPorCavalo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_FRANQUIA_TOTAL_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int FranquiaTotalKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_FRANQUIA_CONTRATO_MENSAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal FranquiaContratoMensal { get; set; }

        /// <summary>
        /// Quando o contrato não possuir franquia de KM, utilizar o valor total mensal.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_TOTAL_MENSAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_POR_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_ESTIMATIVA_CARGAS_MES", TypeType = typeof(int))]
        public virtual int QuantidadeMensalCargas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFT_CODIGO_VALOR_CONTRATO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteValorContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_FRANQUIA_VALOR_KM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FranquiaValorKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_FRANQUIA_VALOR_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal FranquiaValorKmExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_VALOR_FRETE_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_NAO_EMITIR_COMPLEMENTO_FECHAMENTO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirComplementoFechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_PERIODO_ACORDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador PeriodoAcordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TIPO_FRANQUIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador TipoFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TIPO_EMISSAO_COMPLEMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoComplementoContratoFreteTransportador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoComplementoContratoFreteTransportador TipoEmissaoComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstruturaTabela", Column = "CFT_ESTRUTURA_TABELA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela? EstruturaTabela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [Obsolete("Campo migrado para Enum (TipoDisponibilidadeContratoFrete).")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_DISPONIBILIZAR_PARA_TODOS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarParaTodosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFT_TIPO_DISPONIBILIDADE_CONTRATO_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDisponibilidadeContratoFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDisponibilidadeContratoFrete TipoDisponibilidadeContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCusto", Column = "CFT_TIPO_CUSTO", TypeType = typeof(string), NotNull = false)]
        public virtual string TipoCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoAcessorio", Column = "CFT_TIPO_ACESSORIO", TypeType = typeof(string), NotNull = false)]
        public virtual string CustoAcessorio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoTransporteFrete", Column = "CTF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoTransporteFrete ContratoTransporteFrete { get; set; }

        [Obsolete("Campo migrado para uma lista nesta mesma entidade (CanaisEntrega).")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CanaisEntrega", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_CANAL_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CanalEntrega", Column = "CNE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> CanaisEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TabelasFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_TABELA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFrete", Column = "TBF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Frete.TabelaFrete> TabelasFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRota", Column = "CFT_PERCENTUAL_ROTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEntregas", Column = "CFT_QUANTIDADE_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeOTM", Column = "CFT_CAPACIDADE_OTM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CapacidadeOTM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DominioOTM", Column = "CFT_DOMINIO_OTM", TypeType = typeof(DominioOTM), NotNull = false)]
        public virtual DominioOTM? DominioOTM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontoPlanejamentoTransporte", Column = "CFT_PONTO_PLANEJAMENTO_TRANSPORTE", TypeType = typeof(PontoPlanejamentoTransporte), NotNull = false)]
        public virtual PontoPlanejamentoTransporte? PontoPlanejamentoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracao", Column = "CFT_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracaoUnilever), NotNull = false)]
        public virtual TipoIntegracaoUnilever? TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDExterno", Column = "CFT_ID_EXTERNO", TypeType = typeof(string), NotNull = false)]
        public virtual string IDExterno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "StatusAssinaturaContrato", Column = "STC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual StatusAssinaturaContrato StatusAceiteContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoCarga", Column = "CFT_GRUPO_CARGA", TypeType = typeof(TipoGrupoCarga), NotNull = false)]
        public virtual TipoGrupoCarga? GrupoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerenciarCapacidade", Column = "CFT_GERENCIAR_CAPACIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerenciarCapacidade { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ContratoAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_CONTRATO_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaContratoFreteTransportador", Column = "AAC_CODIGO")]
        public virtual ICollection<AprovacaoAlcadaContratoFreteTransportador> ContratoAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_TIPO_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO")]
        public virtual ICollection<TipoDeOcorrenciaDeCTe> TiposOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorCliente", Column = "CFL_CODIGO")]
        public virtual IList<ContratoFreteTransportadorCliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ModelosVeiculares", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorModeloVeicular", Column = "CFM_CODIGO")]
        public virtual IList<ContratoFreteTransportadorModeloVeicular> ModelosVeiculares { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorVeiculo", Column = "CFV_CODIGO")]
        public virtual IList<ContratoFreteTransportadorVeiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorFilial", Column = "CFF_CODIGO")]
        public virtual IList<ContratoFreteTransportadorFilial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Acordos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorAcordo", Column = "CFA_CODIGO")]
        public virtual IList<ContratoFreteTransportadorAcordo> Acordos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ValoresOutrosRecursos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALORES_OUTROS_RECURSOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorValoresOutrosRecursos", Column = "CFR_CODIGO")]
        public virtual IList<ContratoFreteTransportadorValoresOutrosRecursos> ValoresOutrosRecursos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoOperacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorTipoOperacao", Column = "CFO_CODIGO")]
        public virtual IList<ContratoFreteTransportadorTipoOperacao> TipoOperacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TipoCargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorTipoCarga", Column = "CFC_CODIGO")]
        public virtual IList<ContratoFreteTransportadorTipoCarga> TipoCargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FaixasKmFranquia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_FAIXA_KM_FRANQUIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFreteTransportadorFaixaKmFranquia", Column = "CFK_CODIGO")]
        public virtual ICollection<ContratoFreteTransportadorFaixaKmFranquia> FaixasKmFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoSituacaoIntegracao", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN I.INT_SITUACAO_INTEGRACAO = 0 THEN 'Aguardando Integração'
                                                                                                                WHEN I.INT_SITUACAO_INTEGRACAO = 1 THEN 'Integrado'
                                                                                                                WHEN I.INT_SITUACAO_INTEGRACAO = 2 THEN 'Problema na Integração'
                                                                                                                WHEN I.INT_SITUACAO_INTEGRACAO = 3 THEN 'Aguardando Retorno'
                                                                                                                ELSE ''
                                                                                                                END
                                                                                                                FROM T_CONTRATO_FRETE_TRANSPORTADOR_INTEGRACAO I
                                                                                                                WHERE I.CFT_CODIGO_TRANSPORTADOR = CFT_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string DescricaoSituacaoIntegracao { get; set; }


        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual string Numero
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.NumeroEmbarcador) ? this.NumeroSequencial.ToString() : this.NumeroEmbarcador;
            }
        }

        public virtual decimal FranquiaContratoMensalPeriodo
        {
            get
            {
                if (this.PeriodoAcordo == PeriodoAcordoContratoFreteTransportador.Semanal)
                    return Math.Round(this.FranquiaContratoMensal / 4, 2);

                if (this.PeriodoAcordo == PeriodoAcordoContratoFreteTransportador.Decendial)
                    return Math.Round(this.FranquiaContratoMensal / 3, 2);

                if (this.PeriodoAcordo == PeriodoAcordoContratoFreteTransportador.Quinzenal)
                    return Math.Round(this.FranquiaContratoMensal / 2, 2);

                return this.FranquiaContratoMensal;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return this.Situacao.ObterDescricao();
            }
        }

        public virtual string DescricaoPeriodoAcordo
        {
            get
            {
                return this.PeriodoAcordo.ObterDescricao();
            }
        }

        public virtual string DescricaoTipoFranquia
        {
            get
            {
                return this.TipoFranquia.ObterDescricao();
            }
        }

        #endregion Propriedades com Regras
    }
}
