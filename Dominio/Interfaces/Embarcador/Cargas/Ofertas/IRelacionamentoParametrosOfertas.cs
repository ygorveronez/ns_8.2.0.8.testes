using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Interfaces.Embarcador.Cargas.Ofertas
{
    public interface IRelacionamentoParametrosOfertas
    {
        int Codigo { get; set; }
        Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas ParametrosOfertas { get; set; }
    }
}
