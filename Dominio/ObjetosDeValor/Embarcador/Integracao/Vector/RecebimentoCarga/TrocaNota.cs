using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga
{
    public class TrocaNota
    {
        /// <summary>
        /// Número Identificador Interno da Primeira viagem que possui troca de notas
        /// </summary>
        [JsonProperty("TrocaNotaNumber")]
        public string NumeroTrocaNota { get; set; }

        /// <summary>
        /// Máscara: 99.999.999/9999-99
        /// Número do CNPJ do emissor da Segunda Nota
        /// * Obrigatório quando existir
        /// nr_solicitacao_troca_nota
        /// </summary>
        [JsonProperty("TrocaNotaTaxId")]
        public string CNPJEmissorSegundaNota { get; set; }
    }
}
