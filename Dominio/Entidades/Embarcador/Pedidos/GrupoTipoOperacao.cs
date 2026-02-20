using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_TIPO_OPERACAO", DynamicUpdate = true, EntityName = "GrupoTipoOperacao", Name = "Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao", NameType = typeof(GrupoTipoOperacao))]
    public class GrupoTipoOperacao : EntidadeBase, IComparable<GrupoTipoOperacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GTO_DESCRICAO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "GTO_COR", TypeType = typeof(string), NotNull = true)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "GTO_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "GTO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "GTO_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        public virtual string DescricaoAtivo { get { return (this.Ativo) ? "Ativo" : "Inativo"; } }

        public virtual int CompareTo(GrupoTipoOperacao other)
        {
            if (other == null)
                return -1;

            return other.Codigo.CompareTo(Codigo);
        }
    }
}
