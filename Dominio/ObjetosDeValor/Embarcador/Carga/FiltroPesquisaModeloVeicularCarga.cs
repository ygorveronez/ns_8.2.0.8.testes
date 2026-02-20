using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using iTextSharp.text;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaModeloVeicularCarga
    {
        public List<int> ModeloVeicular { get; set; }
        public bool? Ativo { get; set; }
        public List<TipoModeloVeicularCarga> Tipo { get; set; }
    }
}