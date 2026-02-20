namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPreAgrupamentoCarga
    {
        EmEscrita = 1,
        AguardandoProcessamento = 2,
        AguardandoCarregamento = 3,
        Carregado = 4,
        ProblemaCarregamento = 5,
        SemCarga = 6,
        AguardandoRedespacho = 7,
        AguardandoCargasPararEncaixe = 8
    }

    public static class SituacaoPreAgrupamentoCargaHelper
    {
        public static bool IsPermitirAlterar(this SituacaoPreAgrupamentoCarga situacao)
        {
            return (situacao == SituacaoPreAgrupamentoCarga.ProblemaCarregamento) || (situacao == SituacaoPreAgrupamentoCarga.SemCarga);
        }

        public static bool IsPermitirExcluir(this SituacaoPreAgrupamentoCarga situacao)
        {
            return (situacao == SituacaoPreAgrupamentoCarga.Carregado) || (situacao == SituacaoPreAgrupamentoCarga.ProblemaCarregamento) || (situacao == SituacaoPreAgrupamentoCarga.SemCarga);
        }

        public static string ObterDescricao(this SituacaoPreAgrupamentoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoPreAgrupamentoCarga.AguardandoCarregamento: return "Aguardando Carregamento";
                case SituacaoPreAgrupamentoCarga.AguardandoProcessamento: return "Aguardando Processamento";
                case SituacaoPreAgrupamentoCarga.Carregado: return "Carregado";
                case SituacaoPreAgrupamentoCarga.ProblemaCarregamento: return "Problema no Carregamento";
                case SituacaoPreAgrupamentoCarga.AguardandoRedespacho: return "Ag. Carga de Redespacho";
                case SituacaoPreAgrupamentoCarga.AguardandoCargasPararEncaixe: return "Aguardando cargas para fazer o encaixe";
                case SituacaoPreAgrupamentoCarga.SemCarga: return "Sem todas as cargas";
                default: return string.Empty;
            }
        }
    }
}
