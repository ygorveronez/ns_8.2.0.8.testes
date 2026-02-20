using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENTREGA_PEDIDO", EntityName = "EntregaPedido", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido", NameType = typeof(EntregaPedido))]
    public class EntregaPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ENP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoEntregaEtapas", Column = "GEE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoEntregaEtapas Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_PENDENTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_SITUACAO", TypeType = typeof(SituacaoEntregaPedido), NotNull = false)]
        public virtual SituacaoEntregaPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_DATA_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_DATA_REJEITADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRejeitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_AVALIACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? Avaliacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_DADOS_SALVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DadosSalvos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_DATA_FEEDBACK", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFeedback { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_TOKEN", TypeType = typeof(string), Length = 32, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_NOME_RECEBEDOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_DOCUMENTO_RECEBEDOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DocumentoRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_LATITUDE", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ENP_LONGITUDE", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Fotos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ENTREGA_PEDIDO_FOTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ENP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EntregaPedidoFoto", Column = "EPF_CODIGO")]
        public virtual ICollection<EntregaPedidoFoto> Fotos { get; set; }

        public virtual string DescricaoSituacao => Situacao.ObterDescricao();

        public virtual string Descricao => this.Pedido?.NumeroPedidoEmbarcador ?? string.Empty;

        public virtual string DataConfirmacao
        {
            get
            {
                if (Situacao == SituacaoEntregaPedido.Entregue)
                    return DataEntrega?.ToString("dd/MM/yyyy HH:mm:ss");
                else if (Situacao == SituacaoEntregaPedido.Rejeitado)
                    return DataRejeitado?.ToString("dd/MM/yyyy HH:mm:ss");
                else 
                    return "";
            }
        }

        private int? DiffTimeMinutes(DateTime? previsto, DateTime? realizado)
        {
            if (!previsto.HasValue || !realizado.HasValue)
                return null;

            return (int)(previsto.Value - realizado.Value).TotalMinutes;
        }

        public virtual int? DiferencaEntrega
        {
            get
            {
                return DiffTimeMinutes(this.DataPrevisaoEntrega, this.DataEntrega);
            }
        }
    }
}
