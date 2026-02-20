using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_MOTIVO_REJEICAO", EntityName = "MotivoRejeicaoColeta", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta", NameType = typeof(MotivoRejeicaoColeta))]
    public class MotivoRejeicaoColeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CMR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CMR_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Produtos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COLETA_MOTIVO_REJEICAO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> Produtos { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
