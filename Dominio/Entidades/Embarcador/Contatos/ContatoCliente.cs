using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Contatos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTATO_CLIENTE", EntityName = "ContatoCliente", Name = "Dominio.Entidades.Embarcador.Contatos.ContatoCliente", NameType = typeof(ContatoCliente))]
    public class ContatoCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCL_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCL_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCL_DATA_PREVISTA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevistaRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCL_CONTATO_SEM_CADASTRO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ContatoSemCadastro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PessoaContato", Column = "PCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Contatos.PessoaContato Contato { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoContato", Column = "SCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Contatos.SituacaoContato Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposContato", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTATO_CLIENTE_TIPO_CONTATO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoContato", Column = "TCO_CODIGO")]
        public virtual ICollection<TipoContato> TiposContato { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTATO_CLIENTE_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContatoClienteDocumento", Column = "CCD_CODIGO")]
        public virtual IList<ContatoClienteDocumento> Documentos { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                return Situacao?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoTipoContato
        {
            get
            {
                if (TiposContato != null)
                    return string.Join(", ", TiposContato.Select(o => o.Descricao));
                else
                    return string.Empty;
            }
        }
    }
}
