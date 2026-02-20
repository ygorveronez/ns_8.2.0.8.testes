namespace Dominio.Entidades.Embarcador.Cargas.ValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_VALE_PEDAGIO_ROTA", EntityName = "CargaValePedagioRota", Name = "Dominio.Entidades.Embarcador.Cargas.CargaValePedagioRota", NameType = typeof(CargaValePedagioRota))]
    public class CargaValePedagioRota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoRota", Column = "CVP_DESCRICAO_ROTA", TypeType = typeof(string), Length = 64, NotNull = false)]
        public virtual string DescricaoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigosPracaSemParar", Column = "CVP_CODIGOS_PRACA_SEM_PARAR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CodigosPracaSemParar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoValePedagio", Column = "CVP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio CargaValePedagio { get; set; }

        public virtual CargaValePedagioRota Clonar()
        {
            return (CargaValePedagioRota)this.MemberwiseClone();
        }
    }
}
