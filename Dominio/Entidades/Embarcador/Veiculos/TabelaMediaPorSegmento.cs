using Dominio.Entidades.Embarcador.RH;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_MEDIA_POR_SEGMENTO", EntityName = "TabelaMediaPorSegmento", Name = "Dominio.Entidades.Embarcador.Veiculos.MediaPorSegmento", NameType = typeof(TabelaMediaPorSegmento))]
    public class TabelaMediaPorSegmento: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMS_MEDIA_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal MediaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMS_MEDIA_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal MediaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMS_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]

        public virtual SegmentoVeiculo Segmento { get; set; }

        public virtual string Descricao { get; set; }

        public virtual bool Equals(TabelaMediaModeloPeso other)
        {
            return other.Codigo == Codigo;
        }
    }
}
