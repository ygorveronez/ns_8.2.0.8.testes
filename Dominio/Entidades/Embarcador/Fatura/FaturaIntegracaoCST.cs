namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_INTEGRACAO_CST", EntityName = "FaturaIntegracaoCST", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST", NameType = typeof(FaturaIntegracaoCST))]
    public class FaturaIntegracaoCST : EntidadeBase
    {
        public FaturaIntegracaoCST() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCS_CST", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CST { get; set; }

        public virtual string Descricao
        {
            get
            {
                return CST;
            }
        }
    }
}
