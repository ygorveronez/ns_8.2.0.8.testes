using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CEBRACE", EntityName = "IntegracaoCebrace", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCebrace", NameType = typeof(IntegracaoCebrace))]
    public class IntegracaoCebrace : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIC_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [Obsolete("Migrado para URLIntegracao", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLConfirmarRecebimento", Column = "CIC_URL_CONFIRMAR_RECEBIMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConfirmarRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIC_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIC_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiKey", Column = "CIC_API_KEY", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ApiKey { get; set; }
    }
}
