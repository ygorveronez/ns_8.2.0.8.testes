namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DIARIA_AUTOMATICA_COMPOSICAO_FRETE", EntityName = "DiariaAutomaticaComposicaoFrete", Name = "Dominio.Entidades.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete", NameType = typeof(DiariaAutomaticaComposicaoFrete))]
    public class DiariaAutomaticaComposicaoFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DiariaAutomatica", Column = "DAU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica DiariaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParametro", Column = "CCF_TIPO_PARAMETRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete TipoParametro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "CCF_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CCF_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_FORMULA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Formula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_DESCRICAO_COMPONENTE", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string DescricaoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoComponente", Column = "CCF_CODIGO_COMPONENTE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_VALORES_FORMULA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ValoresFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCalculado", Column = "CCF_VALOR_CALCULADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCalculado { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

    }
}
