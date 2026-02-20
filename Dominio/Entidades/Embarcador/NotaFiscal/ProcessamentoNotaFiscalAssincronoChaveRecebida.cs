namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROCESSAMENTO_NOTA_FISCAL_ASSINCRONO_CHAVE_RECEBIDA", EntityName = "ProcessamentoNotaFiscalAssincronoChaveRecebida", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida", NameType = typeof(ProcessamentoNotaFiscalAssincronoChaveRecebida))]

    public class ProcessamentoNotaFiscalAssincronoChaveRecebida : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNotaFiscal", Column = "PNC_CHAVE", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string ChaveNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotaFiscal", Column = "PNC_NUMERO_NOTA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNotaFiscal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ChaveNotaFiscal;
            }
        }
    }
}