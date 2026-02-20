using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaCotacaoEspecial
    {
        public DateTime? DataCotacaoInicial { get; set; }

        public DateTime? DataCotacaoFinal { get; set; }

        public string NumeroPedido { get; set; }

        public int NumeroCotacao { get; set; }

        public StatusCotacaoEspecial? StatusCotacaoEspecial { get; set; }

        public TipoModalCotacaoEspecial? TipoModal { get; set; }

        public double CodigoFornecedor { get; set; }
    }
}
