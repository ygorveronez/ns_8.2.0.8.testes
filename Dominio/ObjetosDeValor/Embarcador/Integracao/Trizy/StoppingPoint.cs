using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class StoppingPoint
    {
        public string status { get; set; }
        public Point? point { get; set; }
        public string? expectedAt { get; set; }
        public string? operation { get; set; }
        public Event events { get; set; }
        public List<Produto> products { get; set; }
        public string? observation { get; set; }
        public StoppingPointFeatures? features { get; set; }
        public List<StoppingPointDocument>? documents { get; set; }
        public string? externalId { get; set; }
        public List<Contact>? contacts { get; set; }
        public TextoInternacionalizado? description { get; set; }
        public string? title { get; set; }
        public string? subtitle { get; set; }
        public List<StoppingPointClient>? clients { get; set; }
        public List<InformacaoAdicional>? additionalInformation { get; set; }
    }

    public class StoppingPointFeatures
    {
        public FeatureDriverReceipt? driverReceipt { get; set; }
        public FeatureDriverReceipt? driverReceiptDocuments { get; set; }
        public bool? canPause { get; set; }
        public bool? selectClientInDriverReceipt { get; set; }
        public bool? selectClientInOccurrence { get; set; }
    }

    public class FeatureDriverReceipt
    {
        public bool? enabled { get; set; }
        public string? checklist { get; set; }
        public List<ChecklistStep>? template { get; set; }
    }

    public class StoppingPointDocument
    {
        public string type { get; set; }
        public string? key { get; set; }
        public string identifier { get; set; }
        public string? externalId { get; set; }
        public StoppingPointDocumentFeatures? features { get; set; }
        public List<StoppingPointDocumentItem>? items { get; set; }

    }

    public class StoppingPointDocumentItem
    {
        public TextoInternacionalizado label { get; set; }
        public string externalId { get; set; }
        public DadosProduto value { get; set; }
    }

    public class StoppingPointDocumentFeatures
    {
        public FeatureDriverReceipt? driverReceipt { get; set; }
    }

    public class StoppingPointClient
    {
        public Company company { get; set; }
    }
}