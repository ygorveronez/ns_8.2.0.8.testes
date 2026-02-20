using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MENSAGEM_ALERTA_FLUXO_GESTAO_PATIO", EntityName = "MensagemAlertaFluxoGestaoPatio", Name = "Dominio.Entidades.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio", NameType = typeof(MensagemAlertaFluxoGestaoPatio))]
    public class MensagemAlertaFluxoGestaoPatio : Alertas.MensagemAlerta<FluxoGestaoPatio>
    {
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override FluxoGestaoPatio Entidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Mensagens", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MENSAGEM_ALERTA_FLUXO_GESTAO_PATIO_MENSAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MAL_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = true)]
        public override ICollection<string> Mensagens { get; set; }
    }
}
