namespace Dominio.Entidades.Embarcador.GerenciamentoIrregularidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IRREGULARIDADE_DEFINICAO_TRATATIVAS", EntityName = "DefinicaoTratativasIrregularidade", Name = "Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade", NameType = typeof(DefinicaoTratativasIrregularidade))]
    public class DefinicaoTratativasIrregularidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PortfolioModuloControle", Column = "PMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle PortfolioModuloControle { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Irregularidade", Column = "IRR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_ATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativa { get; set; }

        public virtual string Descricao
        {
            get { return "Definição de Tratativas da Irregularidade: " + this.Irregularidade.Descricao; }
        }

    }
}
