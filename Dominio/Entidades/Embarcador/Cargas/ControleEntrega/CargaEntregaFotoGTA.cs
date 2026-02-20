using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_FOTO_GTA", EntityName = "CargaEntregaFotoGTA", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA", NameType = typeof(CargaEntregaFotoGTA))]
    public class CargaEntregaFotoGTA : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFG_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFG_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEF_DATA_ENVIO_IMAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEnvioImagem { get; set; }

        public virtual string Descricao => "Foto - " + (this.CargaEntrega?.Descricao ?? string.Empty);

    }
}
