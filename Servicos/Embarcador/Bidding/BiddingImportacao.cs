using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Bidding.Importacao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Bidding
{
    public sealed class BiddingImportacao
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private string _flagOrigem;
        private string _flagDestino;
        private Dominio.Entidades.Localidade _localidadeOrigem;
        private Dominio.Entidades.Localidade _localidadeDestino;
        private readonly Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;
        private readonly CancellationToken _cancellationToken;

        #endregion

        #region Construtores

        public BiddingImportacao(Repositorio.UnitOfWork unitOfWork, Dictionary<string, dynamic> dados, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            _dados = dados;
            _unitOfWork = unitOfWork;
            _configuracaoIntegracao = configuracaoIntegracao;
        }

        public BiddingImportacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancellationToken;
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterTipoCarga(List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga)
        {
            var tipoCarga = string.Empty;

            if (_dados.TryGetValue("TiposCarga", out var tipoCargaString))
                tipoCarga = (string)tipoCargaString;

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipo = tiposCarga.Find(obj => obj.Descricao.Contains(tipoCarga) || (obj.CodigoTipoCargaEmbarcador.Contains(tipoCarga)));

            if (tipo == null) return new[] { new { Codigo = 0, Descricao = "" } };

            return new[]
            {
                new { tipo.Codigo, tipo.Descricao }
            };
        }

        private dynamic ObterModeloVeicular(List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares)
        {
            var modeloVeicular = string.Empty;

            if (_dados.TryGetValue("ModelosVeiculares", out var modeloVeicularString))
                modeloVeicular = (string)modeloVeicularString;

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modelo = modelosVeiculares.Find(obj => obj.Descricao.Contains(modeloVeicular) || (obj.CodigoIntegracao == modeloVeicular));

            if (modelo == null) return new[] { new { Codigo = 0, Descricao = "" } };

            return new[]
            {
                new { modelo.Codigo, modelo.Descricao }
            };
        }

        private dynamic ObterLocalidade(List<Dominio.Entidades.Localidade> localidades, string localidadeOrigemDestino)
        {
            var localidadeCliente = string.Empty;

            if (_dados.TryGetValue(localidadeOrigemDestino, out var localidadeString))
                localidadeCliente = (string)localidadeString;

            Dominio.Entidades.Localidade localidade = localidades.Find(obj => obj.Descricao.Contains(localidadeCliente.Split('-')[0].ToUpper()));

            if (localidade == null) return null;

            if (localidadeOrigemDestino == "CidadesOrigem")
                _localidadeOrigem = localidade;
            else
                _localidadeDestino = localidade;

            _flagOrigem = LocalidadeBiddingFlag.Cidade.ObterDescricao();
            _flagDestino = LocalidadeBiddingFlag.Cidade.ObterDescricao();

            return new[]
            {
                new { localidade.Codigo, localidade.Descricao }
            };
        }

        private dynamic ObterCliente(List<Dominio.Entidades.Cliente> clientes, string clienteOrigemDestino)
        {
            var clienteBiddin = string.Empty;

            if (_dados.TryGetValue(clienteOrigemDestino, out var estadoString))
                clienteBiddin = (string)estadoString;

            Dominio.Entidades.Cliente cliente = clientes.Find(obj => obj.CPF_CNPJ == clienteBiddin.ToDouble() || obj.CodigoIntegracao == clienteBiddin);

            if (cliente == null) return null;

            _flagOrigem = LocalidadeBiddingFlag.Cliente.ObterDescricao();
            _flagDestino = LocalidadeBiddingFlag.Cliente.ObterDescricao();

            return new[]
            {
                new { cliente.Codigo, cliente.Descricao }
            };
        }

        private dynamic ObterEstado(List<Dominio.Entidades.Estado> estados, string estadoOrigemDestino)
        {
            var estadoBiddin = string.Empty;

            if (_dados.TryGetValue(estadoOrigemDestino, out var estadoString))
                estadoBiddin = (string)estadoString;

            Dominio.Entidades.Estado estado = estados.Find(obj => obj.Sigla == estadoBiddin);

            if (estado == null) return null;

            _flagOrigem = LocalidadeBiddingFlag.Estado.ObterDescricao();
            _flagDestino = LocalidadeBiddingFlag.Estado.ObterDescricao();

            return new[]
            {
                new { estado.Codigo, estado.Descricao }
            };
        }

        private dynamic ObterRegiao(List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioes, string regiaoOrigemDestino)
        {
            var regiaoBiddin = string.Empty;

            if (_dados.TryGetValue(regiaoOrigemDestino, out var regiaoString))
                regiaoBiddin = (string)regiaoString;

            Dominio.Entidades.Embarcador.Localidades.Regiao regiao = regioes.Find(obj => obj.CodigoIntegracao == regiaoBiddin || obj.Descricao == regiaoBiddin);

            if (regiao == null) return null;


            _flagOrigem = LocalidadeBiddingFlag.Regiao.ObterDescricao();
            _flagDestino = LocalidadeBiddingFlag.Regiao.ObterDescricao();

            return new[]
            {
                new { regiao.Codigo, regiao.Descricao }
            };
        }

        private dynamic ObterPais(List<Dominio.Entidades.Pais> paises, string paisOrigemDestino)
        {
            var paisBiddin = string.Empty;

            if (_dados.TryGetValue(paisOrigemDestino, out var paisString))
                paisBiddin = (string)paisString;

            Dominio.Entidades.Pais pais = paises.Find(obj => obj.Sigla == paisBiddin || obj.Nome == paisBiddin);

            if (pais == null) return null;

            _flagOrigem = LocalidadeBiddingFlag.Pais.ObterDescricao();
            _flagDestino = LocalidadeBiddingFlag.Pais.ObterDescricao();

            return new[]
            {
                new { pais.Codigo, pais.Descricao }
            };
        }

        private dynamic ObterRotaFrete(List<Dominio.Entidades.RotaFrete> rotasFrete, string rotaOrigemDestino)
        {
            var rotaFrete = string.Empty;

            if (_dados.TryGetValue(rotaOrigemDestino, out var rotaString))
                rotaFrete = (string)rotaString;

            Dominio.Entidades.RotaFrete rota = rotasFrete.Find(obj => obj.CodigoIntegracao == rotaFrete || obj.Descricao == rotaFrete);

            if (rota == null) return null;

            _flagOrigem = LocalidadeBiddingFlag.Rota.ObterDescricao();
            _flagDestino = LocalidadeBiddingFlag.Rota.ObterDescricao();

            return new[]
            {
                new { rota.Codigo, rota.Descricao }
            };
        }

        private dynamic ObterFilialParticipante(List<Dominio.Entidades.Embarcador.Filiais.Filial> filialParticipantes)
        {
            var filialParticipante = string.Empty;

            if (_dados.TryGetValue("FiliaisParticipantes", out var filialString))
                filialParticipante = (string)filialString;

            Dominio.Entidades.Embarcador.Filiais.Filial filial = filialParticipantes.Find(obj => obj.CodigoFilialEmbarcador == filialParticipante || obj.Descricao == filialParticipante);

            if (filial == null) return new[] { new { Codigo = 0, Descricao = "" } };

            return new[] { new { filial.Codigo, filial.Descricao } };
        }

        private dynamic ObterGrupoModeloVeicular(List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> gruposModelosVeiculares)
        {
            var grupoModelo = string.Empty;

            if (_dados.TryGetValue("GrupoModeloVeicular", out var grupoModeloString))
                grupoModelo = (string)grupoModeloString;

            Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupo = gruposModelosVeiculares.Find(obj => obj.Descricao == grupoModelo);

            if (grupo == null) return new { Codigo = 0, Descricao = "" };

            return new { grupo.Codigo, grupo.Descricao };

        }

        private dynamic ObterModeloCarroceria(List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> modelosCarrocerias)
        {
            var modeloCarroceria = string.Empty;

            if (_dados.TryGetValue("CarroceriaVeiculo", out var modeloString))
                modeloCarroceria = (string)modeloString;

            Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modelo = modelosCarrocerias.Find(obj => obj.CodigoIntegracao == modeloCarroceria);

            if (modelo == null) return new[] { new { Codigo = 0, Descricao = "" } };

            return new { modelo.Codigo, modelo.Descricao };

        }

        private dynamic ObterTomador(List<Dominio.Entidades.Cliente> tomadores)
        {
            var tomadorBidding = string.Empty;

            if (_dados.TryGetValue("Tomador", out var tomadorString))
                tomadorBidding = (string)tomadorString;

            Dominio.Entidades.Cliente tomador = tomadores.Find(obj => obj.CPF_CNPJ == tomadorBidding.ToDouble() || obj.CodigoIntegracao == tomadorBidding);

            if (tomador == null) return null;

            return new { tomador.Codigo, tomador.Descricao };

        }

        private dynamic ObterCEP(List<(string De, string Ate)> ceps, string cepsOrigemDestino)
        {
            var cepBiddin = string.Empty;

            if (_dados.TryGetValue(cepsOrigemDestino, out var cepString))
                cepBiddin = (string)cepString;

            (string De, string Ate) cep = ceps.Find(obj => obj.De == cepBiddin.Split('-')[0] && obj.Ate == cepBiddin.Split('-')[1]);

            if (cep.De == null) return null;

            _flagOrigem = LocalidadeBiddingFlag.CEP.ObterDescricao();
            _flagDestino = LocalidadeBiddingFlag.CEP.ObterDescricao();

            return new[]
            {
                new
                {
                    CEPInicial = cep.De,
                    CEPFinal = cep.Ate,
                    Codigo = Guid.NewGuid().ToString()
                }
            };
        }

        private dynamic ObterBaseLine(List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> baselines)
        {
            var baselineBidding = string.Empty;
            decimal valorBaselineBidding = 0;

            if (_dados.TryGetValue("Baseline", out var baselineString))
                baselineBidding = (string)baselineString;

            if (_dados.TryGetValue("ValorBaseline", out var valorBaselineString))
                valorBaselineBidding = ((string)valorBaselineString).ToDecimal();

            if (string.IsNullOrWhiteSpace(baselineBidding))
                throw new ImportacaoException($"Baseline: {baselineBidding} não encontrado");

            if (valorBaselineBidding <= 0)
                throw new ImportacaoException($"Valor Baseline: {valorBaselineBidding} não encontrado");

            Dominio.Entidades.Embarcador.Bidding.TipoBaseline baseline = baselines.Find(obj => obj.CodigoIntegracao == baselineBidding || obj.Descricao == baselineBidding);

            if (baseline == null) return new[] { new { Codigo = 0, Descricao = "" } };

            return new[]
            {
                new
                {
                    Codigo = Guid.NewGuid(),
                    CodigoTipoBaseline = baseline.Codigo,
                    TipoBaseline = baseline.Descricao ?? string.Empty,
                    Valor = valorBaselineBidding.ToString("n2"),
                }
            };
        }

        private string ObterDescricao()
        {
            var descricaoRetornar = string.Empty;

            if (_dados.TryGetValue("Descricao", out var descricao))
                descricaoRetornar = (string)descricao;

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ImportacaoException("Descrição não informada.");

            return descricaoRetornar.Trim();
        }

        private string ObterString(string campo)
        {
            var frequenciaRetornar = string.Empty;

            if (_dados.TryGetValue(campo, out var valor))
                frequenciaRetornar = (string)valor;

            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            return frequenciaRetornar.Trim();
        }

        private int ObterInteiro(string campo)
        {
            var valorRetornar = string.Empty;

            if (_dados.TryGetValue(campo, out var valor))
                valorRetornar = (string)valor;

            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            return valorRetornar.ToInt();
        }

        private int ObterEnumInconterm()
        {
            var valorRetornar = string.Empty;

            if (_dados.TryGetValue("Inconterm", out var valor))
                valorRetornar = (string)valor;

            if (string.IsNullOrWhiteSpace(valor))
                return (int)Inconterm.CIF;

            return (int)valorRetornar.ToNullableEnum<Inconterm>();
        }

        private int ObterEnumSimNaoNA(string campo)
        {
            var valorRetornar = string.Empty;

            if (_dados.TryGetValue(campo, out var valor))
                valorRetornar = (string)valor;

            if (string.IsNullOrWhiteSpace(valor))
                return (int)SimNaoNA.Nao;

            return (int)valorRetornar.ToEnum<SimNaoNA>();
        }

        private decimal ObterDecimal(string campo)
        {
            var valorRetornar = string.Empty;

            if (_dados.TryGetValue(campo, out var valor))
                valorRetornar = (string)valor;

            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            return valorRetornar.ToDecimal();
        }

        private decimal ObterKMMediaRota()
        {
            var valorRetornar = string.Empty;

            if (_dados.TryGetValue("KMMediaRota", out var valor))
                valorRetornar = (string)valor;

            if (string.IsNullOrWhiteSpace(valor))
                return CalcularKmMediaRota();


            return valorRetornar.ToDecimal();
        }

        private TimeSpan? ObterTimeSpan(string campo)
        {
            var valorRetornar = string.Empty;

            if (_dados.TryGetValue(campo, out var valor))
                valorRetornar = (string)valor;

            if (string.IsNullOrWhiteSpace(valor))
                return null;

            TimeSpan? tempoColeta = null;

            DateTime? tempoColetaDateTime = valorRetornar.ToNullableDateTime();

            if (tempoColetaDateTime.HasValue)
            {
                tempoColeta = tempoColetaDateTime.Value.TimeOfDay;
            }
            else
            {
                tempoColeta = valorRetornar.ToNullableTime();

                if (!tempoColeta.HasValue)
                {
                    tempoColetaDateTime = Utilidades.DateTime.ConverterDataExcelToNullableDateTime((valorRetornar).ToDouble());

                    if (tempoColetaDateTime.HasValue)
                    {
                        tempoColeta = tempoColetaDateTime.Value.TimeOfDay;
                    }
                }
            }

            return tempoColeta;
        }
        private async Task<DadosImportacaoBidding> ObterLocalidadesAsync(Dictionary<string, List<string>> linhas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork, _cancellationToken);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork, _cancellationToken);
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork, _cancellationToken);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork, _cancellationToken);
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Localidade> localidadesOrigemDestino = new();
            List<string> listaCombinada = new();

            if (linhas.TryGetValue("CidadesOrigem", out List<string> listaCidadesOrigem) && listaCidadesOrigem?.Count > 0)
                listaCombinada.AddRange(listaCidadesOrigem);

            if (linhas.TryGetValue("CidadesDestino", out List<string> listaCidadesDestino) && listaCidadesDestino?.Count > 0)
                listaCombinada.AddRange(listaCidadesDestino);

            if (listaCombinada.Count > 0)
            {
                List<(string Cidade, string Estado)> pares = listaCombinada
                    .Select(c => c.Split('-'))
                    .Where(p => p.Length == 2)
                    .Select(p => (p[0].Trim(), p[1].Trim()))
                    .Distinct()
                    .ToList();

                localidadesOrigemDestino = await repositorioLocalidade.BuscarPorDescricoesEUFAsync(pares);
            }

            List<Dominio.Entidades.Cliente> clientesOrigemDestino = new();
            List<string> listaCombinadaClientes = new();

            if (linhas.TryGetValue("ClientesOrigem", out List<string> listaClientesOrigem) && listaClientesOrigem?.Count > 0)
                listaCombinadaClientes.AddRange(listaClientesOrigem);

            if (linhas.TryGetValue("ClientesDestino", out List<string> listaClientesDestino) && listaClientesDestino?.Count > 0)
                listaCombinadaClientes.AddRange(listaClientesDestino);

            if (listaCombinadaClientes.Count > 0)
            {
                List<double> listaCpfCnpj = new();
                List<string> listaCodIntegracao = new();

                foreach (string valor in listaCombinadaClientes)
                {
                    string valorLimpo = valor.Trim();
                    if (double.TryParse(valorLimpo, out double cpfcnpj))
                        listaCpfCnpj.Add(cpfcnpj);
                    else
                        listaCodIntegracao.Add(valorLimpo);
                }

                List<Dominio.Entidades.Cliente> encontrados = new();

                if (listaCpfCnpj.Count > 0)
                    encontrados.AddRange(await repositorioCliente.BuscarPorCPFCNPJsAsync(listaCpfCnpj));

                if (listaCodIntegracao.Count > 0)
                    encontrados.AddRange(await repositorioCliente.BuscarPorCodigosIntegracaoAsync(listaCodIntegracao));

                clientesOrigemDestino = encontrados.Distinct().ToList();
            }

            List<Dominio.Entidades.Estado> estadosOrigemDestino = new();
            List<string> listaCombinadaEstados = new();

            if (linhas.TryGetValue("EstadosOrigem", out List<string> listaEstadosOrigem) && listaEstadosOrigem?.Count > 0)
                listaCombinadaEstados.AddRange(listaEstadosOrigem);

            if (linhas.TryGetValue("EstadosDestino", out List<string> listaEstadosDestino) && listaEstadosDestino?.Count > 0)
                listaCombinadaEstados.AddRange(listaEstadosDestino);

            if (listaCombinadaEstados.Count > 0)
            {
                List<string> siglas = listaCombinadaEstados
                    .Select(s => s.Trim().ToUpper())
                    .Distinct()
                    .ToList();

                estadosOrigemDestino = await repositorioEstado.BuscarPorSiglasAsync(siglas);
            }

            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigemDestino = new();
            List<string> listaCombinadaRegioes = new();

            if (linhas.TryGetValue("RegioesOrigem", out List<string> listaRegioesOrigem) && listaRegioesOrigem?.Count > 0)
                listaCombinadaRegioes.AddRange(listaRegioesOrigem);

            if (linhas.TryGetValue("RegioesDestino", out List<string> listaRegioesDestino) && listaRegioesDestino?.Count > 0)
                listaCombinadaRegioes.AddRange(listaRegioesDestino);

            if (listaCombinadaRegioes.Count > 0)
            {
                List<string> valores = listaCombinadaRegioes
                    .Select(v => v.Trim())
                    .Distinct()
                    .ToList();

                List<Dominio.Entidades.Embarcador.Localidades.Regiao> encontrados = new();

                encontrados.AddRange(await repositorioRegiao.BuscarPorCodigosIntegracaoAsync(valores));

                List<string> naoEncontrados = valores
                    .Where(v => !encontrados.Any(r => r.CodigoIntegracao == v))
                    .ToList();

                if (naoEncontrados.Count > 0)
                    encontrados.AddRange(await repositorioRegiao.BuscarPorDescricoesAsync(naoEncontrados));

                regioesOrigemDestino = encontrados.Distinct().ToList();
            }

            List<Dominio.Entidades.RotaFrete> rotasOrigemDestino = new();
            List<string> listaCombinadaRotas = new();

            if (linhas.TryGetValue("RotasOrigem", out List<string> listaRotasOrigem) && listaRotasOrigem?.Count > 0)
                listaCombinadaRotas.AddRange(listaRotasOrigem);

            if (linhas.TryGetValue("RotasDestino", out List<string> listaRotasDestino) && listaRotasDestino?.Count > 0)
                listaCombinadaRotas.AddRange(listaRotasDestino);

            if (listaCombinadaRotas.Count > 0)
            {
                List<string> valores = listaCombinadaRotas
                    .Select(v => v.Trim())
                    .Distinct()
                    .ToList();

                List<Dominio.Entidades.RotaFrete> encontrados = new();

                encontrados.AddRange(await repositorioRotaFrete.BuscarPorCodigosIntegracaoAsync(valores));

                List<string> naoEncontrados = valores
                    .Where(v => !encontrados.Any(r => r.CodigoIntegracao == v))
                    .ToList();

                if (naoEncontrados.Count > 0)
                    encontrados.AddRange(await repositorioRotaFrete.BuscarPorDescricoesParcialAsync(naoEncontrados));

                rotasOrigemDestino = encontrados.Distinct().ToList();
            }

            List<(string De, string Ate)> faixasCEPsOrigemDestino = new();
            List<string> listaCombinadaFaixas = new();

            if (linhas.TryGetValue("FaixasCEPOrigem", out List<string> listaFaixasOrigem) && listaFaixasOrigem?.Count > 0)
                listaCombinadaFaixas.AddRange(listaFaixasOrigem);

            if (linhas.TryGetValue("FaixasCEPDestino", out List<string> listaFaixasDestino) && listaFaixasDestino?.Count > 0)
                listaCombinadaFaixas.AddRange(listaFaixasDestino);

            if (listaCombinadaFaixas.Count > 0)
            {
                faixasCEPsOrigemDestino = listaCombinadaFaixas
                    .Select(faixa => faixa.Split(','))
                    .Where(partes => partes.Length == 2)
                    .Select(partes => (partes[0].Trim(), partes[1].Trim()))
                    .Distinct()
                    .ToList();
            }

            List<Dominio.Entidades.Pais> paisesOrigemDestino = new();
            List<string> listaCombinadaPaises = new();

            if (linhas.TryGetValue("PaisesOrigem", out List<string> listaPaisesOrigem) && listaPaisesOrigem?.Count > 0)
                listaCombinadaPaises.AddRange(listaPaisesOrigem);

            if (linhas.TryGetValue("PaisesDestino", out List<string> listaPaisesDestino) && listaPaisesDestino?.Count > 0)
                listaCombinadaPaises.AddRange(listaPaisesDestino);

            if (listaCombinadaPaises.Count > 0)
            {
                List<string> valores = listaCombinadaPaises
                    .Select(v => v.Trim())
                    .Distinct()
                    .ToList();

                List<Dominio.Entidades.Pais> encontrados = new();

                encontrados.AddRange(await repositorioPais.BuscarPorListaSiglaAsync(valores));

                List<string> naoEncontrados = valores
                    .Where(v => !encontrados.Any(p => p.Sigla.Equals(v, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (naoEncontrados.Count > 0)
                    encontrados.AddRange(await repositorioPais.BuscarPorNomesAsync(naoEncontrados));

                paisesOrigemDestino = encontrados.Distinct().ToList();
            }

            return new DadosImportacaoBidding
            {
                Localidades = localidadesOrigemDestino,
                Clientes = clientesOrigemDestino,
                Estados = estadosOrigemDestino,
                Regioes = regioesOrigemDestino,
                Rotas = rotasOrigemDestino,
                FaixasCEP = faixasCEPsOrigemDestino,
                Paises = paisesOrigemDestino
            };


        }

        private decimal CalcularKmMediaRota()
        {
            if (_localidadeOrigem is null || _localidadeDestino is null)
                return 0;

            double origemLatitude = (double)(_localidadeOrigem.Latitude ?? 0);
            double origemLongitude = (double)(_localidadeOrigem.Longitude ?? 0);
            double destinoLatitude = (double)(_localidadeDestino.Latitude ?? 0);
            double destinoLongitude = (double)(_localidadeDestino.Longitude ?? 0);

            if (origemLatitude == 0 ||
                origemLongitude == 0 ||
                destinoLatitude == 0 ||
                destinoLongitude == 0 ||
                origemLatitude < -90 ||
                origemLatitude > 90 ||
                destinoLatitude < -90 ||
                destinoLatitude > 90)
            {
                return 0;
            }

            Logistica.Roteirizacao rota = new(_configuracaoIntegracao.ServidorRouteOSM);

            rota.Clear();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPoints = new()
            {
                new()
                {
                    Lat = origemLatitude,
                    Lng = origemLongitude,
                    Descricao = _localidadeOrigem.DescricaoCidadeEstado,
                    Sequencia = 1,
                    TipoPonto = TipoPontoPassagem.Coleta,
                },
                new()
                {
                    Lat = destinoLatitude,
                    Lng = destinoLongitude,
                    Descricao = _localidadeDestino.DescricaoCidadeEstado,
                    Sequencia = 2,
                    TipoPonto = TipoPontoPassagem.Entrega,
                }
            };

            rota.Add(wayPoints);

            var opcoes = new Logistica.OpcoesRoteirizar
            {
                AteOrigem = false,
                Ordenar = false,
                PontosNaRota = false
            };

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = rota.Roteirizar(opcoes);

            return respostaRoteirizacao.Distancia;
        }


        #endregion

        #region Métodos Públicos

        public dynamic ObterImportacaoRota(DadosImportacaoBidding dadosImportacaoBidding)
        {
            return new
            {
                Rota = ObterDescricao(),
                Descricao = ObterDescricao(),
                Frequencia = ObterString("FrequenciaMensal"),
                Volume = ObterString("Volume"),
                NumeroEntrega = ObterInteiro("NumeroEntregasMensal"),
                ValorCargaMes = ObterDecimal("ValorCargaMes"),
                Peso = ObterDecimal("Peso"),
                AdicionalAPartirDaEntregaNumero = ObterInteiro("AdicionalPartirEntregaN"),
                Observacao = ObterString("Observacao"),
                TipoCarga = ObterTipoCarga(dadosImportacaoBidding.TiposCarga),
                ModeloVeicular = ObterModeloVeicular(dadosImportacaoBidding.ModelosVeiculares),
                CidadeOrigem = ObterLocalidade(dadosImportacaoBidding.Localidades, "CidadesOrigem"),
                CidadeDestino = ObterLocalidade(dadosImportacaoBidding.Localidades, "CidadesDestino"),
                QuilometragemMedia = ObterKMMediaRota(),
                ClienteOrigem = ObterCliente(dadosImportacaoBidding.Clientes, "ClientesOrigem"),
                ClienteDestino = ObterCliente(dadosImportacaoBidding.Clientes, "ClientesDestino"),
                EstadoOrigem = ObterEstado(dadosImportacaoBidding.Estados, "EstadoOrigem"),
                EstadoDestino = ObterEstado(dadosImportacaoBidding.Estados, "EstadoDestino"),
                RegiaoOrigem = ObterRegiao(dadosImportacaoBidding.Regioes, "RegiaoOrigem"),
                RegiaoDestino = ObterRegiao(dadosImportacaoBidding.Regioes, "RegiaoDestino"),
                PaisOrigem = ObterPais(dadosImportacaoBidding.Paises, "PaisOrigem"),
                PaisDestino = ObterPais(dadosImportacaoBidding.Paises, "PaisDestino"),
                CEPOrigem = ObterCEP(dadosImportacaoBidding.FaixasCEP, "FaixasCEPOrigem"),
                CEPDestino = ObterCEP(dadosImportacaoBidding.FaixasCEP, "FaixasCEPDestino"),
                RotaOrigem = ObterRotaFrete(dadosImportacaoBidding.Rotas, "RotasOrigem"),
                RotaDestino = ObterRotaFrete(dadosImportacaoBidding.Rotas, "RotasDestino"),
                Baseline = ObterBaseLine(dadosImportacaoBidding.TiposBaseline),
                Tomador = ObterTomador(dadosImportacaoBidding.Tomadores),
                GrupoModeloVeicular = ObterGrupoModeloVeicular(dadosImportacaoBidding.GruposModeloVeicular),
                ModeloCarroceria = ObterModeloCarroceria(dadosImportacaoBidding.ModelosCarroceria),
                FrequenciaMensalComAjudante = ObterInteiro("FrequenciaMensalComAjudante"),
                QuantidadeAjudantePorVeiculo = ObterInteiro("QuantidadeAjudantesVeiculo"),
                MediaEntregasFracionada = ObterInteiro("MediaEntregasFracionadas"),
                MaximaEntregasFacionada = ObterInteiro("MaximaEntregasFracionadas"),
                Inconterm = ObterEnumInconterm(),
                QuantidadeViagensPorAno = ObterInteiro("QuantidadeViagensAno"),
                VolumeTonAno = ObterInteiro("VolumeToneladaAno"),
                VolumeTonViagem = ObterInteiro("VolumeToneladaViagem"),
                TempoColeta = ObterTimeSpan("TempoColeta")?.ToTimeString() ?? string.Empty,
                TempoDescarga = ObterTimeSpan("TempoDescarga")?.ToTimeString() ?? string.Empty,
                Compressor = ObterEnumSimNaoNA("Compressor"),
                FiliaisParticipante = ObterFilialParticipante(dadosImportacaoBidding.FiliaisParticipantes),
                ValorMedioNFe = ObterDecimal("ValorMedioNFe"),
                FlagOrigem = _flagOrigem,
                FlagDestino = _flagDestino
            };
        }

        public async Task<DadosImportacaoBidding> ObterDadosImportacao(Dictionary<string, List<string>> linhas)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.GrupoModeloVeicular repositorioGrupoModeloVeicularCarga = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Bidding.TipoBaseline repositorioTipoBaseline = new Repositorio.Embarcador.Bidding.TipoBaseline(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Veiculos.ModeloCarroceria repositorioModeloCarroceria = new(_unitOfWork, _cancellationToken);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork, _cancellationToken);

            DadosImportacaoBidding dados = await ObterLocalidadesAsync(linhas, _unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares = new();

            if (linhas.TryGetValue("ModelosVeiculares", out List<string> listaModelos) && listaModelos?.Count > 0)
            {
                List<string> modelos = listaModelos.Select(v => v.Trim()).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> encontrados = new();

                encontrados.AddRange(await repositorioModeloVeicularCarga.BuscarPorCodigosIntegracaoAsync(modelos));

                List<string> naoEncontrados = modelos
                    .Where(v => !encontrados.Any(m => m.CodigoIntegracao == v))
                    .ToList();

                if (naoEncontrados.Count > 0)
                    encontrados.AddRange(await repositorioModeloVeicularCarga.BuscarPorDescricoesAsync(naoEncontrados));

                modelosVeiculares = encontrados.Distinct().ToList();
            }

            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargas = new();

            if (linhas.TryGetValue("TiposCarga", out List<string> listaTiposCarga) && listaTiposCarga?.Count > 0)
            {
                List<string> registros = listaTiposCarga
                    .Select(v => v.Trim())
                    .Distinct()
                    .ToList();

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> encontrados = new();

                encontrados.AddRange(await repositorioTipoCarga.BuscarPorCodigosIntegracaoAsync(registros));

                List<string> naoEncontrados = registros
                    .Where(v => !encontrados.Any(tc => tc.CodigoTipoCargaEmbarcador == v))
                    .ToList();

                if (naoEncontrados.Count > 0)
                    encontrados.AddRange(await repositorioTipoCarga.BuscarPorDescricoesAsync(naoEncontrados, true));

                tiposCargas = encontrados.Distinct().ToList();
            }

            List<Dominio.Entidades.Cliente> tomadores = new();

            if (linhas.TryGetValue("Tomador", out List<string> listaTomadores) && listaTomadores?.Count > 0)
            {
                tomadores.AddRange(await repositorioCliente.BuscarPorCPFCNPJsAsync(listaTomadores.Select(x => x.ToDouble()).ToList()));
                tomadores.AddRange(await repositorioCliente.BuscarPorCodigosIntegracaoAsync(listaTomadores));
                tomadores = tomadores.Distinct().ToList();
            }


            List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> gruposModeloVeicular = new();

            if (linhas.TryGetValue("GrupoModeloVeicular", out List<string> listaGrupo) && listaGrupo?.Count > 0)
            {
                var valores = listaGrupo.Select(v => v.Trim()).Distinct().ToList();

                gruposModeloVeicular = await repositorioGrupoModeloVeicularCarga.BuscarPorDescricoesAsync(valores);
            }


            List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> modelosCarroceria = new();

            if (linhas.TryGetValue("CarroceriaVeiculo", out List<string> listaCarroceria) && listaCarroceria?.Count > 0)
            {
                var valores = listaCarroceria.Select(v => v.Trim()).Distinct().ToList();

                modelosCarroceria = await repositorioModeloCarroceria.BuscarPorCodigosIntegracaoAsync(valores);
            }


            List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> tiposBaseline = new();
            if (linhas.TryGetValue("Baseline", out List<string> listaBaseline) && listaBaseline?.Count > 0)
            {
                var valores = listaBaseline.SelectMany(v => v.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                                           .Select(v => v.Trim())
                                           .Distinct()
                                           .ToList();

                if (valores.Count > 0)
                {
                    tiposBaseline.AddRange(await repositorioTipoBaseline.BuscarPorCodigosIntegracaoAsync(valores));

                    var naoEncontrados = valores
                        .Where(v => !tiposBaseline.Any(tb => tb.CodigoIntegracao == v))
                        .ToList();

                    if (naoEncontrados.Count > 0)
                        tiposBaseline.AddRange(await repositorioTipoBaseline.BuscarPorDescricoesAsync(naoEncontrados));

                    tiposBaseline = tiposBaseline.Distinct().ToList();
                }
            }

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisParticipantes = new();
            if (linhas.TryGetValue("FiliaisParticipantes", out List<string> listaFiliais) && listaFiliais?.Count > 0)
            {
                var valores = listaFiliais.SelectMany(v => v.Split(';'))
                                          .Select(v => v.Trim())
                                          .Distinct()
                                          .ToList();

                if (valores.Count > 0)
                {
                    filiaisParticipantes.AddRange(await repositorioFilial.BuscarPorCodigosIntegracaoAsync(valores));

                    var naoEncontradas = valores
                        .Where(v => !filiaisParticipantes.Any(f => f.CodigoFilialEmbarcador == v))
                        .ToList();

                    if (naoEncontradas.Count > 0)
                        filiaisParticipantes.AddRange(await repositorioFilial.BuscarPorDescricoesAsync(naoEncontradas));

                    filiaisParticipantes = filiaisParticipantes.Distinct().ToList();
                }
            }


            dados.ModelosCarroceria = modelosCarroceria;
            dados.GruposModeloVeicular = gruposModeloVeicular;
            dados.Tomadores = tomadores;
            dados.ModelosVeiculares = modelosVeiculares;
            dados.FiliaisParticipantes = filiaisParticipantes;
            dados.TiposBaseline = tiposBaseline;
            dados.TiposCarga = tiposCargas;

            return dados;
        }

        #endregion
    }
}