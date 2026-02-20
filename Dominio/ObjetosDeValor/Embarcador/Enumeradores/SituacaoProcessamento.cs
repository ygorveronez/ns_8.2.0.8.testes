
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoProcessamento
    {
        AguardandoProcessamento = 0,
        Processado = 1
        //RegistroNaoEncontrado = 2
    }

    public static class SituacaoProcessamentoHelper
    {
        public static string ObterDescricao(this SituacaoProcessamento situacao)
        {
            switch (situacao)
            {
                case SituacaoProcessamento.AguardandoProcessamento:
                    return "Aguardando Processamento";
                case SituacaoProcessamento.Processado:
                    return "Processado";
                default:
                    return "";
            }
        }
    }
}
