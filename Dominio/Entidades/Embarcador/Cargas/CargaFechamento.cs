namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_FECHAMENTO", EntityName = "CargaFechamento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaFechamento", NameType = typeof(CargaFechamento))]
    public class CargaFechamento : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoFechamento", Column = "CFE_SITUACAO_FECHAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaFechamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaFechamento SituacaoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CFE_VALOR_FRETE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRecalculado", Column = "CFE_VALOR_RECALCULADO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorRecalculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoCalculoFrete", Column = "CFE_MOTIVO_REJEICAO_CALCULO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string MotivoRejeicaoCalculoFrete { get; set; }

    }
}
