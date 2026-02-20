using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_FOTO_NOTA_FISCAL", EntityName = "CargaEntregaFotoNotaFiscal", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal", NameType = typeof(CargaEntregaFotoNotaFiscal))]
    public class CargaEntregaFotoNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFN_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFN_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFN_DATA_ENVIO_IMAGEM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEnvioImagem { get; set; }

        public virtual string Descricao => "Foto NF - " + Codigo.ToString();

    }
}
