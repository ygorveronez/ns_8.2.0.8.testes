namespace Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TIPO_COMPROVANTE", EntityName = "TipoComprovante", Name = "Dominio.Entidades.Embarcador.ComprovanteCarga.TipoComprovante", NameType = typeof(TipoComprovante))]
    public class TipoComprovante : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }
    }
}
