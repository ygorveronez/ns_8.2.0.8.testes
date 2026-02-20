using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaChecklistSuperApp
    {
        public int Codigo { get; set; }
        public string Titulo { get; set; }
        public string IdSuperApp { get; set; }
        public int TipoFluxo { get; set; }
    }
}