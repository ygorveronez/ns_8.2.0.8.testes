using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class PedidoOcorrenciaColetaEntrega
    {
        public string Natureza { get; set; }
        public string GrupoOcorrencia { get; set; }
        public string Razao { get; set; }
        public int NotafiscalDevolucao { get; set; }
        public string SolicitacaoCliente { get; set; }
    }
}
