using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Simulacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_BONIFICACAO", EntityName = "GrupoBonificacao", Name = "Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao", NameType = typeof(GrupoBonificacao))]
    public class GrupoBonificacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GRB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GRB_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "GRB_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GRB_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "GRB_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_BONIFICACAO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Veiculo> Veiculos { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return Situacao ? "Ativo" : "Inativo"; }
        }
    }
}
