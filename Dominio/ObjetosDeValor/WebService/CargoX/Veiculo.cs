namespace Dominio.ObjetosDeValor.WebService.CargoX
{
    public class Veiculo
    {
        public int CapacidadeKG { get; set; }
        public int CapacidadeM3 { get; set; }
        public string Placa { get; set; }
        public Pessoa Proprietario { get; set; }
        public string Renavam { get; set; }
        public string RNTRC { get; set; }
        public int Tara { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria TipoCarroceria { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado TipoRodado { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo TipoVeiculo { get; set; }
        public string UF { get; set; }
    }
}
