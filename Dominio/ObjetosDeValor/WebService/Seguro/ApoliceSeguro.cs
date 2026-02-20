namespace Dominio.ObjetosDeValor.WebService.Seguro
{
    public class ApoliceSeguro
    {
        public int Codigo { get; set; }
        public Embarcador.Pessoas.Pessoa Seguradora { get; set; }
        public string NumeroApolice { get; set; }
        public string NumeroAverbacao { get; set; }
        public string DataInicioVigencia { get; set; }
        public string DataFimVigencia { get; set; }
        public decimal ValorLimiteApolice { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro Responsavel { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao Averbadora { get; set; }
        public string Observacao { get; set; }
        public Averbacao Averbacao { get; set; }

    }
}
