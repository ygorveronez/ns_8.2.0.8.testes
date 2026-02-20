using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_RESTRICAO_MODELO_VEICULAR", EntityName = "ModalidadeFornecedorPessoasRestricaoModeloVeicular", Name = "Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular", NameType = typeof(ModalidadeFornecedorPessoasRestricaoModeloVeicular))]
    public class ModalidadeFornecedorPessoasRestricaoModeloVeicular : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>
    {
        public ModalidadeFornecedorPessoasRestricaoModeloVeicular() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadeFornecedorPessoas", Column = "MFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModalidadeFornecedorPessoas ModalidadeFornecedorPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual bool Equals(ModalidadeFornecedorPessoasRestricaoModeloVeicular other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
