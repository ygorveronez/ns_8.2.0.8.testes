using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Logistica.Veiculo
{
    public class FilaCarregamentoVeiculo
	{
        public string CodigoIntegracaoCentroCarregamento { get; set; }
        public string CPFMotorista { get; set; }
        public string TipoRetorno { get; set; }
        public string PlacaVeiculo { get; set; }
        public List<Cidade> CidadesDestino { get; set; }
        public List<string> CodigosIntegracaoRegiaoDestino { get; set; }
        public List<string> SiglasEstadoDestino { get; set; }
        public List<string> CodigosIntegracaoTipoCarga { get; set; }
        public string PrevisaoChegada { get; set; }

    }
}
