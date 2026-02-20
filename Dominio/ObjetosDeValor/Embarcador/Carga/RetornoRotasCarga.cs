using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class RetornoRotasCarga
    {
        public Enumeradores.SituacaoRetornoRotas situacao { get; set; }

        public List<Dominio.Entidades.Embarcador.Logistica.Rota> rotasNaoEncontradas { get; set; }
    }
}
