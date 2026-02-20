namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class EnviarNotificacaoSuperApp
    {
        public Driver driver { get; set; } = new Driver();
        public string title { get; set; }
        public string message { get; set; }
    }

    public class Driver
    {
        public Document document { get; set; } = new Document();
    }

    public class Document
    {
        public string type { get; set; }
        public string value { get; set; }
    }
}
