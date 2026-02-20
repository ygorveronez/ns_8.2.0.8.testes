namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class shift
    {
        private uint id_shiftField;

        private int totalDistanceField;

        private System.DateTime startInstantField;

        private System.DateTime finishInstantField;

        private shiftResources resourcesField;

        private ShiftOrders ordersField;

        /// <remarks/>
        public uint id_shift
        {
            get
            {
                return this.id_shiftField;
            }
            set
            {
                this.id_shiftField = value;
            }
        }

        /// <remarks/>
        public int totalDistance
        {
            get
            {
                return this.totalDistanceField;
            }
            set
            {
                this.totalDistanceField = value;
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
        public shiftResources resources
        {
            get
            {
                return this.resourcesField;
            }
            set
            {
                this.resourcesField = value;
            }
        }


        public ShiftOrders orders

        {
            get
            {
                return this.ordersField;
            }
            set
            {
                this.ordersField = value;
            }
        }
    }
}
