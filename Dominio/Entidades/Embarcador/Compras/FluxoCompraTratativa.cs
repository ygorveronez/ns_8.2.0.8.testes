using System;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_COMPRA_TRATATIVA", EntityName = "FluxoCompraTratativa", Name = "Dominio.Entidades.Embarcador.Compras.FluxoCompraTratativa", NameType = typeof(FluxoCompraTratativa))]
    public class FluxoCompraTratativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "FCT_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoLivre", Column = "FCT_TEXTO_LIVRE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string TextoLivre { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoCompra", Column = "FCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoCompra FluxoCompra { get; set; }

        public virtual string RegistroFormatado
        {
            get { return Usuario.Nome + " (" + Data.ToDateString() + "): " + TextoLivre; }
        }
    }
}
