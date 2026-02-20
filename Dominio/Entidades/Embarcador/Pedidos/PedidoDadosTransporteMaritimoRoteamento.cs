using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_DADOS_TRANSPORTE_MARITIMO_ROTEAMENTO", EntityName = "PedidoDadosTransporteMaritimoRoteamento", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento", NameType = typeof(PedidoDadosTransporteMaritimoRoteamento))]
    public class PedidoDadosTransporteMaritimoRoteamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoDadosTransporteMaritimo", Column = "CTM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PedidoDadosTransporteMaritimo DadosTransporteMaritimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRoteamento", Column = "TMR_CODIGO_ROTEAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoRoteamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoSCAC", Column = "TMR_CODIGO_SCAC", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoSCAC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FlagNavio", Column = "TMR_FLAG_NAVIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string FlagNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeNavio", Column = "TMR_NOME_NAVIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "TMR_NUMERO_VIAGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMR_PORTO_CARGA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PortoCargaData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortoCargaLocalizacao", Column = "TMR_PORTO_CARGA_LOCALIZACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PortoCargaLocalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMR_PORTO_DESCARGA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PortoDescargaData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortoDescargaLocalizacao", Column = "TMR_PORTO_DESCARGA_LOCALIZACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PortoDescargaLocalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRemessa", Column = "TMR_TIPO_REMESSA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoRemessa { get; set; }
    }
}
