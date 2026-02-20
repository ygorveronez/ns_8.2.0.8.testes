using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum TipoLicenca
    {
        Todas = -1,
        Geral = 0,
        Motorista = 1,
        Veiculo = 2,
        Pessoa = 3,
        Tracao = 4,
        Reboque = 5
    }

    public static class TipoLicencaHelper
    {
        public static string ObterDescricao(this TipoLicenca tipo)
        {
            switch (tipo)
            {
                case TipoLicenca.Geral: return "Geral";
                case TipoLicenca.Motorista: return "Motorista";
                case TipoLicenca.Veiculo: return "Veículo";
                case TipoLicenca.Pessoa: return "Pessoa";
                case TipoLicenca.Tracao: return "Tração";
                case TipoLicenca.Reboque: return "Reboque";
                default: return string.Empty;
            }
        }
    }
}
