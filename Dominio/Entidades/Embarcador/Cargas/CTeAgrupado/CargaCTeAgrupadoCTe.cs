namespace Dominio.Entidades.Embarcador.Cargas.CTeAgrupado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_AGRUPADO_CTE", EntityName = "CargaCTeAgrupadoCTe", Name = "Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe", NameType = typeof(CargaCTeAgrupadoCTe))]
    public class CargaCTeAgrupadoCTe: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeAgrupado", Column = "CCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado CargaCTeAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        public virtual string Descricao
        {
            get
            {
                return CTe?.Descricao ?? string.Empty;
            }
        }
    }
}
