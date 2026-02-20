using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_LAVACAO", EntityName = "VeiculoLavacao", Name = "Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao", NameType = typeof(VeiculoLavacao))]
    public class VeiculoLavacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLavacao", Column = "VEL_DATA_LAVACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLavacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEL_NOME_ARQUIVO_ANTES_LAVACAO_SUMARIZADO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivoAntesLavacaoSumarizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEL_NOME_ARQUIVO_DEPOIS_LAVACAO_SUMARIZADO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivoDepoisLavacaoSumarizado { get; set; }

        public virtual string NomeArquivoAntesLavacaoDescricao
        {
            get { return "Antes da Lavação - " + NomeArquivoAntesLavacaoSumarizado; }
        }

        public virtual string NomeArquivoDepoisLavacaoDescricao
        {
            get { return "Depois da Lavação - " + NomeArquivoDepoisLavacaoSumarizado; }
        }
    }
}
