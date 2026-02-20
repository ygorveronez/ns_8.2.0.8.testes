using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestAtualizarCheckList
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList> respostas;
    }
}
