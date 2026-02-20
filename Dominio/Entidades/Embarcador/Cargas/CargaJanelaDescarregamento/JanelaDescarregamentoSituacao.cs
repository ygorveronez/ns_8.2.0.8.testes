namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_JANELA_DESCARREGAMENTO_SITUACAO", EntityName = "JanelaDescarregamentoSituacao", Name = "Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao", NameType = typeof(JanelaDescarregamentoSituacao))]
    public class JanelaDescarregamentoSituacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JDS_COR", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JDS_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "JDS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoCadastrada), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoCadastrada Situacao { get; set; }
    }
}
