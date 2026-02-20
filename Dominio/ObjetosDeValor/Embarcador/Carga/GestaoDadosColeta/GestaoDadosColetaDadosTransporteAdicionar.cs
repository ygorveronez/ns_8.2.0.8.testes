using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta
{
    public class GestaoDadosColetaDadosTransporteAdicionar
    {
        public int CodigoCargaEntrega { get; set; }

        public OrigemGestaoDadosColeta Origem { get; set; }

        public int CodigoTracao { get; set; }

        public int CodigoReboque { get; set; }

        public int CodigoSegundoReboque { get; set; }

        public List<int> CodigosMotoristas { get; set; }
    }
}
