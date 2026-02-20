using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_INTEGRACAO_AUTENTICACAO", EntityName = "TipoIntegracaoAutenticacao", Name = "Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao", NameType = typeof(TipoIntegracaoAutenticacao))]
    public class TipoIntegracaoAutenticacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TIA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "TIA_TOKEN", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataExpiracao", Column = "TIA_DATA_EXPIRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataExpiracao { get; set; }

        public virtual bool isAtivo => !string.IsNullOrWhiteSpace(Token) && DataExpiracao.HasValue && DataExpiracao.Value > DateTime.Now;
    }
}
