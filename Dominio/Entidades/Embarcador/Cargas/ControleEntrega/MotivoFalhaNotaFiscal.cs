namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    /// <summary>
    /// No app, quando um motorista não consegue ler a nota fiscal que deveria ler, ele escolhe um dessdes motivos pra justificar o erro.
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_FALHA_NOTA_FISCAL", EntityName = "MotivoFalhaNotaFiscal", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal", NameType = typeof(MotivoFalhaNotaFiscal))]
    public class MotivoFalhaNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MFN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MFN_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MFN_OBSERVACAO", TypeType = typeof(string), Length = 2048, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MNF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
