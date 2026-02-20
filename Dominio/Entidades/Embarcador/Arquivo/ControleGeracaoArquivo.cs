using System;

namespace Dominio.Entidades.Embarcador.Arquivo
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_GERACAO_ARQUIVO", EntityName = "ControleGeracaoArquivo", Name = "Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo", NameType = typeof(ControleGeracaoArquivo))]
    public class ControleGeracaoArquivo : EntidadeBase, IEquatable<ControleGeracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioGeracao", Column = "CGA_DATA_INICIO_GERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicioGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimGeracao", Column = "CGA_DATA_FIM_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CGA_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "CGA_GUID_ARQUIVO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CGA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoArquivo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoArquivo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArquivo", Column = "CGA_TIPO_ARQUIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArquivo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoArquivo TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual bool Equals(ControleGeracaoArquivo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
