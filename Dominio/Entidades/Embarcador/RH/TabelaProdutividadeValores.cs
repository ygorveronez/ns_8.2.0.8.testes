using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_PRODUTIVIDADE_VALORES", EntityName = "TabelaProdutividadeValores", Name = "Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores", NameType = typeof(TabelaProdutividadeValores))]
    public class TabelaProdutividadeValores : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPV_VALOR_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPV_VALOR_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaProdutividade", Column = "TAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaProdutividade TabelaProdutividade { get; set; }

        public virtual bool Equals(TabelaProdutividadeValores other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
