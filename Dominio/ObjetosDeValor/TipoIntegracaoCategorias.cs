using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class TipoIntegracaoCategoria
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public string Icone { get; set; }

        public List<TipoIntegracaoCategoriaIntegracao> Integracoes { get; set; }

    }
}
