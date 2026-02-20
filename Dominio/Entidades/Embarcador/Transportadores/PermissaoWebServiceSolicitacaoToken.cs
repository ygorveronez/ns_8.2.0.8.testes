namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMISSAO_WEB_SERVICE_SOLICITACAO_TOKEN", EntityName = "PermissaoWebServiceSolicitacaoToken", Name = "Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken", NameType = typeof(PermissaoWebServiceSolicitacaoToken))]
    public class PermissaoWebServiceSolicitacaoToken : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PWS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMetodo", Column = "PWS_NOME_METODO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string NomeMetodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RequisicoesMinuto", Column = "PWS_REQUISICOES_POR_MINUTO", TypeType = typeof(int), NotNull = true)]
        public virtual int RequisicoesMinuto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeRequisicoes", Column = "PWS_QUANTIDADE_REQUISICOES", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeRequisicoes { get; set; }

    }
}

