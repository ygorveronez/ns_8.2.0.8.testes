namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class shiftOrdersOrderAmount
    {

        private string unit_codeField;

        private decimal valueField;

        /// <remarks/>
        public string unit_code
        {
            get
            {
                return this.unit_codeField;
            }
            set
            {
                this.unit_codeField = value;
            }
        }

        /// <remarks/>
        public decimal value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
