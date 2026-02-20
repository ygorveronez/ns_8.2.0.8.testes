using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class JornadaMotorista
    {
        public string Veiculo { get; set; }
        public string Frota { get; set; }
        public string Motorista { get; set; }
        public int DiasEmViagem { get; set; }
        public int DiasPendentes { get; set; }
        public int DiasForaServico { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador SituacaoAtual { get; set; }
        public int UltimaViagem { get; set; }
        public int EmViagem { get; set; }

        public string DescricaoEmViagem
        {
            get { return EmViagem > 0 ? "SIM" : "NÃO"; }
        }

        public string DescricaoSituacaoAtual
        {
            get { return SituacaoAtual.ObterDescricao(); }
        }
    }
}
