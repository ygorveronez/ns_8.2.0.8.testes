using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_RESULTADO", EntityName = "CentroResultado", Name = "Dominio.Entidades.Embarcador.Financeiro.CentroResultado", NameType = typeof(CentroResultado))]
    public class CentroResultado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CRE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Plano", Column = "CRE_PLANO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Plano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCompanhia", Column = "CRE_CODIGO_COMPANHIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCompanhia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlanoContabilidade", Column = "CRE_PLANO_CONTABILIDADE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string PlanoContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CRE_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnaliticoSintetico", Column = "CRE_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico AnaliticoSintetico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo SegmentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_RESULTADO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_RESULTADO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOC_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposMovimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_MOVIMENTO_CENTRO_RESULTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoMovimentoCentroResultado", Column = "TMC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado> TiposMovimentos { get; set; }

        public virtual string DescricaoAnaliticoSintetico
        {
            get { return AnaliticoSintetico.ObterDescricao(); }
        }

        public virtual string BuscarDescricao
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PlanoContabilidade))
                    return this.PlanoContabilidade + " - " + this.Descricao;
                else
                    return this.Descricao;
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true: return Localization.Resources.Gerais.Geral.Ativo;
                    case false: return Localization.Resources.Gerais.Geral.Inativo;
                    default: return "";
                }
            }
        }

        public virtual bool Equals(CentroResultado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
