using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class CargaEntregaEvento
    {
        public DateTime DataOcorrencia { get; set; }
        public Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega? EventoColetaEntrega { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? DataPosicao { get; set; }
        public DateTime? DataPrevisaoRecalculada { get; set; }
        public Enumeradores.OrigemSituacaoEntrega Origem { get; set; }
        public string descricaoTipoPrevisao { get; set; }
        public bool GerarIntegracao { get; set; }

    }
}
