using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Interfaces.Embarcador.Integracao;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_INTEGRACAO", EntityName = "ChamadoIntegracao", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao", NameType = typeof(ChamadoIntegracao))]
    public class ChamadoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "INT_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaNotaFiscal", Column = "CEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal CargaEntregaNotaFiscal { get; set; }

        /// <summary>
        /// ID Portal de devolução
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloDevolucao", Column = "INT_PROTOCOLO_DEVOLUCAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ProtocoloDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusDevolucao", Column = "INT_STATUS_DEVOLUCAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string StatusDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaDevolucao", Column = "INT_SENHA_DEVOLUCAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoDevolucao", Column = "INT_OBSERVACAO_DEVOLUCAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoDevolucao { get; set; }
    }
}
