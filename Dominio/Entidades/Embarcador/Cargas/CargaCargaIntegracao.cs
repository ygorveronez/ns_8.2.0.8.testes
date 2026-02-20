using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CARGA_INTEGRACAO", EntityName = "CargaCargaIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao", NameType = typeof(CargaCargaIntegracao))]
    public class CargaCargaIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<CargaCargaIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoFilialEmissora", Column = "CAI_INTEGRACAO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CARGA_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CCA_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreProtocolo", Column = "CCA_PRE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PreProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoColeta", Column = "CCA_INTEGRACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarIntegracaoCompleta", Column = "CCA_REALIZAR_INTEGRACAO_COMPLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoCompleta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Stage", Column = "STA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Stage Stage { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMICDTA", Column = "CCA_NUMERO_MICDTA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroMICDTA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSequencialMICDTA", Column = "CCA_NUMERO_SEQUENCIAL_MICDTA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencialMICDTA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SiglaPaisOrigemMICDTA", Column = "CCA_SIGLA_PAIS_ORIGEM_MICDTA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SiglaPaisOrigemMICDTA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLicencaTNTIMICDTA", Column = "CCA_NUMERO_LICENCA_TNTI_MICDTA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroLicencaTNTIMICDTA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardarFinalizarCargaAnterior", Column = "CCA_AGUARDAR_FINALIZAR_CARGA_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardarFinalizarCargaAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteRetorno", Column = "CCA_PENDENTE_RETORNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarCargaAnterior", Column = "CCA_FINALIZAR_CARGA_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarCargaAnterior { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_CARGA_PENDENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga CargaPendente { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoExternoRetornoIntegracao", Column = "CCA_CODIGO_EXTERNO_RETORNO_INTEGRACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoExternoRetornoIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Protocolo;
            }
        }

        public virtual bool Equals(CargaCargaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
