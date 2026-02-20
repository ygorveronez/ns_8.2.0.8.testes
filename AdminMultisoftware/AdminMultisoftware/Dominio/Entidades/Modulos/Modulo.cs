using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Modulos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODULO", EntityName = "Modulo", Name = "AdminMultisoftware.Dominio.Entidades.Modulos.Modulo", NameType = typeof(Modulo))]
    public class Modulo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Modulo", Column = "MOD_MODULO_PAI", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Modulos.Modulo ModuloPai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoModulo", Column = "MOD_CODIGO_MODULO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoModulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "MOD_SEQUENCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int Sequencia { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MOD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_TRANSLATION_RESOURCE_PATH", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TranslationResourcePath { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Icone", Column = "MOD_ICONE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Icone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MOD_ICONE_NOVO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string IconeNovo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MOD_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmHomologacao", Column = "MOD_EM_HOMOLOGACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmHomologacao { get; set; }

        //[NHibernate.Mapping.Attributes.Bag(0, Name = "ModulosFilho", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODULO")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "MOD_MODULO_PAI")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Modulo", Column = "MOD_CODIGO")]
        //public virtual IList<Dominio.Entidades.Modulos.Modulo> ModulosFilho { get; set; }
        
        //[NHibernate.Mapping.Attributes.Bag(0, Name = "Formularios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FORMULARIO")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "MOD_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Formulario", Column = "FOR_CODIGO")]
        //public virtual IList<Dominio.Entidades.Modulos.Formulario> Formularios { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposServicosMultisoftware", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODULO_SERVICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOD_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "TIP_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = true)]
        public virtual ICollection<Dominio.Enumeradores.TipoServicoMultisoftware> TiposServicosMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ClientesModulo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteModulo", Column = "CMO_CODIGO")]
        public virtual IList<Dominio.Entidades.Modulos.ClienteModulo> ClientesModulo { get; set; }

        //[NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODULO_CLIENTE")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "MOD_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CODIGO")]
        //public virtual ICollection<Dominio.Entidades.Pessoas.Cliente> Clientes { get; set; }


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
