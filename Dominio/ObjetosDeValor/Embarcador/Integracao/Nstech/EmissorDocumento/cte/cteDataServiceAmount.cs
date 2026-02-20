using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataServiceAmount
    {
        /// <summary>
        /// Total Service Provision Value(tag vTPrest)
        /// </summary>
        public decimal serviceAmount { get; set; }

        /// <summary>
        /// Amount to Receive(tag vRec)
        /// </summary>
        public decimal receivableServiceAmount { get; set; }

        /// <summary>
        /// Components of the Service Value(tag Comp)
        /// </summary>
        public List<cteDataServiceAmountServiceCompoment> serviceCompoment { get; set; }
    }

    public class cteDataServiceAmountServiceCompoment
    {
        /// <summary>
        /// Tag xNome
        /// </summary>
        public string compomentName { get; set; }

        /// <summary>
        /// Tag vComp
        /// </summary>
        public decimal compomentAmount { get; set; }
    }
}