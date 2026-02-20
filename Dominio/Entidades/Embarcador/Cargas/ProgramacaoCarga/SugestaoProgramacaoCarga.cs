using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_SUGESTAO_PROGRAMACAO_CARGA", EntityName = "SugestaoProgramacaoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga", NameType = typeof(SugestaoProgramacaoCarga))]
    public class SugestaoProgramacaoCarga : EntidadeBase
    {
        public SugestaoProgramacaoCarga()
        {
            DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "SPC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramacao", Column = "SPC_DATA_PROGRAMACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataProgramacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "SPC_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeValidada", Column = "SPC_QUANTIDADE_VALIDADA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeValidada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "SPC_SITUACAO", TypeType = typeof(SituacaoSugestaoProgramacaoCarga), NotNull = true)]
        public virtual SituacaoSugestaoProgramacaoCarga Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoProgramacaoCarga", Column = "CPC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoProgramacaoCarga ConfiguracaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Sugestão para o dia {DataProgramacao.ToDateString()}";
            }
        }
    }
}
