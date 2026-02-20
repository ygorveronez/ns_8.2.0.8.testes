using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class SMDetalhamentoColetaEntrega
    {
        /// <summary>
        /// Tipo: COLETA ou ENTREGA
        /// </summary>
        public string Tipo { get; set; }
        public int? CodIBGECidade { get; set; }
        public DateTime? DataHoraChegada { get; set; }
        public DateTime? DataHoraSaida { get; set; }
        public string Observacao { get; set; }
        public SMDetalhamentoColetaEntregaCliente Cliente { get; set; }
        public List<SMDetalhamentoColetaEntregaProduto> Produtos { get; set; }
    }
}
