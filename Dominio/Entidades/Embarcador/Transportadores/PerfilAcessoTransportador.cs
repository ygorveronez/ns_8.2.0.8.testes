using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_ACESSO_TRANSPORTADOR", EntityName = "PerfilAcessoTransportador", Name = "Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador", NameType = typeof(PerfilAcessoTransportador))]
    public class PerfilAcessoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAT_PERFIL_ADMINISTRADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PerfilAdministrador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModulosLiberados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERFIL_TRANSPORTADOR_MODULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAT_CODIGO")]
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
