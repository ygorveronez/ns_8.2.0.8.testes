using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.Agrupamento
{
    public class PedidoAgrupamento
    {
        public int IDIntegracao { get; set; }

        public string NumeroCarga { get; set; }

        public string NumeroPedido { get; set; }

        public string DataCarregamento { get; set; }

        public string DataPrevisaoEntrega { get; set; }

        public bool PedidoDeEncaixe { get; set; }
        public bool Reentrega { get; set; }

        public List<Embarcador.NFe.NotaFiscal> NotasFiscal { get; set; }

        public Embarcador.Pessoas.Pessoa Expedidor { get; set; }

        public Embarcador.Pessoas.Pessoa Recebedor { get; set; }
    }
}
