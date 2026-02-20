namespace Dominio.ObjetosDeValor.WebService.OrdemServico
{
    public class OrdemServicoItens
    {
        public string Codigo { get; set; }

        public string CodigoItem { get; set; }

        public string Descricao { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal ValorTotal { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario Funcionario { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario FuncionarioAuxiliar { get; set; }

        public int KilometroInicial { get; set; }

        public int KilometroFinal { get; set; }

        public int KilometroTotal { get; set; }

        public decimal ValorKilometro { get; set; }

        public decimal ValorTotalKilometro { get; set; }

        public string HoraInicial { get; set; }

        public string HoraFinal { get; set; }

        public string HoraTotal { get; set; }

        public decimal ValorHora { get; set; }

        public decimal ValorTotalHora { get; set; }

        public decimal ValorDesconto { get; set; }

        public int KilometroInicial2 { get; set; }

        public int KilometroFinal2 { get; set; }

        public int KilometroTotal2 { get; set; }

        public string HoraInicial2 { get; set; }

        public string HoraFinal2 { get; set; }

        public string HoraTotal2 { get; set; }

    }
}
