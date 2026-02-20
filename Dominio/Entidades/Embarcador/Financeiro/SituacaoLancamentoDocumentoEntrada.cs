namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SITUACAO_LANCAMENTO_DOCUMENTO_ENTRADA", EntityName = "SituacaoLancamentoDocumentoEntrada", Name = "Dominio.Entidades.Embarcador.Financeiro.SituacaoLancamentoDocumentoEntrada", NameType = typeof(SituacaoLancamentoDocumentoEntrada))]
    public class SituacaoLancamentoDocumentoEntrada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SLC_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "SLC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get;  set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return string.Empty;
                }
            }
        }
    }
}