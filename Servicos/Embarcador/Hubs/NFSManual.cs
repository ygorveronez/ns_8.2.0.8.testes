namespace Servicos.Embarcador.Hubs
{
    public class NFSManual : HubBase<NFSManual>
    {
        public void InformarNFSManualAtualizada(int codigoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga tipoAcao, string stringConexao, Dominio.Entidades.Usuario usuarioEnviouCarga = null)
        {
            var retorno = new
            {
                CodigoNFSManual = codigoNFSManual,
                TipoAcao = tipoAcao
            };

			if (usuarioEnviouCarga != null)
				SendToAllExcept(usuarioEnviouCarga.Codigo.ToString(), "informarCargaAlterada", retorno);
			else
				SendToAll("informarCargaAlterada", retorno);
        }

        public void InformarNFSManualAtualizadoCancelamento(int codigoNFSManualCancelamento, string stringConexao, Dominio.Entidades.Usuario usuarioEnviouCarga = null)
        {
            var retorno = new
            {
                CodigoNFSManualCancelamento = codigoNFSManualCancelamento
            };

			if (usuarioEnviouCarga != null)
				SendToAllExcept(usuarioEnviouCarga.Codigo.ToString(), "informarNFSManualAtualizadoCancelamento", retorno);
			else
				SendToAll("informarNFSManualAtualizadoCancelamento", retorno);
        }

        public void InformarLancamentoNFSManualAtualizada(int codigoNFSManual)
        {
            var retorno = new
            {
                CodigoNFSManual = codigoNFSManual
            };

            SendToAll("informarLancamentoNFSManualAtualizada", retorno);
        }
    }
}
