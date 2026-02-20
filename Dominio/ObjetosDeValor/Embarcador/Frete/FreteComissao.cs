using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FreteComissao
    {
        public Enumeradores.SituacaoRetornoFreteComissao situacao { get; set; }

        public List<ProdutoComissao> produtos { get; set; }

        public string Tabela { get; set; }

        public dynamic Empresa { get; set; }
    }
}
