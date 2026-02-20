using Dominio.Entidades.Embarcador.Cargas.ControleEntrega;
using Dominio.Entidades.Embarcador.WMS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PEDIDO", EntityName = "Pedido", Name = "Dominio.Entidades.Embarcador.Pedidos.Pedido", NameType = typeof(Pedido))]
    public class Pedido : EntidadeBase, IEquatable<Pedido>, Interfaces.Embarcador.Entidade.IEntidade
    {
        public Pedido()
        {
            this.DataCriacao = DateTime.Now;
            this.PedidoLiberadoMontagemCarga = true;
        }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoEmbarcador", Column = "PED_NUMERO_PEDIDO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoEmbarcadorSemRegra", Column = "PED_NUMERO_PEDIDO_EMBARCADOR_SEM_REGRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoEmbarcadorSemRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraMontarNumeroPedidoEmbarcadorWebService", Column = "PED_REGRA_MONTAR_NUMERO_PEDIDO_EMBARCADOR_WEB_SERVICE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string RegraMontarNumeroPedidoEmbarcadorWebService { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_PRE_CARGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido PedidoPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequenciaPedido", Column = "PED_NUMERO_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequenciaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO_VENDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial FilialVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoDePreCarga", Column = "PED_PEDIDO_DE_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoDePreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.EmpresaSerie EmpresaSerie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoEntrega", Column = "PED_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoEntregaAtual", Column = "PED_PREVISAO_ENTREGA_ATUAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoEntregaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoEntregaTransportador", Column = "PED_PREVISAO_ENTREGA_TRANSPORTADOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoEntregaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "PED_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoCarga", Column = "PED_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoCarga { get; set; }

        [Obsolete("Não utilizar. Campo para definir a data de descarregamento da carga por filial na montagem de carga. Migrado para a entidade CarregamentoFilial")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoTerminoCarga", Column = "PED_DATA_TERMINO_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoTerminoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidade", Column = "PED_DATA_VALIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioJanelaDescarga", Column = "PED_DATA_INICIO_JANELA_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioJanelaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.PlanoConta ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPedido", Column = "PED_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido SituacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAgendamentoEntregaPedido", Column = "PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoEntregaPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoEntregaPedido? SituacaoAgendamentoEntregaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAtualPedidoRetirada", Column = "PED_SITUACAO_ATUAL_PEDIDO_RETIRADA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada SituacaoAtualPedidoRetirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPlanejamentoPedido", Column = "PED_SITUACAO_PLANEJAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedido SituacaoPlanejamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPlanejamentoPedidoTMS", Column = "PED_SITUACAO_PLANEJAMENTO_TMS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedidoTMS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedidoTMS SituacaoPlanejamentoPedidoTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoRoteirizadorIntegracao", Column = "PED_SITUACAO_ROTEIRIZADOR_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizadorIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizadorIntegracao SituacaoRoteirizadorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCargaEmbarcador", Column = "PED_CODIGO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoPedido", Column = "CAR_DATA_CARREGAMENTO_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Localidades.Regiao RegiaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_PONTO_PARTIDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente PontoPartida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoEndereco", Cascade = "all", Column = "PEN_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco EnderecoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarOutroEnderecoOrigem", Column = "PED_USAR_OUTRO_ENDERECO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarOutroEnderecoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoEndereco", Cascade = "all", Column = "PEN_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco EnderecoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarOutroEnderecoDestino", Column = "PED_USAR_OUTRO_ENDERECO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarOutroEnderecoDestino { get; set; }

        [Obsolete("Migrado para entidade PedidoFronteira")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FRONTEIRA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fronteira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONADA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionadaManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DESTINATARIO_NAO_INFORMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DestinatarioNaoInformado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidosRecolhimentoTroca", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_RECOLHIMENTO_TROCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoRecolhimentoTroca", Column = "PRT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca> PedidosRecolhimentoTroca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCarregamentoSituacao", Column = "STP_PEDIDO_CARREGAMENTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoCarregamentoSituacao PedidoCarregamentoSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RestricoesDescarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_RESTRICAO_DESCARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RestricaoEntrega", Column = "REE_CODIGO")]
        public virtual ICollection<Pessoas.RestricaoEntrega> RestricoesDescarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente RecebedorColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_LOCAL_EXPEDICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente LocalExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarTipoTomadorPedido", Column = "PED_USAR_TIPO_TOMADOR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarTipoTomadorPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "PED_TIPO_TOMADOR", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_SUB_CONTRATANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente SubContratante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_CLIENTE_DESLOCAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteDeslocamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_CLIENTE_ADICIONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteAdicional { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_CLIENTE_CONTAINER", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteDonoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPaletesFracionado", Column = "PED_NUMERO_PALETES_FRACIONADO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal NumeroPaletesFracionado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPaletes", Column = "PED_NUMERO_PALETES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_PALETES_PAGOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal NumeroPaletesPagos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_SEMI_PALETES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal NumeroSemiPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_SEMI_PALETES_PAGOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal NumeroSemiPaletesPagos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_COMBIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal NumeroCombis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_COMBIS_PAGAS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal NumeroCombisPagas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteCliente", Column = "CFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente ContratoFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalPaletes", Column = "PED_PESO_TOTAL_PALETES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalPaletes", Column = "PED_VALOR_TOTAL_PALETES", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalPaletes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimaAtualizacao", Column = "PED_ULTIMA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? UltimaAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperacaoEmissao", Column = "PED_TIPO_OPERACAO_EMISSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao TipoOperacaoEmissao { get; set; } //todo:remover isso depois que prosyst sair do sistema.

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoRefaturamento", Column = "PED_PEDIDO_REFATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoRefaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoRefaturado", Column = "PED_PEDIDO_REFATURADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoRefaturado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PED_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoColeta", Column = "TPC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoColeta TipoColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCarga", Column = "ATC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PED_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTe", Column = "PED_OBSERVACAO_CTE", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_OBSERVACAO_CTE_TERCEIRO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ObservacaoCTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Requisitante", Column = "PED_REQUISITANTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta Requisitante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "PED_TIPO_PAGAMENTO", TypeType = typeof(Enumeradores.TipoPagamento), NotNull = false)]
        public virtual Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO", TypeType = typeof(string), NotNull = false, Length = 300)]
        public virtual string ObservacaoSolicitacaoReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarTipoPagamentoNF", Column = "PED_USAR_TIPO_PAGAMENTO_NF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarTipoPagamentoNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarPedidoParaColeta", Column = "PED_DISPONIVEL_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarPedidoParaColeta { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAcompanhamentoPedido", Column = "PED_ETAPA_ACOMPANHAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaAcompanhamentoPedido), NotNull = false)]
        //public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaAcompanhamentoPedido EtapaAcompanhamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAcompanhamentoPedido", Column = "PED_SITUACAO_ACOMPANHAMENTO_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido SituacaoAcompanhamentoPedido { get; set; }

        /// <summary>
        /// Indica quando um pedido é de transbordo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoTransbordo", Column = "PED_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotal", Column = "PED_PESO_TOTAL_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquidoTotal", Column = "PED_PESO_LIQUIDO_TOTAL_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquidoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoSaldoRestante", Column = "PED_SALDO_RESTANTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoSaldoRestante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoVolumesRestante", Column = "PED_SALDO_VOLUMES_RESTANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int SaldoVolumesRestante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaiorAlturaProdutoEmCentimetros", Column = "PED_MAIOR_ALTURA_PRODUTO_EM_CENTIMETROS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MaiorAlturaProdutoEmCentimetros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaiorLarguraProdutoEmCentimetros", Column = "PED_MAIOR_LARGURA_PRODUTO_EM_CENTIMETROS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MaiorLarguraProdutoEmCentimetros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaiorComprimentoProdutoEmCentimetros", Column = "PED_MAIOR_COMPRIMENTO_PRODUTO_EM_CENTIMETROS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MaiorComprimentoProdutoEmCentimetros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MaiorVolumeProdutoEmCentimetros", Column = "PED_MAIOR_VOLUME_PRODUTO_EM_CENTIMETROS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MaiorVolumeProdutoEmCentimetros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CubagemTotal", Column = "PED_CUBAGEM_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CubagemTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PESO_CUBADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalNotasFiscais", Column = "PED_VALOR_TOTAL_NOTAS_FISCAIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPedidoCliente", Column = "PED_CODIGO_PEDIDO_CLIENTE", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string CodigoPedidoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialColeta", Column = "PED_DATA_INICIAL_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalColeta", Column = "PED_DATA_FINAL_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoSaida", Column = "PED_DATA_PREVISAO_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEP_DATA_PREVISAO_CHEGADA_DESTINATARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoChegadaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEP_DATA_PREVISAO_SAIDA_DESTINATARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoSaidaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_ULTIMA_LIBERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_SOLICITACAO_REENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_SOLICITACAO_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_SOLICITACAO_AGENDAMENTO_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSolicitacaoAgendamentoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_SUGESTAO_REAGENDAMENTO_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSugestaoReagendamentoDescarga { get; set; }

        /// <summary>
        /// Data de saída do veículo (data que foi executada, será atualizada de acordo com informações de saída da guarita)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_INICIAL_VIAGEM_EXECUTADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialViagemExecutada { get; set; }

        /// <summary>
        /// Data de chegada do veículo (data que foi executada, será atualizada de acordo com informações de entrada da guarita)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_FINAL_VIAGEM_EXECUTADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalViagemExecutada { get; set; }

        /// <summary>
        /// Data de saída do veículo (data para faturamento, será atualizada de acordo com informações de saída da guarita, permitindo o usuário alterar no pedido)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_INICIAL_VIAGEM_FATURADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialViagemFaturada { get; set; }

        /// <summary>
        /// Data de chegada do veículo (data para faturamento, será atualizada de acordo com informações de entrada da guarita, permitindo o usuário alterar no pedido)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_FINAL_VIAGEM_FATURADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalViagemFaturada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo VeiculoTracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosCarregamento", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_VEICULOS_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Veiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_AUTOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Autor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_VENDEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario FuncionarioVendedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_SUPERVISOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario FuncionarioSupervisor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_GERENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario FuncionarioGerente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoIntegradoEmbarcador", Column = "PED_PEDIDO_INTEGRADO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoIntegradoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAutomaticamenteCargaDoPedido", Column = "PED_GERAR_AUTOMATICAMENTE_CARGA_DO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAutomaticamenteCargaDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirObservacaoCTe", Column = "PED_IMPRIMIR_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoTotalmenteCarregado", Column = "PED_TOTALMENTE_CARREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoTotalmenteCarregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoRedespachoTotalmenteCarregado", Column = "PED_REDESPACHO_TOTALMENTE_CARREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoRedespachoTotalmenteCarregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarUmCTEPorNFe", Column = "PED_GERAR_UM_CTE_POR_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarUmCTEPorNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoSubContratado", Column = "PED_SUB_CONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoSubContratado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoPredominante", Column = "PED_PRODUTO_PREDOMINANTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProdutoPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "PED_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "PED_TEMPERATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NOME_ARQUIVO_GERADOR", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string NomeArquivoGerador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_GUID_ARQUIVO_GERADOR", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string GuidArquivoGerador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAReceber", Column = "PED_VALOR_FRETE_RECEBER", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteNegociado", Column = "PED_VALOR_FRETE_NEGOCIADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteNegociado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_FRETE_INFORMATIVO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteInformativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquota", Column = "PED_PERCENTUAL_ALICOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCofins", Column = "PED_ALICOTA_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPis", Column = "PED_ALICOTA_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaInternaDifal", Column = "PED_PERCENTUAL_ALICOTA_INTERNA_DIFAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaInternaDifal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualInclusaoBC", Column = "PED_PERCENTUAL_INCLUSAO_BC", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualInclusaoBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "PED_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "PED_VALOR_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCofins", Column = "PED_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPis", Column = "PED_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSIncluso", Column = "PED_VALOR_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSIncluso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "PED_INCLUIR_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImpostoNegociado", Column = "PED_IMPOSTOS_NEGOCIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImpostoNegociado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtVolumes", Column = "PED_QUANTIDADE_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int QtVolumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotaCliente", Column = "PED_NUMERO_NOTA_CLIENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNotaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "PED_ORDEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoEntrega", Column = "PED_OBSERVACAO_ENTREGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemColetaProgramada", Column = "PED_ORDEM_COLETA_PROGRAMADA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemColetaProgramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemEntregaProgramada", Column = "PED_ORDEM_ENTREGA_PROGRAMADA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemEntregaProgramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteFilialEmissora", Column = "PED_VALOR_FRETE_FILIAL_EMISSORA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteFilialEmissora { get; set; }

        /// <summary>
        /// Indica que o pedido possui um controle de separacao no WMS.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlaSeparacaoPedido", Column = "PED_CONTROLA_SEPARACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaSeparacaoPedido { get; set; }

        /// <summary>
        /// Inidica que o pedido está disponivel na tela de separacação do WMS.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponivelParaSeparacao", Column = "PED_DISPONIVEL_PARA_SEPARACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponivelParaSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSeparacaoPedido", Column = "PED_PERCENTUAL_SEPARACAO_PEDIDO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualSeparacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoBloqueado", Column = "PED_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoLiberadoMontagemCarga", Column = "PED_LIBERADO_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoLiberadoMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoLiberadoPortalRetira", Column = "PED_LIBERADO_PORTAL_RETIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoLiberadoPortalRetira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoRestricaoData", Column = "PED_RESTRICAO_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoRestricaoData { get; set; }

        /// <summary>
        /// indica que o pedido está aguardando para ser reentregue.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ReentregaSolicitada", Column = "PED_REENTREGA_SOLICITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReentregaSolicitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CONTAGEM_REENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int ContagemReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rota", Column = "PED_ROTA", TypeType = typeof(int), NotNull = false)]
        public virtual int Rota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Facility", Column = "PED_FACILITY", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Facility { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Produtos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoProduto", Column = "PRP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> Produtos { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Blocos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BLOCO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BlocoCarregamento", Column = "BLC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> Blocos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidoNotasParciais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_NOTA_FISCAL_PARCIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoNotaParcial", Column = "CNP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> PedidoNotasParciais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidoCtesParciais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_CTE_PARCIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoCTeParcial", Column = "PCP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> PedidoCtesParciais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Transbordos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_TRANSBORDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoTransbordo", Column = "PET_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> Transbordos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargasPedido", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.Carga> CargasPedido { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidosCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedido", Column = "CPE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaPedido> PedidosCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "NotasFiscais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_NOTAS_FISCAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscal", Column = "NFX_CODIGO")]
        public virtual ICollection<XMLNotaFiscal> NotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdEntregas", Column = "PED_QUANTIDADE_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NecessarioReentrega", Column = "PED_NECESSARIO_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessarioReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rastreado", Column = "PED_RASTREADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Rastreado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRastreamento", Column = "PED_CODIGO_RASTREAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoRastreamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerenciamentoRisco", Column = "PED_GERENCIAMENTO_RISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerenciamentoRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EscoltaArmada", Column = "PED_ESCOLTA_ARMADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscoltaArmada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EscoltaMunicipal", Column = "PED_ESCOLTA_MUNICIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscoltaMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DESPACHO_TRANSITO_ADUANEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DespachoTransitoAduaneiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_DTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDTA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Seguro", Column = "PED_SEGURO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Seguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ajudante", Column = "PED_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ajudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdAjudantes", Column = "PED_QUANTIDADE_AJUDANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdAjudantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCarga", Column = "PED_VALOR_TOTAL_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContainer", Column = "PED_NUMERO_CONTAINER", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TaraContainer", Column = "PED_TARA_CONTAINER", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TaraContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LacreContainerDois", Column = "PED_LACRE_CONTAINER_DOIS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string LacreContainerDois { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LacreContainerTres", Column = "PED_LACRE_CONTAINER_TRES", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string LacreContainerTres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LacreContainerUm", Column = "PED_LACRE_CONTAINER_UM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string LacreContainerUm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBL", Column = "PED_NUMERO_BL", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string NumeroBL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaAgendamento", Column = "PED_SENHA_AGENDAMENTO", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string SenhaAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ENTREGA_AGENDADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregaAgendada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_USUARIO_CRIACAO_REMESSA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string UsuarioCriacaoRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_ORDEM", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroOrdem { get; set; }

        /// <summary>
        /// Valor da NFe sem impostos 
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_GROSS_SALES", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal GrossSales { get; set; }

        /// <summary>
        /// Data faturamento do pedido
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_ALOCACAO_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlocacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_POSSUI_ETIQUETAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiEtiquetagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_POSSUI_ISCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIsca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdIsca", Column = "PED_QUANTIDADE_ISCA", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdIsca { get; set; }

        /// <summary>
        /// Propriedade para identificar se é permitido quebrar o pedido em mais de um carregamento durante o processo de roteirização.
        /// Tarefa 10060
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_QUEBRA_MULTIPLOS_CARREGAMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool QuebraMultiplosCarregamentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Deposito", Column = "DEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual WMS.Deposito Deposito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaAgendamentoCliente", Column = "PED_SENHA_AGENDAMENTO_CLIENTE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string SenhaAgendamentoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNavio", Column = "PED_NUMERO_NAVIO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string NumeroNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Porto Porto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoEntregaImportacao", Column = "PED_ENDERECO_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string EnderecoEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BairroEntregaImportacao", Column = "PED_BAIRRO_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string BairroEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEPEntregaImportacao", Column = "PED_CEP_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string CEPEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontoReferenciaImportacao", Column = "PED_PONTO_REFERENCIA_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string PontoReferenciaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ENTREGA_IMPORTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoArmazenamentoImportacao", Column = "PED_DATA_VENCIMENTO_ARMAZENAMENTO_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoArmazenamentoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArmadorImportacao", Column = "PED_ARMADOR_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string ArmadorImportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoTerminalImportacao TipoTerminalImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaPedido", Column = "PED_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido EtapaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RESPONSAVEL_REDESPACHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ResponsavelRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_COTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_FRETE_COTADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteCotado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_COLETA_EM_PRODUTOR_RURAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ColetaEmProdutorRural { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_OBSERVACAO_INTERNA", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string ObservacaoInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_MOTIVO_CANCELAMENTO", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidoImportacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_IMPORTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoImportacao", Column = "PEI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao> PedidoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidosTransbordo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_TRANSBORDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoTransbordo", Column = "PET_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> PedidosTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidosComponente", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_COMPONENTES_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoComponenteFrete", Column = "PCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> PedidosComponente { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PedidoAutorizacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_AUTORIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoAutorizacao", Column = "PEA_CODIGO")]
        public virtual IList<PedidoAutorizacao> PedidoAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedido", Column = "CTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoPedido.CotacaoPedido CotacaoPedido { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_QUANTIDADE_NOTAS_FISCAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeNotasFiscais { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_POSSUI_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiDescarga { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_DESCARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorDescarga { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_POSSUI_DESLOCAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiDeslocamento { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_DESLOCAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorDeslocamento { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_POSSUI_DIARIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiDiaria { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorDiaria { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_POSSUI_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCarga { get; set; }

        /// <summary>
        /// Campo meramente informativo, usuário pode digitar no pedido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOrder", Column = "PED_DATA_ORDER", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataOrder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChip", Column = "PED_DATA_CHIP", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancel", Column = "PED_DATA_CANCEL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_QUANTIDADE_ESCOLTA", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdEscolta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_QUANTIDADE_VOLUMES_PREVIOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeVolumesPrevios { get; set; }

        /// <summary>
        /// Campo utilizado para controle do protocolo de integração do pedido (retornado nas integrações para os clientes)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PROTOCOLO", TypeType = typeof(int), NotNull = false)]
        public virtual int Protocolo { get; set; }

        /// <summary>
        /// Deve ser utilizada como informação de dica recebida do remetente.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_OBSERVACAO_EMISSAO_REMETENTE", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaRemetente { get; set; }

        /// <summary>
        /// Deve ser utilizada como informação de dica recebida do destinatario.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_OBSERVACAO_EMISSAO_DESTINATARIO", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VENDEDOR", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Vendedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PORTO_SAIDA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string PortoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PORTO_CHEGADA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string PortoChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_COMPANHIA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Companhia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_RESERVA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Reserva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_RESUMO", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Resumo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DELIVERY_TERM", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string DeliveryTerm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ID_AUTORIZACAO", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string IdAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataETA", Column = "PED_DATA_ETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInclusaoPCP", Column = "PED_DATA_INCLUSAO_PCP", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInclusaoPCP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInclusaoBooking", Column = "PED_DATA_INCLUSAO_BOOKING", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInclusaoBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_TIPO_EMBARQUE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string TipoEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteCobradoCliente", Column = "PED_VALOR_FRETE_COBRADO_CLIENTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteCobradoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCustoFrete", Column = "PED_VALOR_CUSTO_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorCustoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiGenset", Column = "PED_POSSUI_GENSET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiGenset { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPedido", Column = "PED_TIPO_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPedido TipoPedido { get; set; }

        //Multimodal
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ID_BAF", TypeType = typeof(int), NotNull = false)]
        public virtual int IDBAF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_AD_VALOREM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_BAF", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorBAF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CODIGO_BOOKING", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string CodigoBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_BOOKING", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CODIGO_OS", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string CodigoOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_OS", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_EMBARQUE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Embarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_MASTER_BL", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string MasterBL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_DI", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CODIGO_PROPOSTA", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string CodigoProposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_PROPOSTA", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroProposta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_PROVEDOR_OS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ProvedorOS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NECESSITA_AVERBACAO_AUTOMATICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessitaAverbacaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAverbacaoCTE", Column = "PED_FORMA_AVERBACAO_CTE", TypeType = typeof(Enumeradores.FormaAverbacaoCTE), NotNull = false)]
        public virtual Enumeradores.FormaAverbacaoCTE FormaAverbacaoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_POSSUI_CARGA_PERIGOSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCargaPerigosa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CONTEM_CARGA_REFRIGERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ContemCargaRefrigerada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NECESSITA_ENERGIA_CONTAINER_REFRIGERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessitaEnergiaContainerRefrigerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALIDAR_DIGITO_VERIFICADOR_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDigitoVerificadorContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PEDIDO_SVM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PEDIDO_SVM_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoDeSVMTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Navio Navio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO_BALSA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio Balsa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoTerminalImportacao TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoTerminalImportacao TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DIRECAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal DirecaoViagemMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Container Container { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO_RESERVA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo ContainerTipoReserva { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO_LONGO_CURSO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoViagemNavio PedidoViagemNavioLongoCurso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MontagemContainer", Column = "MTC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MontagemContainer MontagemContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_QUANTIDADE_TIPO_CONTAINER_RESERVA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeTipoContainerReserva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoAverbacao", Column = "PED_TIPO_DOCUMENTO_AVERBACAO_CTE", TypeType = typeof(Enumeradores.TipoDocumentoAverbacao), NotNull = false)]
        public virtual Enumeradores.TipoDocumentoAverbacao TipoDocumentoAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaFeeder", Column = "PED_TIPO_PROPOSTA_FEEDER", TypeType = typeof(Enumeradores.TipoPropostaFeeder), NotNull = false)]
        public virtual Enumeradores.TipoPropostaFeeder TipoPropostaFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DESCRICAO_TIPO_PROPOSTA_FEEDER", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string DescricaoTipoPropostaFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ELEMENTO_PEP", TypeType = typeof(string), NotNull = false, Length = 24)]
        public virtual string ElementoPEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DESCRICAO_CARRIER_NAVIO_VIAGEM", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string DescricaoCarrierNavioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_REALIZAR_COBRANCA_TAXA_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarCobrancaTaxaDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_QUANTIDADE_CONHECIMENTOS_TAXA_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeConhecimentosTaxaDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_TAXA_DOCUMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTaxaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CONTAINER_A_DEFINIR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ContainerADefinir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_CUSTEIO_SVM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorCusteioSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_QUANTIDADE_CONTAINER_BOOKING", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeContainerBooking { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoEmpresaResponsavel", Column = "PER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoEmpresaResponsavel PedidoEmpresaResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCentroCusto", Column = "PCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoCentroCusto PedidoCentroCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_EMBARQUE_AFRETAMENTO_FEEDER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmbarqueAfretamentoFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_MANIFESTO_FEEDER", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroManifestoFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PROTOCOLO_ANTAQ_FEEDER", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string ProtocoloANTAQFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_CE_FEEDER", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroCEFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaMultimodal", Column = "PED_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCalculoCargaFracionada", Column = "PED_TIPO_CALCULO_CARGA_FFRACIONADA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCargaFracionada), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCargaFracionada? TipoCalculoCargaFracionada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoTipoPagamento", Column = "PTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoTipoPagamento PedidoTipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_TAXA_FEEDER", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTaxaFeeder { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM_BOOKING", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade OrigemBooking { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO_BOOKING", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade DestinoBooking { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CTesTerceiro", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_CTE_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CTeTerceiro", Column = "CPS_CODIGO")]
        public virtual ICollection<CTe.CTeTerceiro> CTesTerceiro { get; set; }

        /// <summary>
        /// Campo utilizado para evitar duplicidade em banco com chave única permitindo quando pedido é cancelado
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CONTROLE_NUMERACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ControleNumeracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DestinatariosBloqueados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_DESTINATARIOS_BLOQUEADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "PED_CNPJ_CPF_DESTINATARIO_BLOQUEADO", TypeType = typeof(string), NotNull = true, Length = 80)]
        public virtual ICollection<string> DestinatariosBloqueados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DISTANCIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_TERMINO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTerminoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamento { get; set; }

        [Obsolete("Criada a coluna PED_DATA_PRIMEIRO_AGENDAMENTO para salvar a primeira data inserida e as datas reagendadas irão na coluna PED_DATA_AGENDAMENTO")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_REAGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_PRIMEIRO_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrimeiroAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "PED_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<PedidoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoTrocaNota", Column = "PED_PEDIDO_TROCA_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoTrocaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoTrocaNota", Column = "PED_NUMERO_TROCA_NOTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoTrocaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CIOT", Column = "PED_CIOT", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual String CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGlobalizarPedido", Column = "PED_NAO_GLOBALIZAR_PEDIDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoGlobalizarPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONAL1", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Adicional1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONAL2", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Adicional2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONAL3", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Adicional3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONAL4", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Adicional4 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONAL5", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Adicional5 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONAL6", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Adicional6 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ADICIONAL7", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Adicional7 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCargaEncaixar", Column = "PED_NUMERO_CARGA_ENCAIXAR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCargaEncaixar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControle", Column = "PED_NUMERO_CONTROLE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDespachante", Column = "PED_DESPACHANTE_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoDespachante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoDespachante", Column = "PED_DESPACHANTE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoDespachante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ViaTransporte", Column = "TVT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ViaTransporte ViaTransporte { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "CodigoViaTransporte", Column = "PED_VIA_TRANSPORTE_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        //public virtual string CodigoViaTransporte { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoViaTransporte", Column = "PED_VIA_TRANSPORTE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        //public virtual string DescricaoViaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPortoOrigem", Column = "PED_PORTO_ORIGEM_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoPortoOrigem", Column = "PED_PORTO_ORIGEM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PaisPortoOrigem", Column = "PED_PORTO_ORIGEM_PAIS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PaisPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaPaisPortoOrigem", Column = "PED_PORTO_ORIGEM_SIGLA_PAIS", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SiglaPaisPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPortoDestino", Column = "PED_PORTO_DESTINO_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoPortoDestino", Column = "PED_PORTO_DESTINO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PaisPortoDestino", Column = "PED_PORTO_DESTINO_PAIS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PaisPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaPaisPortoDestino", Column = "PED_PORTO_DESTINO_SIGLA_PAIS", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SiglaPaisPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoInLand", Column = "PED_INLAND_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoInLand { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoInLand", Column = "PED_INLAND_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoInLand { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentoMaritimo", Column = "PED_PAGAMENTO_MARITIMO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PagamentoMaritimo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PagamentoMaritimo? PagamentoMaritimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoProbe", Column = "PED_TIPO_PROBE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoProbe), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoProbe? TipoProbe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPaletizada", Column = "PED_CARGA_PALETIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPaletizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNavioViagem", Column = "PED_NAVIO_VIAGEM_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoNavioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeNavioViagem", Column = "PED_NAVIO_VIAGEM_NOME", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeNavioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeadLineNavioViagem", Column = "PED_NAVIO_VIAGEM_DATA_DEADLINE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeadLineNavioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeadLCargaNavioViagem", Column = "PED_NAVIO_VIAGEM_DATA_DEADLCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeadLCargaNavioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataETS", Column = "PED_DATA_ETS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_FREE_DETEN", TypeType = typeof(int), NotNull = false)]
        public virtual int FreeDeten { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEXP", Column = "PED_NUMERO_EXP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroEXP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RefEXPTransferencia", Column = "PED_REF_EXP_TRANSFERENCIA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RefEXPTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEXP", Column = "PED_STATUS_EXP", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusEXP), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusEXP? StatusEXP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoProvisorio", Column = "PED_NUMERO_PEDIDO_PROVISORIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedidoProvisorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEspecie", Column = "PED_ESPECIE_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoEspecie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoEspecie", Column = "PED_ESPECIE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoEspecie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusPedidoEmbarcador", Column = "PED_STATUS_PEDIDO_EMBARCADOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoEmbarcador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoEmbarcador? StatusPedidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcondicionamentoCarga", Column = "PED_ACONDICIONAMENTO_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AcondicionamentoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AcondicionamentoCarga? AcondicionamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEstufagem", Column = "PED_DATA_ESTUFAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEstufagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Onda", Column = "PED_ONDA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Onda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClusterRota", Column = "PED_CLUSTER_ROTA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClusterRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoInicioViagem", Column = "PED_DATA_PREVISAO_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoEndereco", Cascade = "all", Column = "PEN_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco EnderecoRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoEndereco", Cascade = "all", Column = "PEN_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco EnderecoExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IMOUnidade", Column = "PED_IMO_UNIDADE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IMOUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IMOClasse", Column = "PED_IMO_CLASSE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IMOClasse { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IMOSequencia", Column = "PED_IMO_SEQUENCIA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IMOSequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RotaEmbarcador", Column = "PED_ROTA_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RotaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AntecipacaoICMS", Column = "PED_ANTECIPACAO_ICMS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AntecipacaoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DIAS_ITINERARIO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasItinerario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DIAS_PRAZO_TRANSPORTADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasUteisPrazoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ID_PROPOSTA_TRIZY", TypeType = typeof(int), NotNull = false)]
        public virtual int IDPropostaTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ID_LOTE_TRIZY", TypeType = typeof(int), NotNull = false)]
        public virtual int IDLoteTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_PEDAGIO_ROTA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagioRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PLPCorreios", Column = "PED_PLP_CORREIOS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PLPCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEtiquetaCorreios", Column = "PED_NUMERO_ETIQUETA_CORREIOS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroEtiquetaCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRastreioCorreios", Column = "PED_NUMERO_RASTREIO_CORREIOS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroRastreioCorreios { get; set; }

        /// <summary>
        /// Pedidos que estiverem no mesmo pallet precisam obrigatóriamente ir juntos na separação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PalletAgrupamento", Column = "PED_PALLET_AGRUPAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PalletAgrupamento { get; set; }


        /// <summary>
        /// Apenas altera a localidade de início da prestação dos documentos emitidos, não afetando os demais processos do sistema (cálculo de frete, rota, etc.)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_INICIO_PRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeInicioPrestacao { get; set; }

        /// <summary>
        /// Apenas altera a localidade de término da prestação dos documentos emitidos, não afetando os demais processos do sistema (cálculo de frete, rota, etc.)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_TERMINO_PRESTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadeTerminoPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_FRETE_TRANSPORTADOR_TERCEIRO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_ENTREGAS_FINAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEntregasFinais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAdicional", Column = "PED_OBSERVACAO_ADICIONAL", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ObservacaoAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeiculares", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeiculares { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_FRETE_TONELADA_TERCEIRO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteToneladaTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_FRETE_TONELADA_NEGOCIADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteToneladaNegociado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Safra", Column = "PED_SAFRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Safra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdDisponibilizada", Column = "PED_QTD_DISPONIBILIZADA", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdDisponibilizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdNaoEmbarcadas", Column = "PED_QTD_NAO_EMBARCADA", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdNaoEmbarcadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadesContabilidade", Column = "PED_UNIDADES_CONTABILIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int UnidadesContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ALIQUOTA_ICMS_CONTABILIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMSContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_ICMS_CONTABILIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PTAX_CONTABILIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PTAXContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_VALOR_USD_CONTABILIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorUSDContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PONumberContabilidade", Column = "PED_PO_NUMBER_CONTABILIDADE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string PONumberContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PERCENTUAL_ADIANTAMENTO_TERCEIRO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualAdiantamentoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PERCENTUAL_MINIMO_ADIANTAMENTO_TERCEIRO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualMinimoAdiantamentoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PERCENTUAL_MAXIMO_ADIANTAMENTO_TERCEIRO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualMaximoAdiantamentoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PEDIDO_TAKE_OR_PAY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoTakeOrPay { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PEDIDO_DEMURRAGE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoDemurrage { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PEDIDO_DETETION", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoDetention { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_MOTORISTA_AVISADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaAvisado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_MOTORISTA_CIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaCiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_COM_TRATATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComTratativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FileIdMichelin", Column = "PED_FIELD_ID_MICHELIN", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string FileIdMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MessageIdentifierCodeMichelin", Column = "PED_MESSAGE_IDENTIFIER_CODE_MICHELIN_MICHELIN", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string MessageIdentifierCodeMichelin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CANCELADO_APOS_VINCULO_COM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CanceladoAposVinculoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoParaFaturamento", Column = "PED_OBSERVACAO_PARA_FATURAMENTO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ObservacaoParaFaturamento { get; set; }

        /// <summary>
        /// Número arbitrário de um lote de pedidos. Originalmente criado para a integração com a E-Millenium.
        /// Também pode ser chamado de número de Embarque.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLote", Column = "PED_NUMERO_LOTE", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string NumeroLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoDevolucao", Column = "PED_NUMERO_PEDIDO_DEVOLUCAO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string NumeroPedidoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_PEDIDO_DE_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoDeDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO_DEVOLUCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido PedidoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_MOTIVO_REAGENDAMENTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ObservacaoReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoResponsavelAtrasoEntrega", Column = "PED_RESPONSAVEL_MOTIVO_REAGENDAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoResponsavelAtrasoEntrega ResponsavelMotivoReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_COLETA_CONTAINER", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColetaContainer { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_AG_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoIntegracao { get; set; }

        /// <summary>
        /// Esse número é um número arbitrário que identifica um carregamento. É usado hoje nas telas NumeroCarregamentoPorLote
        /// e na busca pra encontrar pedidos na tela MontagemCargaMapa
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarregamento", Column = "PED_NUMERO_CARREGAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_CRIACAO_PEDIDO_ERP", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacaoPedidoERP { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cotacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoPedido", Column = "CTP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> Cotacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiChatAtivo", Column = "PED_POSSUI_CHAT_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiChatAtivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO_ORIGEM_DEVOLUCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido PedidoOrigemDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoReagendamento", Column = "MRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento MotivoReagendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_KM_ASFALTO_ATE_DESTINO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal KMAsfaltoAteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_KM_CHAO_ATE_DESTINO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal KMChaoAteDestino { get; set; }

        /// <summary>
        /// Esse número é usado no Servico do Pedido, ao fazer uma importação para salvar o número da Doca para gerar a précarga/carga com o número da doca abaixo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_NUMERO_DOCA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDeDoca", Column = "PED_TEMPO_DOCA_SEGUNDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDeDoca { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_LOCAL_PARQUEAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente LocalParqueamento { get; set; }

        /// <summary>
        /// #49608 - SIMONETTI - Atributo recebido no método adicionar carga com o objetivo de agrupar os pedidos com o mesmo CodigoAgrupamento ao efetuar uma montagem de carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAgrupamentoCarregamento", Column = "PED_CODIGO_AGRUPAMENTO_CARREGAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoAgrupamentoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotoristaEscala", Column = "PED_MOTORISTA_ESCALA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MotoristaEscala { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataComparecerEscala", Column = "PED_DATA_COMPARECER_ESCALA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataComparecerEscala { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AceiteMotorista", Column = "PED_ACEITE_MOTORISTA_ESCALA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumAceiteMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumAceiteMotorista? AceiteMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaConsultaCorreios", Column = "PED_DATA_ULTIMA_CONSULTA_CORREIOS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaConsultaCorreios { get; set; }

        /// <summary>
        /// Usado apenas para customizar a timeline da carga para projeto cabotagem, não pode interferir com o TipoContratacaoCarga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoCarga", Column = "CAR_TIPO_SERVICO_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoCarga? TipoServicoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDistribuicao", Column = "CDI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDistribuicao CDDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "StagesPedido", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_STAGE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoStage", Column = "PES_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> StagesPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Substituicao", Column = "PED_SUBSTITUICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Substituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiStage", Column = "PED_POSSUI_STAGE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiStage { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoFrete", Column = "PED_CUSTO_FRETE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CustoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemCriacaoDataAgendamentoPedido", Column = "PED_ORIGEM_CRIACAO_PEDIDO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacao? OrigemCriacaoDataAgendamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PalletSaldoRestante", Column = "PED_PALLET_SALDO_RESTANTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PalletSaldoRestante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ResponsavelCIOT", Column = "PED_RESPONSAVEL_CIOT", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string ResponsavelCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RESPONSAVEL_AGENDAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioAssumiuAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAssumiuAgendamento", Column = "PED_DATA_ASSUMIU_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAssumiuAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeResponsavelAgendamentoCliente", Column = "PED_NOME_RESPONSAVEL_AGENDAMENTO_CLIENTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomeResponsavelAgendamentoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloAgendamento", Column = "PED_PROTOCOLO_AGENDAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ProtocoloAgendamento { get; set; }

        [Obsolete("Campo não é mais utilizado.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEntrega", Column = "PED_DATA_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_LEAD_TIME", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_LEAD_TIME_FILIAL_EMISSORA", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTimeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoEntregaFilialEmissora", Column = "PED_PREVISAO_ENTREGA_FILIAL_EMISSORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoEntregaFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaMercadoLivre", Column = "PED_NOTA_MERCADO_LIVRE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NotaMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaFaturamentoMercadoLivre", Column = "PED_SIGLA_FATURAMENTO_MERCADO_LIVRE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SiglaFaturamentoMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PFMercadoLivre", Column = "PED_PF_MERCADO_LIVRE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PFMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemFaturadoMercadoLivre", Column = "PED_ITEM_FATURADO_MERCADO_LIVRE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ItemFaturadoMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoColetaEntrega", Column = "PED_PEDIDO_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoImportadoPorPlanilha", Column = "PED_PEDIDO_IMPORTADO_POR_PLANILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedidoImportadoPorPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomadorCabotagem", Column = "PED_TIPO_TOMADOR_CABOTAGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTomadorCabotagem TipoTomadorCabotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModalPropostaCabotagem", Column = "PED_MODAL_PROPOSTA_CABOTAGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoModalPropostaCabotagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoModalPropostaCabotagem TipoModalPropostaCabotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaCabotagem", Column = "PED_TIPO_PROPOSTA_CABOTAGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem TipoPropostaCabotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_DE_CRIACAO_PEDIDO_ERP", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeCriacaoPedidoERP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCustoViagem", Column = "CCV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem CentroDeCustoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoComercialPedido", Column = "SCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido SituacaoComercialPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCobrancaFreteCombinado", Column = "PED_VALOR_COBRANCA_FRETE_COMBINADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? ValorCobrancaFreteCombinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPaleteCliente", Column = "PED_TIPO_PALETE_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente? TipoPaleteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_PREVISAO_TERMINO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoTerminoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BookingReference", Column = "PED_BOOKING_REFERENCE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string BookingReference { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoPedido", Column = "PM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.MotivoPedido MotivoPedido { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DevolucaoPacote", Column = "PED_DEVOLUCAO_PACOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DevolucaoPacote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "PCA_CARGA_SVM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga CargaSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaOS", Column = "PED_CATEGORIA_OS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CategoriaOS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CategoriaOS? CategoriaOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NecessariaAverbacao", Column = "PED_NECESSARIA_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessariaAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoProvedor", Column = "PED_DOCUMENTO_PROVEDOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DocumentoProvedor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DocumentoProvedor? DocumentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalProvedor", Column = "PED_VALOR_TOTAL_PROVEDOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarPagamento", Column = "PED_LIBERAR_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOS", Column = "PED_TIPO_OS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOS? TipoOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOSConvertido", Column = "PED_TIPO_OS_CONVERTIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido? TipoOSConvertido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DirecionamentoOS", Column = "PED_DIRECIONAMENTO_OS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DirecionamentoOS), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DirecionamentoOS? DirecionamentoOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoXML", Column = "PED_TIPO_SERVICO_XML", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoXML), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoXML? TipoServicoXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicLiberacaoOk", Column = "PED_INDIC_LIBERACAO_OK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicLiberacaoOk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgendamentoEntrega", Column = "PED_TIPO_AGENDAMENTO_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgendamentoEntrega), NotNull = false, Length = 100)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgendamentoEntrega? TipoAgendamentoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_E_HORA_ENVIO_EMAIL_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataeHoraEnvioEmailAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCancelamentoPedido", Column = "MCP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido MotivoCancelamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CANCELAMENTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_E_HORA_ENVIO_EMAIL_NOTIFICACAO_AGENDAMENTO_TRANSPORTADOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataeHoraEnvioEmailNotificacaoAgendamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_ESCRITORIO_VENDA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EscritorioVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_TIPO_MERCADORIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_EQUIPE_VENDAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EquipeVendas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_LOCAL_PALETIZACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente LocalPaletizacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CotacoesEspeciais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_COTACAO_ESPECIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoEspecial", Column = "COE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial> CotacoesEspeciais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedidoCritico", Column = "PED_PEDIDO_CRITICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PedidoCritico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItensAtualizados", Column = "PED_ITENS_ATUALIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ItensAtualizados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO_RESERVA_FLUXO_CONTAINER", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo ContainerTipoReservaFluxoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DECLARACAO_OBSERVACAO_CRT", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string DeclaracaoObservacaoCRT { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string DescricaoTipoPagamentoCIFFOB
        {
            get
            {
                if (TipoTomador == Enumeradores.TipoTomador.Remetente)
                    return "CIF";

                if (TipoTomador == Enumeradores.TipoTomador.Destinatario)
                    return "FOB";

                return "Outro";
            }
        }

        public virtual string NomeMotoristas
        {
            get
            {
                if (this.Motoristas != null && this.Motoristas.Count > 0)
                    return string.Join(", ", from obj in Motoristas select obj.Nome);

                return "";
            }
        }

        public virtual string DescricaoSituacaoPedido
        {
            get
            {
                if (this.CanceladoAposVinculoCarga)
                    return "Cancelado após vincular carga";

                switch (this.SituacaoPedido)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao:
                        return "Ag. Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente:
                        return "Autorização Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Rejeitado:
                        return "Rejeitado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoEtapaPedido
        {
            get
            {
                switch (this.EtapaPedido)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.AgAutorizacao:
                        return "Ag. Autorização";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Cancelada:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Finalizada:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Pedido:
                        return "Pedido";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Integracao:
                        return "Integração";
                    default:
                        return "";
                }
            }
        }

        public virtual string NumerosLacres
        {
            get
            {
                //if (!string.IsNullOrWhiteSpace(this.LacreContainerUm) && !string.IsNullOrWhiteSpace(this.LacreContainerDois) && !string.IsNullOrWhiteSpace(this.LacreContainerTres))
                //    return " (" + this.LacreContainerUm + ", " + this.LacreContainerDois + ", " + this.LacreContainerTres + ")";
                //else if (!string.IsNullOrWhiteSpace(this.LacreContainerUm) && !string.IsNullOrWhiteSpace(this.LacreContainerDois))
                //    return " (" + this.LacreContainerUm + ", " + this.LacreContainerDois + ")";
                //else if (!string.IsNullOrWhiteSpace(this.LacreContainerUm))
                //    return " (" + this.LacreContainerUm + ")";

                return "";
            }
        }

        public virtual string Descricao
        {
            get
            {
                return NumeroPedidoEmbarcador ?? string.Empty;
            }
        }

        public virtual string Despachante
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoDespachante))
                    descricao.Add(CodigoDespachante);

                if (!string.IsNullOrWhiteSpace(DescricaoDespachante))
                    descricao.Add(DescricaoDespachante);

                return string.Join(" - ", descricao);
            }
        }

        public virtual bool Provisorio
        {
            get { return (Filial != null) && (Destinatario != null) && (Filial.CNPJ == Destinatario.CPF_CNPJ_SemFormato); }
        }

        public virtual decimal TotalPallets
        {
            get { return NumeroPaletes + NumeroPaletesFracionado; }
        }

        public virtual string InLand
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoInLand))
                    descricao.Add(CodigoInLand);

                if (!string.IsNullOrWhiteSpace(DescricaoInLand))
                    descricao.Add(DescricaoInLand);

                return string.Join(" - ", descricao);
            }
        }

        public virtual string NavioViagem
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoNavioViagem))
                    descricao.Add(CodigoNavioViagem);

                if (!string.IsNullOrWhiteSpace(NomeNavioViagem))
                    descricao.Add(NomeNavioViagem);

                return string.Join(" - ", descricao);
            }
        }

        public virtual string PortoViagemDestino
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoPortoDestino))
                    descricao.Add(CodigoPortoDestino);

                if (!string.IsNullOrWhiteSpace(DescricaoPortoDestino))
                    descricao.Add(DescricaoPortoDestino);

                if (!string.IsNullOrWhiteSpace(PaisPortoDestino))
                    descricao.Add(PaisPortoDestino);

                if (!string.IsNullOrWhiteSpace(SiglaPaisPortoDestino))
                    descricao.Add(SiglaPaisPortoDestino);

                return string.Join(" - ", descricao);
            }
        }

        public virtual string PortoViagemOrigem
        {
            get
            {
                List<string> descricao = new List<string>();

                if (!string.IsNullOrWhiteSpace(CodigoPortoOrigem))
                    descricao.Add(CodigoPortoOrigem);

                if (!string.IsNullOrWhiteSpace(DescricaoPortoOrigem))
                    descricao.Add(DescricaoPortoOrigem);

                if (!string.IsNullOrWhiteSpace(PaisPortoOrigem))
                    descricao.Add(PaisPortoOrigem);

                if (!string.IsNullOrWhiteSpace(SiglaPaisPortoOrigem))
                    descricao.Add(SiglaPaisPortoOrigem);

                return string.Join(" - ", descricao);
            }
        }

        public virtual Localidade LocalidadeEnderecoDestinoAtual
        {
            get
            {
                return this.UsarOutroEnderecoDestino ? this.EnderecoDestino?.ClienteOutroEndereco?.Localidade : this.EnderecoDestino?.Localidade;
            }
        }

        public virtual string CEPEnderecoDestinoAtual
        {
            get
            {
                return this.UsarOutroEnderecoDestino ? this.EnderecoDestino?.ClienteOutroEndereco?.CEP : this.EnderecoDestino?.CEP;
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Clonar()
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoClonado = (Dominio.Entidades.Embarcador.Pedidos.Pedido)this.MemberwiseClone();
            pedidoClonado.ControleNumeracao = 0;
            return pedidoClonado;
        }

        public virtual string ObterNumeroControle()
        {
            if (string.IsNullOrWhiteSpace(NumeroControle))
                return "";
            else
                return NumeroControle;
        }

        public virtual string DescricaoTipoTomador()
        {
            if (TipoTomador == Enumeradores.TipoTomador.Remetente)
                return "Remetente";
            else if (TipoTomador == Enumeradores.TipoTomador.Destinatario)
                return "Destinatario";
            else if (TipoTomador == Enumeradores.TipoTomador.Outros)
                return "Outro";
            else if (TipoTomador == Enumeradores.TipoTomador.Recebedor)
                return "Recebedor";
            else if (TipoTomador == Enumeradores.TipoTomador.Expedidor)
                return "Expedidor";
            else
                return "";
        }

        public virtual bool Equals(Pedido other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual Dominio.Entidades.Cliente ObterDestinatario()
        {
            if (Recebedor != null)
                return Recebedor;

            return Destinatario;
        }

        public virtual Dominio.Entidades.Cliente ObterTomador()
        {
            if (TipoTomador == Enumeradores.TipoTomador.Remetente)
                return Remetente;
            else if (TipoTomador == Enumeradores.TipoTomador.Destinatario)
                return Destinatario;
            else if (TipoTomador == Enumeradores.TipoTomador.Outros || TipoTomador == Enumeradores.TipoTomador.Tomador)
                return Tomador;
            else if (TipoTomador == Enumeradores.TipoTomador.Recebedor)
                return Recebedor;
            else if (TipoTomador == Enumeradores.TipoTomador.Expedidor)
                return Expedidor;
            else
                return null;
        }

        public virtual string ObterProprietario()
        {
            string proprietario = string.Empty;
            if (VeiculoTracao != null)
                proprietario = VeiculoTracao.Proprietario?.Nome;

            if (string.IsNullOrWhiteSpace(proprietario))
                proprietario = Veiculos.Where(x => x.IsTipoVeiculoTracao()).FirstOrDefault()?.Proprietario?.Nome;

            return proprietario;
        }

        #endregion Métodos Públicos
    }
}
