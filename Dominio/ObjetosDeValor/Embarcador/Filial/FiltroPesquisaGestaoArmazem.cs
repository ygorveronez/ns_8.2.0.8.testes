using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Filial
{
    public class FiltroPesquisaGestaoArmazem
    {
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosProdutoEmbarcador { get; set; }
        public List<int> CodigosArmazem { get; set; }
    }
}
