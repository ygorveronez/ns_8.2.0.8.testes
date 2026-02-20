using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANCELAMENTO_PAGAMENTO_INTEGRACAO", EntityName = "CancelamentoPagamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao", NameType = typeof(CancelamentoPagamentoIntegracao))]
    public class CancelamentoPagamentoIntegracao : Integracao.Integracao, IEquatable<CancelamentoPagamentoIntegracao>, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoPagamento", Column = "CPG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CancelamentoPagamento CancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoFaturamento", Column = "DFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento DocumentoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaIntegracao", Column = "CPI_SEQUENCIA_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPI_DATA_ULTIMO_DOWNLOAD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoDownload { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANCELAMENTO_PAGAMENTO_INTEGRACAO_ARQUIVO_TRANSACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(CancelamentoPagamentoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual int ObterSequencia()
        {
            if (!this.DataUltimoDownload.HasValue || this.DataUltimoDownload.Value.Date < DateTime.Today)
                this.SequenciaIntegracao = 0;

            this.DataUltimoDownload = DateTime.Now;

            return ++this.SequenciaIntegracao;
        }
    }
}
