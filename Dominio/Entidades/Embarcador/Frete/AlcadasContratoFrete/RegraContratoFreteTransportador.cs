using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_CONTRATO_FRETE", EntityName = "RegraContratoFreteTransportador", Name = "Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador", NameType = typeof(RegraContratoFreteTransportador))]
    public class RegraContratoFreteTransportador : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCF_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCF_PRODUTO_EMBARCADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorContrato", Column = "RCF_REGRA_POR_VALOR_CONTRATO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCF_DEPOSITO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RCF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_APROVADORES_REGRA_CONTRATO_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaTransportador", Column = "RTF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador> AlcadasTransportadores { get; set; }
        // --------------------------------------
        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo.ObterDescricaoAtivo();
            }
        }


        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Frete.AlcadaFilial", Column = "RCA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.AlcadaFilial> AlcadasFilial { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasContratoValorContrato", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CONTRATO_FRETE_VALOR_CONTRATO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasContratoValorContrato", Column = "RVC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato> RegrasContratoValorContrato { get; set; }

        // --------------------------------------
    }
}
