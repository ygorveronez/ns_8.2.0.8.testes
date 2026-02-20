using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Atendimento
{
    public class FiltroPesquisaRelatorioChamado
    {
        public DateTime DataChamadoInicial { get; set; }
        public DateTime DataChamadoFinal { get; set; }
        public DateTime DataAtendimentoInicial { get; set; }
        public DateTime DataAtendimentoFinal { get; set; }
        public int Tela { get; set; }
        public int Modulo { get; set; }
        public int Sistema { get; set; }
        public int Tipo { get; set; }
        public int Funcionario { get; set; }
        public int Empresa { get; set; }
        public int EmpresaFilho { get; set; }
        public int Solicitante { get; set; }
        public string Titulo { get; set; }
        public StatusAtendimentoTarefa Status { get; set; }
        public PrioridadeAtendimento Prioridade { get; set; }
    }
}
