namespace Dominio.Entidades.Embarcador.Documentos.Alcadas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_DOCUMENTO_MOTIVO_REJEICAO", EntityName = "Alcadas.AlcadaMotivoRejeicao", Name = "Dominio.Entidades.Embarcador.Documentos.Alcadas.AlcadaMotivoRejeicao", NameType = typeof(AlcadaMotivoRejeicao))]
    public class AlcadaMotivoRejeicao : RegraAutorizacao.Alcada<RegraAutorizacaoDocumento, ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return ((int)PropriedadeAlcada).ToString(); }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_MOTIVO_REJEICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento), NotNull = true)]
        public override ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento PropriedadeAlcada { get; set; }

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
