using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.FaturamentoMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_EMISSAO_NFE", EntityName = "PlanoEmissaoNFe", Name = "Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe", NameType = typeof(PlanoEmissaoNFe))]
    public class PlanoEmissaoNFe : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PEN_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PEN_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaNFe", Column = "PEN_COBRANCA_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaNFSe", Column = "PEN_COBRANCA_NFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaBoleto", Column = "PEN_COBRANCA_BOLETO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaTitulo", Column = "PEN_COBRANCA_TITULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdesao", Column = "PEN_VALOR_ADESAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdesao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObservacaoFaturamentoMensal", Column = "PEN_TIPO_OBSERVACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal TipoObservacaoFaturamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PEN_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Valores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PLANO_EMISSAO_NFE_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PlanoEmissaoNFeValor", Column = "PNV_CODIGO")]
        public virtual IList<PlanoEmissaoNFeValor> Valores { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(PlanoEmissaoNFe other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
