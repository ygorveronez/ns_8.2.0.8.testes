namespace Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_AVARIA_PALLET_MOTIVO_AVARIA", EntityName = "AlcadasAvaria.AlcadaMotivoAvaria", Name = "Dominio.Entidades.Embarcador.Pallets.AlcadasAvaria.AlcadaMotivoAvaria", NameType = typeof(AlcadaMotivoAvaria))]
    public class AlcadaMotivoAvaria : RegraAutorizacao.Alcada<RegraAutorizacaoAvaria, MotivoAvariaPallet>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvariaPallet", Column = "PMA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override MotivoAvariaPallet PropriedadeAlcada { get; set; }

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
