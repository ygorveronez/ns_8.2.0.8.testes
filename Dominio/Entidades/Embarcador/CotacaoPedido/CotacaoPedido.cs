using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_PEDIDO", EntityName = "CotacaoPedido", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido", NameType = typeof(CotacaoPedido))]
    public class CotacaoPedido : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>
    {
        public CotacaoPedido()
        {
            this.DataCriacao = DateTime.Now;
            this.UltimaAtualizacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        /// <summary>
        /// aqui começa a primeira aba
        /// </summary>

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CTP_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CTP_CODIGO_INTEGRACAO", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimaAtualizacao", Column = "CTP_ULTIMA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime UltimaAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Previsao", Column = "CTP_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Previsao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoClienteCotacaoPedido", Column = "CTP_TIPO_PESSOA_COTACAO_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido TipoClienteCotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_ATIVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteAtivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_INATIVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteInativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClienteNovo", Column = "CTP_CLIENTE_NOVO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ClienteNovo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteProspect", Column = "CPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CRM.ClienteProspect ClienteProspect { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPedido", Column = "CTP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido SituacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Solicitante", Column = "CTP_SOLICITANTE", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContato", Column = "CTP_EMAIL_CONTATO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string EmailContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TelefoneContato", Column = "CTP_TELEFONE_CONTATO", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string TelefoneContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusCotacaoPedido", Column = "CTP_STATUS_COTACAO_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido StatusCotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoCotacao", Column = "SCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao SolicitacaoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataColetaPrevista", Column = "CTP_DATA_COLETA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColetaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarTipoTomadorCotacaoPedido", Column = "CTP_USAR_TIPO_TOMADOR_COTACAO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarTipoTomadorCotacaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTomador", Column = "CTP_TIPO_TOMADOR", TypeType = typeof(Enumeradores.TipoTomador), NotNull = false)]
        public virtual Enumeradores.TipoTomador TipoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        /// <summary>
        /// aqui começa a segunda aba
        /// </summary>

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarOutroEnderecoOrigem", Column = "CTP_USAR_OUTRO_ENDERECO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarOutroEnderecoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedidoEndereco", Column = "CPE_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoPedidoEndereco EnderecoOrigem { get; set; }

        /// <summary>
        /// Aqui começa a terceira aba
        /// </summary>

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarOutroEnderecoDestino", Column = "CTP_USAR_OUTRO_ENDERECO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarOutroEnderecoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoPedidoEndereco", Column = "CPE_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoPedidoEndereco EnderecoDestino { get; set; }

        /// <summary>
        /// Aqui começa a quarta aba
        /// </summary>

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialColeta", Column = "CTP_DATA_INICIAL_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalColeta", Column = "CTP_DATA_FINAL_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModal", Column = "CTP_TIPO_MODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoModal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoModal TipoModal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPaletes", Column = "CTP_NUMERO_PALETES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LeadTimeEntrega", Column = "CTP_LEAD_TIME_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTimeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotal", Column = "CTP_PESO_TOTAL_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalNotasFiscais", Column = "CTP_VALOR_TOTAL_NOTAS_FISCAIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeNotas", Column = "CTP_QUANTIDADE_NOTAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdEntregas", Column = "CTP_QUANTIDADE_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdEntregas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "CTP_TEMPERATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTotal", Column = "CTP_KM_TOTAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTonelada", Column = "CTP_VALOR_FRETE_TONELADA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTonelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPorKM", Column = "CTP_VALOR_POR_KM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CTP_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rastreado", Column = "CTP_RASTREADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Rastreado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerenciamentoRisco", Column = "CTP_GERENCIAMENTO_RISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerenciamentoRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EscoltaArmada", Column = "CTP_ESCOLTA_ARMADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscoltaArmada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdEscoltas", Column = "CTP_QUANTIDADE_ESCOLTA", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdEscoltas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ajudante", Column = "CTP_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ajudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdAjudantes", Column = "CTP_QUANTIDADE_AJUDANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int QtdAjudantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CubagemTotal", Column = "CTP_CUBAGEM_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CubagemTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_PESO_CUBADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCubado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtVolumes", Column = "CTP_QUANTIDADE_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int QtVolumes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cubagens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_PEDIDO_CUBAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoPedidoCubagem", Column = "CTC_CODIGO")]
        public virtual IList<CotacaoPedidoCubagem> Cubagens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CTP_TIPO_PAGAMENTO", TypeType = typeof(Enumeradores.TipoPagamento), NotNull = false)]
        public virtual Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_OBSERVACAO_INTERNA", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string ObservacaoInterna { get; set; }

        /// <summary>
        /// Aqui começa a quinta aba
        /// </summary>

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCotacao", Column = "CTP_VALOR_COTACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAcrescimo", Column = "CTP_PERCENTUAL_ACRESCIMO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "CTP_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCotacao", Column = "CTP_TOTAL_VALOR_COTACAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "CTP_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "CTP_VALOR_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCotacaoComICMS", Column = "CTP_TOTAL_VALOR_COM_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalCotacaoComICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirValorICMSBaseCalculo", Column = "CTP_INCLUIR_VALOR_ICMS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirValorICMSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Componentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_PEDIDO_COMPONENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoPedidoComponente", Column = "CPC_CODIGO")]
        public virtual IList<CotacaoPedidoComponente> Componentes { get; set; }

        /// <summary>
        /// Aqui começa a setima aba
        /// </summary>      


        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContainer", Column = "CTP_NUMERO_CONTAINER", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBL", Column = "CTP_NUMERO_BL", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string NumeroBL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNavio", Column = "CTP_NUMERO_NAVIO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string NumeroNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_PORTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Porto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoEntregaImportacao", Column = "CTP_ENDERECO_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string EnderecoEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BairroEntregaImportacao", Column = "CTP_BAIRRO_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string BairroEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEPEntregaImportacao", Column = "CTP_CEP_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string CEPEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontoReferenciaImportacao", Column = "CTP_PONTO_REFERENCIA_ENTREGA_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string PontoReferenciaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ENTREGA_IMPORTACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeEntregaImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoArmazenamentoImportacao", Column = "CTP_DATA_VENCIMENTO_ARMAZENAMENTO_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoArmazenamentoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArmadorImportacao", Column = "CTP_ARMADOR_IMPORTACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string ArmadorImportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TipoTerminalImportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Prospeccao", Column = "PRO_CODIGO_PROSPECCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CRM.Prospeccao Prospeccao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_VALOR_FRETE_TERCEIRO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CotacaoPedidoImportacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_PEDIDO_IMPORTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoPedidoImportacao", Column = "CPI_CODIGO")]
        public virtual IList<CotacaoPedidoImportacao> CotacaoPedidoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CotacaoPedidoAutorizacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_PEDIDO_AUTORIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoPedidoAutorizacao", Column = "CPA_CODIGO")]
        public virtual IList<CotacaoPedidoAutorizacao> CotacaoPedidoAutorizacao { get; set; }

        public virtual string DescricaoSituacaoPedido
        {
            get
            {
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

        public virtual string DescricaoStatusCotacaoPedido
        {
            get
            {
                switch (this.StatusCotacaoPedido)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.EmAnalise:
                        return "Em Analise";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Fechada:
                        return "Fechada";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PerdaPorAnaliseCadastral:
                        return "Perda por analise cadastral";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PerdaPorDesistenciaDoServico:
                        return "Perda por desistencia do serviço";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PerdaPorInfraestrutura:
                        return "Perda por infraestrutura";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PerdaPorNaoJustificativaPeloCliente:
                        return "Perda por não justificativa pelo cliente";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PerdaPorPrazoDeResposta:
                        return "Perca por prazo de resposta";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PerdaPorPreco:
                        return "Perda por preço";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PerdaPorQualificacaoTecnica:
                        return "Perda por qualificação técnica";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.PertaPorQualificacaoDocumental:
                        return "Perda por qualificação documental";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Sondagem:
                        return "Sondagem";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoClienteCotacaoPedido
        {
            get
            {
                switch (this.TipoClienteCotacaoPedido)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteAtivo:
                        return "Cliente Ativo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteInativo:
                        return "Cliente Inativo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteNovo:
                        return "Cliente Novo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteProspect:
                        return "Cliente Prospect";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.GrupoPessoa:
                        return "Grupo de Pessoa";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string ClienteCotacaoPedido
        {
            get
            {
                switch (this.TipoClienteCotacaoPedido)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteAtivo:
                        return this.ClienteAtivo?.Descricao ?? string.Empty;
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteInativo:
                        return this.ClienteInativo?.Descricao ?? string.Empty;
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteNovo:
                        return this.ClienteNovo;
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.ClienteProspect:
                        return this.ClienteProspect?.Descricao ?? string.Empty;
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.GrupoPessoa:
                        return this.GrupoPessoas?.Descricao ?? string.Empty;
                    default:
                        return "";
                }
            }
        }

        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido Clonar()
        {
            return (Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido)this.MemberwiseClone();
        }

        public virtual bool Equals(CotacaoPedido other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
