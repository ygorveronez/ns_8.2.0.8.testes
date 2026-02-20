using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class RecebedoresAutorizadosPorDestinatario
    {
        public long CodigoDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public List<RecebedorAutorizado> RecebedoresAutorizados { get; set; }
    }

    public class RecebedorAutorizado
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Foto { get; set; }
    }

}
