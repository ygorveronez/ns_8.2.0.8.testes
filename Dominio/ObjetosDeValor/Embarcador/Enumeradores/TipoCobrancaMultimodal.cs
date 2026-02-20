namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCobrancaMultimodal
    {
        Nenhum = 0,
        BLLongoCurso = 1,
        NotaFiscalServico = 2,
        CTeRodoviario = 3,
        CTeMultimodal = 4,
        CTEAquaviario = 5
    }

    public static class TipoCobrancaMultimodalHelper
    {
        public static string ObterDescricao(this TipoCobrancaMultimodal tipo)
        {
            switch (tipo)
            {
                case TipoCobrancaMultimodal.Nenhum: return "Nenhum";
                case TipoCobrancaMultimodal.BLLongoCurso: return "1 - BL de Longo Curso";
                case TipoCobrancaMultimodal.NotaFiscalServico: return "2 - Nota Fiscal de Serviço";
                case TipoCobrancaMultimodal.CTeRodoviario: return "3 - CT-e Rodoviário";
                case TipoCobrancaMultimodal.CTeMultimodal: return "4 - CT-e Multimodal";
                case TipoCobrancaMultimodal.CTEAquaviario: return "5 - CT-e Aquaviario";
                default: return string.Empty;
            }
        }
        public static string ObterDescricaoModal(this TipoCobrancaMultimodal tipo)
        {
            switch (tipo)
            {
                case TipoCobrancaMultimodal.Nenhum: return "";
                case TipoCobrancaMultimodal.BLLongoCurso: return "1 - BL de Longo Curso";
                case TipoCobrancaMultimodal.NotaFiscalServico: return "2 - Nota Fiscal de Serviço";
                case TipoCobrancaMultimodal.CTeRodoviario: return "3 - CT-e Rodoviário";
                case TipoCobrancaMultimodal.CTeMultimodal: return "4 - CT-e Multimodal";
                case TipoCobrancaMultimodal.CTEAquaviario: return "5 - CT-e Aquaviario";
                default: return string.Empty;
            }
        }

        public static TipoModal ConverterTipoCobrancaMultimodal(this TipoCobrancaMultimodal tipo)
        {
            switch (tipo)
            {
                case TipoCobrancaMultimodal.Nenhum: return TipoModal.Todos;
                case TipoCobrancaMultimodal.BLLongoCurso: return TipoModal.Aquaviario;
                case TipoCobrancaMultimodal.NotaFiscalServico: return TipoModal.Rodoviario;
                case TipoCobrancaMultimodal.CTeRodoviario: return TipoModal.Rodoviario;
                case TipoCobrancaMultimodal.CTeMultimodal: return TipoModal.Multimodal;
                case TipoCobrancaMultimodal.CTEAquaviario: return TipoModal.Aquaviario;
                default: return TipoModal.Todos;
            }
        }

    }
}
