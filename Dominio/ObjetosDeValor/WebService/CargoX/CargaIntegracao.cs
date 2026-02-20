using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.CargoX
{
    public class CargaIntegracao
    {
        public string CNPJTransportadoraEmitente { get; set; }
        public List<Motorista> Motoristas { get; set; }
        public List<NotaFiscal> NotasFiscais { get; set; }
        public string NumeroDoEmbarcador { get; set; }
        public string NumeroDoFrete { get; set; }
        public string Observacao { get; set; }
        public Pessoa Subcontratado { get; set; }
        public Embarcador.Enumeradores.TipoAverbacao TipoAverbacao { get; set; }
        public Embarcador.Enumeradores.TipoRateio TipoRateio { get; set; }        
        public Pessoa Tomador { get; set; }
        public FreteValor ValorFrete { get; set; }
        public List<Veiculo> Veiculos { get; set; }

    }
}
