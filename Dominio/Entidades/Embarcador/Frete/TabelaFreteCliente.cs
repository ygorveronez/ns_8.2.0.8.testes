using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE", EntityName = "TabelaFreteCliente", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente", NameType = typeof(TabelaFreteCliente))]
    public class TabelaFreteCliente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [Obsolete("Utilizar a lista de clientes de origem.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [Obsolete("Utilizar a lista de clientes de destino.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VigenciaTabelaFrete", Column = "TFV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual VigenciaTabelaFrete Vigencia { get; set; }

        [Obsolete("Utilizar a lista de regiões de destino.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidades.Regiao RegiaoDestino { get; set; }

        [Obsolete("Utilizar a lista de estados de destino.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_DATA_PROCESSAMENTO_VALORES", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamentoValores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_DATA_HISTORICO_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHistoricoAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_HISTORICO_ALTERACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioHistoricoAlteracao { get; set; }

        [Obsolete("Utilizar a lista de tipos de operação.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Rateio.RateioFormula FormulaRateio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AjusteTabelaFrete", Column = "TFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AjusteTabelaFrete AjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_TABELA_ORIGINARIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaOriginaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LicitacaoParticipacao", Column = "LIP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LicitacaoParticipacao LicitacaoParticipacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TFC_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "TFC_TIPO_PAGAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAlteracao", Column = "TFC_SITUACAO_ALTERACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete SituacaoAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSValorFrete", Column = "TFC_INCLUIR_ICMS_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HerdarInclusaoICMSTabelaFrete", Column = "TFC_HERDAR_INCLUSAO_ICMS_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HerdarInclusaoICMSTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VinculadoSemParar", Column = "TFC_VINCULADO_SEM_PARAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? VinculadoSemParar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGrupoCarga", Column = "TFC_TIPO_GRUPO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoCarga TipoGrupoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerenciarCapacidade", Column = "TFC_GERENCIAR_CAPACIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerenciarCapacidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstruturaTabela", Column = "TFC_ESTRUTURA_TABELA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela? EstruturaTabela { get; set; }

        /// <summary>
        /// true: vai ser duplicada a tabela e também sera aplicada as alterações
        /// false: o ajuste é duplicado apenas para replicar a vigência
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_APLICAR_ALTERACOES_DO_AJUSTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AplicarAlteracoesDoAjuste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualICMSIncluir", Column = "TFC_PERCENTUAL_ICMS_INCLUIR", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualICMSIncluir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TFC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TFC_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_OBSERVACAO_TERCEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoTerceiro { get; set; }

        /// <summary>
        /// Utilizado para o ajuste da tabela de frete (não é utilizado para cálculo).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoGarantidoOriginal", Column = "TFC_VALOR_MINIMO_GARANTIDO_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoGarantidoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoGarantido", Column = "TFC_VALOR_MINIMO_GARANTIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoGarantido { get; set; }

        /// <summary>
        /// Utilizado para o ajuste da tabela de frete (não é utilizado para cálculo).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMaximoOriginal", Column = "TFC_VALOR_MAXIMO_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMaximo", Column = "TFC_VALOR_MAXIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_BASE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_BASE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirObservacaoCTe", Column = "TFC_IMPRIMIR_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_FRETE_VALIDO_PARA_QUALQUER_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FreteValidoParaQualquerOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_FRETE_VALIDO_PARA_QUALQUER_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FreteValidoParaQualquerDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobrancaPadraoTerceiros", Column = "TFC_PERCENTUAL_COBRANCA_PADRAO_TERCEIROS", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualCobrancaPadraoTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobrancaVeiculoFrota", Column = "TFC_PERCENTUAL_COBRANCA_VEICULO_FROTA", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualCobrancaVeiculoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRateioDocumentos", Column = "TFC_TIPO_RATEIO_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual TipoEmissaoCTeDocumentos TipoRateioDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TFC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_ENTREGA_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorEntregaExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_ENTREGA_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorEntregaExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_PACOTE_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPacoteExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_PACOTE_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPacoteExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_PALLET_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPalletExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_PALLET_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPalletExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_QUILOMETRAGEM_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuilometragemExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_QUILOMETRAGEM_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuilometragemExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_PESO_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPesoExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_PESO_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPesoExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_AJUDANTE_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAjudanteExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_HORA_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorHoraExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_VALOR_AJUDANTE_EXCEDENTE_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAjudanteExcedenteOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_DESCRICAO_ORIGEM", Type = "StringClob", NotNull = false)]
        public virtual string DescricaoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_DESCRICAO_DESTINO", Type = "StringClob", NotNull = false)]
        public virtual string DescricaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PERCENTUAL_PAGAMENTO_AGREGADO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualPagamentoAgregado { get; set; }

        //[Obsolete("Esta propriedade será foi substituída pela Fronteiras.")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fronteira", Column = "FRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Embarcador.Logistica.Fronteira Fronteira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FRONTEIRA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fronteira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_QUILOMETRAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Quilometragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoInterna", Column = "TFC_OBSERVACAO_INTERNA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PRIORIDADE_USO", TypeType = typeof(int), NotNull = false)]
        public virtual int? PrioridadeUso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_LEAD_TIME", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_LEAD_TIME_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTimeMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_LEAD_TIME_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTimeTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLeadTime", Column = "TFC_TIPO_LEAD_TIME", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos TipoLeadTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRota", Column = "TFC_PERCENTUAL_ROTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? PercentualRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEntregas", Column = "TFC_QUANTIDADE_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeOTM", Column = "TFC_CAPACIDADE_OTM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CapacidadeOTM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DominioOTM", Column = "TFC_DOMINIO_OTM", TypeType = typeof(DominioOTM), NotNull = false)]
        public virtual DominioOTM? DominioOTM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontoPlanejamentoTransporte", Column = "TFC_PONTO_PLANEJAMENTO_TRANSPORTE", TypeType = typeof(PontoPlanejamentoTransporte), NotNull = false)]
        public virtual PontoPlanejamentoTransporte? PontoPlanejamentoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracao", Column = "TFC_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracaoUnilever), NotNull = false)]
        public virtual TipoIntegracaoUnilever? TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDExterno", Column = "TFC_ID_EXTERNO", TypeType = typeof(string), NotNull = false)]
        public virtual string IDExterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusAceiteTabela", Column = "TFC_STATUS_ACEITE_TABELA", TypeType = typeof(string), NotNull = false)]
        public virtual string StatusAceiteTabela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoTransporteFrete", Column = "CTF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete ContratoTransporteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_SITUACAO_INTEGRACAO_TABELA_FRETE_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoTabelaFreteCliente), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoTabelaFreteCliente SituacaoIntegracaoTabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PENDENTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_PERMITIR_CALCULAR_FRETE_EM_ALTERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCalcularFreteEmAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarValePedagioCarga", Column = "TFC_OBRIGATORIO_INFORMAR_VALE_PEDAGIO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarValePedagioCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ParametrosBaseCalculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Inverse = true, Table = "T_TABELA_FRETE_PARAMETRO_BASE_CALCULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ParametroBaseCalculoTabelaFrete", Column = "TBC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> ParametrosBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ItensBaseCalculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemParametroBaseCalculoTabelaFrete", Column = "TPI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> ItensBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Origens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Origens { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_ESTADO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PaisesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_PAIS_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pais", Column = "PAI_CODIGO")]
        public virtual ICollection<Pais> PaisesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegioesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_REGIAO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Regiao", Column = "REG_CODIGO")]
        public virtual ICollection<Localidades.Regiao> RegioesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_ROTA_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CEPsOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_CEP_ORIGEM", Inverse = true)]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "TabelaFreteClienteCEPOrigem")]
        public virtual ICollection<TabelaFreteClienteCEPOrigem> CEPsOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_ESTADO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PaisesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_PAIS_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pais", Column = "PAI_CODIGO")]
        public virtual ICollection<Pais> PaisesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegioesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_REGIAO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Regiao", Column = "REG_CODIGO")]
        public virtual ICollection<Localidades.Regiao> RegioesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_ROTA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CEPsDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_CEP_DESTINO", Inverse = true)]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "TabelaFreteClienteCEPDestino")]
        public virtual ICollection<TabelaFreteClienteCEPDestino> CEPsDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Subcontratacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_SUB_CONTRATACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteClienteSubContratacao", Column = "TCS_CODIGO")]
        public virtual IList<TabelaFreteClienteSubContratacao> Subcontratacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "SubcontratacoesGerais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_SUB_CONTRATACAO_ACRESCIMO_DESCONTO_GERAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral", Column = "SADG_CODIGO")]
        public virtual IList<TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral> SubcontratacoesGerais { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TransportadoresTerceiros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_TRANSPORTADOR_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> TransportadoresTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Fronteiras", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_FRONTEIRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Fronteiras { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> TiposCarga { get; set; }

        public virtual TabelaFreteCliente Clonar()
        {
            return (TabelaFreteCliente)this.MemberwiseClone();
        }

        public virtual bool Equals(TabelaFreteCliente other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string DescricaoVigencia
        {
            get { return (this.Vigencia == null) ? "" : Vigencia.Descricao; }
        }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual string Descricao
        {
            get { return TabelaFrete?.Descricao + " - " + Codigo.ToString(); }
        }
    }
}
