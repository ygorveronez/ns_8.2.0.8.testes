namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AcaoLiberacaoTransportadores
	{
		Liberar = 1,
		Cancelar = 2,
		Descartar = 3
	}

	public static class AcaoLiberacaoTransportadoresHelper
	{
		public static string ObterDescricao(this AcaoLiberacaoTransportadores acao)
		{
			switch (acao)
			{
				case AcaoLiberacaoTransportadores.Liberar: return "Liberar";
				case AcaoLiberacaoTransportadores.Cancelar: return "Cancelar";
				case AcaoLiberacaoTransportadores.Descartar: return "Descartar";
				default: return string.Empty;
			}
		}
	}

}
