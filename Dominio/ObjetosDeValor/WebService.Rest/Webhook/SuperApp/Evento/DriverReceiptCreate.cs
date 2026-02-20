using System;
using System.Collections.Generic;
using System.Text.Json;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy;
using Newtonsoft.Json.Linq;

namespace Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento
{
    public class DataDriverReceiptCreate : SuperAppData
    {
        public StoppingPointDocument StoppingPointDocument { get; set; }
        public Location Location { get; set; }
        public Response Response { get; set; }
        public string DriverReceipt { get; set; }
    }

    public class Response
    {
        public string Checklist { get; set; }
        public string Flow { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        private object _response;

        [Newtonsoft.Json.JsonProperty("step")]
        public string StepId { get; set; }

        public string Type { get; set; }
        public string Label { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public string Status { get; set; }
        public object Response
        {
            get => _response; 
            set
            {
                _response = value;

                if (_response == null)
                    return;

                // Caso seja string direta
                if (_response is string s)
                {
                    ResponseString = s;
                }
                // Caso seja JsonElement (System.Text.Json)
                else if (_response is JsonElement el)
                {
                    switch (el.ValueKind)
                    {
                        case JsonValueKind.String:
                            ResponseString = el.GetString();
                            break;

                        case JsonValueKind.Array:
                            ResponseListString = new List<string>();
                            foreach (var item in el.EnumerateArray())
                            {
                                if (item.ValueKind == JsonValueKind.String)
                                    ResponseListString.Add(item.GetString());
                            }
                            break;

                        case JsonValueKind.Object:
                            // Verifica se contém as propriedades típicas de LocationResponse
                            if (el.TryGetProperty("location", out _))
                            {
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                };
                                ResponseLocation = JsonSerializer.Deserialize<LocationResponse>(el.GetRawText(), options);
                            }
                            break;
                    }
                }
            }
        }
        public string? ResponseString { get; set; }
        public List<string>? ResponseListString { get; set; }
        public LocationResponse? ResponseLocation { get; set; }
        public string ImageData { get; set; }
        public bool? Checked { get; set; }
        public ExternalInfo ExternalInfo { get; set; }
    }

    public class LocationResponse
    {
        public string? image { get; set; }
        public string situation { get; set; }
        public Location? location { get; set; }
    }
}
