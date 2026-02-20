using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class SincronizacaoLoteResultado
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; } = new List<string>();
    }
}


