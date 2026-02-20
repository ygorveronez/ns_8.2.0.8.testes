namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoEncerramentoCarga
    {
        EmEncerramento = 1,
        AgEncerramentoDocumentos = 2,
        AgEncerramentoCIOT = 3,
        AgEncerramentoMDFe = 4,
        AgIntegracao = 5,
        RejeicaoEncerramento = 6,
        Encerrada = 7,
    }

    public static class SituacaoEncerramentoCargaHelper
    {
        public static string Descricao(this SituacaoEncerramentoCarga situacaoCancelamentoCarga)
        {
            switch (situacaoCancelamentoCarga)
            {
                case SituacaoEncerramentoCarga.EmEncerramento: return "Em Encerramento";
                case SituacaoEncerramentoCarga.AgEncerramentoDocumentos: return "Aguarando Encerramento dos Documentos";
                case SituacaoEncerramentoCarga.AgEncerramentoCIOT: return "Encerrando o CIOT";
                case SituacaoEncerramentoCarga.AgEncerramentoMDFe: return "Encerrando os MDF-es";
                case SituacaoEncerramentoCarga.AgIntegracao: return "Ag. Integrações";
                case SituacaoEncerramentoCarga.RejeicaoEncerramento: return "Encerramento Rejeitado";
                case SituacaoEncerramentoCarga.Encerrada: return "Encerrada";
                default: return string.Empty;
            }
        }
    }
}
