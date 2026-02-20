namespace Servicos.Embarcador.Hubs
{
    public class MDFeAquaviario : HubBase<MDFeAquaviario>
    {
        public void InformarCargaMDFeAquaviarioAtualizado(int codigoCargaMDFeAquaviario, Dominio.Entidades.Usuario usuarioAtualizouMDFAquaviario = null)
        {
            var retorno = new
            {
                CodigoCargaMDFeAquaviario = codigoCargaMDFeAquaviario
            };

            if (usuarioAtualizouMDFAquaviario != null)
                SendToAllExcept(usuarioAtualizouMDFAquaviario.Codigo.ToString(), "informarCargaMDFeAquaviarioAtualizado", retorno);
            else 
                SendToAll("informarCargaMDFeAquaviarioAtualizado", retorno);
        }

        public void InformarCargaMDFeAquaviarioAtualizadoCancelamento(int codigoCargaMDFeAquaviarioCancelamento, Dominio.Entidades.Usuario usuarioAtualizouMDFAquaviario = null)
        {
            var retorno = new
            {
                CodigoCargaMDFeAquaviarioCancelamento = codigoCargaMDFeAquaviarioCancelamento
            };

			if (usuarioAtualizouMDFAquaviario != null)
				SendToAllExcept(usuarioAtualizouMDFAquaviario.Codigo.ToString(), "informarCargaMDFeAquaviarioAtualizadoCancelamento", retorno);
			else
				SendToAll("informarCargaMDFeAquaviarioAtualizadoCancelamento", retorno);
        }
    }
}
