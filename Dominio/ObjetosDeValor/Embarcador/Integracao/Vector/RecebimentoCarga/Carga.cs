using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga
{
    public class Carga
    {
        /// <summary>
        /// Número da Carga do ME
        /// </summary>
        [JsonProperty("TripIdentifier")]
        public string NumeroCarga { get; set; }

        /// <summary>
        /// Número do protocolo da carga
        /// </summary>
        [JsonProperty("ShipperRequestNumber")]
        public int Protocolo { get; set; }

        /// <summary>
        /// Data da criação da Carga
        /// </summary>
        [JsonProperty("TripDate")]
        public DateTime DataCricao { get; set; }

        /// <summary>
        /// Data final da disponibilidade da carga 
        /// </summary>
        [JsonProperty("EndDate")]
        public DateTime? DataFinalizacao { get; set; }

        /// <summary>
        /// Descrição do Tipo de Carga da carga
        /// </summary>
        [JsonProperty("CargoType")]
        public string TipoCarga { get; set; }

        /// <summary>
        /// Enum do Tipo de Carga da carga
        /// </summary>
        [JsonProperty("CargoTypeEnum")]
        public string TipoCargaEnum { get; set; }

        /// <summary>
        /// Carga Manual
        /// </summary>
        [JsonProperty("IsManual")]
        public bool EhManual { get; set; }

        /// <summary>
        /// Descrição do Modelo Veicular da Carga
        /// </summary>
        [JsonProperty("VehicleType")]
        public string ModeloVeicular { get; set; }

        /// <summary>
        /// Valor de Frete da Carga
        /// </summary>
        [JsonProperty("FreightRate")]
        public decimal ValorFrete { get; set; }

        /// <summary>
        /// Valor Total de Frete da Carga
        /// </summary>
        [JsonProperty("FreightRateTotal")]
        public decimal ValorTotalFrete { get; set; }

        /// <summary>
        /// Identificador da forma de divulgação *
        /// </summary>
        [JsonProperty("DisclousureType")]
        public string IdentificadorFormaDivulgacao { get; set; }

        /// <summary>
        /// Observações necessárias
        /// </summary>
        [JsonProperty("Comments")]
        public string Observacoes { get; set; }

        /// <summary>
        /// Mercadoria destinada a Exportação?
        ///   false - Não(default)
        ///   true - Para Exportacao
        /// </summary>
        [JsonProperty("IsExport")]
        public bool EhExportacao { get; set; }

        /// <summary>
        /// Lista de CPF/CNPJ para divulgar com o
        /// método de empresas / ou Motoristas aos
        /// quais estaría direccionado o Lote, no caso
        /// em que o Embarcador quizer definir
        /// </summary>
        [JsonProperty("Parceiros")]
        public List<string> Parceiros { get; set; }

        /// <summary>
        /// Lista de Objetos Troca Nota
        /// </summary>
        [JsonProperty("TrocaNotaList")]
        public List<TrocaNota> TrocaNota { get; set; }

        /// <summary>
        /// Lista de Objetos Troca Nota
        /// </summary>
        [JsonProperty("Stops")]
        public List<Parada> Paradas { get; set; }
    }
}
