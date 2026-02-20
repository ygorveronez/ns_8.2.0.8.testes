using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTechnicalManager
    {
        /// <summary>
        /// Name of the Person to be Contacted(tag xContato)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// CNPJ of the Legal Entity Responsible for the System Used in the issue of the Electronic Tax Document(tag CNPJ)
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Tag fone
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// Tag idCSRT
        /// </summary>
        public string idCSRT { get; set; }

        /// <summary>
        /// Tag hashCSRT
        /// </summary>
        public string hashCSRT { get; set; }
    }
}