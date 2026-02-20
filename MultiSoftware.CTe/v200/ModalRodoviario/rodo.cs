namespace MultiSoftware.CTe.v200.ModalRodoviario
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class rodo
    {

        private string rNTRCField;

        private string dPrevField;

        private rodoLota lotaField;

        private string cIOTField;

        private rodoOcc[] occField;

        private rodoValePed[] valePedField;

        private rodoVeic[] veicField;

        private rodoLacRodo[] lacRodoField;

        private rodoMoto[] motoField;

        /// <remarks/>
        public string RNTRC
        {
            get
            {
                return this.rNTRCField;
            }
            set
            {
                this.rNTRCField = value;
            }
        }

        /// <remarks/>
        public string dPrev
        {
            get
            {
                return this.dPrevField;
            }
            set
            {
                this.dPrevField = value;
            }
        }

        /// <remarks/>
        public rodoLota lota
        {
            get
            {
                return this.lotaField;
            }
            set
            {
                this.lotaField = value;
            }
        }

        /// <remarks/>
        public string CIOT
        {
            get
            {
                return this.cIOTField;
            }
            set
            {
                this.cIOTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("occ")]
        public rodoOcc[] occ
        {
            get
            {
                return this.occField;
            }
            set
            {
                this.occField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("valePed")]
        public rodoValePed[] valePed
        {
            get
            {
                return this.valePedField;
            }
            set
            {
                this.valePedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("veic")]
        public rodoVeic[] veic
        {
            get
            {
                return this.veicField;
            }
            set
            {
                this.veicField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("lacRodo")]
        public rodoLacRodo[] lacRodo
        {
            get
            {
                return this.lacRodoField;
            }
            set
            {
                this.lacRodoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("moto")]
        public rodoMoto[] moto
        {
            get
            {
                return this.motoField;
            }
            set
            {
                this.motoField = value;
            }
        }
    }
}
