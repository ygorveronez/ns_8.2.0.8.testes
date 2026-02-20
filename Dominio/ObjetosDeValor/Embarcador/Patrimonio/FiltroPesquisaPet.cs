using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.Entidades;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Patrimonio
{
	public class FiltroPesquisaPet
	{
		public bool Ativo { get; set; }
		public bool Castrado { get; set; }
		public bool Microchip { get; set; }
		public double TutorCodigo { get; set; }
		public int EspecieCodigo { get; set; }
		public int RacaCodigo { get; set; }
		public int CorCodigo { get; set; }
		public int PlanoServicoCodigo { get; set; }
		public int EmpresaCodigo { get; set; }
		public SituacaoAtivoPesquisa SituacaoAtivo { get; set; }
		public Sexo Sexo { get; set; }
		public Porte Porte { get; set; }
		public Pelagem Pelagem { get; set; }
		public Comportamento Comportamento { get; set; }
		public DateTime DataNascimento { get; set; }
		public DateTime UltimaVisita { get; set; }
		public string Nome { get; set; }
		public string Observacao { get; set; }
		public string CaminhoFoto { get; set; }
		public decimal Peso { get; set; }
	}
}
