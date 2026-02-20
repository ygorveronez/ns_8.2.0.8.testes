namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MotivoInconsistenciaGestaoDocumento
    {
        Todos = 0,
        SemCarga = 1,
        ValorFretePrestacao = 2,
        AliquotaICMS = 3,
        BaseCalculo = 4,
        ICMS = 5,
        CST = 6,
        CFOP = 7,
        Tomador = 8,
        Remetente = 9,
        Destinatario = 10,
        Expedidor = 11,
        Recebedor = 12,
        Origem = 13,
        Destino = 14,
        Emissor = 15,
        ValorTotalReceber = 16,
        TipoAmbiente = 17,
        TipoCTe = 18,
        CTeAnterior = 19,
        NotasFiscais = 20,
        EnvioPosteriorCarga = 21,
        CargaJaPossuiCTe = 22,
        ComponentesFreteDivergentes = 23,
        AprovacaoObrigatoria = 24
    }

    public static class MotivoInconsistenciaGestaoDocumentoHelper
    {
        public static string ObterDescricao(this MotivoInconsistenciaGestaoDocumento motivo)
        {
            switch (motivo)
            {
                case MotivoInconsistenciaGestaoDocumento.SemCarga: return "Sem Carga";
                case MotivoInconsistenciaGestaoDocumento.ValorFretePrestacao: return "Valor da Prestação";
                case MotivoInconsistenciaGestaoDocumento.AliquotaICMS: return "Alíquota ICMS";
                case MotivoInconsistenciaGestaoDocumento.BaseCalculo: return "Base de Cálculo ICMS";
                case MotivoInconsistenciaGestaoDocumento.ICMS: return "Valor do ICMS";
                case MotivoInconsistenciaGestaoDocumento.CST: return "CST";
                case MotivoInconsistenciaGestaoDocumento.CFOP: return "CFOP";
                case MotivoInconsistenciaGestaoDocumento.Tomador: return "Tomador";
                case MotivoInconsistenciaGestaoDocumento.Remetente: return "Remetente";
                case MotivoInconsistenciaGestaoDocumento.Destinatario: return "Destinatário";
                case MotivoInconsistenciaGestaoDocumento.Expedidor: return "Expedidor";
                case MotivoInconsistenciaGestaoDocumento.Recebedor: return "Recebedor";
                case MotivoInconsistenciaGestaoDocumento.Origem: return "Origem";
                case MotivoInconsistenciaGestaoDocumento.Destino: return "Destino";
                case MotivoInconsistenciaGestaoDocumento.Emissor: return "Emissor";
                case MotivoInconsistenciaGestaoDocumento.ValorTotalReceber: return "Valor Total a Receber";
                case MotivoInconsistenciaGestaoDocumento.TipoAmbiente: return "Tipo do Ambiente";
                case MotivoInconsistenciaGestaoDocumento.TipoCTe: return "Tipo do CTe";
                case MotivoInconsistenciaGestaoDocumento.CTeAnterior: return "CT-e Anterior";
                case MotivoInconsistenciaGestaoDocumento.EnvioPosteriorCarga: return "Carga enviada posteriormente";
                case MotivoInconsistenciaGestaoDocumento.NotasFiscais: return "Nota Fiscal";
                case MotivoInconsistenciaGestaoDocumento.CargaJaPossuiCTe: return "Carga já possui CTe";
                case MotivoInconsistenciaGestaoDocumento.ComponentesFreteDivergentes: return "Componentes de Frete Divergentes";
                case MotivoInconsistenciaGestaoDocumento.AprovacaoObrigatoria: return "Aprovação Obrigatória";
                default: return string.Empty;
            }
        }
    }
}
