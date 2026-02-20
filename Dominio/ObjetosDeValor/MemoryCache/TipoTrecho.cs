using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MemoryCache
{
    public class TipoTrecho
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public List<int> TiposOperacao { get; set; }

        public List<int> CategoriasOrigem { get; set; }

        public List<int> CategoriasDestino { get; set; }

        public List<int> CategoriasExpedidor { get; set; }

        public List<int> CategoriasRecebedor { get; set; }

        public List<int> ModelosVeiculares { get; set; }

        public List<double> ClientesOrigem { get; set; }

        public List<double> ClientesDestino { get; set; }
    }
}
