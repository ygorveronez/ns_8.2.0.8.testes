namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoTelaLicenca
    {
        Todos = 0,
        Funcionario = 1,
        Motorista = 2,
        Pessoa = 3,
        Veiculo = 4
    }

    public static class TipoTelaLicencaHelper
    {
        public static string ObterDescricao(this TipoTelaLicenca tipoTelaLicenca)
        {
            switch (tipoTelaLicenca)
            {
                case TipoTelaLicenca.Funcionario: return "Funcionário";
                case TipoTelaLicenca.Motorista: return "Motorista";
                case TipoTelaLicenca.Pessoa: return "Pessoa";
                case TipoTelaLicenca.Veiculo: return "Veículo";
                default: return "Todos";
            }
        }
    }
}
