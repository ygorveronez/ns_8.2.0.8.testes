using System;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public sealed class FiltroDadosDocsys
    {
        public DateTime DataEmissaoInicial { get; set; }

        public DateTime DataEmissaoFinal { get; set; }
        public int PedidoViagemNavio { get; set; }
    }
}
