using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_INTEGRACAO", EntityName = "ContratoFreteIntegracao", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao", NameType = typeof(ContratoFreteTransportadorIntegracao))]
    public class ContratoFreteTransportadorIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<ContratoFreteTransportadorIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LBC", Column = "LBC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LBC LBC { get; set; }

        [Obsolete("Criado errado")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoTransporteFrete", Column = "CTF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete ContratoTransporteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO_TRANSPORTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CFI_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreProtocolo", Column = "CFI_PRE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PreProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoColeta", Column = "CFI_INTEGRACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarEncerramento", Column = "CFI_INTEGRAR_ENCERRAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarEncerramento { get; set; }     
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarAnexo", Column = "CFI_INTEGRAR_ANEXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarCustoFixo", Column = "CFI_INTEGRAR_CUSTO_FIXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCustoFixo { get; set; }

        public virtual bool Equals(ContratoFreteTransportadorIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
