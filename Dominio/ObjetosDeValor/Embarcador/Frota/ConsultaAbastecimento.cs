namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class ConsultaAbastecimento
    {
        public int Codigo { get; set; }
        public string Data { get; set; }
        public Veiculo Veiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.Motorista Motorista { get; set; }
        public Equipamento Equipamento { get; set; }
        public int Horimetro { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento TipoAbastecimento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Posto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto Produto { get; set; }
        public string Documento { get; set; }
        public decimal Kilometragem { get; set; }
        public decimal Litros { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }
        public string Situacao { get; set; }
        public string NumeroAcerto { get; set; }
    }
}
