using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONITORAMENTO_EVENTO_TIPO_INTEGRACAO", EntityName = "MonitoramentoEventoTipoIntegracao", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao", NameType = typeof(MonitoramentoEventoTipoIntegracao))]
    public class MonitoramentoEventoTipoIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ETI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MonitoramentoEvento", Column = "MEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MonitoramentoEvento MonitoramentoEvento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        public virtual string Descricao { get { return TipoIntegracaoHelper.ObterDescricao(this.TipoIntegracao.Tipo);} }
    }
}
