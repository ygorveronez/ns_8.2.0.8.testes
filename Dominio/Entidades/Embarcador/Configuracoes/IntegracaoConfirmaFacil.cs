using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CONFIRMAFACIL", EntityName = "IntegracaoConfirmaFacil", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil", NameType = typeof(IntegracaoConfirmaFacil))]
    public class IntegracaoConfirmaFacil : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIC_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIC_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CIC_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIC_SENHA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDCliente", Column = "CIC_IDCLIENTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IDCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDProduto", Column = "CIC_IDPRODUTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IDProduto { get; set; }
    }
}
