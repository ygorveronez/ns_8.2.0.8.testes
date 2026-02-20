namespace Dominio.ObjetosDeValor
{
    public class ConfiguracaoAverbacaoClientes
    {
        public int Id { get; set; }

        public string CnpjCliente { get; set; }

        public double Codigo { get; set; }

        public string Nome { get; set; }

        public string Tipo { get; set; }

        public string IntegradoraAverbacao { get; set; }

        public string CodigoAverbacao { get; set; }

        public string UsuarioAverbacao { get; set; }

        public string SenhaAverbacao { get; set; }

        public string TokenAverbacao { get; set; }

        public string TipoCTeAverbacao { get; set; }

        public bool RaizCNPJ { get; set; }

        public bool NaoAverbar { get; set; }

        public bool Excluir { get; set; }
    }
}
