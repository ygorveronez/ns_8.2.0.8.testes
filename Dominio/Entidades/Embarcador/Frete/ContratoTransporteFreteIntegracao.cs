using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_TRANSPORTE_FRETE_INTEGRACAO", EntityName = "ContratoTransporteFreteIntegracao", Name = "Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao", NameType = typeof(ContratoTransporteFreteIntegracao))]
    public class ContratoTransporteFreteIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<ContratoFreteTransportadorIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LBC", Column = "LBC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LBC LBC { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoTransporteFrete", Column = "CTF_CODIGO_CONTRATO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete ContratoTransporteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_TRANSPORTE_FRETE_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CTF_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreProtocolo", Column = "CTF_PRE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PreProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoColeta", Column = "CTF_INTEGRACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarEncerramento", Column = "CTF_INTEGRAR_ENCERRAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarAnexos", Column = "CTF_INTEGRAR_ANEXOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarAnexos { get; set; }

        public virtual bool Equals(ContratoFreteTransportadorIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
