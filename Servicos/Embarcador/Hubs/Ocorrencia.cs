namespace Servicos.Embarcador.Hubs
{
	public class Ocorrencia : HubBase<Ocorrencia>
	{
		public void InformarOcorrenciaAtualizada(int codigoOcorrencia, Dominio.Entidades.Usuario usuarioEnviouOcorrencia = null)
		{
			var retorno = new
			{
				CodigoOcorrencia = codigoOcorrencia
			};

			if (usuarioEnviouOcorrencia != null)
				SendToAllExcept(usuarioEnviouOcorrencia.Codigo.ToString(), "informarOcorrenciaAtualizada", retorno);
			else
				SendToAll("informarOcorrenciaAtualizada", retorno);
		}

		public void InformarCancelamentoAtualizado(int codigoCancelamento, Dominio.Entidades.Usuario usuarioEnviouOcorrencia = null)
		{
			var retorno = new
			{
				CodigoCancelamento = codigoCancelamento
			};

			if (usuarioEnviouOcorrencia != null)
				SendToAllExcept(usuarioEnviouOcorrencia.Codigo.ToString(), "informarCancelamentoOcorrenciaAtualizada", retorno);
			else
				SendToAll("informarCancelamentoOcorrenciaAtualizada", retorno);
		}

		public void InformarCancelamentoDocumentoAtualizado(int codigoCancelamento, Dominio.Entidades.Usuario usuarioEnviouOcorrencia = null)
		{
			var retorno = new
			{
				CodigoCancelamento = codigoCancelamento
			};

			if (usuarioEnviouOcorrencia != null)
				SendToAllExcept(usuarioEnviouOcorrencia.Codigo.ToString(), "informarCancelamentoOcorrenciaDocumentoAlterado", retorno);
			else
				SendToAll("informarCancelamentoOcorrenciaDocumentoAlterado", retorno);
		}

		public void InformarOcorrenciaLoteAtualizada(int codigoOcorrenciaLote)
		{
			var retorno = new
			{
				CodigoOcorrenciaLote = codigoOcorrenciaLote
			};

			SendToAll("informarOcorrenciaLoteAtualizada", retorno);
		}
	}
}
