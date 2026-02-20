namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AVERBACOES_FECHAMENTO", EntityName = "AverbacoesFechamento", Name = "Dominio.Entidades.Embarcador.Seguros.AverbacoesFechamento", NameType = typeof(AverbacoesFechamento))]
    public class AverbacoesFechamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AFE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoAverbacao", Column = "FAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoAverbacao FechamentoAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AverbacaoCTe", Column = "AVE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.AverbacaoCTe AverbacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFE_ADICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Adicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFE_IOF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal IOF { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Fechamento - " + this.FechamentoAverbacao.Descricao;
            }
        }
    }
}
