using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm
{
    public class RespostaConsultaPosicoesRastreSat
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("tempFuel")]
        public int TemperaturaCombustivel { get; set; }

        [JsonPropertyName("roamingOn")]
        public bool RoamingAtivo { get; set; }

        [JsonPropertyName("bigShiftY")]
        public int BigShiftY { get; set; }

        [JsonPropertyName("speed")]
        public decimal Velocidade { get; set; }

        [JsonPropertyName("tanksCount")]
        public int ContadorTanques { get; set; }

        [JsonPropertyName("regNum")]
        public string NumeroRegistro { get; set; }

        [JsonPropertyName("iconURL")]
        public string UrlIcone { get; set; }

        [JsonPropertyName("llsCount")]
        public int ContadorLls { get; set; }

        [JsonPropertyName("ignition")]
        public bool Ignicao { get; set; }

        [JsonPropertyName("regType")]
        public string TipoRegistro { get; set; }

        [JsonPropertyName("direction")]
        public int Direcao { get; set; }

        [JsonPropertyName("gsmOn")]
        public bool GsmAtivo { get; set; }

        [JsonPropertyName("fuelVolume")]
        public int VolumeCombustivel { get; set; }

        [JsonPropertyName("fuelVolume2")]
        public int VolumeCombustivel2 { get; set; }

        [JsonPropertyName("address")]
        public string Endereco { get; set; }

        [JsonPropertyName("useDiscreteOutput")]
        public bool UsarSaidaDiscreta { get; set; }

        [JsonPropertyName("discreteOutputStatus")]
        public bool StatusSaidaDiscreta { get; set; }

        [JsonPropertyName("engineWork")]
        public string TrabalhoMotor { get; set; }

        [JsonPropertyName("gpsDataValid")]
        public bool DadosGpsValidos { get; set; }

        [JsonPropertyName("voltage")]
        public decimal Voltagem { get; set; }

        [JsonPropertyName("lastDataDate")]
        public long DataUltimoDado { get; set; }

        [JsonPropertyName("lastConnDate")]
        public long DataUltimaConexao { get; set; }

        [JsonPropertyName("actualDate")]
        public long DataAtual { get; set; }

        [JsonPropertyName("eventDate")]
        public long DataEvento { get; set; }

        [JsonPropertyName("date")]
        public long Data { get; set; }

        [JsonPropertyName("bigIconURL")]
        public string UrlIconeGrande { get; set; }

        [JsonPropertyName("iconHeight")]
        public int AlturaIcone { get; set; }

        [JsonPropertyName("bigIconWidth")]
        public int LarguraIconeGrande { get; set; }
        
        [JsonPropertyName("bigIconHeight")]
        public int AlturaIconeGrande { get; set; }

        [JsonPropertyName("vehicleID")]
        public long IdVeiculo { get; set; }

        [JsonPropertyName("shiftY")]
        public int ShiftY { get; set; }

        [JsonPropertyName("terminal_id")]
        public long IdTerminal { get; set; }

        [JsonPropertyName("uuid")]
        public string UUID { get; set; }
    }
}
