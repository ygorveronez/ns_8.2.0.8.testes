using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_CARGA_INTEGRACAO", EntityName = "CargaMDFeManualIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaMDFeManualIntegracao", NameType = typeof(CargaMDFeManualIntegracao))]
    public class CargaMDFeManualIntegracao : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualIntegracao>, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        public CargaMDFeManualIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoExternoRetornoIntegracao", Column = "CMC_CODIGO_EXTERNO_RETORNO_INTEGRACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoExternoRetornoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_CARGA_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CAA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
        public virtual bool Equals(CargaMDFeManualIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
