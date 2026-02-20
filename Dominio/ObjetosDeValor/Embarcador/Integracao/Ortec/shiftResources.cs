namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class shiftResources
    {

        private shiftResourcesTruck truckField;

        private shiftResourcesDriver driverField;

        /// <remarks/>
        public shiftResourcesTruck truck
        {
            get
            {
                return this.truckField;
            }
            set
            {
                this.truckField = value;
            }
        }

        /// <remarks/>
        public shiftResourcesDriver driver
        {
            get
            {
                return this.driverField;
            }
            set
            {
                this.driverField = value;
            }
        }
    }
}
