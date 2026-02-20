namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DirecionamentoOS
    {
        Abonar = 0,
        Faturar = 1,
        GerarND = 2,
        Nenhum = 3
    }

    public static class EnumDirecionamentoOSeHelper
    {
        public static string ObterDescricao(this DirecionamentoOS situacao)
        {
            switch (situacao)
            {
                case DirecionamentoOS.Abonar: return "Abonar";
                case DirecionamentoOS.Faturar: return "Faturar";
                case DirecionamentoOS.GerarND: return "Gerar ND";
                case DirecionamentoOS.Nenhum: return "Nenhum";
                default: return string.Empty;
            }
        }
    }
}
