namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_TRATATIVA_OCORRENCIA_ENTREGA", EntityName = "TratativaOcorrenciaEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega", NameType = typeof(TratativaOcorrenciaEntrega))]
    public class TratativaOcorrenciaEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOE_TRATATIVA_DEVOLUCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega TratativaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOE_DEVOLUCAO_PARCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DevolucaoParcial { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}