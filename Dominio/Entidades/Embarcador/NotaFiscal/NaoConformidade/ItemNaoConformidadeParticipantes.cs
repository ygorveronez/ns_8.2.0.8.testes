
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ITEM_NAO_CONFORMIDADE_PARTICIPANTE", EntityName = "ItemNaoConformidadeParticipantes", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes", NameType = typeof(ItemNaoConformidadeParticipantes))]
    public class ItemNaoConformidadeParticipantes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ItemNaoConformidade", Column = "INC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade CodigoItemNaoConformidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Participante", Column = "INP_PARTICIPANTE", TypeType = typeof(TipoParticipante), NotNull = true)]
        public virtual TipoParticipante Participante { get; set; }
    }
}
