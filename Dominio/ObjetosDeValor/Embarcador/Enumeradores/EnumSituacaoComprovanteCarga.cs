namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoComprovanteCarga
    {
        Pendente = 0,
        Recebido = 1,
        Justificado = 2,
    }

    public static class SituacaoComprovanteCargaHelper
    {
        public static string ObterDescricao(this SituacaoComprovanteCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoComprovanteCarga.Pendente: return "Pendente";
                case SituacaoComprovanteCarga.Recebido: return "Recebido";
                case SituacaoComprovanteCarga.Justificado: return "Justificado";
                default: return "Pendente";
            }
        }

    }
}
