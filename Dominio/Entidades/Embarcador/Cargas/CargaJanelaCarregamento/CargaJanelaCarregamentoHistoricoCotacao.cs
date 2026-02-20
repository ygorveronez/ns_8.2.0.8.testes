using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_HISTORICO_COTACAO", EntityName = "CargaJanelaCarregamentoHistoricoCotacao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoHistoricoCotacao", NameType = typeof(CargaJanelaCarregamentoHistoricoCotacao))]
    public class CargaJanelaCarregamentoHistoricoCotacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JHC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamento CargaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "JHC_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JHC_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
