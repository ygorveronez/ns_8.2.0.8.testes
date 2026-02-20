using System;

namespace Dominio.Entidades.Embarcador.Relatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RELATORIO_COLUNA", EntityName = "RelatorioColuna", Name = "Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna", NameType = typeof(RelatorioColuna))]
    public class RelatorioColuna : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Relatorio", Column = "REL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Relatorios.Relatorio Relatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "REC_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "REC_TITULO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Propriedade", Column = "REC_PROPRIEDADE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Propriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tamanho", Column = "REC_TAMANHO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Tamanho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Visivel", Column = "REC_VISIVEL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Visivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAgrupamento", Column = "REC_PERMITE_AGRUPAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarFormatoTexto", Column = "REC_UTILIZAR_FORMATO_TEXTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarFormatoTexto { get; set; }

        /// <summary>
        /// Quando exportar via CSV pode ajustar aqui a quantidade de casas na exportação.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PrecisaoDecimal", Column = "REC_PRECISAO_DECIMAL", TypeType = typeof(int), NotNull = false)]
        public virtual int PrecisaoDecimal { get; set; }

        /// <summary>
        /// Quando exportar via CSV pode ajustar aqui o tipo de dados a ser exportado.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTypeExportacao", Column = "REC_DATA_TYPE_EXPORTACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DataTypeExportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Alinhamento", Column = "REC_ALINHAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento Alinhamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSumarizacao", Column = "REC_TIPO_SUMARIZACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao TipoSumarizacao { get; set; }

        /// <summary>
        /// Quando a coluna é gerada a partir de um cadastro no banco de dados, informar o código 
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDinamico", Column = "REC_CODIGO_DINAMICO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoDinamico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabName", Column = "REC_TAB_NAME", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TabName { get; set; } 

        public virtual bool Equals(RelatorioColuna other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
