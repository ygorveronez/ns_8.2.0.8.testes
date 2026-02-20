using System;
using System.Collections.Generic;
using Dominio.Entidades.WebService;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROCESSAMENTO_NOTA_FISCAL_ASSINCRONO", EntityName = "ProcessamentoNotaFiscalAssincrono", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono", NameType = typeof(ProcessamentoNotaFiscalAssincrono))]
    public class ProcessamentoNotaFiscalAssincrono : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Integradora Integradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNA_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNA_SUCESSO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Sucesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNA_MENSAGEM", TypeType = typeof(string), Length = 450, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNA_PROTOCOLO_PEDIDO", TypeType = typeof(int), NotNull = false)]
        public virtual int ProtocoloPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNA_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "PNA_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int Tentativas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PROCESSAMENTO_NOTA_FISCAL_ASSINCRONO_ARQUIVOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PNA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProcessamentoNotaFiscalAssincronoArquivo", Column = "PNF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoArquivo> ArquivosIntegracaoRetorno { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PROCESSAMENTO_NOTA_FISCAL_ASSINCRONO_CHAVE_RECEBIDAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PNA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProcessamentoNotaFiscalAssincronoChaveRecebida", Column = "PNC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida> NotasFiscais { get; set; }


        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
        public virtual string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

    }
}
