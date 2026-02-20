using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.CTeAgrupado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_AGRUPADO_INTEGRACAO", EntityName = "CargaCTeAgrupadoIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao", NameType = typeof(CargaCTeAgrupadoIntegracao))]
    public class CargaCTeAgrupadoIntegracao : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao>, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        public CargaCTeAgrupadoIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeAgrupado", Column = "CCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado CargaCTeAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcaoIntegracao", Column = "CCI_TIPO_ACAO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao TipoAcaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoExternoRetornoIntegracao", Column = "CCI_CODIGO_EXTERNO_RETORNO_INTEGRACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoExternoRetornoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_AGRUPADO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CAA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
        public virtual bool Equals(CargaCTeAgrupadoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
