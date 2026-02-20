namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DevolucaoAcaoMotorista
    {
        AguardandoInstrucao = 0,
        AceitarDevolucao = 1,
        AutorizarRetornoMotorista = 2,
        Reentrega = 3
    }

    public static class DevolucaoAcaoHelper
    {
        public static string ObterDescricao(this DevolucaoAcaoMotorista tipo)
        {
            switch (tipo)
            {
                case DevolucaoAcaoMotorista.AguardandoInstrucao: return "Aguardando instrução";
                case DevolucaoAcaoMotorista.AceitarDevolucao: return "Aceitar devolução";
                case DevolucaoAcaoMotorista.AutorizarRetornoMotorista: return "Autorizar retorno motorista";
                case DevolucaoAcaoMotorista.Reentrega: return "Reentrega";

                default: return string.Empty;
            }
        }
    }


}
