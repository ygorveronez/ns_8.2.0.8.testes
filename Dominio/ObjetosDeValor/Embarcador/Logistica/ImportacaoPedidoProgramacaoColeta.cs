using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class ImportacaoPedidoProgramacaoColeta
    {
        public bool Sucesso { get; set; }
        public string MensagemFalha { get; set; }

        public int Agrupamento { get; set; }
        public int Sequencia { get; set; }
        public Dominio.Entidades.Cliente Remetente { get; set; }
        public decimal Distancia { get; set; }
        public decimal QuantidadePlanejada { get; set; }
        public Dominio.Entidades.Veiculo Veiculo { get; set; }
        public DateTime DataCarregamento { get; set; }
        public Dominio.Entidades.Empresa Transportador { get; set; }
    }
}
