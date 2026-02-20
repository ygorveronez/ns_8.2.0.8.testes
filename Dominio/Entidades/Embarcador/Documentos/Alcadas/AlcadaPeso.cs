namespace Dominio.Entidades.Embarcador.Documentos.Alcadas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_DOCUMENTO_PESO", EntityName = "Alcadas.AlcadaPeso", Name = "Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaPeso", NameType = typeof(AlcadaPeso))]
    public class AlcadaPeso : RegraAutorizacao.Alcada<RegraAutorizacaoDocumento, decimal>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.ToString("n2"); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public override decimal PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoDocumento", Column = "RAD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoDocumento RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
