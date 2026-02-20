namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_JANELA_CARREGAMENTO_SITUACAO", EntityName = "CargaJanelaCarregamentoSituacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao", NameType = typeof(CargaJanelaCarregamentoSituacao))]
    public class CargaJanelaCarregamentoSituacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JTS_COR", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JTS_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "JCS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoAdicional), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoAdicional Situacao { get; set; }
    }
}
