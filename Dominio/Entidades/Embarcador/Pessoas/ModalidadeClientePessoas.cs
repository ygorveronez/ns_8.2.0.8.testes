using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE_CLIENTES", EntityName = "ModalidadeClientePessoas", Name = "Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas", NameType = typeof(ModalidadeClientePessoas))]
    public class ModalidadeClientePessoas : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>
    {
        public ModalidadeClientePessoas() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadePessoas", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas ModalidadePessoas { get; set; }

        public virtual bool Equals(ModalidadeClientePessoas other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.ModalidadePessoas?.DescricaoTipoModalidade ?? string.Empty;
            }
        }
    }
}
