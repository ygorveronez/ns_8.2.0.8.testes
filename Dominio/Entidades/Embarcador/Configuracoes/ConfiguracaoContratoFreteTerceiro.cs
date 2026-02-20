namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CONTRATO_FRETE_TERCEIRO", EntityName = "ConfiguracaoContratoFreteTerceiro", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro", NameType = typeof(ConfiguracaoContratoFreteTerceiro))]
    public class ConfiguracaoContratoFreteTerceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_TEXTO_ADICIONAL", TypeType = typeof(string), Length = 100000, NotNull = false)]
        public virtual string TextoAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_RETER_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterImpostos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_RETENCAO_POR_RAIZ_CNPJ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetencaoPorRaizCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_DIAS_VENCIMENTO_ADIANTAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVencimentoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_DIAS_VENCIMENTO_SALDO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVencimentoSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_CALCULAR_ADIANTAMENTO_COM_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularAdiantamentoComPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_UTILIZAR_TAXA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarTaxaPagamentoContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_UTILIZAR_NOVO_LAYOUT_PAGAMENTO_AGREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNovoLayoutPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_PERCENTUAL_ADIANTAMENTO_FRETE_TERCEIROS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualAdiantamentoFreteTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_EXIBIR_PEDIDOS_IMPRESSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPedidosImpressaoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_NAO_CONSIDERAR_ACRESCIMO_DESCONTO_CALCULO_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoConsiderarDescontoCalculoImpostos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_PERMITIR_AUTORIZAR_PAGAMENTO_CIOT_COM_CANHOTOS_RECEBIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAutorizarPagamentoCIOTComCanhotosRecebidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_HABILITAR_LAYOUT_FATURA_PAGAMENTO_AGREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarLayoutFaturaPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_GERAR_CARGA_TERCEIRO_APENAS_PROVEDOR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaTerceiroApenasProvedorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_OBRIGAR_ANEXOS_CONTRATO_TRANSPORTADOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarAnexosContratoTransportadorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_GERAR_NUMERO_CONTRATO_TRANSPORTADOR_FRETE_SEQUENCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNumeroContratoTransportadorFreteSequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_PERMITE_ALTERAR_DADOS_CONTRATO_INDEPENDENTE_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarDadosContratoIndependenteSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSubtrairValePedagioDoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_UTILIZAR_FECHAMENTO_DE_AGREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarFechamentoDeAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_PERMITIR_INFORMAR_PERCENTUAL_100_ADIANTAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarPercentual100AdiantamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_EM_ACRESCIMO_DESCONTO_CIOT_AJUSTA_VALOR_TOTAL_E_NAO_ALTERA_IMPOSTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmAcrescimoDescontoCiotNaoAlteraImpostos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_GERAR_CIOT_MARCADO_AO_CADASTRAR_TRANSPORTADOR_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOTMarcadoAoCadastrarTransportadorTerceiro { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para contratos de fretes"; }
        }
    }
}
