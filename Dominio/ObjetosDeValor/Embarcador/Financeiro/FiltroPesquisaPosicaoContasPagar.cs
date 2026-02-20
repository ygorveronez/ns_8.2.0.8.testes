using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaPosicaoContasPagar
    {
        public DateTime DataPosicao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo Situacao { get; set; }

    }
}
