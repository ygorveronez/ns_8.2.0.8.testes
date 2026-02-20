namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class Cliente
    {
        public double CpfCnpj { get; set; }

        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public string Tipo { get; set; }
    }
}
