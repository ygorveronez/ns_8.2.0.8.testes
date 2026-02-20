using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_COLETA_PEDIDO", EntityName = "AgendamentoColetaPedido", Name = "Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido", NameType = typeof(AgendamentoColetaPedido))]
    public class AgendamentoColetaPedido : EntidadeBase, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AgendamentoColeta", Column = "ACO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AgendamentoColeta AgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumesEnviar", Column = "ACP_VOLUMES_ENVIAR", TypeType = typeof(int), NotNull = true)]
        public virtual int VolumesEnviar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorVolumesEnviar", Column = "ACP_VALOR_VOLUMES_ENVIAR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorVolumesEnviar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SKU", Column = "ACP_SKU", TypeType = typeof(int), NotNull = true)]
        public virtual int SKU { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AGENDAMENTO_COLETA_PEDIDO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Produtos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AGENDAMENTO_COLETA_PEDIDO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AgendamentoColetaPedidoProduto", Column = "APP_CODIGO")]
        public virtual IList<AgendamentoColetaPedidoProduto> Produtos { get; set; }

        public virtual DateTime DataIntegracao { get; set; }

        public virtual string ProblemaIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Pedido Agendamento Coleta " + this.Codigo;
            }
        }
    }
}
