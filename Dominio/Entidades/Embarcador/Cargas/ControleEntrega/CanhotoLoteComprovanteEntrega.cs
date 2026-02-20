/*
 * Faz a ligação entre um LoteComprovanteEntrega e todos os canhotos que ele agrupa. Também identifica de qual CargaEntregaLoteComprovanteEntrega saiu cada canhoto.
 */

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANHOTO_LOTE_COMPROVANTE_ENTREGA", EntityName = "CanhotoLoteComprovanteEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CanhotoLoteComprovanteEntrega", NameType = typeof(CanhotoLoteComprovanteEntrega))]
    public class CanhotoLoteComprovanteEntrega : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteComprovanteEntrega", Column = "LCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual LoteComprovanteEntrega LoteComprovanteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Canhoto", Column = "CNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Canhotos.Canhoto Canhoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaLoteComprovanteEntrega", Column = "CEL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega CargaEntregaLoteComprovanteEntrega { get; set; }

    }
}