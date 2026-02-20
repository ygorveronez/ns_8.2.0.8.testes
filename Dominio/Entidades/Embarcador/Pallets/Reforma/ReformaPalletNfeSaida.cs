namespace Dominio.Entidades.Embarcador.Pallets.Reforma
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_REFORMA_NFE_SAIDA", EntityName = "ReformaPalletNfeSaida", Name = "Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida", NameType = typeof(ReformaPalletNfeSaida))]
    public class ReformaPalletNfeSaida : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XmlNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ReformaPallet", Column = "PAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ReformaPallet ReformaPallet { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(ReformaPalletNfeSaida other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
