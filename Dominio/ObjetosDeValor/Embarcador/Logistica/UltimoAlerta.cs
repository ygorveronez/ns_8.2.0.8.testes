using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class UltimoAlerta
    {
        public virtual int Codigo { get; set; }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }

        public virtual int Veiculo { get; set; }

        public virtual DateTime Data { get; set; }

        public virtual DateTime? DataTratativa { get; set; }

        public virtual DateTime DataCadastro { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Status { get; set;}
    }
}
