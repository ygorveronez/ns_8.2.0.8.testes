using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BONIFICACAO_TRANSPORTADOR", EntityName = "BonificacaoTransportador", Name = "Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador", NameType = typeof(BonificacaoTransportador))]
    public class BonificacaoTransportador : EntidadeBase, IEquatable<BonificacaoTransportador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BNT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "BNT_TIPO", TypeType = typeof(TipoAjusteValor), NotNull = false)]
        public virtual TipoAjusteValor Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "BNT_PERCENTUAL_BONIFICACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "BNT_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "BNT_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "BNT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "BNT_INCLUIR_BASE_CALCULO_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposDeCarga", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BONIFICACAO_TRANSPORTADOR_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BNT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BONIFICACAO_TRANSPORTADOR_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BNT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filiais.Filial> Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOcorrencia", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BONIFICACAO_TRANSPORTADOR_TIPOS_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BNT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.TipoDeOcorrenciaDeCTe> TiposOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FiliasTransportador", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_BONIFICACAO_TRANSPORTADOR_FILIAIS_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BNT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Empresa> FiliasTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoIncluirComponentesFreteCalculoBonificacao", Column = "BNT_NAO_INCLUIR_COMPONENTESFRETE_CALCULO_BONIFICACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirComponentesFreteCalculoBonificacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Empresa.Descricao;
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual string DescricaoVigencia
        {
            get
            {
                if (DataInicial.HasValue && DataFinal.HasValue)
                    return $"De {DataInicial.Value.ToDateString()} até {DataFinal.Value.ToDateString()}";

                if (DataInicial.HasValue)
                    return $"À partir de {DataInicial.Value.ToDateString()}";

                if (DataFinal.HasValue)
                    return $"Até {DataFinal.Value.ToDateString()}";

                return string.Empty;
            }
        }

        public virtual decimal PercentualAplicar
        {
            get
            {
                return (Tipo == TipoAjusteValor.Acrescimo) ? Percentual : -Percentual;
            }
        }

        public virtual bool Equals(BonificacaoTransportador other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
