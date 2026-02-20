using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_PERFIL_TABELA_VALOR", EntityName = "GrupoPessoasPerfilTabelaValor", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor", NameType = typeof(GrupoPessoasPerfilTabelaValor))]
    public class GrupoPessoasPerfilTabelaValor : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "GFT_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoa { get; set; }

        public virtual string Descricao
        {
            get { return ModeloVeicularCarga?.Descricao + " - R$ " + Valor.ToString("n2"); }
        }

        public virtual bool Equals(GrupoPessoasPerfilTabelaValor other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
