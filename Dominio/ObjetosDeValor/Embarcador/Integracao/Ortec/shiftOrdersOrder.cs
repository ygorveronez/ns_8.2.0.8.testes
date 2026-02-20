namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class shifOrderOrder
    {

        private string order_referenceField;

        private int invoice_numberField;

        private decimal longitudeField;

        private decimal latitudeField;

        private int sequenceField;

        private System.DateTime startInstantField;

        private System.DateTime finishInstantField;

        private string orderStatusField;

        private shiftOrdersOrderAmount[] amountsField;

        /// <remarks/>
        public string order_reference
        {
            get
            {
                return this.order_referenceField;
            }
            set
            {
                this.order_referenceField = value;
            }
        }

        /// <remarks/>
        public int invoice_number
        {
            get
            {
                return this.invoice_numberField;
            }
            set
            {
                this.invoice_numberField = value;
            }
        }

        /// <remarks/>
        public decimal longitude
        {
            get
            {
                return this.longitudeField;
            }
            set
            {
                this.longitudeField = value;
            }
        }

        /// <remarks/>
        public decimal latitude
        {
            get
            {
                return this.latitudeField;
            }
            set
            {
                this.latitudeField = value;
            }
        }

        /// <remarks/>
        public int sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        public System.DateTime startInstant
        {
            get
            {
                return this.startInstantField;
            }
            set
            {
                this.startInstantField = value;
            }
        }

        /// <remarks/>
        public System.DateTime finishInstant
        {
            get
            {
                return this.finishInstantField;
            }
            set
            {
                this.finishInstantField = value;
            }
        }

        /// <remarks/>
        public string orderStatus
        {
            get
            {
                return this.orderStatusField;
            }
            set
            {
                this.orderStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("amount", IsNullable = false)]
        public shiftOrdersOrderAmount[] amounts
        {
            get
            {
                return this.amountsField;
            }
            set
            {
                this.amountsField = value;
            }
        }
    }
}
