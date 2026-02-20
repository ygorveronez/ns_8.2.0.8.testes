using System;

namespace Dominio.Entidades.Embarcador.PedidoVenda
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VENDA_DIRETA_CONTESTACAO_ANEXO", EntityName = "VendaDiretaContestacaoAnexo", Name = "Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo", NameType = typeof(VendaDiretaContestacaoAnexo))]
    public class VendaDiretaContestacaoAnexo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaAnexo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VDX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VDX_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "VDX_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "VDX_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VendaDireta", Column = "VED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual VendaDireta VendaDireta { get; set; }

        public virtual bool Equals(VendaDiretaAnexo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
