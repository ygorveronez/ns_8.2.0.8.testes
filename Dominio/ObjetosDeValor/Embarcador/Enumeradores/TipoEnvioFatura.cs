namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEnvioFatura
    {
        Todos = 0,
        SomenteFatura = 1,
        SomenteCTe = 2,
        SomenteCTeSemXML = 3,
        CTeFaturaSemXML = 4,
        PDFCTeFaturaAgrupado = 5,
        TodosOsDocumentosDaFatura = 6,
        EnviarTodosDocumentosPDFNFSe = 7

    }

    public static class TipoEnvioFaturaHelper
    {
        public static string ObterDescricao(this TipoEnvioFatura situacao)
        {
            switch (situacao)
            {
                case TipoEnvioFatura.Todos: return "Todos";
                case TipoEnvioFatura.SomenteFatura: return "Somente a Fatura";
                case TipoEnvioFatura.SomenteCTe: return "Somente o CT-e";
                case TipoEnvioFatura.SomenteCTeSemXML: return "PDF do CTe sem o xml do CTe";
                case TipoEnvioFatura.CTeFaturaSemXML: return "PDF do CTe e PDF da Fatura sem o xml do CTe";
                case TipoEnvioFatura.PDFCTeFaturaAgrupado: return "PDF de CT-e + Fatura agrupado";
                case TipoEnvioFatura.TodosOsDocumentosDaFatura: return "Todos os documentos da fatura";
                case TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe: return "Enviar Todos os documentos + PDF da NFSe";
                default: return string.Empty;
            }
        }
    }
}
