using System;

namespace Dominio.Entidades.Embarcador.PedidoVenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_PRECO_VENDA", EntityName = "TabelaPrecoVenda", Name = "Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda", NameType = typeof(TabelaPrecoVenda))]
    public class TabelaPrecoVenda : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TPV_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "TPV_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigencia", Column = "TPV_DATA_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimVigencia", Column = "TPV_DATA_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TPV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "TPV_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProdutoTMS", Column = "GPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.GrupoProdutoTMS GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual bool Equals(TabelaPrecoVenda other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
