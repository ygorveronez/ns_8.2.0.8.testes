using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec
{

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ShiftOrders
    {

        private List<shifOrderOrder> orderField;


        [System.Xml.Serialization.XmlArrayItemAttribute("order", IsNullable = false)]
        public List<shifOrderOrder> shiftOrders
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
            }
        }
    }
}

