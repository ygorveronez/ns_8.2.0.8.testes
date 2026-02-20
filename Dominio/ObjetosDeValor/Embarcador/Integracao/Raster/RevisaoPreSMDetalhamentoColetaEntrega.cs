using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class RevisaoSMDetalhamentoColetaEntrega
    {
        /// <summary>
        /// Tipo: COLETA ou ENTREGA
        /// </summary>
        public string Tipo { get; set; }
        public int? CodIBGECidade { get; set; }
        public RevisaoSMDetalhamentoColetaEntregaCliente Cliente { get; set; }
        public DateTime? DataHoraChegada { get; set; }
        public DateTime? DataHoraSaida { get; set; }
        public string Observacao { get; set; }
        public List<RevisaoSMDetalhamentoColetaEntregaProduto> Produtos { get; set; }
    }
}
