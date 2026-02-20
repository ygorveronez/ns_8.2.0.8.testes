using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE", EntityName = "ModalidadePessoas", Name = "Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas", NameType = typeof(ModalidadePessoas))]
    public class ModalidadePessoas : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>
    {
        public ModalidadePessoas() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoModalidade", Column = "MOD_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade TipoModalidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CPF_CNPJ", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ModalidadesFornecedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModalidadeFornecedorPessoas", Column = "MOF_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ModalidadeFornecedorPessoas> ModalidadesFornecedores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ModalidadesTransportadora", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE_TRANSPORTADORAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MOD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModalidadeTransportadoraPessoas", Column = "MOT_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ModalidadeTransportadoraPessoas> ModalidadesTransportadora { get; set; }

        public virtual string DescricaoTipoModalidade
        {
            get
            {
                if (this.TipoModalidade == ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente)
                    return "Cliente";
                else if (this.TipoModalidade == ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)
                    return "Fornecedor";
                else
                    return "Transportador Terceiro";
            }
        }

        public virtual bool Equals(ModalidadePessoas other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
