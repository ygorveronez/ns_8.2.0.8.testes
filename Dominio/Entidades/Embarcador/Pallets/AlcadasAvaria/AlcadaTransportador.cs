namespace Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_AVARIA_PALLET_TRANSPORTADOR", EntityName = "AlcadasAvaria.AlcadaTransportador", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaTransportador", NameType = typeof(AlcadaTransportador))]
    public class AlcadaTransportador : RegraAutorizacao.Alcada<RegraAutorizacaoAvaria, Empresa>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoAvaria", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoAvaria RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
