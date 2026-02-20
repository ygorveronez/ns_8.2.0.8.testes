namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class RotaDestinatarioOrdem
    {
        public Dominio.Entidades.Cliente Cliente { get; set; }
        public int Ordem { get; set; }
        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }
    }
}

