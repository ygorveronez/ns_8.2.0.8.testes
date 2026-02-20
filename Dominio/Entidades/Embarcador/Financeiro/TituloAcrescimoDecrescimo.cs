using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_ACRESCIMO_DECRESCIMO", EntityName = "TituloAcrescimoDecrescimo", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloAcrescimoDecrescimo", NameType = typeof(TituloAcrescimoDecrescimo))]
    public class TituloAcrescimoDecrescimo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /*
         * Id que vem da integração da Marfrig
         */
        [NHibernate.Mapping.Attributes.Property(0, Name = "IdIntegracao", Column = "TAD_ID_INTEGRACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string IdIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TAD_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "TAD_TIPO", TypeType = typeof(TipoAcrescimoDecrescimo), NotNull = true)]
        public virtual TipoAcrescimoDecrescimo Tipo { get; set; }

        /*
         * Descricao que vem da integração da Marfrig
         */
        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TAD_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        /*
         * Historico que vem da integração da Marfrig
         */
        [NHibernate.Mapping.Attributes.Property(0, Name = "Historico", Column = "TAD_HISTORICO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Historico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAD_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }
    }
}
