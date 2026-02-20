namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_JANELA_DESCARREGAMENTO_SITUACAO", EntityName = "CargaJanelaDescarregamentoSituacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao", NameType = typeof(CargaJanelaDescarregamentoSituacao))]
    public class CargaJanelaDescarregamentoSituacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JDS_COR", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JDS_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "JDS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoAdicional), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoAdicional Situacao { get; set; }
    }
}
