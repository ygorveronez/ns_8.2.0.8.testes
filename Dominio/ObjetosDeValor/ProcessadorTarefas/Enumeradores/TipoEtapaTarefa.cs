using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores
{
    public enum TipoEtapaTarefa
    {
        QuebrarRequest = 0,
        ProcessarRequest = 1,
        RetornarIntegracao = 2,
        AdicionaRequestParaProcessamento = 3,
        GerarCarregamento = 4,
        AdicionarPedido = 5,
        GerarCarregamentoComRedespachos = 6,
        FecharCarga = 7,
        EnviarDigitalizacaoCanhoto = 8,
        AdicionarAtendimento = 9,
        GerarCarregamentoRoterizacao = 10
    }

    public static class TipoEtapaTarefaExtensions
    {
        public static string ObterDescricao(this TipoEtapaTarefa tipoEtapaTarefa)
        {
            return tipoEtapaTarefa switch
            {
                TipoEtapaTarefa.QuebrarRequest => "Quebrar Request",
                //case TipoEtapaTarefa.ProcessarRequest:
                //    return "Processando Request";
                TipoEtapaTarefa.RetornarIntegracao => "Retornar Integração",
                //case TipoEtapaTarefa.AdicionaRequestParaProcessamento:
                //    return "Adicionando Request para Processamento";
                TipoEtapaTarefa.GerarCarregamento => "Gerar Carregamento",
                TipoEtapaTarefa.AdicionarPedido => "Adicionar Pedido",
                TipoEtapaTarefa.GerarCarregamentoComRedespachos => "Gerar Carregamento com Redespachos",
                TipoEtapaTarefa.FecharCarga => "Fechar Carga",
                TipoEtapaTarefa.EnviarDigitalizacaoCanhoto => "Enviar Digitalização de Canhoto",
                TipoEtapaTarefa.AdicionarAtendimento => "Adicionar Atendimento",
                TipoEtapaTarefa.GerarCarregamentoRoterizacao => "Gerar Carregamento Roteirização",
                _ => string.Empty,
            };
        }

        public class ConfigFila
        {
            public string Nome { get; set; }
            public int WorkerCount { get; set; }
        }

        public static List<ConfigFila> ObterFilasComWorkers()
        {
            return new List<ConfigFila>
            {
                new ConfigFila { Nome = "default", WorkerCount = 5 },
                new ConfigFila { Nome = "quebrar_request", WorkerCount = 50 },
                new ConfigFila { Nome = "retornar_integracao", WorkerCount = 25 },
                new ConfigFila { Nome = "gerar_carregamento", WorkerCount = 10 },
                new ConfigFila { Nome = "adicionar_pedido", WorkerCount = 10 },
                new ConfigFila { Nome = "gerar_carregamento_com_redespachos", WorkerCount = 5 },
                new ConfigFila { Nome = "fechar_carga", WorkerCount = 10 },
                new ConfigFila { Nome = "enviar_digitalizacao_canhoto", WorkerCount = 10 },
                new ConfigFila { Nome = "adicionar_atendimento", WorkerCount = 10 },
                new ConfigFila { Nome = "gerar_carregamento_roteirizacao", WorkerCount = 10 },
            };
        }

        public static string ObterFila(this TipoEtapaTarefa tipoEtapaTarefa)
        {
            return tipoEtapaTarefa switch
            {
                TipoEtapaTarefa.QuebrarRequest => "quebrar_request",
                //case TipoEtapaTarefa.ProcessarRequest:
                //    return "processar_request";
                TipoEtapaTarefa.RetornarIntegracao => "retornar_integracao",
                //case TipoEtapaTarefa.AdicionaRequestParaProcessamento:
                //    return "adiciona_request_para_processamento";
                TipoEtapaTarefa.GerarCarregamento => "gerar_carregamento",
                TipoEtapaTarefa.AdicionarPedido => "adicionar_pedido",
                TipoEtapaTarefa.GerarCarregamentoComRedespachos => "gerar_carregamento_com_redespachos",
                TipoEtapaTarefa.FecharCarga => "fechar_carga",
                TipoEtapaTarefa.EnviarDigitalizacaoCanhoto => "enviar_digitalizacao_canhoto",
                TipoEtapaTarefa.AdicionarAtendimento => "adicionar_atendimento",
                TipoEtapaTarefa.GerarCarregamentoRoterizacao => "gerar_carregamento_roteirizacao",
                _ => string.Empty,
            };
        }
    }
}
