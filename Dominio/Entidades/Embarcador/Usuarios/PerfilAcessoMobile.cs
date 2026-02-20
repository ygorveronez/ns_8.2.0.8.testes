using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_ACESSO_MOBILE", EntityName = "PerfilAcessoMobile", Name = "Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile", NameType = typeof(PerfilAcessoMobile))]
    public class PerfilAcessoMobile : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_PERFIL_ADMINISTRADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PerfilAdministrador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModulosLiberados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERFIL_MODULO_MOBILE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAM_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MOD_CODIGO_MODULO", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> ModulosLiberados { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
