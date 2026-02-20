using Dominio.Interfaces.Embarcador.Integracao;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DADOS_COLETA_INTEGRACAO", EntityName = "GestaoDadosColetaIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao", NameType = typeof(GestaoDadosColetaIntegracao))]
    public class GestaoDadosColetaIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo>
    {
        public GestaoDadosColetaIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDadosColeta", Column = "GDC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDadosColeta GestaoDadosColeta { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DADOS_COLETA_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GestaoDadosColetaIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo> ArquivosTransacao { get; set; }
    }
}

