using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_EMBARQUE_HISTORICO_INTEGRACAO", EntityName = "OrdemEmbarqueHistoricoIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao", NameType = typeof(OrdemEmbarqueHistoricoIntegracao))]
    public class OrdemEmbarqueHistoricoIntegracao : EntidadeBase, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "INT_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "INT_PROBLEMA_INTEGRACAO", Type = "StringClob", NotNull = true)]
        public virtual string ProblemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "INT_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoIntegracao), NotNull = true)]
        public virtual SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "INT_TIPO", TypeType = typeof(TipoOrdemEmbarqueHistoricoIntegracao), NotNull = true)]
        public virtual TipoOrdemEmbarqueHistoricoIntegracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemEmbarque", Column = "OEM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemEmbarque OrdemEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO_ADICIONADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido PedidoAdicionado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO_REMOVIDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido PedidoRemovido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ORDEM_EMBARQUE_HISTORICO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get { return $"Integração da ordem de embarque {OrdemEmbarque.Numero}"; }
        }
    }
}
