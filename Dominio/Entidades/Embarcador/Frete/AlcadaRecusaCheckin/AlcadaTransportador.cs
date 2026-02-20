namespace Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_RECUSA_CHECKIN_TRANSPORTADOR", EntityName = "AlcadaRecusaCheckin.AlcadaTransportador", Name = "Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaTransportador", NameType = typeof(AlcadaTransportador))]
    public class AlcadaTransportador : RegraAutorizacao.Alcada<RegraAutorizacaoRecusaCheckin, Empresa>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Empresa PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoRecusaCheckin", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoRecusaCheckin RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
