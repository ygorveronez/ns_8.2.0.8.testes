using Dominio.Entidades.Embarcador.Cargas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_SITUACAO_CARGA_JANELA_DESCARREGAMENTO ", EntityName = "HistoricoSituacaoCargaJanelaDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento", NameType = typeof(HistoricoSituacaoCargaJanelaDescarregamento))]
    public class HistoricoSituacaoCargaJanelaDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HSJ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaDescarregamento", Column = "CJD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaDescarregamento CargaJanelaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAnterior", Column = "HSJ_SITUACAO_ANTERIOR", TypeType = typeof(SituacaoCargaJanelaDescarregamento), NotNull = true)]
        public virtual SituacaoCargaJanelaDescarregamento SituacaoAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoNova", Column = "HSJ_SITUACAO_NOVA", TypeType = typeof(SituacaoCargaJanelaDescarregamento), NotNull = true)]
        public virtual SituacaoCargaJanelaDescarregamento SituacaoNova { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "HSJ_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAlteracao { get; set; }
        
    }
}
