using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalRodoviario
    {
        public string RNTRC { get; set; }
        public string DataEntrega { get; set; }
        public bool Lotacao { get; set; }
        public string CIOT { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo> Veiculos { get; set; }
        public List<Dominio.ObjetosDeValor.Motorista> Motoristas { get; set; }
    }
}
