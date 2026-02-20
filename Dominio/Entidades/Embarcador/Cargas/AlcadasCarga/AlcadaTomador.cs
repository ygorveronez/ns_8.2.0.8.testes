namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARGA_TOMADOR", EntityName = "AlcadasCarga.AlcadaTomador", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTomador", NameType = typeof(AlcadaTomador))]
    public class AlcadaTomador : RegraAutorizacao.Alcada<RegraAutorizacaoCarga, Cliente>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cliente PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}

