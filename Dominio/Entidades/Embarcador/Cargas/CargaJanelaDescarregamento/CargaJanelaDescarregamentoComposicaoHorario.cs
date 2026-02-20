using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_DESCARREGAMENTO_COMPOSICAO_HORARIO", EntityName = "CargaJanelaDescarregamentoComposicaoHorario", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario", NameType = typeof(CargaJanelaDescarregamentoComposicaoHorario))]
    public class CargaJanelaDescarregamentoComposicaoHorario : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaDescarregamento", Column = "CJD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaDescarregamento CargaJanelaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "DCH_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        #endregion Propriedades

        #region Construtores

        public CargaJanelaDescarregamentoComposicaoHorario()
        {
            DataCriacao = DateTime.Now;
        }

        #endregion Construtores
    }
}
