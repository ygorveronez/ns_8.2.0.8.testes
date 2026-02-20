using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CARREGAMENTO", EntityName = "AprovacaoAlcadaCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento", NameType = typeof(AprovacaoAlcadaCarregamento))]
    public class AprovacaoAlcadaCarregamento : RegraAutorizacao.AprovacaoAlcada<MontagemCarga.CarregamentoSolicitacao, RegraAutorizacaoCarregamento>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CarregamentoSolicitacao", Column = "CRS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override MontagemCarga.CarregamentoSolicitacao OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarregamento", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarregamento RegraAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_GUID_CARREGAMENTO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string GuidCarregamento { get; set; }

        #endregion

        #region Métodos Sobrescritos

        public override bool IsPermitirAprovacaoOuReprovacao(int codigoUsuario)
        {
            return (OrigemAprovacao.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado) && base.IsPermitirAprovacaoOuReprovacao(codigoUsuario);
        }

        #endregion
    }
}
