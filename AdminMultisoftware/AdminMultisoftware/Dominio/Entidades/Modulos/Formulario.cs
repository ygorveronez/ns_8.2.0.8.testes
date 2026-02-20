using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Dominio.Entidades.Modulos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FORMULARIO", EntityName = "Formulario", Name = "AdminMultisoftware.Dominio.Entidades.Modulos.Formulario", NameType = typeof(Formulario))]
    public class Formulario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FOR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Modulo", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Modulos.Modulo Modulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFormulario", Column = "FOR_CODIGO_FORMULARIO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoFormulario { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "FOR_SEQUENCIA", TypeType = typeof(int), NotNull = true)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FOR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmHomologacao", Column = "FOR_EM_HOMOLOGACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmHomologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoPagina", Column = "FOR_CAMINHO_PAGINA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string CaminhoPagina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOR_TRANSLATION_RESOURCE_PATH", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TranslationResourcePath { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FOR_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }
        
        [NHibernate.Mapping.Attributes.Bag(0, Name = "PermissoesPersonalizadas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERMISSAO_PERSONALIZADA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FOR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PermissaoPersonalizada", Column = "PPS_CODIGO")]
        public virtual IList<Dominio.Entidades.Modulos.PermissaoPersonalizada> PermissoesPersonalizadas { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposServicosMultisoftware", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FORMULARIO_SERVICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FOR_CODIGO" )]
        [NHibernate.Mapping.Attributes.Element(2, Column = "TIP_SERVICO", TypeType = typeof(Dominio.Enumeradores.TipoOperadora), NotNull = true)]
        public virtual ICollection<Dominio.Enumeradores.TipoServicoMultisoftware> TiposServicosMultisoftware { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "ClientesFormulario", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_FORMULARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FOR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteFormulario", Column = "CFO_CODIGO")]
        public virtual IList<Dominio.Entidades.Modulos.ClienteFormulario> ClientesFormulario { get; set; }

        //[NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FORMULARIO_CLIENTE")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "FOR_CODIGO")]
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
