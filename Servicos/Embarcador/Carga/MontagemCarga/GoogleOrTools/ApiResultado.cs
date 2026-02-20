using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    public class ApiResultado
    {
        public bool status { get; set; }
        public string msg { get; set; }
        public int qtde { get; set; }
        public List<Resultado> result { get; set; }
    }
}
