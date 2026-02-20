namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class Equipamento
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }


        public string Chassi { get; set; }
        public bool Ativo { get; set; }
        public decimal Hodometro { get; set; }
        public decimal Horimetro { get; set; }
        public string DataAquisicao { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Segmento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }
        public string Cor { get; set; }
        public string Renavan { get; set; }
        public decimal CapacidadeTanque { get; set; }
    }
}
