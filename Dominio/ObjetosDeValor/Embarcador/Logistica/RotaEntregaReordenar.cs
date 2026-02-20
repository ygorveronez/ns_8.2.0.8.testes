namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class RotaEntregaReordenar
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa { get; set; }
        public Coordenadas coordenadas { get; set; }
        public bool coleta { get; set; }
        public bool Finalizada { get; set; }
        public int Ordem { get; set; }
        public int CodigoEntrega { get; set; }
    }
}