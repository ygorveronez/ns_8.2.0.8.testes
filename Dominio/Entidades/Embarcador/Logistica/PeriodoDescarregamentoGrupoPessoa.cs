namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_GRUPO_PESSOA", EntityName = "PeriodoDescarregamentoGrupoPessoa", Name = "Dominio.Entidades.Embarcador.Logistica", NameType = typeof(PeriodoDescarregamentoGrupoPessoa))]
    public class PeriodoDescarregamentoGrupoPessoa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PeriodoDescarregamento", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PeriodoDescarregamento PeriodoDescarregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }
        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}