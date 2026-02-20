using Dominio.Interfaces.Embarcador.Entidade;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_CHECKLIST", EntityName = "CargaJanelaCarregamentoTransportadorChecklist", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist", NameType = typeof(CargaJanelaCarregamentoTransportadorChecklist))]
    public class CargaJanelaCarregamentoTransportadorChecklist : EntidadeCargaBase, IEntidade
    {
        public CargaJanelaCarregamentoTransportadorChecklist() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportador", Column = "CJT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador CargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChecklist", Column = "CTC_DATA_CHECKLIST", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataChecklist { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegimeLimpeza", Column = "CTC_REGIME_LIMPEZA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumRegimeLimpeza), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumRegimeLimpeza RegimeLimpeza { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemCargaChecklist", Column = "CTC_ORDEM_CARGA_CHECKLIST", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrdemCargaChecklist), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrdemCargaChecklist OrdemCargaChecklist { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChecklistVisualizado", Column = "CTC_CHECKLIST_VISUALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChecklistVisualizado { get; set; }

        public virtual string Descricao
        {
            get { return this.Codigo.ToString(); }
        }
    }
}
