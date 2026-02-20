/*
 * Extende a entidade CargaEntrega, ligando ela em um LoteComprovante e sorbeescrevendo dados de geolocalização e Dados de Recebedor. (Ver tela de Lote de Comprovante de Entregas)
 */

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_LOTE_COMPROVANTE_ENTREGA", EntityName = "CargaEntregaLoteComprovanteEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega", NameType = typeof(CargaEntregaLoteComprovanteEntrega))]
    public class CargaEntregaLoteComprovanteEntrega : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteComprovanteEntrega", Column = "LCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual LoteComprovanteEntrega LoteComprovanteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CEL_LATITUDE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CEL_LONGITUDE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DadosRecebedor", Column = "DRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor DadosRecebedor { get; set; }

    }
}