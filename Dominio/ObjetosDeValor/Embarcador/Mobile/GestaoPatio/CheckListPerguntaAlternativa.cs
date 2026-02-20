namespace Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio
{
    public sealed class CheckListPerguntaAlternativa
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public int Ordem { get; set; }

        public int Valor { get; set; }

        public bool Marcado { get; set; }
    }
}