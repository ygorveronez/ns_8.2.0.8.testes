using System;

namespace Servicos.Global
{
    public class TempoDeExecucao
    {
        #region Construtores

        #endregion Construtores

        #region Métodos Públicos

        public void SalvarLogExecucao(string nomeMetodo, int protocoloCarga, int protocoloPedido, string nomeArquivo, TimeSpan ts, bool tipo, string motivoErro)
        {
            Servicos.Log.TratarErro($"{nomeMetodo} - {(protocoloCarga > 0 ? $"ProtocoloCarga {protocoloCarga}" : "")} {(protocoloPedido > 0 ? "| Protocolo Pedido " + protocoloPedido : "")}| Tempo total levado: {ts.ToString(@"mm\:ss\:fff")} | Tipo Integração: {(tipo ? "Sucesso" : $"Falha | Motivo: {motivoErro}")}", nomeArquivo);
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}