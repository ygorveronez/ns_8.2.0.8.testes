using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envViagem
    {
        public string identificador { get; set; }

        public int? idOperacaoLogistico { get; set; }

        public int? tipoTransporte { get; set; }

        public int? idVeiculo { get; set; }

        public int? idCarreta1 { get; set; }

        public int? idCarreta2 { get; set; }

        public int? idMotorista { get; set; }

        public int? idMotorista2 { get; set; }

        public string dataInicio { get; set; }

        public string dataPrevisaoInicio { get; set; }

        public string dataFim { get; set; }

        public string dataPrevisaoFim { get; set; }

        public int? idClienteOrigem { get; set; }

        public int? idClienteOrigemEndereco { get; set; }

        public int? idClienteDestino { get; set; }

        public int? idClienteDestinoEndereco { get; set; }

        public int? idTomadorServico { get; set; }

        public int? idTomadorServicoEndereco { get; set; }

        public int? idFaixaTemperatura { get; set; }

        public int? idRota { get; set; }

        public string numeroPedido { get; set; }

        public string numeroCarga { get; set; }

        public string dataPrevisaoChegadaOrigem { get; set; }

        public string dataPrevisaoChegadaDestino { get; set; }

        public List<int> coletasEntregas { get; set; }

        public bool preferencial { get; set; }
    }
}