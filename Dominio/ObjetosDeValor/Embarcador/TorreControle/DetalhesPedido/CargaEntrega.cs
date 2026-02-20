using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido
{
    public class CargaEntrega
    {
        public int Codigo { get; set; }

        public DateTime? DataPrevisaoEntrega { get; set; }      

        public DateTime? DataEntrega { get; set; }

        public DateTime? DataReprogramada { get; set; }
        public DateTime? DataPrevisaoEntregaAjustada { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao { get; set; }

        #region Propriedades com regras 

        public string DataPrevisaoEntregaFormatada
        {
            get
            {
                return DataPrevisaoEntrega.HasValue ? DataPrevisaoEntrega.Value.ToDateTimeString() : string.Empty;
            }
        }
        
        public string DataPrevisaoEntregaAjustadaFormatada
        {
            get
            {
                return DataPrevisaoEntregaAjustada.HasValue ? DataPrevisaoEntregaAjustada.Value.ToDateTimeString() : string.Empty;
            }
        }
        
        public string DataReprogramadaFormatada
        {
            get
            {
                return DataReprogramada.HasValue ? DataReprogramada.Value.ToDateTimeString() : string.Empty;
            }
        }

        public string DataEntregaFormatada
        {
            get
            {
                return DataEntrega.HasValue ? DataEntrega.Value.ToDateTimeString() : string.Empty;
            }
        }

        public string FarolStatus
        {
            get { return SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(Situacao) && DataPrevisaoEntrega >= DateTime.Now ? "#00ff1e" : "#ff2600"; }
        }
        #endregion
    }
}
