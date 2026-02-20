using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas
{
    public sealed class GrupoPessoa
    {
        public string Descricao { get; set; }

        public string CodigoIntegracao { get; set; }

        public List<Carga.TipoOperacao> TiposOperacaoes;

        public List<Carga.ModeloVeicular> ModelosVeiculares;
    }
}
