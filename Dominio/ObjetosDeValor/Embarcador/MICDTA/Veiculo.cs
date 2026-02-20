namespace Dominio.ObjetosDeValor.Embarcador.MICDTA
{
    public class Veiculo
    {
        public string chassi { get; set; }
        public int anoFabricacao { get; set; }
        public string marca { get; set; }
        public string capacidadeTracao { get; set; }
        public Truck truck { get; set; }
        public Condutor condutor { get; set; }
        public Proprietario proprietario { get; set; }
        public string observacoes { get; set; }
    }
}
