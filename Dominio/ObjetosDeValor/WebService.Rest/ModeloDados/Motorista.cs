using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Motorista
    {
        public string Nome { get; set; }

        public string CpfCnpj { get; set; }

        public string Telefone { get; set; }

        public DateTime? DataNascimento { get; set; }

        public DateTime? DataValidadeGerenciadoraRisco { get; set; }
    }
}
