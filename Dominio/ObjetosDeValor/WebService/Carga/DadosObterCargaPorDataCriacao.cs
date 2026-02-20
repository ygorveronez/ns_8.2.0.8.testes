using System;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class DadosObterCargaPorDataCriacao
    {
        public DateTime? DataCriacaoInicial { get; set; }
        public DateTime? DataCriacaoFinal { get; set; }
        public DateTime? DataEntregaInicial { get; set; }
        public DateTime? DataEntregaFinal { get; set; }
        public string NumeroCarga { get; set; }
        public string CodigoIntegracaoCliente { get; set; }
    }
}