using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO", EntityName = "CargaJanelaCarregamentoTransportadorHistorico", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico", NameType = typeof(CargaJanelaCarregamentoTransportadorHistorico))]
    public class CargaJanelaCarregamentoTransportadorHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JTH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportador", Column = "JCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoTransportador CargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "JTH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JTH_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "JTH_TIPO", TypeType = typeof(TipoCargaJanelaCarregamentoTransportadorHistorico), NotNull = true)]
        public virtual TipoCargaJanelaCarregamentoTransportadorHistorico Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRetiradaFilaCarregamento", Column = "FMR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento MotivoRetiradaFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "JTH_JUSTIFICATIVA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Justificativa { get; set; }

    }
}
