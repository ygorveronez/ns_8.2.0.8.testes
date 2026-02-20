using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalMultimodalContainer
    {
        public int Container { get; set; }
        public string Lacre1 { get; set; }
        public string Lacre2 { get; set; }
        public string Lacre3 { get; set; }
        public List<ModalMultimodalContainerDocumento> Documentos { get; set; }
    }
}
