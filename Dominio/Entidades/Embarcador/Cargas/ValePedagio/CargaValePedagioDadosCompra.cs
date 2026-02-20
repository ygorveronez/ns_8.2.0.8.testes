using System;

namespace Dominio.Entidades.Embarcador.Cargas.ValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_VALE_PEDAGIO_DADOS_COMPRA", EntityName = "CargaValePedagioDadosCompra", Name = "Dominio.Entidades.Embarcador.Cargas.CargaValePedagioDadosCompra", NameType = typeof(CargaValePedagioDadosCompra))]
    public class CargaValePedagioDadosCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoViagem", Column = "CVD_CODIGO_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFilialCliente", Column = "CVD_CODIGO_FILIAL_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoFilialCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProcessoCliente", Column = "CVD_CODIGO_PROCESSO_CLIENTE", TypeType = typeof(string), Length = 18, NotNull = false)]
        public virtual string CodigoProcessoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalPedagios", Column = "CVD_VALOR_TOTAL_PEDAGIOS", TypeType = typeof(decimal), Scale = 2, Precision = 11, NotNull = false)]
        public virtual decimal ValorTotalPedagios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CVD_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }
    }
}
