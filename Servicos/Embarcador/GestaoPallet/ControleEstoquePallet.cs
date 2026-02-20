using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet;

namespace Servicos.Embarcador.GestaoPallet
{
	public sealed class ControleEstoquePallet
	{
		#region Atributos Privados Somente Leitura

		private readonly Repositorio.UnitOfWork _unitOfWork;
		private readonly Repositorio.Embarcador.GestaoPallet.ControleEstoquePallet _repositorioControleEstoquePallet;

		#endregion Atributos Privados Somente Leitura

		#region Construtores

		public ControleEstoquePallet(Repositorio.UnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_repositorioControleEstoquePallet = new Repositorio.Embarcador.GestaoPallet.ControleEstoquePallet(_unitOfWork);
		}

		#endregion Construtores

		#region Métodos Private

		private Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet ObterEstoque(DadosControlePallet dadosControlePallet)
		{
			Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = _repositorioControleEstoquePallet.BuscarEstoquePallet(dadosControlePallet);

			return controleEstoquePallet;
		}

		private ResponsavelPallet ObterResponsavelEstoquePallet(AdicionarMovimentacaoPallet adicionarMovimentacaoPallet)
		{
			return adicionarMovimentacaoPallet.RegraPallet switch
			{
				RegraPallet.Estoque => adicionarMovimentacaoPallet.ResponsavelPallet,
				_ => ResponsavelPallet.Filial,
			};
		}

		private int ObterSaldoAtual(DadosControlePallet dadosControlePallet)
		{
			int quantidadeSaldo = _repositorioControleEstoquePallet.ObterSaldoAtual(dadosControlePallet);

			return quantidadeSaldo;
		}

        private void EnviarNotificacaoEstoqueBaixo(Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet)
        {
            string email = ObterEmailResponsavel(controleEstoquePallet);

            if (email == string.Empty)
                return;

            string assuntoEmail = "Estoque de pallet baixo - Ypê";
            string corpoEmail = "Olá, caro Cliente / Transportador, seu estoque de pallet disponivel esta abaixo de 100 unidades, gentileza realizar o abastecimento nas unidades Ype";

            new Servicos.Email(_unitOfWork).EnviarEmail(string.Empty, string.Empty, string.Empty, email, null, null, assuntoEmail, corpoEmail, string.Empty, null, string.Empty, false, string.Empty, 0, _unitOfWork);
        }

        private string ObterEmailResponsavel(Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet)
        {
            return controleEstoquePallet.ResponsavelPallet switch
            {
                ResponsavelPallet.Cliente => controleEstoquePallet.Cliente.Email,
                ResponsavelPallet.Filial => controleEstoquePallet.Filial.Email,
                ResponsavelPallet.Transportador => controleEstoquePallet.Transportador.Email,
                _ => string.Empty
            };
        }

        #endregion Métodos Private

		#region Métodos Publicos

		public Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet AdicionarEstoque(DadosControlePallet dadosControlePallet)
		{
			Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = ObterEstoque(dadosControlePallet);

			if (controleEstoquePallet != null)
				return controleEstoquePallet;

			controleEstoquePallet = new Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet();

			if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Cliente)
			{
				Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

				controleEstoquePallet.Cliente = repositorioCliente.BuscarPorCPFCNPJ(dadosControlePallet.CodigoCliente);

				if (controleEstoquePallet.Cliente == null)
					throw new ServicoException("Cliente não encontrado!");
			}
			else if (dadosControlePallet.ResponsavelPallet == ResponsavelPallet.Transportador)
			{
				Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

				controleEstoquePallet.Transportador = repositorioEmpresa.BuscarPorCodigo(dadosControlePallet.CodigoTransportador);

				if (controleEstoquePallet.Transportador == null)
					throw new ServicoException("Transportador não encontrado!");
			}
			else
			{
				Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

				controleEstoquePallet.Filial = repositorioFilial.BuscarPorCodigo(dadosControlePallet.CodigoFilial);

				if (controleEstoquePallet.Filial == null)
					throw new ServicoException("Filial não encontrada!");
			}

			controleEstoquePallet.ResponsavelPallet = dadosControlePallet.ResponsavelPallet;
			controleEstoquePallet.QuantidadeTotalPallets = 0;
			controleEstoquePallet.TipoEstoquePallet = dadosControlePallet.TipoEstoquePallet;

			_repositorioControleEstoquePallet.Inserir(controleEstoquePallet);

			return controleEstoquePallet;
		}

		public Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet AtualizarSaldo(Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet, int quantidadePallet, TipoEntradaSaida tipoMovimentacao)
		{
			if (tipoMovimentacao == TipoEntradaSaida.Entrada)
				controleEstoquePallet.QuantidadeTotalPallets += quantidadePallet;
			else
				controleEstoquePallet.QuantidadeTotalPallets -= quantidadePallet;

			_repositorioControleEstoquePallet.Atualizar(controleEstoquePallet);

			return controleEstoquePallet;
		}

		public Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet ObterEstoqueParaMovimentacao(AdicionarMovimentacaoPallet adicionarMovimentacaoPallet)
		{
			DadosControlePallet dadosControlePallet = new DadosControlePallet(adicionarMovimentacaoPallet.Cliente, adicionarMovimentacaoPallet.Carga.Empresa, adicionarMovimentacaoPallet.Carga.Filial)
			{
				ResponsavelPallet = ObterResponsavelEstoquePallet(adicionarMovimentacaoPallet),
				TipoEstoquePallet = TipoEstoquePallet.Movimentacao,
			};

			Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = ObterEstoque(dadosControlePallet);

			if (controleEstoquePallet == null)
				throw new ServicoException($"Não foi possível encontrar estoque de Pallet para {dadosControlePallet.ResponsavelPallet.ObterDescricao()}");

			return controleEstoquePallet;
		}

		public Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet ObterEstoquePallet(DadosControlePallet dadosControlePallet, TipoEstoquePallet tipoEstoquePallet)
		{
			dadosControlePallet.TipoEstoquePallet = tipoEstoquePallet;

			Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = AdicionarEstoque(dadosControlePallet);

			return controleEstoquePallet;
		}

		public bool PossuiSaldoSuficiente(DadosControlePallet dadosControlePallet, int quantidadePallet)
		{
			int saldoAtual = ObterSaldoAtual(dadosControlePallet);

			return (saldoAtual - quantidadePallet) > 0;
		}

		public void ValidarSaldoSuficiente(Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet, int quantidadePallet)
		{
			int saldoAtual = controleEstoquePallet.QuantidadeTotalPallets - quantidadePallet;

			if (saldoAtual < 0)
				throw new ServicoException("Saldo insuficiente para realizar a movimentação!");
		}

		public bool PossuiSaldoBaixo(DadosControlePallet dadosControlePallet, int quantidadePalletLimite)
		{
			int saldoAtual = ObterSaldoAtual(dadosControlePallet);

			return saldoAtual <= quantidadePalletLimite;
		}

        public void VerficarEstoqueBaixo(Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet)
        {
            if (controleEstoquePallet.QuantidadeTotalPallets < 100)
                EnviarNotificacaoEstoqueBaixo(controleEstoquePallet);
        }

        #endregion Métodos Públicos
    }
}
