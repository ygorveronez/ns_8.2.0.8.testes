namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GatilhoIrregularidade
    {
        MIROBloqueioR = 1,
        PendenteSubstituicaoDocumento = 2,
        SemLink = 3,
        SemCalculo = 4,
        CTeCancelado = 5,
        NFeCancelada = 6,
        AliquotaICMSValorICMS = 7,
        CNPJTransportadora = 8,
        CSTICMS = 9,
        NFeVinculadaAoFrete = 10,
        TomadorFreteUnilever = 11,
        ValorPrestacaoServico = 12,
        MunicipioPrestacaoServico = 13,
        ValidarDadosNFSe = 14,
        ValorTotalReceber = 15,
        CFOP = 16,
        Participantes = 17,
        AliquotaISSValorISS = 18,

    }

    public static class GatilhoIrregularidadeHelper
    {
        public static string ObterDescricao(this GatilhoIrregularidade gatilhoIrregularidade)
        {
            switch (gatilhoIrregularidade)
            {
                case GatilhoIrregularidade.MIROBloqueioR: return "1 - MIRO Bloqueio R - Sem tratativas prévia de preço Módulo de Controle";
                case GatilhoIrregularidade.PendenteSubstituicaoDocumento: return "2 - Pendente de substituição de documento";
                case GatilhoIrregularidade.SemLink: return "3 - Sem Link";
                case GatilhoIrregularidade.SemCalculo: return "4 - Sem Cálculo - Falta Pré CT-e";
                case GatilhoIrregularidade.CTeCancelado: return "5 - CT-e Cancelado";
                case GatilhoIrregularidade.NFeCancelada: return "6 - NF-e Cancelada";
                case GatilhoIrregularidade.AliquotaICMSValorICMS: return "7 - Alíquota ICMS/Valor ICMS";
                case GatilhoIrregularidade.CNPJTransportadora: return "8 - CNPJ Transportadora";
                case GatilhoIrregularidade.CSTICMS: return "9 - CST ICMS";
                case GatilhoIrregularidade.NFeVinculadaAoFrete: return "10 - NF-e Vinculada ao Frete";
                case GatilhoIrregularidade.TomadorFreteUnilever: return "11 - Tomador do Frete Unilever";
                case GatilhoIrregularidade.ValorPrestacaoServico: return "12 - Valor prestação serviço";
                case GatilhoIrregularidade.MunicipioPrestacaoServico: return "13 - Município Prestação Serviço";
                case GatilhoIrregularidade.ValidarDadosNFSe: return "14 - Validar dados NFS-e";
                case GatilhoIrregularidade.ValorTotalReceber: return "15 - Valor Total a Receber";
                case GatilhoIrregularidade.CFOP: return "16 - CFOP";
                case GatilhoIrregularidade.Participantes: return "17 - Remetente / Destinatário / Expedidor e Recebedor";
                case GatilhoIrregularidade.AliquotaISSValorISS: return "18 - Alíquota ISS/Valor ISS";
                default: return "";
            }
        }

    }
}