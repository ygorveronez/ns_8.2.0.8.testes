using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class Coordenadas
    {
        public string latitude { get; set; }
        public string longitude { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao tipoLocalizacao { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RestricaoEntrega> RestricoesEntregas { get; set; }
        public bool possuiPrimeiraEntrega {
            get
            {
                if (this.RestricoesEntregas != null)
                    return this.RestricoesEntregas.Any(o => o.PrimeiraEntrega);

                return false;
            }
        }

        public bool PrimeiraEntrega { get; set; }

        public int CodigoOutroEndereco { get; set; }
    }
}
