using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet;

namespace Servicos.Embarcador.GestaoPallet
{
	public sealed class ManutencaoPallet
	{
		#region Atributos Privados Somente Leitura

		private readonly Repositorio.UnitOfWork _unitOfWork;
		private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
		private readonly Repositorio.Embarcador.GestaoPallet.ManutencaoPallet _repositorioManutencaoPallet;
		private readonly ControleEstoquePallet _servicoControleEstoquePallet;

		#endregion Atributos Privados Somente Leitura

		#region Construtores

		public ManutencaoPallet(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
		{
			_unitOfWork = unitOfWork;
			_auditado = auditado;
			_repositorioManutencaoPallet = new Repositorio.Embarcador.GestaoPallet.ManutencaoPallet(_unitOfWork);
			_servicoControleEstoquePallet = new ControleEstoquePallet(_unitOfWork);
		}

		#endregion Construtores

		#region Configurações

		#endregion

		#region Métodos Private

		private void AdicionarManutencao(AdicionarManutencaoPallet adicionarManutencaoPallet, Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet, bool atualizarSaldo)
		{
			if (adicionarManutencaoPallet.TipoMovimentacao == TipoEntradaSaida.Saida && atualizarSaldo)
				_servicoControleEstoquePallet.ValidarSaldoSuficiente(controleEstoquePallet, adicionarManutencaoPallet.QuantidadePallet);

			Dominio.Entidades.Embarcador.GestaoPallet.ManutencaoPallet manutencaoPallet = new Dominio.Entidades.Embarcador.GestaoPallet.ManutencaoPallet()
			{
				XMLNotaFiscal = adicionarManutencaoPallet.XMLNotaFiscal,
				Filial = adicionarManutencaoPallet.Filial,
				Carga = adicionarManutencaoPallet.Carga,
				QuantidadePallets = adicionarManutencaoPallet.QuantidadePallet,
				TipoManutencaoPallet = adicionarManutencaoPallet.TipoManutencaoPallet,
				ControleEstoquePallet = controleEstoquePallet,
				Observacao = adicionarManutencaoPallet.Observacao,
				TipoMovimentacao = adicionarManutencaoPallet.TipoMovimentacao
			};

			_repositorioManutencaoPallet.Inserir(manutencaoPallet);

			if (atualizarSaldo)
				_servicoControleEstoquePallet.AtualizarSaldo(controleEstoquePallet, adicionarManutencaoPallet.QuantidadePallet, adicionarManutencaoPallet.TipoMovimentacao);

			if (manutencaoPallet.TipoMovimentacao == TipoEntradaSaida.Saida && manutencaoPallet.TipoManutencaoPallet == TipoManutencaoPallet.Disponivel)
				new MovimentacaoPallet(_unitOfWork, _auditado).AdicionarMovimentacaoRetornoManutencao(manutencaoPallet);
		}

		#endregion Métodos Private

		#region Métodos Públicos

		public void AdicionarManutencaoPallet(AdicionarManutencaoPallet adicionarManutencaoPallet)
		{
			DadosControlePallet dadosControlePallet = new DadosControlePallet(adicionarManutencaoPallet.Filial.Codigo)
			{
				ResponsavelPallet = ResponsavelPallet.Filial,
				TipoEstoquePallet = TipoEstoquePallet.Manutencao,
			};

			Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = _servicoControleEstoquePallet.AdicionarEstoque(dadosControlePallet);

			AdicionarManutencao(adicionarManutencaoPallet, controleEstoquePallet, atualizarSaldo: true);
		}

		public void AdicionarManutencaoPallet(AdicionarManutencaoPallet adicionarManutencaoPallet, Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet, bool atualizarSaldo = true)
		{
			AdicionarManutencao(adicionarManutencaoPallet, controleEstoquePallet, atualizarSaldo: atualizarSaldo);
		}

		#endregion Métodos Públicos
	}
}
