using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_DATA_CARREGAMENTO", EntityName = "CargaDataCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaDataCarregamento", NameType = typeof(CargaDataCarregamento))]
    public class CargaDataCarregamento : EntidadeBase, IEquatable<CargaDataCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCargaEmbarcador", Column = "CDC_CODIGO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFilialEmbarcador", Column = "CDC_CODIGO_FILIAL_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoFilialEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoCliente", Column = "CDC_CODIGO_INTEGRACAO_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamento", Column = "CDC_DATA_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "CDC_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFaturamento", Column = "CDC_DATA_FATURAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaidaCentroCarregamento", Column = "CDC_DATA_SAIDA_CENTRO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaCentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual bool Equals(CargaDataCarregamento other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
