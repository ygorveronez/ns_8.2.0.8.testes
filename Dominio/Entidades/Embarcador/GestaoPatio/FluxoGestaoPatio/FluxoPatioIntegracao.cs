using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_PATIO_INTEGRACAO", EntityName = "FluxoPatioIntegracao", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio.FluxoPatioIntegracao", NameType = typeof(FluxoPatioIntegracao))]
    public class FluxoPatioIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>, IEquatable<FluxoPatioIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "EtapaFluxoGestaoPatio", Column = "FCE_ETAPA_FLUXO_GESTAO_PATIO", TypeType = typeof(EtapaFluxoGestaoPatio), NotNull = true)]
        public virtual EtapaFluxoGestaoPatio EtapaFluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FLUXO_PATIO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CCA_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreProtocolo", Column = "CCA_PRE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PreProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoColeta", Column = "CCA_INTEGRACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarIntegracaoCompleta", Column = "CCA_REALIZAR_INTEGRACAO_COMPLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoCompleta { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Fluxo Pátio Integração";
            }
        }

        public virtual bool Equals(FluxoPatioIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
