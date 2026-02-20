using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Filiais;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_COLETA", EntityName = "AgendamentoColeta", Name = "Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta", NameType = typeof(AgendamentoColeta))]
    public class AgendamentoColeta : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "REM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_CODIGO_CONTROLE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        /// <summary>
        /// Senha sequencial para compor a propriedade Senha.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_SENHA_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int? SenhaSequencial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "DES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "ACT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario ResponsavelConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataColeta", Column = "ACO_DATA_COLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "ACO_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicioFaixa", Column = "ACO_HORA_INICIO_FAIXA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicioFaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimiteFaixa", Column = "ACO_HORA_LIMITE_FAIXA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraLimiteFaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "ACO_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendamento", Column = "ACO_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ACO_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 300)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailSolicitante", Column = "ACO_SOLICITANTE", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string EmailSolicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDistribuicao", Column = "CDI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDistribuicao CDDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_ERRO_BUSCAR_SENHA_AUTOMATICAMENTE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string ErroBuscarSenhaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "ACO_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalVolumes", Column = "ACO_VALOR_TOTAL_VOLUMES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalVolumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "ACO_PESO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_CARGA_PERIGOSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPerigosa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_APENAS_GERAR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApenasGerarPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACO_AGENDAMENTO_PAI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AgendamentoPai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAgendamentoColeta", Column = "ACO_ETAPA", TypeType = typeof(EtapaAgendamentoColeta), NotNull = true)]
        public virtual EtapaAgendamentoColeta EtapaAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ACO_SITUACAO", TypeType = typeof(SituacaoAgendamentoColeta), NotNull = false)]
        public virtual SituacaoAgendamentoColeta? Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "ACO_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Reboque", Column = "ACO_REBOQUE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Reboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "ACO_PLACA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motorista", Column = "ACO_MOTORISTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TransportadorManual", Column = "ACO_TRANSPORTADOR_MANUAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TransportadorManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_VEICULO_SELECIONADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo VeiculoSelecionado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_REBOQUE_SELECIONADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo ReboqueSelecionado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_SELECIONADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario MotoristaSelecionado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_SOLICITANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Categoria", Column = "CTG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Categoria Categoria { get; set; }

        [Obsolete("Migrado para uma String")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida UnidadeDeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedida", Column = "ACO_UNIDADE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AGENDAMENTO_COLETA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AgendamentoColetaPedido", Column = "ACP_CODIGO")]
        public virtual ICollection<AgendamentoColetaPedido> Pedidos { get; set; }

        public virtual string Descricao => $"Agendamento Coleta {Codigo}";

        public virtual string DescricaoSituacao => Situacao?.ObterDescricao() ?? string.Empty;

        public virtual string DescricaoEtapa => EtapaAgendamentoColeta.ObterDescricao();

        public virtual bool Cancelado => Situacao.HasValue ? SituacaoAgendamentoColetaHelper.Canceladas.Contains(Situacao.Value) : false;
    }
}
