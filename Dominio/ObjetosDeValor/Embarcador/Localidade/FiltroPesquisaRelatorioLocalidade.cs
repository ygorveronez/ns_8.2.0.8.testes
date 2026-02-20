using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Localidade
{
    public sealed class FiltroPesquisaRelatorioLocalidade
    {
        public string Descricao { get; set; }
        public List<string> Estados { get; set; }
        public List<int> Paises { get; set; }
        public List<int> Regioes { get; set; }
    }
}
