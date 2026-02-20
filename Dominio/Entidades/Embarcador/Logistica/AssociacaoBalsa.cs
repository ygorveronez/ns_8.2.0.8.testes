using Dominio.Entidades.Embarcador.Pedidos;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_ASSOCIACAO_BALSA", EntityName = "AssociacaoBalsa", Name = "Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa", NameType = typeof(AssociacaoBalsa))]
    public class AssociacaoBalsa : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ABA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Planilha", Column = "ABA_PLANILHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Planilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoPlanilha", Column = "ABA_CAMINHO_PLANILHA", Type = "StringClob", NotNull = false)]
        public virtual string CaminhoPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLinhas", Column = "ABA_QTDE_LINHAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeLinhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "ABA_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ABA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "ABA_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioProcessamento", Column = "ABA_DATA_INICIO_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimProcessamento", Column = "ABA_DATA_FIM_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalSegundosProcessamento", Column = "ABA_TOTAL_SEGUNDOS_PROCESSAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? TotalSegundosProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Booking", Column = "ABA_BOOKING", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Booking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROC_ENTIDADE_PAI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntidadePai { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio Navio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO_TRANSBORDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio NavioTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_TRANSBORDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo ContainerTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO_BALSA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio Balsa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroControle", Column = "ABA_NUMERO_CONTROLE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "AssociacaoBalsas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ASSOCIACAO_BALSA_ASSOCIACAO_BALSA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ABA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AssociacaoBalsa", Column = "ABA_CODIGO_FILHO")]
        public virtual ICollection<AssociacaoBalsa> AssociacaoBalsas { get; set; }

        public virtual TimeSpan? Tempo()
        {
            if (TotalSegundosProcessamento != null)
                return TimeSpan.FromSeconds(TotalSegundosProcessamento.Value);
            else
                return null;
        }
    }
}
