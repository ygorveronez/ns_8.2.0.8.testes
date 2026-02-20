namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaMercante
    {
        Cancelada = 1,
        AguardandoEmissao = 2,
        PendenteEmissaoCTe = 3,
        PendenteMDFe = 4,
        PendenteMercante = 5,
        PendenteFaturamento = 6,
        PendenteIntegracaoCTe = 7,
        PendenteIntegracaoFatura = 8,
        ComErro = 9,
        Finalizada = 10,
        Anulada = 11,
        PendenteSVM = 12
    }

    public static class SituacaoCargaMercanteHelper
    {
        public static string ObterDescricao(this SituacaoCargaMercante situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaMercante.Cancelada: return "Canceladas";
                case SituacaoCargaMercante.AguardandoEmissao: return "Aguardando Emissão";
                case SituacaoCargaMercante.PendenteEmissaoCTe: return "Pendente Emissão CT-e";
                case SituacaoCargaMercante.PendenteMDFe: return "Pendente MDF-e";
                case SituacaoCargaMercante.PendenteMercante: return "Pendente Mercante";
                case SituacaoCargaMercante.PendenteSVM: return "Pendente SVM";
                case SituacaoCargaMercante.PendenteFaturamento: return "Pendente Faturamento";
                case SituacaoCargaMercante.PendenteIntegracaoCTe: return "Pendente Integração CT-e";
                case SituacaoCargaMercante.PendenteIntegracaoFatura: return "Pendente Integração Fatura";
                case SituacaoCargaMercante.ComErro: return "Com Erro";
                case SituacaoCargaMercante.Finalizada: return "Finalizada";
                case SituacaoCargaMercante.Anulada: return "Anuladas";
                default: return string.Empty;
            }
        }
    }
}
