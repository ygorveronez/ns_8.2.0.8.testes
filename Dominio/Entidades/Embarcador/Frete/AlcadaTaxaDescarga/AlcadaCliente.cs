namespace Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_TAXA_DESCARGA_CLIENTE", EntityName = "AlcadasTaxaDescarga.AlcadaCliente", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasLeilao.AlcadaCliente", NameType = typeof(AlcadaCliente))]
    public class AlcadaCliente : RegraAutorizacao.Alcada<RegraAutorizacaoTaxaDescarga, Cliente>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cliente PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTaxaDescarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTaxaDescarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
