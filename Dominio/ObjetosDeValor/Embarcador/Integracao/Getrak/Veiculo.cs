using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Getrak
{
    public class Veiculo
    {
        public int id_veiculo { get; set; }
        public string modulo { get; set; }
        public string placa { get; set; }
        public string icone { get; set; }
        public int lig { get; set; }
        public string subcliente_nome { get; set; }
        public int subcliente_id { get; set; }
        public string cliente_nome { get; set; }
        public int cliente_id { get; set; }
        public DateTime data { get; set; }
        public DataVeiculo datastatus { get; set; }
        public string color { get; set; }
        public DataVeiculo dataServidor { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
        public double? lng { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string ano { get; set; }
        public string anomodelo { get; set; }
        public string cor { get; set; }
        public string contato { get; set; }
        public string telcontato { get; set; }
        public int vel { get; set; }
        public int velocidade { get; set; }
        public int hodometro { get; set; }
        public int horimetro { get; set; }
        public decimal latencia { get; set; }
        public int fix { get; set; }
        public string equipament { get; set; }
        public string num_chip { get; set; }
        public string celmodulo { get; set; }
        public string apn { get; set; }
        public string operadora { get; set; }
        public string apelido { get; set; }
        public string chassi { get; set; }
        public int status_online { get; set; }
        public string tipo { get; set; }
    }

}
