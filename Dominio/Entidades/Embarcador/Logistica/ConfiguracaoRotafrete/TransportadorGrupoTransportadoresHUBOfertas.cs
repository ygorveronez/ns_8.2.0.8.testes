using Dominio.Entidades.Embarcador.Transportadores;

namespace Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRANSPORTADOR_GRUPO_TRANSPORTADOR_HUB_OFERTA", EntityName = "TransportadorGrupoTransportadoresHUBOfertas", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorGrupoTransportadoresHUBOfertas", NameType = typeof(TransportadorGrupoTransportadoresHUBOfertas))]
    public class TransportadorGrupoTransportadoresHUBOfertas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TGT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTransportadoresHUBOfertas", Column = "GTH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoTransportadoresHUBOfertas GrupoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao 
        {
            get { return $"{GrupoTransportador.Descricao} - {Empresa.Descricao}"; }
        }
    }
}
