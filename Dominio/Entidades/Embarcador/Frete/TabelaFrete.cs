using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE", EntityName = "TabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFrete", NameType = typeof(TabelaFrete))]
    public class TabelaFrete : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TBF_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TBF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTabelaFrete", Column = "TBF_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete TipoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_TIPO_CALCULO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete TipoCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_APLICACAO_TABELA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoTabela AplicacaoTabela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroBase", Column = "TBF_PARAMETRO_BASE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete? ParametroBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAlteracao", Column = "TBF_SITUACAO_ALTERACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete SituacaoAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFreteTabelaFrete", Column = "TBF_TIPO_FRETE_TABELA_FRETE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFreteTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFreteTabelaFrete TipoFreteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TBF_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Filiais.Filial Filial { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frete.ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteCliente", Column = "CFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente ContratoFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        [Obsolete("Utilizar a lista de tipos de operações. Será removido.")]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TBF_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_OBSERVACAO_TERCEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirObservacaoCTe", Column = "TBF_IMPRIMIR_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoAutomaticaCTe", Column = "TBF_EMISSAO_AUTOMATICA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoAutomaticaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAlterarValor", Column = "TBF_PERMITE_ALTERAR_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSValorFrete", Column = "TBF_INCLUIR_ICMS_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_FRETE_DESTINO_PRIORITARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFreteDestinoPrioritario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_VALOR_PARAMETRO_BASE_OBRIGATORIO_PARA_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorParametroBaseObrigatorioParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAlteracaoValor", Column = "TBF_TIPO_ALTERACAO_VALOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlteracaoValorTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlteracaoValorTabelaFrete TipoAlteracaoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualICMSIncluir", Column = "TBF_PERCENTUAL_ICMS_INCLUIR", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualICMSIncluir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobrancaPadraoTerceiros", Column = "TBF_PERCENTUAL_COBRANCA_PADRAO_TERCEIROS", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualCobrancaPadraoTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Padrao", Column = "TBF_PADRAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Padrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabelaCalculoCliente", Column = "TBF_TABELA_CALCULO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaCalculoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_VALOR_ADICIONAL_ENTREGA_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteValorAdicionalPorEntregaExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_VALOR_ADICIONAL_PACOTE_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteValorAdicionalPorPacoteExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_VALOR_ADICIONAL_PALLET_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteValorAdicionalPorPalletExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_TABELA_EXCLUSIVA_PARA_PALLETS_CONTINDOS_NAS_FAIXAS_CADASTRADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaUsoExclusivaParaPalletsContidosNasFaixasCadastradas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_VALOR_ADICIONAL_PESO_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteValorAdicionalPorPesoExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PESO_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoExcecente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_VALOR_ADICIONAL_QUILOMETRAGEM_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteValorAdicionalPorQuilometragemExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_QUILOMETRAGEM_EXCEDENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuilometragemExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_POSSUI_HORAS_MINIMAS_COBRANCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiHorasMinimasCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_POSSUI_MINIMO_GARANTIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiMinimoGarantido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_POSSUI_VALOR_MAXIMO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiValorMaximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_POSSUI_VALOR_BASE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiValorBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_DIFERENCA_VALOR_BASE_APENAS_FRETE_PAGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDiferencaDoValorBaseApenasFretePagos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoDiferencaFreteNegativo", Column = "TBF_VALOR_MINIMO_DIFERENCA_FRETE_NEGATIVO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoDiferencaFreteNegativo { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "TBF_HORAS_MINIMAS_COBRANCA", TypeType = typeof(int), NotNull = false)]
        //public virtual int? HorasMinimasCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_HORAS_MINIMAS_COBRANCA_TEMPO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HorasMinimasCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_ARREDONDAMENTO_HORAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarArredondamentoHoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_MINUTOS_INFORMADOS_PARA_ARREDONDAMENTO_CORTE_HORA_EXATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMinutosInformadosComoCorteArredondamentoHoraExata { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_MINUTOS_ARREDONDAMENTO_HORAS", TypeType = typeof(int), NotNull = false)]
        public virtual int? MinutosArredondamentoHoras { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_QUILOMETRAGEM_EXCEDENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteQuilometragemExcedente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_QUILOMETRAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteQuilometragem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_PESO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFretePeso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_AJUDANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteAjudante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_HORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteHora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_TEMPO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteTempo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_PALLET", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFretePallet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_NUMEROENTREGAS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteNumeroEntregas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_PACOTES", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFretePacotes { get; set; }


        [Obsolete("Utilizar a propriedade TipoCalculo. Será removido.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "CalcularFretePorPedido", Column = "TBF_CALCULAR_FRETE_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFretePorPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_CONTRATANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaContratante { get; set; }

        /// <summary>
        /// esse campo foi feito para multiplicar o valor informado na faixa pela quantidade (ver se ficará assim, por enquanto está somente no banco de dados).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_MULTIPLICAR_VALOR_FAIXA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarValorDaFaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_MULTIPLICAR_VALOR_FAIXA_HORA_PELA_HORA_CORRIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarValorFaixaHoraPelaHoraCorrida { get; set; }

        [Obsolete("Utilizar a propriedade TipoArredondamentoHoras.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_ARREDONDAR_HORAS_PARA_CIMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArredondarHorasParaCima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_TIPO_ARREDONDAMENTO_HORAS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoTabelaFrete TipoArredondamentoHoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_COM_TODAS_FAIXAS_HORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularComTodasFaixasHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_MULTIPLICAR_VALOR_POR_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarValorPorPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_USA_CUBAGEM_COMO_PARAMETRO_DE_DISTANCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarCubagemComoParametroDeDistancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_VALOR_MINIMO_PARA_RATEIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorMinimoParaRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_AGRUPAR_POR_RECEBEDOR_AO_CALCULAR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgrupaPorRecebedorAoCalcularPorPedidoAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_VALOR_MINIMO_PARA_RATEIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoParaRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_MULTIPLICAR_VALOR_TEMPO_HORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarValorTempoPorHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_NAO_PERMITIR_LANCAR_VALOR_POR_TIPO_DE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirLancarValorPorTipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_VALOR_ADICIONAL_AJUDANTE_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteValorAdicionalPorAjudanteExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_VALOR_ADICIONAL_HORA_EXCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteValorAdicionalPorHoraExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PAGAMENTO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagamentoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_OBRIGATORIO_INFORMAR_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_PARTICIPANTE_PEDIDO_PARA_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarParticipantePedidoParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_MODELO_VEICULAR_DA_CARGA_PARA_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarModeloVeicularDaCargaParaCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_USAR_APENAS_QUANDO_DISTANCIA_INFORMADA_NA_INTEGRACAO_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarTabelaApenasQuandoDistanciaInformadaNaIntegracaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_OBRIGATORIO_VALOR_FRETE_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioValorFretePeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_PESO_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_OBSERVACAO_CONTRATO_FRETE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_TEXTO_ADICIONAL_CONTRATO_FRETE", TypeType = typeof(string), Length = 100000, NotNull = false)]
        public virtual string TextoAdicionalContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_RETER_IMPOSTOS_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterImpostosContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_DIAS_VENCIMENTO_ADIANTAMENTO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVencimentoAdiantamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_DIAS_VENCIMENTO_SALDO_CONTRATO_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVencimentoSaldoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_TABELA_MINIMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaFreteMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZA_TABELA_MINIMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaTabelaFreteMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZA_MODELO_VEICULAR_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaModeloVeicularVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_FATOR_PESO_PELA_KM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFatorPesoPelaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PESO_PARAMETRO_BASE_CALCULO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesoParametroCalculoFrete PesoParametroCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_VALOR_ENTREGA_POR_PERCENTUAL_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularValorEntregaPorPercentualFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_VALOR_ENTREGA_POR_PERCENTUAL_FRETE_COM_COMPONENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularValorEntregaPorPercentualFreteComComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirOcorrenciaAutomatica", Column = "TBF_OCORRENCIA_AUTOMATICA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmitirOcorrenciaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarPorDataCarregamento", Column = "TBF_VALIDAR_POR_DATA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarPorDataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITIR_VIGENCIAS_SOBREPOSTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVigenciasSobrepostas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarComoDataBaseVigenciaDataAtual", Column = "TBF_USAR_DATA_VIGENCIA_DATA_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoDataBaseVigenciaDataAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "TBF_TIPO_OCORRENCIA_AUTOMATICA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaTabelaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_QUANTIDADE_ENTREGA_POR_PARTICIPANTES_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularQuantidadeEntregaPorParticipantesPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_QUANTIDADE_ENTREGA_POR_NUMERO_DE_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularQuantidadeEntregaPorNumeroDePedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_INFORMAR_DIAS_UTEIS_POR_FAIXA_CEP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarDiasUteisPorFaixaCEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_CALCULAR_FRETE_POR_PESO_CUBADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularFretePorPesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_APLICAR_MAIOR_VALOR_ENTRE_PESO_E_PESO_CUBADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AplicarMaiorValorEntrePesoEPesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_FATOR_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FatorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_ISENCAO_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal IsencaoCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_UTILIZAR_TIPO_OPERACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarTipoOperacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_POSSUI_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_NAO_CALCULA_SEM_FRETE_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCalculaSemFreteValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_INCLUIR_ICM_VALOR_FRETE_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSValorFreteNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_NAO_DESTACAR_RESULTADO_CONSULTA_PEDAGIO_COMO_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoDestacarResultadoConsultaPedagioComoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_PERMITE_ALTERAR_VALOR_FRETE_PEDIDO_POS_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarValorFretePedidoPosCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_NAO_USAR_CANAL_VENDA_COMO_FILTRO_PARA_COTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUsarCanalEntregaComoFiltroParaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoCubagemCalculoFrete", Column = "TBF_DESCONTO_CUBAGEM_CALCULO_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal DescontoCubagemCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobrancaVeiculoFrotaTerceiros", Column = "TBF_PERCENTUAL_COBRANCA_VEICULO_FROTA_TERCEIROS", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualCobrancaVeiculoFrotaTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TransportadoresTerceiros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TRANSPORTADOR_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> TransportadoresTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposTerceiros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TIPO_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerceiro", Column = "TPT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> TiposTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TIPOS_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposDeOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TIPOS_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.TipoDeOcorrenciaDeCTe> TiposDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoEmbalagens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TIPO_EMBALAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoEmbalagem", Column = "MRC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> TipoEmbalagens { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filiais.Filial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosTracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_MODELO_TRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosTracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosReboque", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_MODELO_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosReboque { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Vigencias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_VIGENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VigenciaTabelaFrete", Column = "TFV_CODIGO")]
        public virtual IList<VigenciaTabelaFrete> Vigencias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Componentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ComponenteFreteTabelaFrete", Column = "TFC_CODIGO")]
        public virtual IList<ComponenteFreteTabelaFrete> Componentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RotasFreteEmbarcador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_ROTA_EMBARCADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaEmbarcadorTabelaFrete", Column = "TFR_CODIGO")]
        public virtual IList<RotaEmbarcadorTabelaFrete> RotasFreteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NumeroEntregas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_NUMERO_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NumeroEntregaTabelaFrete", Column = "TFN_CODIGO")]
        public virtual IList<NumeroEntregaTabelaFrete> NumeroEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pacotes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_PACOTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PacoteTabelaFrete", Column = "TFP_CODIGO")]
        public virtual IList<PacoteTabelaFrete> Pacotes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pallets", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_PALLETS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PalletTabelaFrete", Column = "TFP_CODIGO")]
        public virtual IList<PalletTabelaFrete> Pallets { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PesosTransportados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_PESO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PesoTabelaFrete", Column = "TFP_CODIGO")]
        public virtual IList<PesoTabelaFrete> PesosTransportados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Distancias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_DISTANCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DistanciaTabelaFrete", Column = "TFD_CODIGO")]
        public virtual IList<DistanciaTabelaFrete> Distancias { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Subcontratacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_SUBCONTRATACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SubcontratacaoTabelaFrete", Column = "TFS_CODIGO")]
        public virtual IList<SubcontratacaoTabelaFrete> Subcontratacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Relatorios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_RELATORIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Relatorio", Column = "REL_CODIGO")]
        public virtual ICollection<Relatorios.Relatorio> Relatorios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Tempos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_TEMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteTempo", Column = "TFT_CODIGO")]
        public virtual IList<TabelaFreteTempo> Tempos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ajudantes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_AJUDANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteAjudante", Column = "TFA_CODIGO")]
        public virtual IList<TabelaFreteAjudante> Ajudantes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Horas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_HORA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteHora", Column = "TFH_CODIGO")]
        public virtual IList<TabelaFreteHora> Horas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteAnexo", Column = "ANX_CODIGO")]
        public virtual IList<TabelaFreteAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Fronteiras", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_FRONTEIRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Fronteiras { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ContratosTransporteFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CONTRATO_TRANSPORTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoTransporteFrete", Column = "CTF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> ContratosTransporteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Layouts", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_LAYOUT")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ImportacaoTabelaFreteLayout", Column = "ITF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLayout> Layouts { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBF_LOCAL_FREE_TIME", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime LocalFreeTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestacarComponenteFrete", Column = "TBF_DESTACAR_COMPONENTE_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DestacarComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_DESTACAR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteDestacar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSComponenteDestacado", Column = "TBF_VALOR_ICMS_COMPONENTE_DESTACADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSComponenteDestacado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalAReceber", Column = "TBF_NAO_SOMAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalPrestacao", Column = "TBF_NAO_SOMAR_VALOR_TOTAL_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteFreteLiquido", Column = "TBF_DESCONTAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoConsiderarExpedidorERecebedor", Column = "TBF_NAO_CONSIDERAR_EXPEDIDOR_E_RECEBEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoConsiderarExpedidorERecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarValorDaTabelaMesmoInformandoUmValorDeFreteOperador", Column = "TBF_UTILIZAR_VALOR_DA_TABELA_MESMO_INFORMANDO_UM_VALOR_DE_FRETE_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorDaTabelaMesmoInformandoUmValorDeFreteOperador { get; set; }

        [Obsolete("O campo não será mais utilizado", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorTotalAReceber", Column = "TBF_DESCONTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorTotalAReceber { get; set; }

        [Obsolete("O campo não será mais utilizado", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescentaValorTotalAReceber", Column = "TBF_ACRESENTA_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        [Obsolete("O campo não será mais utilizado", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "SomarComponenteFreteLiquido", Column = "TBF_SOMAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomarComponenteFreteLiquido { get; set; }

        [Obsolete("O campo não será mais utilizado", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteNotaFiscalServico", Column = "TBF_DESCONTAR_COMPONENTE_NOTA_FISCAL_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteNotaFiscalServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberOICMSDoComponente", Column = "TBF_DESCONTAR_DO_VALOR_A_RECEBER_ICMS_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberOICMSDoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarDoValorAReceberValorComponente", Column = "TBF_DESCONTAR_DO_VALOR_A_RECEBER_VALOR_COMPONENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarDoValorAReceberValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS", Column = "TBF_NAO_ADICIONAR_COMPONENTE_BASE_CALCULO_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAdicionarOValorDoComponenteABaseDeCalculoDoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarCalculoFretePorPedido", Column = "TBF_USAR_CALCULO_FRETE_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarCalculoFretePorPedido { get; set; }

        public virtual string DescricaoTipoTabelaFrete
        {
            get
            {
                switch (this.TipoTabelaFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaRota:
                        return "Tabela por Rotas";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaComissaoProduto:
                        return "Tabela de Comissão de Produto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente:
                        return "Tabela por Clientes";
                    default:
                        return "";
                }
            }
        }

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
    }
}