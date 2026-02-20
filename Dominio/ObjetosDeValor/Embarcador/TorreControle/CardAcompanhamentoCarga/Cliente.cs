using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga
{
    public class Cliente
    {
        public double CLI_CGCCPF { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Numero { get; set; }
        public string Nome { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DescricaoCidade { get; set; }
        public List<NotaFiscal> NotaFiscal { get; set; }
        public List<Pedido> Pedido { get; set; }
        public int PedidoEmMaisCargas { get; set; }
    }
}
