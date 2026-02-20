using System;

namespace Dominio.Entidades.Embarcador.Usuarios.Comissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_COMISSAO_TITULO", EntityName = "FuncionarioComissaoTitulo", Name = "Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo", NameType = typeof(FuncionarioComissaoTitulo))]
    public class FuncionarioComissaoTitulo : EntidadeBase, IEquatable<FuncionarioComissaoTitulo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "FCT_VALOR_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "FCT_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualImpostoFederal", Column = "FCT_PERCENTUAL_IMPOSTO_FEDERAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PercentualImpostoFederal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLiquido", Column = "FCT_VALOR_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFinal", Column = "FCT_VALOR_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FuncionarioComissao", Column = "FCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FuncionarioComissao FuncionarioComissao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Titulo?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(FuncionarioComissaoTitulo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
