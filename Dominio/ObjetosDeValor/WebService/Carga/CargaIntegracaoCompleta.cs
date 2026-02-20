using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CargaIntegracaoCompleta
    {
        public string DataCriacaoCarga { get; set; }
        public bool? FecharCargaAutomaticamente { get; set; }
        public Embarcador.Filial.Filial Filial { get; set; }
        public string NumeroCarga  { get; set; }
        public string NumeroPreCarga { get; set; }
        public List<Pedido> Pedidos { get; set; }
        public bool? CargaSVMProprio { get; set; }
    }
}
