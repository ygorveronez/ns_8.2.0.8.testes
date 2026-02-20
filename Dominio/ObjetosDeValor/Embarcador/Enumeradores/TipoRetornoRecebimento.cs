namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRetornoRecebimento
	{
		Recebimento = 1,
		Retorno = 2
	}

	public static class TipoRetornoRecebimentoHelper
	{
		public static string ObterDescricao(this TipoRetornoRecebimento tipo)
		{
			switch (tipo)
			{
				case TipoRetornoRecebimento.Recebimento: return "Recebimento";
				case TipoRetornoRecebimento.Retorno: return "Retorno";
				default: return string.Empty;
			}
		}
	}
}
