using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_NCM_ABASTECIMENTO", EntityName = "ProdutoNCMAbastecimento", Name = "Dominio.Entidades.Embarcador.Embarcador.ProdutoNCMAbastecimento", NameType = typeof(ProdutoNCMAbastecimento))]
    public class ProdutoNCMAbastecimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "PNA_NCM", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAbastecimento", Column = "PNA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento TipoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PNA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual bool Equals(GrupoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.NCM;
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string DescricaoTipoAbastecimento
        {
            get
            {
                switch (this.TipoAbastecimento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla:
                        return "ARLA";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel:
                        return "Combustível";
                    default:
                        return "";
                }
            }
        }
    }
}
