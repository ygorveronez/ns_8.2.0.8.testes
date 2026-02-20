using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class TipoIntegracaoCategoriaIntegracao
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public Dictionary<string, TipoIntegracaoCategoriaParametro> Parametros { get; set; }

        public TipoIntegracaoCategoriaIntegracao()
        {
            Parametros = new Dictionary<string, TipoIntegracaoCategoriaParametro>();
        }

    }
}
