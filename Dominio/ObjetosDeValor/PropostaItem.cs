namespace Dominio.ObjetosDeValor
{
    public class PropostaItem
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public bool Excluir { get; set; }

        public string Tipo { get; set; }
    }
}
