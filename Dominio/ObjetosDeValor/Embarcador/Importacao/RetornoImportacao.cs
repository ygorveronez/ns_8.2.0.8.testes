using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Importacao
{
    public class RetornoImportacao
    {
        public int Total { get; set; }
        public int Importados { get; set; }
        public string MensagemAviso { get; set; }
        public List<RetonoLinha> Retornolinhas { get; set; } = new List<RetonoLinha>();
        public dynamic Reimportar { get; set; }
        public object Retorno { get; set; }
        public dynamic RegistrosAlterados { get; set; }
    }
}
