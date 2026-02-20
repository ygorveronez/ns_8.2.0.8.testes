using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PRODUTO", EntityName = "GrupoProduto", Name = "Dominio.Entidades.Embarcador.Embarcador.GrupoProduto", NameType = typeof(GrupoProduto))]
    public class GrupoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoGrupoProdutoEmbarcador", Column = "GRP_CODIGO_GRUPO_PRODUTO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoGrupoProdutoEmbarcador { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePorCaixa", Column = "GRP_QUANTIDADE_POR_CAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadePorCaixa{ get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "PorcentagemCorrecao", Column = "GRP_PORCENTAGEM_CORRECAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PorcentagemCorrecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GRP_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "GRP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio", Column = "GRP_LISTAR_PRODUTOS_DESTE_GRUPO_NO_RELATORIO_DE_SINTESE_DE_MATERIAIS_DO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornarNoChecklist", Column = "GRP_RETORNAR_NO_CHECKLIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarNoChecklist { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirCarregamento", Column = "GRP_NAO_PERMITIR_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirCarregamento { get; set; }

        public virtual bool Equals(GrupoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
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
    }
}
