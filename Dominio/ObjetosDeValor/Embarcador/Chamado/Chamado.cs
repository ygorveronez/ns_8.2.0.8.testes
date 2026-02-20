using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public class Chamado
    {
        public int Codigo { get; set; }
        public int NumeroChamado { get; set; }
        public string Retorno { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado Situacao { get; set; }
        public int CodigoNotaFiscal { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public string MotivoChamadoDescricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool FreteRetornoDevolucao { get; set; }
        public string DataCriacaoFormatada
        {
            get
            {
                return DataCriacao.ToString("dd/MM/yyyy hh:mm:ss");
            }
        }
        public string SituacaoDescricao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }
    }
}