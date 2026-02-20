namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_JUSTIFICATIVA_ACRESCIMO_DESCONTO", EntityName = "FechamentoJustificativaAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto", NameType = typeof(FechamentoJustificativaAcrescimoDesconto))]
    public class FechamentoJustificativaAcrescimoDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FAD_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoJustificativa", Column = "FAD_TIPO_JUSTIFICATIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaPesquisa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativaPesquisa TipoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "FAD_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }
    }
}
