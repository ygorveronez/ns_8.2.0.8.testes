namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Cliente
    {
        public string CpfCnpj { get; set; }

        public string Nome { get; set; }

        public string IE { get; set; }

        public GrupoPessoas GrupoPessoas { get; set; }

        public Endereco Endereco { get; set; }
    }
}
