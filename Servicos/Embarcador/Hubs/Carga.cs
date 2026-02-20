namespace Servicos.Embarcador.Hubs
{
	public class Carga : HubBase<Carga>
	{
		public void InformarCargaAlterada(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga tipoAcao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware = null)
		{
			InformarCargaAtualizada(codigoCarga, tipoAcao, stringConexao: string.Empty, usuarioEnviouCarga: null, clienteMultisoftware);
		}

		public void InformarCargaAtualizada(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga tipoAcao, string stringConexao, Dominio.Entidades.Usuario usuarioEnviouCarga = null, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware = null)
		{
			var retorno = new
			{
				CodigoCarga = codigoCarga,
				TipoAcao = tipoAcao,
				usuarioEnviouCarga = usuarioEnviouCarga
			};

			if (clienteMultisoftware == null)
			{
				if (usuarioEnviouCarga != null)
					SendToAllExcept(usuarioEnviouCarga.Codigo.ToString(), "informarCargaAlterada", retorno);

				SendToAll("informarCargaAlterada", retorno);
			}
			else
			{
				// MSMQ
				Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
					retorno,
					clienteMultisoftware.Codigo,
					Dominio.MSMQ.MSMQQueue.SGTWebAdmin,
					Dominio.SignalR.Hubs.EtapasCarga,
					Servicos.SignalR.Hubs.EtapasCarga.GetHub(Servicos.SignalR.Hubs.EtapasCargaHubs.InformarCargaAtualizada)
					);

				Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
			}
		}

		public void InformarTransbordoCargaAtualizada(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga tipoAcao, Dominio.Entidades.Usuario usuarioEnviouTransbordo = null)
		{
			var retorno = new
			{
				Transbordo = transbordo.Codigo,
				TipoAcao = tipoAcao
			};

			if (usuarioEnviouTransbordo != null)
				SendToAllExcept(usuarioEnviouTransbordo.Codigo.ToString(), "informarTransbordoCargaAtualizada", retorno);

			SendToAll("informarTransbordoCargaAtualizada", retorno);

			InformarCargaAlterada(transbordo.Carga.Codigo, tipoAcao);
		}

		public void InformarQuantidadeDocumentosEmitidos(int codigoCarga, int quantidadeTotal, int quantidadeGerada)
		{
			var retorno = new
			{
				CodigoCarga = codigoCarga,
				QuantidadeDocumentosGerados = quantidadeGerada,
				QuantidadeDocumentosTotal = quantidadeTotal
			};

			SendToAll("informarQuantidadeDocumentosGerados", retorno);
		}

		public void InformarQuantidadeDocumentosEnviadosSefaz(int codigoCarga, int quantidadeTotal, int quantidadeEmitida, bool erro, string mensagem)
		{
			var retorno = new
			{
				CodigoCarga = codigoCarga,
				QuantidadeDocumentosEmitidos = quantidadeEmitida,
				QuantidadeDocumentosTotal = quantidadeTotal,
				Erro = erro,
				Mensagem = mensagem
			};

			SendToAll("informarQuantidadeDocumentosEmitidos", retorno);
		}

		public void InformarCancelamentoAtualizado(int codigoCancelamento)
		{
			var retorno = new
			{
				CodigoCancelamento = codigoCancelamento
			};

			SendToAll("InformarCancelamentoAtualizado", retorno);
		}

		public void InformarRetornoCalculoFrete(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
		{
			if (clienteMultisoftware == null)
				return;

			var ret = new
			{
				CodigoCarga = codigoCarga,
				Retorno = retorno
			};

			// MSMQ
			Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
				ret,
				clienteMultisoftware.Codigo,
				Dominio.MSMQ.MSMQQueue.SGTWebAdmin,
				Dominio.SignalR.Hubs.EtapasCarga,
				Servicos.SignalR.Hubs.EtapasCarga.GetHub(Servicos.SignalR.Hubs.EtapasCargaHubs.InformarRetornoCalculoFrete)
				);

			Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
		}

		public void InformarMensagemAlerta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosMensagemAlerta retorno)
		{
			var ret = new
			{
				CodigoCarga = codigoCarga,
				Retorno = retorno
			};

			SendToAll("InformarMensagemAlerta", ret);
		}

		public void InformarRetornoProcessamentoDocumentosFiscais(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Carga.RetornoProcessamentoDocumentosFiscais retorno, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
		{
			var ret = new
			{
				CodigoCarga = codigoCarga,
				Retorno = retorno
			};

			// MSMQ
			Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
				ret,
				clienteMultisoftware.Codigo,
				Dominio.MSMQ.MSMQQueue.SGTWebAdmin,
				Dominio.SignalR.Hubs.AcompanhamentoCarga,
				Servicos.SignalR.Hubs.AcompanhamentoCarga.GetHub(Servicos.SignalR.Hubs.AcompanhamentoCargaHubs.InformarRetornoProcessamentoDocumentosFiscais)
				);

			Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
		}

		public void InformarCargaDadosTransporteIntegracaoAtualizado(int codigoCarga)
		{
			var retorno = new
			{
				CodigoCarga = codigoCarga
			};

			SendToAll("InformarCargaDadosTransporteIntegracaoAtualizado", retorno);
		}

		public void InformarEncerramentoAtualizado(int codigoEncerramento)
		{
			var retorno = new
			{
				CodigoEncerramento = codigoEncerramento
			};

			SendToAll("InformarEncerramentoAtualizado", retorno);
		}

		public void InformarTransbordoAtualizado(int codigoTransbordo)
		{
			var retorno = new
			{
				CodigoTransbordo = codigoTransbordo
			};

			SendToAll("informarTransbordoAtualizado", retorno);
		}
	}
}
