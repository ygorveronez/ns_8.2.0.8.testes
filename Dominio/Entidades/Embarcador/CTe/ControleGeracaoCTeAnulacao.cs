using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_GERACAO_CTE_ANULACAO", EntityName = "ControleGeracaoCTeAnulacao", Name = "Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao", NameType = typeof(ControleGeracaoCTeAnulacao))]
    public class ControleGeracaoCTeAnulacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_ORIGINAL", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTeOriginal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_ANULACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTeAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO_SUBSTITUICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTeSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGA_OBSERVACAO_ANULACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoAnulacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGA_OBSERVACAO_SUBSTITUICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGA_DATA_EVENTO_DESACORDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEventoDesacordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGA_VALOR_CTE_SUBSTITUICAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorCTeSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_OUTRO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente OutroTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarCTeSubstituicao", Column = "CGA_NAO_GERAR_CTE_SUBSTITUICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarCTeSubstituicao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return CargaCTeOriginal.Descricao;
            }
        }
    }
}
