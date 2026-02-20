using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest
{
    public class AgrupamentoCargasReceberDadosTransporte
    {
        public List<Operation> operation { get; set; }
    }

    public class Driver
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string dni { get; set; }  // CPF motorista
    }

    public class Event
    {

        public string code { get; set; }

        public string description { get; set; }

        public string purchase_order { get; set; }

        public string timestamp { get; set; }
    }

    public class Operation
    {

        public string code { get; set; }  // Código da carga agrupada no multi

        public List<string> order_container { get; set; }  // Informação necessária apenas para o evento “MS”) (Será utilizado para agrupar as cargas no Multi. É o campo Numero Ordem no Multi)

        public string vehicle_code { get; set; }  // Placas

        public string employer_code { get; set; }  // Código integração transportador

        public Driver driver { get; set; }

        public List<Event> events { get; set; }
    }


}
