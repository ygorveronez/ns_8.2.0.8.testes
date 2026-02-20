namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotivoIrregularidade
    {
        NaoSelecionado = 0,
        ProblemaComercial = 1,
        ErroFaturamento = 2,
        ProblemaOperacional = 3,
        Desacordo = 4
    }

    public static class TipoMotivoIrregularidadeHelper
    {
        public static string ObterDescricao(this TipoMotivoIrregularidade tipoMotivo)
        {
            switch (tipoMotivo)
            {
                case TipoMotivoIrregularidade.NaoSelecionado: return "Não selecionado";
                case TipoMotivoIrregularidade.ProblemaComercial: return "Problema comercial";
                case TipoMotivoIrregularidade.ErroFaturamento: return "Erro faturamento";
                case TipoMotivoIrregularidade.ProblemaOperacional: return "Problema operacional";
                case TipoMotivoIrregularidade.Desacordo: return "Desacordo";
                default: return string.Empty;
            }
        }
    }
}