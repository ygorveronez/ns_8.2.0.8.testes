
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoValePedagio
    {
        SemParar = 1,
        Repom = 2,
        Target = 3,
        PagBem = 4,
        QualP = 5,
        DBTrans = 6,
        Pamcard = 7,
        DigitalCom = 8
    }

    public static class TipoIntegracaoValePedagioHelper
    {
        public static TipoIntegracao ObterTipoIntegracao(this TipoIntegracaoValePedagio tipoValePedagio)
        {
            switch (tipoValePedagio)
            {
                case TipoIntegracaoValePedagio.SemParar: return TipoIntegracao.SemParar;
                case TipoIntegracaoValePedagio.Repom: return TipoIntegracao.Repom;
                case TipoIntegracaoValePedagio.Target: return TipoIntegracao.Target;
                case TipoIntegracaoValePedagio.PagBem: return TipoIntegracao.PagBem;
                case TipoIntegracaoValePedagio.QualP: return TipoIntegracao.QualP;
                case TipoIntegracaoValePedagio.DBTrans: return TipoIntegracao.DBTrans;
                case TipoIntegracaoValePedagio.Pamcard: return TipoIntegracao.Pamcard;
                case TipoIntegracaoValePedagio.DigitalCom: return TipoIntegracao.DigitalCom;
                default: return TipoIntegracao.NaoPossuiIntegracao;
            }
        }

        public static string ObterDescricao(this TipoIntegracaoValePedagio tipoValePedagio)
        {
            switch (tipoValePedagio)
            {
                case TipoIntegracaoValePedagio.SemParar: return "SemPara";
                case TipoIntegracaoValePedagio.Repom: return "Repom";
                case TipoIntegracaoValePedagio.Target: return "Target";
                case TipoIntegracaoValePedagio.PagBem: return "PagBem";
                case TipoIntegracaoValePedagio.QualP: return "QualP";
                case TipoIntegracaoValePedagio.DBTrans: return "DBTrans";
                case TipoIntegracaoValePedagio.Pamcard: return "Pamcard";
                case TipoIntegracaoValePedagio.DigitalCom: return "DigitalCom";
                default: return string.Empty;
            }
        }
        public static string ObterPastaPorTipoIntegracao(TipoIntegracao tipoValePedagio)
        {
            switch (tipoValePedagio)
            {
                case TipoIntegracao.SemParar: return "SemPara";
                case TipoIntegracao.Repom: return "Repom";
                case TipoIntegracao.Target: return "Target";
                case TipoIntegracao.PagBem: return "PagBem";
                case TipoIntegracao.QualP: return "QualP";
                case TipoIntegracao.DBTrans: return "DBTrans";
                case TipoIntegracao.Pamcard: return "Pamcard";
                case TipoIntegracao.DigitalCom: return "DigitalCom";
                default: return string.Empty;
            }
        }
    }
}
