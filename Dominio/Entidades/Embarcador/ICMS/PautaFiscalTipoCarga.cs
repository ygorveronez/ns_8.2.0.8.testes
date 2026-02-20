using Dominio.Interfaces.Embarcador.Entidade;

namespace Dominio.Entidades.Embarcador.ICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAUTA_FISCAL_TIPO_CARGA", EntityName = "PautaFiscalTipoCarga", Name = "Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga", NameType = typeof(PautaFiscalTipoCarga))]
    public class PautaFiscalTipoCarga : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PautaFiscal", Column = "PFS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PautaFiscal PautaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoCarga { get; set; }

        public virtual string Descricao => $"{PautaFiscal.Descricao} - {TipoCarga.Descricao}";
    }
}

