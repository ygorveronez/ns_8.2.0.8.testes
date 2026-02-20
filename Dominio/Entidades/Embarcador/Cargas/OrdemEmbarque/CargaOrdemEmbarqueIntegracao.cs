using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ORDEM_EMBARQUE_INTEGRACAO", EntityName = "CargaOrdemEmbarqueIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao", NameType = typeof(CargaOrdemEmbarqueIntegracao))]
    public class CargaOrdemEmbarqueIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cancelada", Column = "INT_CANCELADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFalhaIntegracao", Column = "INT_TIPO_FALHA_INTEGRACAO", TypeType = typeof(TipoFalhaCargaOrdemEmbarqueIntegracao), NotNull = false)]
        public virtual TipoFalhaCargaOrdemEmbarqueIntegracao TipoFalhaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ORDEM_EMBARQUE_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get { return $"Integração da carga {Carga.CodigoCargaEmbarcador} para criação de ordem de embarque"; }
        }
    }
}
