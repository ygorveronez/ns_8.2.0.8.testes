using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PACOTE", EntityName = "Pacote", Name = "Dominio.Entidades.Embarcador.Cargas.Pacote", NameType = typeof(Pacote))]
    public class Pacote : EntidadeBase
    {
        public Pacote() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_CONTRATANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Contratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "PCT_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoOrigem", Column = "PCT_CODIGO_INTEGRACAO_ORIGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoDestino", Column = "PCT_CODIGO_INTEGRACAO_DESTINO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoContratante", Column = "PCT_CODIGO_INTEGRACAO_CONTRATANTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoContratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogKey", Column = "PCT_LOG_KEY", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string LogKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cubagem", Column = "PCT_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Cubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PCT_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        // Esse campo é utilizado apenas para a importação de pacotes através da tela de importação de pedidos ;)
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "PCT_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemIntegracao", Column = "PCT_MENSAGEM_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string MensagemIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiroXML", Column = "CTX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML CTeTerceiroXML { get; set; }


        public virtual string Descricao
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.LogKey) ? this.LogKey : string.Empty;
            }
        }
    }
}
