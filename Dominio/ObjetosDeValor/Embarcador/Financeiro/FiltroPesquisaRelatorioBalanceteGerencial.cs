using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaRelatorioBalanceteGerencial
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public AnaliticoSintetico? TipoConta { get; set; }
        public int CodigoPlanoContaSintetica { get; set; }
        public string NumeroPlanoContaSintetica { get; set; }
        public int CodigoEmpresa { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public List<int> CodigosCentroResultado { get; set; }
    }
}
