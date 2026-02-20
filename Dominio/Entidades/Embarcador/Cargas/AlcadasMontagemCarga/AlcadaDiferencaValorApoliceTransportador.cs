namespace Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARREGAMENTO_DIFERENCA_VALOR_APOLICE_TRANSPORTADOR", EntityName = "AlcadasMontagemCarga.AlcadaDiferencaValorApoliceTransportador", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaDiferencaValorApoliceTransportador", NameType = typeof(AlcadaDiferencaValorApoliceTransportador))]
    public class AlcadaDiferencaValorApoliceTransportador : RegraAutorizacao.Alcada<RegraAutorizacaoCarregamento, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_TOLERANCIA_VALOR_APOLICE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarregamento", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarregamento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
