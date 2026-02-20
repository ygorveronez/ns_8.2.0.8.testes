using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar
{
    public class RetornoConsultaSituacaoVeiculo
    {
        public int status { get; set; }
        public DateTime Data { get; set; }
        public string Erro { get; set; }
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
    }
}
