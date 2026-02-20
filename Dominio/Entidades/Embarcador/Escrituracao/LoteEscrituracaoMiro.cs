using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [Obsolete("Classe nao deve ser usada. Informações da MIRO estao em DocumentoFaturamento.cs")]
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_ESCRITURACAO_MIRO", EntityName = "LoteEscrituracaoMiro", Name = "Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro", NameType = typeof(LoteEscrituracaoMiro))]
    public class LoteEscrituracaoMiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LEM_GERACAO_LOTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracaoLote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LEM_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoMiro Situacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
