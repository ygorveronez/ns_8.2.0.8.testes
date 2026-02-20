using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_CANCELAMENTO_INTEGRACAO", EntityName = "CargaMDFeManualCancelamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao", NameType = typeof(CargaMDFeManualCancelamentoIntegracao))]

    public class CargaMDFeManualCancelamentoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<CargaMDFeManualCancelamentoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManualCancelamento", Column = "CMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaMDFeManualCancelamento CargaMDFeManualCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoFilialEmissora", Column = "CMI_INTEGRACAO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_CANCELAMENTO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CMI_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreProtocolo", Column = "CMI_PRE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PreProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoColeta", Column = "CMI_INTEGRACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Protocolo;
            }
        }

        public virtual bool Equals(CargaMDFeManualCancelamentoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
