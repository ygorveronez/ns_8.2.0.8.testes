using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MENSAGEM_ALERTA_TABELA_FRETE_CLIENTE", EntityName = "MensagemAlertaTabelaFreteCliente", Name = "Dominio.Entidades.Embarcador.Frete.MensagemAlertaTabelaFreteCliente", NameType = typeof(MensagemAlertaTabelaFreteCliente))]
    public class MensagemAlertaTabelaFreteCliente : Alertas.MensagemAlerta<TabelaFreteCliente>
    {
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TabelaFreteCliente Entidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Mensagens", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MENSAGEM_ALERTA_TABELA_FRETE_CLIENTE_MENSAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MAL_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = true)]
        public override ICollection<string> Mensagens { get; set; }
    }
}
