using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MENSAGEM_ALERTA", EntityName = "MensagemAlertaCarga", Name = "Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga", NameType = typeof(MensagemAlertaCarga))]
    public class MensagemAlertaCarga : Alertas.MensagemAlerta<Carga>
    {
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Entidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Mensagens", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MENSAGEM_ALERTA_MENSAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MAL_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = true)]
        public override ICollection<string> Mensagens { get; set; }
    }
}
