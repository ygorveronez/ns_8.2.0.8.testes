namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class DivisaoCapacidadeModeloVeicular
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public decimal Capacidade { get; set; }
        public int? Piso { get; set; }
        public int? Coluna { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.UnidadeDeMedida UnidadeDeMedida { get; set; }
    }
}
