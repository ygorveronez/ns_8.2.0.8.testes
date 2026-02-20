using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class DadosTransporte
    {
        public int ProtocoloCarga { get; set; }
        public string NumeroCarga { get; set; }
        public string TipoCheckin { get; set; }
        public int EixosNocheckin { get; set; }
        public int NumeroVeiculo { get; set; }
        public string DataAgendamento { get; set; }
        public string NumeroAgendamento { get; set; }
        public string DataCarregamento { get; set; }
        public string DataDescarregamento { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Reboque1 { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Reboque2 { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Transportador { get; set; }

        public string DataPrevisaoInicioViagem { get; set; }
        public string PrevisaoSaidaDestino { get; set; }
        public string PrevisaoStopTracking { get; set; }
        public string PrevisaoTerminoViagem { get; set; }
        public string NumeroOrdem { get; set; }

    }
}
