using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTA_PAGAR", EntityName = "ContaPagar", Name = "Dominio.Entidades.Embarcador.Financeiro.ContaPagar", NameType = typeof(ContaPagar))]
    public class ContaPagar : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoAProcessar { get; set; }  
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro TermoQuitacaoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_NOME_ORIGINAL_ARQUIVO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NomeOriginalArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoArquivo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoArquivo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_TIPO_REGISTRO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistro), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistro TipoRegistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPA_MENSAGEM_PROCESSAMENTO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MensagemProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "MensagensProcessamento", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTA_PAGAR_MENSAGENS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CPA_MENSAGEM_PROCESSAMENTO_LINHA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual ICollection<string> MensagensProcessamento { get; set; }

        #region Propiedade Virtuales
        public virtual string Descricao
        {
            get { return NomeOriginalArquivo; }
        }

        #endregion
    }
}
