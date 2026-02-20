using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VEICULO_TABELA_COMISSAO", EntityName = "AcertoViagemTabelaComissao", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao", NameType = typeof(AcertoViagemTabelaComissao))]
    public class AcertoViagemTabelaComissao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoMotoristaMedia", Column = "TCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoMotoristaMedia TabelaComissaoMotoristaMedia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoMotoristaRepresentacao", Column = "TCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoMotoristaRepresentacao TabelaComissaoMotoristaRepresentacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasFrete", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_VEICULO_TABELA_COMISSAO_ROTAS_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AVT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaComissaoMotoristaRotaFrete", Column = "TRF_CODIGO")]
        public virtual ICollection<TabelaComissaoMotoristaRotaFrete> RotasFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoFaturamentoDia", Column = "TFD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoFaturamentoDia TabelaComissaoFaturamentoDia { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(AcertoViagemTabelaComissao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
