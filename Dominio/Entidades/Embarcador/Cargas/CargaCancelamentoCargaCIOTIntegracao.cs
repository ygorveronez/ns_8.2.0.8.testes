using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CANCELAMENTO_CARGA_CIOT_INTEGRACAO", EntityName = "CargaCancelamentoCargaCIOTIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCIOTIntegracao", NameType = typeof(CargaCancelamentoCargaCIOTIntegracao))]
    public class CargaCancelamentoCargaCIOTIntegracao : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCIOTIntegracao>, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        public CargaCancelamentoCargaCIOTIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCIOT", Column = "CCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCIOT CargaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CANCELAMENTO_CARGA_CIOT_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return CargaCIOT?.CIOT?.Numero ?? string.Empty;
            }
        }

        public virtual bool Equals(CargaCancelamentoCargaCIOTIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
