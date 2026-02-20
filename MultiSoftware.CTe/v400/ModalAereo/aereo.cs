namespace MultiSoftware.CTe.v400.ModalAereo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class aereo
    {

        private string nMinuField;

        private string nOCAField;

        private string dPrevAereoField;

        private aereoNatCarga natCargaField;

        private aereoTarifa tarifaField;

        private aereoPeri[] periField;

        /// <remarks/>
        public string nMinu
        {
            get
            {
                return this.nMinuField;
            }
            set
            {
                this.nMinuField = value;
            }
        }

        /// <remarks/>
        public string nOCA
        {
            get
            {
                return this.nOCAField;
            }
            set
            {
                this.nOCAField = value;
            }
        }

        /// <remarks/>
        public string dPrevAereo
        {
            get
            {
                return this.dPrevAereoField;
            }
            set
            {
                this.dPrevAereoField = value;
            }
        }

        /// <remarks/>
        public aereoNatCarga natCarga
        {
            get
            {
                return this.natCargaField;
            }
            set
            {
                this.natCargaField = value;
            }
        }

        /// <remarks/>
        public aereoTarifa tarifa
        {
            get
            {
                return this.tarifaField;
            }
            set
            {
                this.tarifaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("peri")]
        public aereoPeri[] peri
        {
            get
            {
                return this.periField;
            }
            set
            {
                this.periField = value;
            }
        }
    }
}
