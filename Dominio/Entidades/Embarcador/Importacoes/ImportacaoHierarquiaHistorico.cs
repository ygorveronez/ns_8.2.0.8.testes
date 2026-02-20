using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Importacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPORTACAO_HIERARQUIA_HISTORICO", EntityName = "ImportacaoHierarquiaHistorico", Name = "Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico", NameType = typeof(ImportacaoHierarquiaHistorico))]
    public class ImportacaoHierarquiaHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IHH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IHH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IHH_QUANTIDADE_REGISTROS_IMPORTADOS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeRegistrosImportados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IHH_QUANTIDADE_REGISTROS_TOTAL", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeRegistrosTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IHH_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeArquivo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "IHH_TIPO_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string TipoArquivo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "IHH_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "IHH_SITUACAO", TypeType = typeof(SituacaoImportacaoHierarquia), NotNull = true)]
        public virtual SituacaoImportacaoHierarquia Situacao { get; set; }
    }
}
