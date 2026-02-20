using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba
{
    public class RetornoCanhoto
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public bool Sucesso { get; set; }
        public string Message { get; set; }
        public List<DadosProcessamento> DadosProcessamento { get; set; }
    }
}
