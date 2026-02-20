using System;

/// <summary>
/// Objeto de valor genérico para funções de reprocessamento.
/// </summary>
namespace Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaGenerico
{
    #region Metodo BuscarReprocessarGeracaoOcorrenciaEventosEntrega
    /// <summary>
    /// Classe para teste do método BuscarReprocessarGeracaoOcorrenciaEventosEntrega.
    /// São dados necessários para reprocessamento da geração dos eventos de integração de ocorrências de entregas.
    /// </summary>
    public class BuscarReprocessarGeracaoOcorrenciaEventosEntrega
    {
        public int CodigoEntrega { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public int CodigoOcorrencia { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int OrigemOcorrencia { get; set; }
        public int CodigoCargaEntregaEvento { get; set; }
    }
    #endregion
}
