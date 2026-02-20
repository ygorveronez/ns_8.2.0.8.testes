namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCheckin
    {
        Invalido = 0,
        Carregamento = 1,
        Descarregamento = 2,
        Devolucao = 3,
        Transferencia = 4,
        Contingenciacarregamento = 5,
        Contingenciadescarregamento = 6,
        Contingenciatransferencia = 7,
        Contingenciadevolucao = 8
    }

    public static class TipoCheckinHelper
    {
        public static string ObterDescricao(this TipoCheckin tipoCheckin)
        {
            switch (tipoCheckin)
            {
                case TipoCheckin.Invalido: return "Inválido";
                case TipoCheckin.Carregamento: return "Carregamento";
                case TipoCheckin.Descarregamento: return "Descarregamento";
                case TipoCheckin.Devolucao: return "Devolução";
                case TipoCheckin.Transferencia: return "Transferência";
                case TipoCheckin.Contingenciacarregamento: return "Contingência Carregamento";
                case TipoCheckin.Contingenciadescarregamento: return "Contingência Descarregamento";
                case TipoCheckin.Contingenciatransferencia: return "Contingência Transferência";
                case TipoCheckin.Contingenciadevolucao: return "Contingência Devolução";
                default: return "";
            }
        }
        public static TipoCheckin ObterTipoPorDescricao(string descricao)
        {
            switch (descricao)
            {
                case "Invalido": return TipoCheckin.Invalido;
                case "Carregamento": return TipoCheckin.Carregamento;
                case "Descarregamento": return TipoCheckin.Descarregamento;
                case "Devolucao": return TipoCheckin.Devolucao;
                case "Transferencia": return TipoCheckin.Transferencia;
                case "Contingenciacarregamento": return TipoCheckin.Contingenciacarregamento;
                case "Contingenciadescarregamento": return TipoCheckin.Contingenciadescarregamento;
                case "Contingenciatransferência": return TipoCheckin.Contingenciatransferencia;
                case "Contingenciadevolucao": return TipoCheckin.Contingenciadevolucao;
                default: return TipoCheckin.Invalido;
            }
        }

    }

}
