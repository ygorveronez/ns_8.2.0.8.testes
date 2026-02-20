using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_FOTO", EntityName = "CargaEntregaFoto", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto", NameType = typeof(CargaEntregaFoto))]
    public class CargaEntregaFoto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEF_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEF_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CEF_LATITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CEF_LONGITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEF_DATA_ENVIO_IMAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEnvioImagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEF_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        public virtual string Descricao => "Foto - " + (this.CargaEntrega?.Descricao ?? string.Empty);
    }
}
