using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Veiculo
    {
        public int id { get; set; }
        public int codigorf { get; set; }
        public string odometroGps { get; set; }
        public long dataAquisicao { get; set; }
        public int distanciaKmFrete { get; set; }
        public int kmManual { get; set; }
        public string horimetroManual { get; set; }
        public string horimetroAtual { get; set; }
        public int kmAtual { get; set; }
        public string statusVenda { get; set; }
        public long dataAtivado { get; set; }
        public long dataCadastrado { get; set; }
        public long dataCancelado { get; set; }
        public string fuso { get; set; }
        public string deletado { get; set; }
        public string status { get; set; }
        public string finalizado { get; set; }
        public long renavam { get; set; }
        public string vin { get; set; }
        public int anoFabricacao { get; set; }
        public int anoModelo { get; set; }
        public string placa { get; set; }
        public long dataInstalacao { get; set; }
        public string tipoMonitoramento { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string cor { get; set; }
        public string descricao { get; set; }
        public string frota { get; set; }
        public string tipo { get; set; }
        public string assistencia { get; set; }
        public string usuarioCriacao { get; set; }
        public int proprietarioId { get; set; }
        public string proprietario { get; set; }
        public List<Grupo> grupos { get; set; }
        public List<Motorista> motoristas { get; set; }
        public List<Dispositivo> dispositivos { get; set; }
    }
}
