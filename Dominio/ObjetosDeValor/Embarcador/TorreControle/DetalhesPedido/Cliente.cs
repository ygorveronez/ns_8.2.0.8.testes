namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido
{
    public class Cliente
    {
        public double CPFCNPJ { get; set; }

        public string CodigoCliente { get; set; }

        public string Nome { get; set; }

        public string IE { get; set; }

        public Endereco Endereco { get; set; }
    }
}
