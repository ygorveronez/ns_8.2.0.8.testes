namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_DESCARREGAMENTO_COMPOSICAO_HORARIO_DETALHE", EntityName = "CargaJanelaDescarregamentoComposicaoHorarioDetalhe", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe", NameType = typeof(CargaJanelaDescarregamentoComposicaoHorarioDetalhe))]
    public class CargaJanelaDescarregamentoComposicaoHorarioDetalhe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaDescarregamentoComposicaoHorario", Column = "DCH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaDescarregamentoComposicaoHorario ComposicaoHorario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "DCD_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "DCD_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
