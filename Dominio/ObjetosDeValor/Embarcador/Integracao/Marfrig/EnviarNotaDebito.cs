using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig
{
    public class EnviarNotaDebito
    {
        public ArquivoND arquivo { get; set; }
        public Ocorrencia ocorrencia { get; set; }
        public OutrosND outros { get; set; }
        public List<NotasFiscaisND> notasFiscais { get; set; }
    }
}
