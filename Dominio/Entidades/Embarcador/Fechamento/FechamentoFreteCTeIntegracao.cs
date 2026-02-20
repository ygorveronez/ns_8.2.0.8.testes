using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE_CTE_INTEGRACAO", EntityName = "FechamentoFreteCTeIntegracao", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao", NameType = typeof(FechamentoFreteCTeIntegracao))]
    public class FechamentoFreteCTeIntegracao : Integracao.Integracao, IIntegracaoComArquivo<FechamentoFreteCTeIntegracaoArquivo>, IEquatable<FechamentoFreteCTeIntegracao>
    {
        public FechamentoFreteCTeIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeComplementoInfo", Column = "CCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTeComplementoInfo Complemento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoFrete FechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "FIC_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FECHAMENTO_FRETE_CTE_INTEGRACAO_ARQUIVO_RELACIONAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FechamentoFreteCTeIntegracaoArquivo", Column = "FIA_CODIGO")]
        public virtual ICollection<FechamentoFreteCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaIntegracao", Column = "FEF_SEQUENCIA_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEF_DATA_ULTIMO_DOWNLOAD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoDownload { get; set; }

        public virtual bool Equals(FechamentoFreteCTeIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string Descricao
        {
            get
            {
                return this.Complemento?.CTe?.Descricao ?? string.Empty;
            }
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
