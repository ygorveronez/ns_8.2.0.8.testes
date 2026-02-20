namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_LOTE_CTE", EntityName = "FaturaLoteCTe", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe", NameType = typeof(FaturaLoteCTe))]
    public class FaturaLoteCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }
    }
}
