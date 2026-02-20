using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_TIPOS", EntityName = "CheckListTipo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo", NameType = typeof(CheckListTipo))]
    public class CheckListTipo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLT_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLT_ENVIAR_EMAIL_PARA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailParaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLT_ENVIAR_EMAIL_PARA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailParaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PerfisAcesso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECK_LIST_TIPO_PERFIL_ACESSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PerfilAcesso", Column = "PAC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> PerfisAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECK_LIST_TIPO_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Cliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Perguntas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECK_LIST_OPCOES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CheckListOpcoes", Column = "CLO_CODIGO")]
        public virtual ICollection<CheckListOpcoes> Perguntas { get; set; }

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
