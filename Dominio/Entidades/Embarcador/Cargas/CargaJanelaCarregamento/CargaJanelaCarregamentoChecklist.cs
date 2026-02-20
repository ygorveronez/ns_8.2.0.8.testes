using Dominio.Interfaces.Embarcador.Entidade;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_CHECKLIST", EntityName = "CargaJanelaCarregamentoChecklist", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist", NameType = typeof(CargaJanelaCarregamentoChecklist))]
    public class CargaJanelaCarregamentoChecklist : EntidadeCargaBase, IEntidade
    {
        public CargaJanelaCarregamentoChecklist() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento CargaJanelaCarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoChecklistCarga", Column = "ULTIMA_CARGA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga UltimaCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoChecklistCarga", Column = "PENULTIMA_CARGA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga PenultimaCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoChecklistCarga", Column = "ANTEPENULTIMA_CARGA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga AntepenultimaCarga { get; set; }

    }
}
