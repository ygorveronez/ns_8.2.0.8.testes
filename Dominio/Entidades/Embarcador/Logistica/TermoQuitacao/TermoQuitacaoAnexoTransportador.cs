namespace Dominio.Entidades.Embarcador.Logistica.TermoQuitacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TERMO_QUITACAO_ANEXOS_TRANSPORTADOR", EntityName = "TermoQuitacaoAnexoTransportador", Name = "Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacaoAnexoTransportador", NameType = typeof(TermoQuitacaoAnexoTransportador))]
    public class TermoQuitacaoAnexoTransportador : Anexo.Anexo<TermoQuitacao>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacao", Column = "TEQ_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TermoQuitacao EntidadeAnexo { get; set; }

        #endregion
    }
}
