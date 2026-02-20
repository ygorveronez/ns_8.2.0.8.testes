using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.TransSat
{
    public class SolicitacaoMonitoramento
    {
        public Cliente CLIENTE { get; set; }
        public Viagem VIAGEM { get; set; }
        public Veiculo VEICULO { get; set; }
        public Carga CARGA { get; set; }
        public Planejamento PLANEJAMENTO { get; set; }
        public string OBS { get; set; }
        public string MANTER_SINAL { get; set; }
    }

    public class Viagem
    {
        public string LOCAL_ORIGEM { get; set; }
        public List<string> LOCAL_DESTINO { get; set; }
        public string ENDERECO_ORIGEM { get; set; }
        public List<string> ENDERECO_DESTINO { get; set; }
        public string DATA_INICIO { get; set; }
        public string HORA_INICIO { get; set; }
        public string DATA_FIM { get; set; }
        public string HORA_FIM { get; set; }
        public string NF { get; set; }
    }

    public class Cliente
    {
        public string SOLICITANTE { get; set; }
        public string EMAIL_RETORNO { get; set; }
    }

    public class Telefone
    {
        public List<string> TELEFONES { get; set; }
        public List<string> TIPO { get; set; }
        public List<string> CONTATO { get; set; }
    }

    public class Veiculo
    {
        public string PLACA { get; set; }
        public List<string> PLACA_CARRETA { get; set; }
        public string MOTORISTA_CPF { get; set; }
        public string AJUDANTE_CPF { get; set; }
        public Telefone MOTORISTA_TELEFONE { get; set; }
        public Telefone AJUDANTE_TELEFONE { get; set; }
    }
    public class Carga
    {
        public string DATA { get; set; }
        public decimal VALOR { get; set; } 
        public string TIPO { get; set; }
        public string COD_TIPO { get; set; }
        public int PESO { get; set; }
        public string TEMPERATURA { get; set; }
    }
    public class Planejamento
    {
        public List<string> ROTEIRO { get; set; }
        public int ROTA { get; set; }
    }

    public class SolicitacaoMonitoramentoEnvio
    {
        public SolicitacaoMonitoramento sm { get; set; }
    }
}
