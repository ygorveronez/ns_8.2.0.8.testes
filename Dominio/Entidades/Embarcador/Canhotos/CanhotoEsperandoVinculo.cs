using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    /// <summary>
    /// Entidade que representa a foto de um canhoto que está esperando para ser vinculado à uma nota fiscal.
    /// É utilisada quando o app manda apenas a foto de um canhoto.
    /// Ao gerar o canhoto com a nota fiscal, busca essa entidade para digitalizar o canhoto.
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANHOTO_ESPERANDO_VINCULO", EntityName = "CanhotoEsperandoVinculo", Name = "Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo", NameType = typeof(CanhotoEsperandoVinculo))]
    public class CanhotoEsperandoVinculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidNomeArquivo", Column = "CEV_GUID_NOME_ARQUIVO", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string GuidNomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CEV_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CEV_SITUACAO", TypeType = typeof(SituacaoCanhotoEsperandoVinculo), NotNull = false)]
        public virtual SituacaoCanhotoEsperandoVinculo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEV_CANHOTO_AVULSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CanhotoAvulso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "CEV_NUMERO_DOCUMENTO", Length = 150, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
    }
}
