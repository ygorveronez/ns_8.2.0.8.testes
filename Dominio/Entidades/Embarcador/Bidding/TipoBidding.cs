using System.Collections.Generic;
using Dominio.Entidades.Embarcador.Frete;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_BIDDING", EntityName = "TipoBidding", Name = "Dominio.Entidades.Embarcador.Produtos.TipoBidding", NameType = typeof(TipoBidding))]
    public class TipoBidding : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TBI_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TBI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "TBI_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirRankOfertas", Column = "TBI_EXIBIR_RANK_OFERTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirRankOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoIncluirImpostoValorTotalOferta", Column = "TBI_NAO_INCLUIR_IMPOSTO_VALOR_TOTAL_OFERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirImpostoValorTotalOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPossuiPedagioFluxoOferta", Column = "TBI_NAO_POSSUI_PEDAGIO_FLUXO_OFERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPossuiPedagioFluxoOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirOfertasComponentes", Column = "TBI_PERMITIR_OFERTAS_COMPONENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirOfertasComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ComponentesFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_BIDDING_COMPONENTES_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TBI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ComponenteFrete", Column = "CFR_CODIGO")]
        public virtual ICollection<ComponenteFrete> ComponentesFrete { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return "";
                }
            }
        }

        public virtual bool Equals(TipoBidding other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
