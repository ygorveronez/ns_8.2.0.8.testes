namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoProcessamentoExtratoValePedagio
	{
		AguardandoProcessamento = 1,
		Processado = 2,
	}

	public static class SituacaoProcessamentoExtratoValePedagioHelper
	{
		public static string ObterDescricao(this SituacaoProcessamentoExtratoValePedagio situacao)
		{
			switch (situacao)
			{
				case SituacaoProcessamentoExtratoValePedagio.AguardandoProcessamento: return "Aguardando Processamento";
				case SituacaoProcessamentoExtratoValePedagio.Processado: return "Processado";
				default: return string.Empty;
			}
		}
	}
}
