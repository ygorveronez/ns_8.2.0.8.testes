using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat
{
    public class RetornoSolicitacaoMonitoramentoSucesso
    {
        public bool STATUS { get; set; }
        public SolicitacaoMonitoramentoEnvio SM { get; set; }
        public string CDSOLICITACAO { get; set; }
    }

    public class RetornoSolicitacaoMonitoramentoFalha
    {
        public bool STATUS { get; set; }
        public List<string> ERRO { get; set; }

    }

    public class RetornoSolicitacaoMonitoramento
    {
        public string IP { get; set; }
        public RetornoSolicitacaoMonitoramentoInterna SM { get; set; }
        public SolicitacaoMonitoramentoEnvio REQUEST { get; set; }
        public string MANTER_SINAL { get; set; }
    }

    public class RetornoSolicitacaoMonitoramentoInterna
    {
        public string EMPRESA { get; set; }
        public string CNPJ { get; set; }
        public string TELEFONE { get; set; }
        public string EMAIL { get; set; }
        public Viagem VIAGEM { get; set; }
        public Carga CARGA { get; set; }
        public RetornoPlanejamento PLANEJAMENTO { get; set; }
        public string OBS { get; set; }
    }

    public class RetornoPlanejamento
    {
        public Rota ROTA { get; set; }
        public List<string> ROTEIRO { get; set; }
    }

    public class Rota
    {
        public string CDROTA { get; set; }
    }
}
