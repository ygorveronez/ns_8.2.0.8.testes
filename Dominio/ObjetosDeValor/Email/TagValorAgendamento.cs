namespace Dominio.ObjetosDeValor.Email
{
    public class TagValorAgendamento
    {
        public TagValorAgendamento() { }

        public TagValorAgendamento(string tag)
        {
            Tag = tag;
            Valor = string.Empty;
        }

        public string Tag { get; set; }
        public string Valor { get; set; }
    }
}
