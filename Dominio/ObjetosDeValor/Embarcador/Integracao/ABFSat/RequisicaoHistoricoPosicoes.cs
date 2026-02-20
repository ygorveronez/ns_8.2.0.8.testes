using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat
{
    public class RequisicaoHistoricoPosicoes
    {
        [JsonPropertyName("PropertyName")]
        public string NomeDaPropriedade { get; set; }

        [JsonPropertyName("Condition")]
        public string Condicao { get; set; }

        [JsonPropertyName("Value")]
        public string Valor { get; set; }

        public RequisicaoHistoricoPosicoes CarregarRequisicaoAPartirDasOpcoesDaConfiguracaoDaTecnologia(
            List<Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao> configuracaoIntegracaoTecnologiaMonitoramentoOpcoes
        )
        {
            for (int i = 0; i < configuracaoIntegracaoTecnologiaMonitoramentoOpcoes.Count; i++)
            {
                var opcao = configuracaoIntegracaoTecnologiaMonitoramentoOpcoes[i];
                
                switch(opcao.Key)
                {
                    case "Condition":
                        Condicao = opcao.Value;
                        break;
                    case "PropertyName":
                        NomeDaPropriedade = opcao.Value;
                        break;
                    case "MinutosDescontarHoraAtualValue":
                        if (int.TryParse(opcao.Value, out int minutosDescontar))
                        {
                            Valor = DateTime.Now.AddMinutes(-minutosDescontar).ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
                        }
                        break;
                    default:
                        break;
                }
            }

            return this;
        }
    }
}
