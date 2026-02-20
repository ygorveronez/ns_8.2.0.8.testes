using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_OCORRENCIA_COLETA_ENTREGA", EntityName = "PedidoOcorrenciaColetaEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega", NameType = typeof(PedidoOcorrenciaColetaEntrega))]
    public class PedidoOcorrenciaColetaEntrega : EntidadeBase, IEquatable<PedidoOcorrenciaColetaEntrega>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        /// <summary>
        /// Essa propriedade pode ser nula pois só será setada para eventos que ocorrem ao pedido quando ele tiver carga para ser utilizado a nível de informação, sempre verificar se é nulo e tomar cuidado ao utilizar.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Alvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOcorrencia", Column = "POC_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_PENDENTE_ENVIO_SMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteEnvioSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_RETORNO_ENVIO_SMS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string RetornoEnvioSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_PENDENTE_ENVIO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_PENDENTE_ENVIO_WHATSAPP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteEnvioWhatsApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_RETORNO_ENVIO_WHATSAPP", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string RetornoEnvioWhatsApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_RETORNO_ENVIO_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string RetornoEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteIntegracaoERP", Column = "POC_PENDENTE_INTEGRACAO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracaoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "POC_OBSERVACAO", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POC_PACOTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Pacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "POC_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EventoColetaEntrega", Column = "POC_EVENTO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega? EventoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "POC_LATITUDE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "POC_LONGITUDE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPosicao", Column = "POC_DATA_POSICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoRecalculada", Column = "POC_DATA_PREVISAO_RECALCULADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoRecalculada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DistanciaAteDestino", Column = "POC_DISTANCIA_DESTINO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal DistanciaAteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoPercurso", Column = "POC_TEMPO_PERCURSO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TempoPercurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOcorrencia", Column = "POC_OBSERVACAO_OCORRENCIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Natureza", Column = "POC_NATUREZA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Natureza { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoOcorrencia", Column = "POC_GRUPO_OCORRENCIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string GrupoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Razao", Column = "POC_RAZAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Razao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaFiscalDevolucao", Column = "POC_NOTA_FISCAL_DEVOLUCAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NotaFiscalDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SolicitacaoCliente", Column = "POC_SOLICITACAO_CLIENTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string SolicitacaoCliente { get; set; }

        public virtual string Descricao
        {
            get { return $"Ocorrência de coleta/entrega do pedido {Pedido.NumeroPedidoEmbarcador}";  }
        }

        public virtual bool Equals(PedidoOcorrenciaColetaEntrega other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
