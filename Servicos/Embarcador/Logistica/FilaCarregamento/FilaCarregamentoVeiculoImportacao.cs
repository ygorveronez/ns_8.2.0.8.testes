using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoImportacao
	{
		#region Atributos Privados Somente Leitura

		private readonly Dictionary<string, dynamic> _dados;
		private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
		private readonly Repositorio.UnitOfWork _unitOfWork;
		private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;
		private readonly dynamic _parametros;

		#endregion

		#region Construtores

		public FilaCarregamentoVeiculoImportacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dictionary<string, dynamic> dados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, dynamic parametros)
		{
			_dados = dados;
			_tipoServicoMultisoftware = tipoServicoMultisoftware;
			_unitOfWork = unitOfWork;
			_configuracao = configuracao;
			_parametros = parametros;
		}

		#endregion

		#region Métodos Privados

		private int ObterVeiculo()
		{
			var placaBuscar = string.Empty;

			if (_dados.TryGetValue("Placa", out var placa))
				placaBuscar = (string)placa;

			if (string.IsNullOrWhiteSpace(placaBuscar))
				throw new ImportacaoException("Placa não informada");

			placaBuscar = placaBuscar.Replace("-", "").Trim().ToUpper();

			Repositorio.Veiculo repositorio = new Repositorio.Veiculo(_unitOfWork);
			Dominio.Entidades.Veiculo veiculo = repositorio.BuscarPorPlaca(placaBuscar);

			if (veiculo == null)
				throw new ImportacaoException("Veículo não encontrado.");

			return veiculo.Codigo;
		}

		private int ObterMotorista()
		{
			Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
			Repositorio.Usuario repositorio = new Repositorio.Usuario(_unitOfWork);

			string cpfMotoristaBuscar = string.Empty;
			if (_dados.TryGetValue("CPFMotorista", out var cpfMotorista))
				cpfMotoristaBuscar = (string)cpfMotorista;

			cpfMotoristaBuscar = Utilidades.String.OnlyNumbers(cpfMotoristaBuscar).PadLeft(11, '0');

			if (string.IsNullOrWhiteSpace(cpfMotoristaBuscar))
				throw new ImportacaoException("CPF do motorista não informado.");

			Dominio.Entidades.Usuario motorista = repositorio.BuscarMotoristaPorCPF(cpfMotoristaBuscar);

			if (motorista == null)
				motorista = repositorio.BuscarMotoristaPorCPF(cpfMotoristaBuscar.TrimStart('0'));

			if (motorista == null)
				throw new ImportacaoException("Motorista não encontrado.");

			return motorista.Codigo;
		}

		private DateTime? ObterDataProgramada()
		{
			DateTime? data = null;
			if (_dados.TryGetValue("DataPrevisaoChegada", out var dataPrevisaoChegada))
				data = ((string)dataPrevisaoChegada).ToNullableDateTime();

			if (data == null && !string.IsNullOrWhiteSpace(dataPrevisaoChegada))
			{
				double.TryParse(dataPrevisaoChegada, out double dataValidadeFormatoExcel);
				
				if (dataValidadeFormatoExcel > 0)
					data = DateTime.FromOADate(dataValidadeFormatoExcel);
			}

			if (data == null)
				throw new ImportacaoException("Data de Previsão de Chegada inválida.");

			return data;
		}
		
		private List<int> ObterTiposDeCarga()
		{
			Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);

			string strTiposDeCarga = string.Empty;
			if (_dados.TryGetValue("TipoDeCarga", out var tiposDeCarga))
				strTiposDeCarga = (string)tiposDeCarga;

			List<string> retorno = new List<string>();
			List<int> codigosTipoDeCarga = new List<int>();
			if (!string.IsNullOrWhiteSpace(strTiposDeCarga))
			{
				strTiposDeCarga = strTiposDeCarga.Replace(" ", "");
				string[] tipos = strTiposDeCarga.Split(',');

				if (tipos.Length > 0)
				{
					for (int i = 0; i < tipos.Length; i++)
					{
						Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(tipos[i]);
						if (tipoDeCarga != null)
							codigosTipoDeCarga.Add(tipoDeCarga.Codigo);
						else
							retorno.Add(tipos[i]);
					}
				}
			}

			if (retorno.Count > 0)
				throw new ImportacaoException($"Não foi possível identificar os tipos de carga ({string.Join(", ", retorno)})");

			return codigosTipoDeCarga;
		}

		private List<string> ObterEstados()
		{
			Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork);

			string strEstados = string.Empty;
			if (_dados.TryGetValue("Estado", out var outEstados))
				strEstados = (string)outEstados;

			List<string> retorno = new List<string>();
			List<string> siglasEstados = new List<string>();
			if (!string.IsNullOrWhiteSpace(strEstados))
			{
				strEstados = strEstados.Replace(" ", "");
				string[] estados = strEstados.Split(',');

				if (estados.Length > 0)
				{
					for (int i = 0; i < estados.Length; i++)
					{
						Dominio.Entidades.Estado estado = repositorioEstado.BuscarPorSigla(estados[i]);
						if (estado != null)
							siglasEstados.Add(estado.Sigla);
						else
							retorno.Add(estados[i]);
					}
				}
			}

			if (retorno.Count > 0)
				throw new ImportacaoException($"Não foi possível identificar os estados ({string.Join(", ", retorno)})");

			return siglasEstados;
		}

		private List<int> ObterRegioes()
		{
			Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(_unitOfWork);

			string strRegioes = string.Empty;
			if (_dados.TryGetValue("Regiao", out var outRegiao))
				strRegioes = (string)outRegiao;

			List<string> retorno = new List<string>();
			List<int> codigosRegioes = new List<int>();
			if (!string.IsNullOrWhiteSpace(strRegioes))
			{
				strRegioes = strRegioes.Replace(" ", "");
				string[] regioes = strRegioes.Split(',');

				if (regioes.Length > 0)
				{
					for (int i = 0; i < regioes.Length; i++)
					{
						Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repositorioRegiao.BuscarPorCodigoIntegracao(regioes[i]);
						if (regiao != null)
							codigosRegioes.Add(regiao.Codigo);
						else
							retorno.Add(regioes[i]);
					}
				}
			}

			if (retorno.Count > 0)
				throw new ImportacaoException($"Não foi possível identificar as regiões ({string.Join(", ", retorno)}).");

			return codigosRegioes;
		}

		private List<int> ObterLocalidades()
		{
			Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);

			string strLocalidade = string.Empty;
			if (_dados.TryGetValue("Cidade", out var outCidade))
				strLocalidade = (string)outCidade;

			List<string> retorno = new List<string>();
			List<int> codigosLocalidades = new List<int>();
			if (!string.IsNullOrWhiteSpace(strLocalidade))
			{
				strLocalidade = strLocalidade.Replace(" ", "");
				string[] localidades = strLocalidade.Split(',');

				if (localidades.Length > 0)
				{
					for (int i = 0; i < localidades.Length; i++)
					{
						Dominio.Entidades.Localidade localidade = repositorioLocalidade.BuscarPorCodigoIBGE(localidades[i].ObterSomenteNumeros().ToInt());
						if (localidade != null)
							codigosLocalidades.Add(localidade.Codigo);
						else
							retorno.Add(localidades[i]);
					}
				}
			}

			if (retorno.Count > 0)
				throw new ImportacaoException($"Não foi possível identificar as cidades ({string.Join(", ", retorno)}).");

			return codigosLocalidades;
		}

		#endregion

		#region Métodos Públicos

		public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculoImportar(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
		{

			Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork, Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(_tipoServicoMultisoftware));

			Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar()
			{
				CodigoCentroCarregamento = (int)_parametros.CentroCarregamento,
				CodigoFilial = (int)_parametros.Filial,
				CodigoMotorista = ObterMotorista(),
				CodigoTipoRetornoCarga = (int)_parametros.Tipo,
				CodigoVeiculo = ObterVeiculo(),
				DataProgramada = ObterDataProgramada(),
				CodigosTipoCarga = ObterTiposDeCarga(),
				SiglasEstadoDestino = ObterEstados(),
				CodigosRegiaoDestino = ObterRegioes(),
				CodigosDestino = ObterLocalidades()
			};

			if (filaCarregamentoVeiculoAdicionar.CodigosDestino.Count == 0 && filaCarregamentoVeiculoAdicionar.SiglasEstadoDestino.Count == 0 && filaCarregamentoVeiculoAdicionar.CodigosRegiaoDestino.Count == 0)
				throw new ImportacaoException("Necessário informar Cidade, Estado ou Região de destino.");

			return servicoFilaCarregamentoVeiculo.Adicionar(filaCarregamentoVeiculoAdicionar, tipoServicoMultisoftware);
		}

		#endregion

	}
}
