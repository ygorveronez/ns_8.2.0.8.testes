using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RemarkSped
    {
        OutrosServicos = 0,
        Cabotagem = 1,
        Container = 2,
        DnD = 3,
        FaturamentoMercadoInterno = 4,
        Feeder = 5,
        NoShow = 6,
        Rodo = 7,
        TrocaDeEspaco = 8
    }

    public static class RemarkSpedHelper
    {
        public static string ObterDescricao(this RemarkSped? tipo)
        {
            switch (tipo)
            {
                case RemarkSped.OutrosServicos: return "Outros Serviços";
                case RemarkSped.Cabotagem: return "Cabotagem";
                case RemarkSped.Container: return "Container";
                case RemarkSped.DnD: return "DnD";
                case RemarkSped.FaturamentoMercadoInterno: return "Faturamento Mercado Interno";
                case RemarkSped.Feeder: return "Feeder";
                case RemarkSped.NoShow: return "No Show";
                case RemarkSped.Rodo: return "Rodo";
                case RemarkSped.TrocaDeEspaco: return "Troca de Espaço";
                default: return string.Empty;
            }
        }
    }
}
