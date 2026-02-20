namespace Dominio.ObjetosDeValor.Embarcador.Integracao.EagleTrack
{
    public class Positions
    {
        public string placa { get; set; }
        public string data { get; set; }
        public bool? ignicao { get; set; }
        public int? velocidade { get; set; }
        public string endereco { get; set;}
        public string cidade { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string idRastreador { get; set; }
    }
}
