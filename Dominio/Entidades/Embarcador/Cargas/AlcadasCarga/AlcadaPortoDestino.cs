namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARGA_PORTO_DESTINO", EntityName = "AlcadasCarga.AlcadaPortoDestino", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoDestino", NameType = typeof(AlcadaPortoDestino))]
    public class AlcadaPortoDestino : RegraAutorizacao.Alcada<RegraAutorizacaoCarga, Pedidos.Porto>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pedidos.Porto PropriedadeAlcada { get; set; }

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

