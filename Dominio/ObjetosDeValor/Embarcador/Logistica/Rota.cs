using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class Rota : IEquatable<Dominio.ObjetosDeValor.Embarcador.Logistica.Rota>
    {
        public int Codigo { get; set; }
        public Dominio.Entidades.Localidade Origem { get; set; }
        public Dominio.Entidades.Localidade Destino { get; set; }

        public Dominio.Entidades.Cliente Remetente { get; set; }
        public Dominio.Entidades.Cliente Destinatario { get; set; }

        public int DistanciaKM { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota TipoRota { get; set; }

        public bool Equals(Rota other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
