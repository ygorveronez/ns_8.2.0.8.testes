namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMPONENTE_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR", EntityName = "CargaJanelaCarregamentoTransportadorComponente", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente", NameType = typeof(CargaJanelaCarregamentoTransportadorComponente))]
    public class CargaJanelaCarregamentoTransportadorComponente : EntidadeBase
    {
        public CargaJanelaCarregamentoTransportadorComponente() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportador", Column = "JCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador CargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JTC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JTC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

    }
}