using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_BALANCA_KIKI", EntityName = "IntegracaoBalancaKIKI", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBalancaKIKI", NameType = typeof(IntegracaoBalancaKIKI))]
    public class IntegracaoBalancaKIKI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIB_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIB_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [Obsolete("Foi criado sem a necessidade")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIB_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [Obsolete("Foi criado sem a necessidade")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIB_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }
    }
}