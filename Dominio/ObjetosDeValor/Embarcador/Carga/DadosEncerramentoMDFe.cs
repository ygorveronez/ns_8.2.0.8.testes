using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class DadosEncerramentoMDFe
    {
        public int Codigo { get; set; }

        public Dominio.Entidades.Estado Estado { get; set; }

        public DateTime DataEncerramento { get; set; }

        public List<Dominio.Entidades.Localidade> Localidades { get; set; }
    }
}
