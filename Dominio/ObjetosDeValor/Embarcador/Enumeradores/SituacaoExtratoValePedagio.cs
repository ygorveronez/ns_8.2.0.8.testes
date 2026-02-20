namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoExtratoValePedagio 
	{
		SemExtrato = 1,
		EmExtrato = 2,
		SemValePedagio = 3
	}

	public static class SituacaoExtratoVelePedagioHelper
	{
		public static string ObterDescricao(this SituacaoExtratoValePedagio situacao)
		{
			switch (situacao)
			{
				case SituacaoExtratoValePedagio.SemExtrato: return "Sem extrato";
				case SituacaoExtratoValePedagio.EmExtrato: return "Em extrato";
				case SituacaoExtratoValePedagio.SemValePedagio: return "Sem vale pedágio";
				default: return string.Empty;
			}
		}
	}
}
