using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA", EntityName = "CargaEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega", NameType = typeof(CargaEntrega))]
    public class CargaEntrega : EntidadeBase
    {
        public CargaEntrega() { }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime? DataCriacao { get; set; }

        /// <summary>
        /// Data da Previsão do Inicio da Entrega/Coleta Prevista  (descarregamento ou carregamento).
        /// </summary>
        /// 
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_ENTREGA_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevista { get; set; }

        /// <summary>
        /// Data da Previsão do Fim da Entrega/Coleta (descarregamento ou carregamento).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_FIM_PREVISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimPrevista { get; set; }

        /// <summary>
        /// Data da previsão da Entrega/Coleta vai sendo reajustada automaticamente conforme andamento via tracking.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_ENTREGA_REPROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReprogramada { get; set; }


        /// <summary>
        /// Data do início da entrega/coleta. Quando carregamento ou descarregamento é controlado, ela armazena a data de seu início.
        /// No app, em algumas circunstâncias, mesmo sem carregamento/descarregamento, também é pedido essa data de início (quando suínos são carregados, por exemplo)
        /// Também pode registrar o inicio da Entrega via Traking.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_INICIO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data da Confirmação da Entrega/Coleta. Quando é feito o controle de carregmento/descarregamento, sua data de TÉRMINO é armazenada aqui.
        /// Em coletas/entregas sem controle de carregmento/descarregamento, é a data em que o usuário do app finaliza a parada (igual a DataConfirmacao).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_FIM_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        /// <summary>
        /// É a data em que o usuário do app finaliza a parada ou a de quando ela é confirmada manualmente.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacao { get; set; }

        /// <summary>
        /// É a data em que a entrega é cofirmada, seja pelo app, manualmente, ou pelo sistema. Só é setada quando a entrega é confirmada e não pode mais ser alterada depois disso.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_ENTREGA_ORIGINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacaoEntregaOriginal { get; set; }

        /// <summary>
        /// É a data em que o usuário do app informa manualmente no evento de "Confirmação de Data e Hora do Canhoto" (Devops: #3678).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_CONFIRMACAO_ENTREGA_APP", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacaoApp { get; set; }

        /// <summary>
        /// Data da Rejeição da Entrega/Coleta
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_REJEITADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRejeitado { get; set; }


        /// <summary>
        /// Data que entra no raio da entrega. Pode ser setada pelo GerenciadorApp automaticamente baseado na posição do GPS do motorista
        /// ou explicitamente pelo app em alguns casos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_ENTRADA_RAIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntradaRaio { get; set; }

        /// <summary>
        /// tempo de permanecia, setado após o controle de entrega confirmar a entreda no raio e retorna somente na tela do controle de entrega, avaliar o motivo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_LIMITE_PERMANENCIA_RAIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLimitePermanenciaRaio { get; set; }

        /// <summary>
        /// Data de saída do raio.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_SAIDA_RAIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaRaio { get; set; }

        /// <summary>
        /// Data de avaliação do controle entrega
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_AVALIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAvaliacao { get; set; }

        /// <summary>
        /// Criada para replicar a data agendamento do pedido (caso exista)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CEN_SITUACAO", TypeType = typeof(SituacaoEntrega), NotNull = true)]
        public virtual SituacaoEntrega Situacao { get; set; }

        /// <summary>
        /// Guarda qual a origem do dado da situação atual da carga. Por exemplo, se foi confirmado no app, no ME, no portal do transportador, etc.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemSituacao", Column = "CEN_ORIGEM_SITUACAO", TypeType = typeof(OrigemSituacaoEntrega), NotNull = false)]
        public virtual OrigemSituacaoEntrega? OrigemSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemSituacaoFimViagem", Column = "CEN_ORIGEM_SITUACAO_FIM_VIAGEM", TypeType = typeof(OrigemSituacaoEntrega), NotNull = false)]
        public virtual OrigemSituacaoEntrega? OrigemSituacaoFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusPrazoEntrega", Column = "CEN_STATUS_PRAZO_ENTREGA", TypeType = typeof(StatusPrazoEntrega), NotNull = false)]
        public virtual StatusPrazoEntrega StatusPrazoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoOnTime", Column = "CEN_SITUACAO_ON_TIME", TypeType = typeof(SituacaoOnTime), NotNull = false)]
        public virtual SituacaoOnTime SituacaoOnTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_COLETA_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ColetaEquipamento { get; set; }

        /// <summary>
        /// Verdadeira quando a coleta é adicionar a carga no decorrer da viagem.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_COLETA_ADICIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ColetaAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_PESO_PEDIDOS_REENTREGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoPedidosReentrega { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "CEN_POSSUI_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool PossuiReentrega { get; set; }   

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_POSSUI_NOTA_COBERTURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNotaCobertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_CODIGO_RASTREIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoRastreio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Reentrega { get; set; }

        /// <summary>
        /// Quando a entrega possua pedidos de reentrega junto dos pedidos normais
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_POSSUI_PEDIDOS_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiPedidosReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_ORDEM_REALIZADA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DISTANCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_TEMPO_EXTRA_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoExtraEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

        /// <summary>
        /// Localidade para entrega, enquanto não houver cliente definido - Devops 1374.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_JUSTIFICATIVA_ENTREGA_FORA_RAIO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string JustificativaEntregaForaRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeFinalizada", Column = "CEN_LATITUDE_FINALIZADA", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LatitudeFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeFinalizada", Column = "CEN_LONGITUDE_FINALIZADA", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LongitudeFinalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_FINALIZADA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizadaManualmente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_RESPONSAVEL_FINALIZACAO_MANUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario ResponsavelFinalizacaoManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_RESPONSAVEL_AGENDAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario ResponsavelAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Repasse", Column = "CEN_REPASSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Repasse { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "CEN_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DevolucaoParcial", Column = "CEN_DEVOLUCAO_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DevolucaoParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarDiferencaDevolucao", Column = "CEN_NOTIFICAR_DIFERENCA_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarDiferencaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRetificacaoColeta", Column = "MRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta MotivoRetificacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoFalhaNotaFiscal", Column = "MFN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal MotivoFalhaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChamadoEmAberto", Column = "CEN_CHAMADO_EM_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChamadoEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_CARGA_ENTREGA_ORIGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int CargaEntregaOrigem { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaPedido", Column = "CEP_CODIGO")]
        public virtual ICollection<CargaEntregaPedido> Pedidos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaNotaFiscal", Column = "CEF_CODIGO")]
        public virtual ICollection<CargaEntregaNotaFiscal> NotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Fotos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_FOTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaFoto", Column = "CEF_CODIGO")]
        public virtual ICollection<CargaEntregaFoto> Fotos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ProdutosDevolucao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaProduto", Column = "CPP_CODIGO")]
        public virtual IList<CargaEntregaProduto> ProdutosDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Avaliacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_AVALIACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaAvaliacao", Column = "CEA_CODIGO")]
        public virtual IList<CargaEntregaAvaliacao> Avaliacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NFesDevolucao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_NFE_DEVOLUCAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaNFeDevolucao", Column = "CND_CODIGO")]
        public virtual IList<CargaEntregaNFeDevolucao> NFesDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Coleta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_FRONTEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Fronteira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_PARQUEAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Parqueamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_POSTO_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PostoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_AVALIACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? AvaliacaoGeral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_OBSERVACAO_AVALIACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoAvaliacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvaliacao", Column = "TMA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoAvaliacao MotivoAvaliacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DISTANCIA_ATE_DESTINO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? DistanciaAteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoFalhaGTA", Column = "MFG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA MotivoFalhaGTA { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DadosRecebedor", Column = "DRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor DadosRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_PERMITIR_ENTREGAR_MAIS_TARDE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEntregarMaisTarde { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeConfirmacaoChegada", Column = "CEN_LATITUDE_CONFIRMACAO_CHEGADA", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LatitudeConfirmacaoChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeConfirmacaoChegada", Column = "CEN_LONGITUDE_CONFIRMACAO_CHEGADA", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LongitudeConfirmacaoChegada { get; set; }

        //Tendencia do EmTransito, thread que monitora e atualiza essa variavel de acordo com DataPrevista e DataReprogramada
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tendencia", Column = "CEN_TENDENCIA", TypeType = typeof(TendenciaEntrega), NotNull = false)]
        public virtual TendenciaEntrega Tendencia { get; set; }

        //calculado no finalizar entrega, de acordo com coordenadas do cliente e coordenadas da entrega (LatitudeFinalizada, LongitudeFinalizada)
        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregueNoRaio", Column = "CEN_ENTREGA_NO_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregueNoRaio { get; set; }

        /// <summary>
        /// Se o motorista está a caminho da entrega ou não
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_MOTORISTA_A_CAMINHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaACaminho { get; set; }

        /// <summary>
        /// Utilizado até o momento pela integracao Ortec INICIO janela descarga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_INICIO_JANELA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? InicioJanela { get; set; }

        /// <summary>
        /// Utilizado até o momento pela integracao Ortec FIM janela descarga
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_FIM_JANELA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? FimJanela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoResponsavelAtrasoEntrega", Column = "TRA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega TipoResponsavelAtrasoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_OBSERVACAO_RESPONSAVEL_ATRASO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoResponsavelAtraso { get; set; }

        /// <summary>
        /// Nota dada para a coleta/entrega no app. Caso seja uma coleta, a nota é dada pelo produtor do produto.
        /// Caso seja uma entrega, a nota é dada pelo recebedor do produto.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_AVALIACAO_COLETA_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int AvaliacaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_JUSTIFICATIVA_ON_TIME", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string JustificativaOnTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_REENTREGA_MESMA_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReentregaEmMesmaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemCriacaoDataAgendamentoCargaEntrega", Column = "CEN_ORIGEM_CRIACAO_CARGA_ENTREGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacao? OrigemCriacaoDataAgendamentoCargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_ID_TRIZY", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_AGENDAMENTO_ENTREGA_TRANSPORTADOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamentoEntregaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemCriacaoDataAgendamentoEntregaTransportador", Column = "CEN_ORIGEM_CRIACAO_DATA_AGENDAMENTO_ENTREGA_TRANSPORTADOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacao? OrigemCriacaoDataAgendamentoEntregaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimiteDescarga", Column = "CEN_HORA_LIMITE_DESCARGA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string HoraLimiteDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaFinalizadaViaFinalizacaoMonitoramento", Column = "CEN_ENTREGA_FINALIZADA_VIA_FINALIZACAO_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregaFinalizadaViaFinalizacaoMonitoramento { get; set; }

        /// <summary>
        /// Quantidade de pacotes que o motorista confirmou que coletou via Super App Trizy.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_QUANTIDADE_PACOTES_COLETADOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadePacotesColetados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_PREVISAO_ENTREGA_TRANSPORTADOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntregaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaFinalizacaoAssincrona", Column = "EFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaEntregaFinalizacaoAssincrona CargaEntregaFinalizacaoAssincrona { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaEntrega", Column = "CEN_SENHA_ENTREGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaQualidade", Column = "CEQ_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaEntregaQualidade CargaEntregaQualidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaComNotasFiscaisReprocessada", Column = "CEN_ENTREGA_COM_NOTAS_REPROCESSADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregaComNotasFiscaisReprocessada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizadaNoPrazo", Column = "CEN_REALIZADA_NO_PRAZO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RealizadaNoPrazo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEN_DATA_PREVISAO_ENTREGA_AJUSTADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntregaAjustada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConfirmacaoEntrega", Column = "CEN_DATA_CONFIRMACAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProntoParaDescarregar", Column = "CEN_PRONTO_PARA_DESCARREGAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProntoParaDescarregar { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }

        public virtual double TempoDescargaEmMinutos
        {
            get
            {

                if (!this.DataPrevista.HasValue || !this.DataFimPrevista.HasValue)
                    return 0;

                double minutos = (DataFimPrevista.Value - DataPrevista.Value).TotalMinutes;

                if (minutos < 0)
                    minutos = 0;

                return minutos;

            }
        }

        public virtual int? DiferencaEntrega
        {
            get
            {
                return DiffTimeMinutes(this.DataFimPrevista, this.DataFim);
            }
        }

        public virtual bool EntregaEmOrdem
        {
            get
            {
                return Ordem == OrdemRealizada;
            }
        }

        public virtual SituacaoEntrega SituacaoParaMobile
        {
            get
            {
                if (this.Situacao == SituacaoEntrega.EmCliente)
                    return SituacaoEntrega.NaoEntregue;
                else
                    return this.Situacao;
            }
        }

        public virtual TipoCargaEntrega TipoCargaEntrega
        {
            get
            {
                if (this.Fronteira) return TipoCargaEntrega.Fronteira;
                if (this.Coleta) return TipoCargaEntrega.Coleta;
                return TipoCargaEntrega.Entrega;
            }
        }

        public virtual TipoColetaEntregaDevolucao TipoDevolucao
        {
            get
            {
                return DevolucaoParcial ? TipoColetaEntregaDevolucao.Parcial : TipoColetaEntregaDevolucao.Total;
            }
        }

        public virtual bool EstaPendente
        {
            get
            {
                return Situacao != SituacaoEntrega.Entregue && Situacao != SituacaoEntrega.Reentergue;
            }
        }

        public virtual string DescricaoEntregaNoPrazo
        {
            get
            {
                if (this.RealizadaNoPrazo.HasValue)
                {
                    return this.RealizadaNoPrazo.Value ? "Sim" : "Não";
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion Propriedades com Regras

        #region Métodos

        private int? DiffTimeMinutes(DateTime? previsto, DateTime? realizado)
        {
            if (!previsto.HasValue || !realizado.HasValue)
                return null;

            return (int)(previsto.Value - realizado.Value).TotalMinutes;
        }

        #endregion Métodos
    }
}