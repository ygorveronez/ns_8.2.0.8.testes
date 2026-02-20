using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.Cancelamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_DADOS_CANCELAMENTO", EntityName = "IntegracaoDadosCancelamento", Name = "Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento", NameType = typeof(IntegracaoDadosCancelamento))]
    public class IntegracaoDadosCancelamento : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.Cargas.Cancelamento.IntegracaoDadosCancelamento>, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_DADOS_CANCELAMENTO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return CargaCancelamento?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(IntegracaoDadosCancelamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
