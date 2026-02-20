using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Mobile
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_USUARIO_MOBILE", EntityName = "UsuarioMobile", Name = "AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile", NameType = typeof(UsuarioMobile))]
    public class UsuarioMobile : EntidadeBase, IEquatable<AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "UMB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "UMB_CPF", TypeType = typeof(string), Length = 15, NotNull = true)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "UMB_NOME", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sessao", Column = "UMB_SESSAO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Sessao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSessao", Column = "UMB_DATA_SESSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataSessao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Celular", Column = "UMB_CELULAR", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Celular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "UMB_SENHA", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "UMB_CONTRA_SENHA", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string ContraSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoAPP", Column = "UMB_VERSAO_APP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string VersaoAPP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OneSignalPlayerId", Column = "UMB_ONE_SIGNAL_PLAYER_ID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string OneSignalPlayerId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoBloquearAcessoSimultaneo", Column = "UMB_NAO_BLOQUEAR_ACESSO_SIMULTANEO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoBloquearAcessoSimultaneo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "UMB_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativasAcessoInvalido", Column = "UMB_TENTATIVAS_ACESSO_INVALIDO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativasAcessoInvalido { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_USUARIO_MOBILE_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "UMB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "UsuarioMobileCliente", Column = "UBC_CODIGO")]
        public virtual IList<Dominio.Entidades.Mobile.UsuarioMobileCliente> Clientes { get; set; }


        public virtual bool Equals(UsuarioMobile other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
