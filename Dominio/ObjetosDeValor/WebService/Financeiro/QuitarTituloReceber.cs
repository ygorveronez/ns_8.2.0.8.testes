using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.WebService.Financeiro
{
    public class QuitarTituloReceber
    {
        public int protocolo { get; set; }
        public string dataPagamento { get; set; }
        public string observacao { get; set; }
        public decimal valorAcrescimo { get; set; }
        public decimal valorDesconto { get; set; }
        public decimal valorOriginal { get; set; }
        public decimal valorPago { get; set; }
        public string codigoIntegracaoFormaPagamento { get; set; }
    }
}
