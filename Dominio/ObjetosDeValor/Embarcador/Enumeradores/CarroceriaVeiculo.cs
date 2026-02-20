namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CarroceriaVeiculo
    {
        GradeBaixa = 1,
        GradeAlta = 2,
        Sider = 3,
        Bau = 4,
        GradeBaixaEOuSider = 5,
        SiderEOuBau = 6,
        Silo = 7,
        Basculante = 8,
    }
    public static class CarroceriaVeiculoHelper
    {
        public static string ObterDescricao(this CarroceriaVeiculo tipo)
        {
            switch (tipo)
            {
                case CarroceriaVeiculo.GradeBaixa: return "Grade Baixa";
                case CarroceriaVeiculo.GradeAlta: return "Grade Alta";
                case CarroceriaVeiculo.Sider: return "Sider";
                case CarroceriaVeiculo.Bau: return "Baú";
                case CarroceriaVeiculo.GradeBaixaEOuSider: return "Grade Baixa e/ou Sider";
                case CarroceriaVeiculo.SiderEOuBau: return "Sider e/ou Baú";
                case CarroceriaVeiculo.Silo: return "Silo";
                case CarroceriaVeiculo.Basculante: return "Basculante";

                default: return string.Empty;
            }
        }
    }
}
