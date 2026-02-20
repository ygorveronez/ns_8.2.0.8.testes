using Dominio.Entidades;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class ResponseCanhoto
    {
        public int Protocolo { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }
    }
}
