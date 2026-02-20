using System;

namespace Dominio.Entidades.Embarcador.Usuarios.Colaborador
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLABORADOR_LANCAMENTO_ANEXO", EntityName = "ColaboradorLancamentoAnexo", Name = "Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo", NameType = typeof(ColaboradorLancamentoAnexo))]
    public class ColaboradorLancamentoAnexo : EntidadeBase, IEquatable<ColaboradorLancamentoAnexo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CLA_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CLA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "CLA_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColaboradorLancamento", Column = "CLS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ColaboradorLancamento ColaboradorLancamento { get; set; }

        public virtual bool Equals(ColaboradorLancamentoAnexo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
