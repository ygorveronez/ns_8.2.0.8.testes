namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIrregularidade
    {
        Nenhum = 1,
        ValorPrestacao = 2,
        SemLink = 3,
        CTeCancelado = 5,
        AliquotaICMSValorICMS = 7,
        CNPJTransportadora = 8,
        CSTICMS = 9,
        NFeVinculadaFrete = 10,
        TomadorFreteUnilever = 11,
        ValorPrestacaoServico = 12,
        MunicipioPrestacaoServico = 13,
        ValorTotalReceber = 15,
        CFOP = 16,
        RemetenteDestinatarioExpedidorRecebedor = 17,
        AliquotaISSValorISS = 18
    }

    public static class TipoIrregularidadeHelper
    {
        public static string ObterDescricao(this TipoIrregularidade tipoIrregularidade)
        {
            switch (tipoIrregularidade)
            {
                case TipoIrregularidade.Nenhum: return "Nenhum";
                case TipoIrregularidade.ValorPrestacao: return "Valor Prestação";
                case TipoIrregularidade.SemLink: return "Sem Link";
                case TipoIrregularidade.CTeCancelado: return "CT-e Cancelado";
                case TipoIrregularidade.AliquotaICMSValorICMS: return "Aliquota ICMS/Valor ICMS";
                case TipoIrregularidade.CNPJTransportadora: return "CNPJ Transportadora";
                case TipoIrregularidade.CSTICMS: return "CST ICMS";
                case TipoIrregularidade.NFeVinculadaFrete: return "NF-e Vinculada ao Frete";
                case TipoIrregularidade.TomadorFreteUnilever: return "Tomador do Frete Unilever";
                case TipoIrregularidade.ValorPrestacaoServico: return "Valor Prestação Serviço";
                case TipoIrregularidade.MunicipioPrestacaoServico: return "Município Prestção Serviço";
                case TipoIrregularidade.ValorTotalReceber: return "Valor Total a Receber";
                case TipoIrregularidade.CFOP: return "CFOP";
                case TipoIrregularidade.RemetenteDestinatarioExpedidorRecebedor: return "Remetente/Destinatario/Expedidor e Recebedor";
                case TipoIrregularidade.AliquotaISSValorISS: return "Aliquota ISS/ValorISS";
                default: return "";
            }
        }

    }
}