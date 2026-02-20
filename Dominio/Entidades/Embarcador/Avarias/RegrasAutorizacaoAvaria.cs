using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_AVARIA", EntityName = "RegrasAutorizacaoAvaria", Name = "Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria", NameType = typeof(RegrasAutorizacaoAvaria))]
    public class RegrasAutorizacaoAvaria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RAA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RAA_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RAA_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeAprovacao", Column = "RAA_PRIORIDADE_APROVACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int PrioridadeAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RAA_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaAutorizacaoAvaria", Column = "RAA_ETAPA_AUTORIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria EtapaAutorizacaoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorMotivoAvaria", Column = "RAA_MOTIVA_AVARIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorMotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorOrigem", Column = "RAA_ORIGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDestino", Column = "RAA_DESTINO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAA_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportadora", Column = "RAA_TRANSPORTADORA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAA_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }
        

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RAA_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_AVARIA_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }



        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasMotivoAvaria", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_MOTIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasMotivoAvaria", Column = "RMA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria> RegrasMotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOrigem", Column = "ROR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem> RegrasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasDestino", Column = "RDE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.RegrasDestino> RegrasDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasFilial", Column = "RFI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.RegrasFilial> RegrasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTransportadora", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_TRANSPORTADORA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasTransportadora", Column = "RTR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora> RegrasTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasTipoOperacao", Column = "RTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao> RegrasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasValorAvaria", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AVARIA_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasValorAvaria", Column = "RVA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria> RegrasValorAvaria { get; set; }

        public virtual string DescricaoEtapaAutorizacaoAvaria
        {
            get
            {
                switch (EtapaAutorizacaoAvaria)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao:
                        return "Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Integracao:
                        return "Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Lote:
                        return "Lote";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(RegrasAutorizacaoAvaria other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}    