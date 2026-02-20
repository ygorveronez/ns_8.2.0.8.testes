using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.PortalCliente
{
    public class ParcelaView
    {
        public int ParcelaId { get; set; }
        public int Sequencia { get; set; }
        public string Vencimento { get; set; }
        public string Situacao { get; set; }
        public decimal Valor { get; set; }
    }
}
