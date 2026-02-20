namespace Dominio.ObjetosDeValor.WebService.Rest.Monitoramento
{
    public class Posicao
    {
        public string id_dispositivo { get; set; }
        public string placa { get; set; }
        public string data { get; set; }
        public string data_rastreadora { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }
}
