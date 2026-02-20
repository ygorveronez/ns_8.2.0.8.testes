using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class ConfiguracaoOcorrenciaCoordenadas
    {
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? DataPosicao { get; set; }
        public DateTime? DataExecucao { get; set; }
        public DateTime? DataPrevisaoRecalculada { get; set; }
        public string TempoPercurso { get; set; }
        public decimal? DistanciaAteDestino { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega? OrigemSituacaoEntrega;
    }
}
