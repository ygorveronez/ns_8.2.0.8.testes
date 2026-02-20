using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class DetalhesTorre
    {
        public int Codigo { get; set; }
        public string MotoristaNome { get; set; }
        public string MotoristaTelefone { get; set; }
        public string Veiculo { get; set; }
        public string MotoristaCPF { get; set; }
        public string Tecnologia { get; set; }
        public int RastreadorStatus { get; set; }
        public string InicioMonitoramento { get; set; }
        public string FimMonitoramento { get; set; }
        public string Velocidade { get; set; }
        public string Localizacao { get; set; }
        public string Cidade { get; set; }
        public string PrimeiraPosicao { get; set; }
        public string DataPosicao { get; set; }
        public string Temperatura { get; set; }
        public bool Ignicao { get; set; }
        public List<DetalhesTorreStatus> Status { get; set; }
        public DetalhesTorreStatus StatusAtual { get; set; }
        public string DistanciaRealizada { get; set; }
        public string DistanciaPrevista { get; set; }
        public string PrevisaoChegada { get; set; }
        public string DistanciaDestino { get; set; }
        public bool MonitoramentoCritico { get; set; }
        public string Observacao { get; set; }
        public List<DetalhesTorreParadas> Paradas { get; set; }
        public List<DetalhesTorreVelocidade> Velocidades { get; set; }
        public List<DetalhesTorreTemperatura> Temperaturas { get; set; }
        public List<DetalhesTorreHistoricoStatus> Historico { get; set; }
    }
}
