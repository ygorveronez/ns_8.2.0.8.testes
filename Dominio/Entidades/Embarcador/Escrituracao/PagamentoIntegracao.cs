using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_INTEGRACAO", EntityName = "PagamentoIntegracao", Name = "Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao", NameType = typeof(PagamentoIntegracao))]
    public class PagamentoIntegracao : Integracao.Integracao, IEquatable<PagamentoIntegracao>, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pagamento Pagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoFaturamento", Column = "DFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento DocumentoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaIntegracao", Column = "PIN_SEQUENCIA_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PIN_DATA_ULTIMO_DOWNLOAD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoDownload { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "PAG_TIPO_DOCUMENTO", TypeType = typeof(Enumeradores.TipoDocumento), NotNull = false)]
        public virtual Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_INTEGRACAO_ARQUIVO_TRANSACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PIN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(PagamentoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.DocumentoFaturamento?.Numero.ToString() ?? Pagamento.Numero.ToString();
            }
        }

        public virtual int ObterSequencia()
        {
            if (!this.DataUltimoDownload.HasValue || this.DataUltimoDownload.Value.Date < DateTime.Today)
                this.SequenciaIntegracao = 0;

            this.DataUltimoDownload = DateTime.Now;

            return ++this.SequenciaIntegracao;
        }

        public virtual PagamentoIntegracao Clonar()
        {
            return (PagamentoIntegracao)this.MemberwiseClone();
        }
    }
}
