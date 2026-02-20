namespace Servicos.Embarcador.Hubs
{
    public class MDFeManual : HubBase<MDFeManual>
    {
        public void InformarCargaMDFeManualAtualizado(int codigoCargaMDFeManual, Dominio.Entidades.Usuario usuarioAtualizouMDFEManual = null)
        {
            var retorno = new
            {
                CodigoCargaMDFeManual = codigoCargaMDFeManual
            };

			if (usuarioAtualizouMDFEManual != null)
				SendToAllExcept(usuarioAtualizouMDFEManual.Codigo.ToString(), "informarCargaMDFeManualAtualizado", retorno);
			else
				SendToAll("informarCargaMDFeManualAtualizado", retorno);
        }

        public void InformarCargaMDFeManualAtualizadoCancelamento(int codigoCargaMDFeManualCancelamento, Dominio.Entidades.Usuario usuarioAtualizouMDFEManual = null)
        {
            var retorno = new
            {
                CodigoCargaMDFeManualCancelamento = codigoCargaMDFeManualCancelamento
            };

			if (usuarioAtualizouMDFEManual != null)
				SendToAllExcept(usuarioAtualizouMDFEManual.Codigo.ToString(), "informarCargaMDFeManualAtualizadoCancelamento", retorno);
			else
				SendToAll("informarCargaMDFeManualAtualizadoCancelamento", retorno);
        }
    }
}
