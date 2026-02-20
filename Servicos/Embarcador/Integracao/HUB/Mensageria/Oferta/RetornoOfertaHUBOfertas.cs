using Dominio.Entidades;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoDemandaTransporte;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao.HUB.Base;
using Servicos.Embarcador.Integracao.HUB.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB.Mensageria.Oferta
{
    public class RetornoOfertaHUBOfertas
    {
        #region Propriedades Privadas
        private readonly Repositorio.Empresa _repositorioEmpresa;
        private readonly Repositorio.Veiculo _repositorioVeiculo;
        private readonly Repositorio.Localidade _repositorioLocalidade;
        protected readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Usuario _repositorioUsuario;
        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;
        private readonly string _tipoCNPJ = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a";
        private readonly Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas _repositorioCargaIntegracaoHUB;
        private readonly HubHttpClient _hubHttp;
        #endregion

        #region Construtores
        public RetornoOfertaHUBOfertas(Repositorio.UnitOfWork unitOfWork)
        {
            _repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            _repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            _repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            _unitOfWork = unitOfWork;
            _repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            _repositorioCargaIntegracaoHUB = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(unitOfWork);
            _hubHttp = new HubHttpClient(new Repositorio.Embarcador.Configuracoes.IntegracaoHUB(unitOfWork).BuscarPrimeiroRegistro());
        }
        #endregion

        #region Métodos Públicos

        public async Task<bool> ProcessarRetornoOfertaHUBOfertas(string jsonRetorno)
        {
            MensagemRetorno retornoDemanda = JsonConvert.DeserializeObject<MensagemRetorno>(jsonRetorno);

            HttpRequisicaoResposta respostaHttp = await _hubHttp.GetAsync($"/api/Offers/{retornoDemanda.Id}");

            if (respostaHttp.httpStatusCode != HttpStatusCode.OK)
                return false;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta.Oferta oferta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta.Oferta>(respostaHttp.conteudoResposta);
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracaoHUB = await _repositorioCargaIntegracaoHUB.BuscarPorVinculoDemanda(oferta.DemandaTransporte.Id);

            if (cargaIntegracaoHUB == null)
                throw new ArgumentNullException("Mensagem não possui vinculo a nenhuma carga desse embarcador.");

            try
            {
                RetornoOfertaHUBOfertas retornoOfertaHUBOfertas = new RetornoOfertaHUBOfertas(_unitOfWork);
                await retornoOfertaHUBOfertas.ProcessarOferta(cargaIntegracaoHUB, oferta);
                cargaIntegracaoHUB.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaIntegracaoHUB.ProblemaIntegracao = $"Carga - {cargaIntegracaoHUB.Carga.CodigoCargaEmbarcador} retornou do TNS Match!!";
            }
            catch (ServicoException excecao)
            {
                cargaIntegracaoHUB.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoHUB.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                cargaIntegracaoHUB.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoHUB.ProblemaIntegracao = "Problema ao tentar integrar com HUB de ofertas.";
                Log.TratarErro($"Erro integração Transportador: {excecao.Message}", "HUBOfertas");
            }
            finally
            {
                await _repositorioCargaIntegracaoHUB.AtualizarAsync(cargaIntegracaoHUB);
            }

            return true;
        }


        #endregion

        #region Metodos Privados
        private async Task<bool> LimparDadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            carga.Motoristas = null;
            carga.VeiculosVinculados = null;
            carga.Empresa = null;
            carga.Veiculo = null;
            await _repositorioCarga.AtualizarAsync(carga);

            return true;
        }

        private async Task<bool> ProcessarOferta(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracaoHUB, Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta.Oferta oferta)
        {
            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracaoHUB.Carga;

                if (oferta.Status == 4 || oferta.Status == 5)
                {
                    await LimparDadosCarga(carga);
                    return true;
                }

                List<string> placasVeiculos = oferta.VeiculosOferta.Where(x => !string.IsNullOrEmpty(x.Veiculo?.VeiculoTerrestre?.Placa ?? "")).Select(x => x.Veiculo?.VeiculoTerrestre?.Placa ?? "").ToList();

                Dominio.Entidades.Empresa empresa = await CadastrarEmpresa(oferta.Transportadora);

                await GerarVeiculos(carga, oferta.VeiculosOferta, empresa, placasVeiculos);
                await GerarMotoristas(carga, oferta.EquipesOferta);
                await AtualizarCarga(carga, oferta, placasVeiculos, empresa);

                return true;
            }
            catch (Exception ex)
            {
                Log.TratarErro($"Erro em ProcessarOferta: {ex.Message}", "HUBOfertas");
                return false;
            }

        }

        private async Task<bool> AtualizarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta.Oferta oferta, List<string> placas, Dominio.Entidades.Empresa empresa)
        {

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                carga.ValorFrete = oferta.PrecoFreteNegociado ?? 0;
                carga.ValorFreteAPagar = oferta.PrecoFreteNegociado ?? 0;
                carga.ValorFreteContratoFrete = oferta.PrecoFreteNegociado ?? 0;

                carga.Empresa = empresa;

                await repositorioCarga.AtualizarAsync(carga);

                return true;
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Falha no AtualizarCarga : {excecao.Message}", "HUBOfertas");
                return false;
            }
        }

        private async Task<bool> GerarMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<EquipeOferta> equipesOferta)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);

            try
            {
                List<string> cpfsMotoristas = equipesOferta.Where(x => !string.IsNullOrEmpty(x.OperadorTransporte?.Documentos?.FirstOrDefault(d => d.IdTipoDocumento == "8f917d47-b613-47d9-9284-89c5366e2406")?.NumeroDocumento ?? "")).Select(x => x.OperadorTransporte.Documentos.FirstOrDefault(d => d.IdTipoDocumento == "8f917d47-b613-47d9-9284-89c5366e2406").NumeroDocumento).ToList();
                if (cpfsMotoristas.IsNullOrEmpty()) return false;

                List<Dominio.Entidades.Usuario> funcionarios = await CadastrarNovosFuncionarios(equipesOferta, cpfsMotoristas);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristas = await repositorioCargaMotorista.BuscarPorCargaAsync(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista motorista in motoristas)
                    repositorioCargaMotorista.Deletar(motorista);

                foreach (Dominio.Entidades.Usuario motorista in funcionarios)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista = new Dominio.Entidades.Embarcador.Cargas.CargaMotorista
                    {
                        Carga = carga,
                        Motorista = motorista
                    };

                    await repositorioCargaMotorista.InserirAsync(cargaMotorista);
                }
                return true;
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Falha no GerarMotoristas : {excecao.Message}", "HUBOfertas");
                return false;
            }
        }

        public async Task<bool> GerarVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<VeiculoOferta> veiculosOferta, Dominio.Entidades.Empresa empresa, List<string> placasVeiculos)
        {
            try
            {

                if (empresa is null)
                    return false;

                if (veiculosOferta.IsNullOrEmpty())
                    return false;

                List<Dominio.Entidades.Veiculo> veiculos = await CadastrarNovosVeiculos(veiculosOferta, placasVeiculos, empresa, carga.ModeloVeicularCarga);

                string placaVeiculoPrincipal = veiculosOferta.FirstOrDefault(x => x.Sequencia == 0).Veiculo.VeiculoTerrestre.Placa;

                carga.Veiculo = veiculos.FirstOrDefault(x => x.Placa == placaVeiculoPrincipal);

                carga.VeiculosVinculados.Clear();

                foreach (Dominio.Entidades.Veiculo veiculo in veiculos.Where(x => x.Placa != placaVeiculoPrincipal))
                {
                    if (veiculo != null)
                        carga.VeiculosVinculados.Add(veiculo);
                }

                return true;
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Falha no CadastrarNovosVeiculos : {excecao.Message}", "HUBOfertas");
                return false;
            }
        }

        #endregion

        #region Metodos Cadastro

        public async Task<List<Dominio.Entidades.Usuario>> CadastrarNovosFuncionarios(List<EquipeOferta> equipesOferta, List<string> cpfsMotoristas)
        {
            try
            {

                List<Dominio.Entidades.Usuario> funcionariosExistentes = await _repositorioUsuario.BuscarCPFsMotoristasPorCPFsAsync(cpfsMotoristas) ?? new List<Dominio.Entidades.Usuario>();
                List<string> cpfExistentes = funcionariosExistentes.Select(x => x.CPF).ToList();

                List<EquipeOferta> novosFuncionarios = equipesOferta
                    .Where(v => !cpfExistentes.Contains(v.OperadorTransporte?.Documentos?.FirstOrDefault(d => d.IdTipoDocumento == "8f917d47-b613-47d9-9284-89c5366e2406")?.NumeroDocumento))
                    .ToList();


                foreach (EquipeOferta equipe in novosFuncionarios)
                {
                    string cpf = equipe.OperadorTransporte?.Documentos?.FirstOrDefault(d => d.IdTipoDocumento == "8f917d47-b613-47d9-9284-89c5366e2406")?.NumeroDocumento ?? "";
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Endereco endereco = equipe?.OperadorTransporte?.Enderecos?.FirstOrDefault(x => x.Tipo == 1);

                    string cep = endereco?.CEP ?? "";

                    Dominio.Entidades.Localidade localidade = null;

                    if (!string.IsNullOrEmpty(cep))
                        localidade = await _repositorioLocalidade.BuscarPorCEPAsync(cep);

                    if (localidade == null)
                        localidade = await CadastrarLocalidade(endereco);

                    Dominio.Entidades.Usuario funcionario = new Dominio.Entidades.Usuario
                    {
                        Nome = equipe?.OperadorTransporte?.EntidadeIndividual?.NomeCompleto ?? "",
                        Tipo = "M",
                        CPF = cpf,
                        DataNascimento = equipe?.OperadorTransporte?.EntidadeIndividual?.DataNascimento ?? null,
                        Telefone = equipe?.OperadorTransporte?.EntidadeIndividual.TelefoneCelular ?? "",
                        Localidade = localidade,
                        Status = "A"

                    };

                    await _repositorioUsuario.InserirAsync(funcionario);
                    funcionariosExistentes.Add(funcionario);
                }

                return funcionariosExistentes;
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Falha no CadastrarNovosFuncionarios : {excecao.Message}", "HUBOfertas");
                return new List<Dominio.Entidades.Usuario>();
            }
        }

        public async Task<List<Dominio.Entidades.Veiculo>> CadastrarNovosVeiculos(List<VeiculoOferta> veiculosOferta, List<string> placasVeiculos, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga)
        {
            try
            {

                List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();
                List<Dominio.Entidades.Veiculo> veiculosExistentes = await _repositorioVeiculo.BuscarPlacasPorListaPlacaCodigoEmpresa(placasVeiculos, empresa.Codigo);
                List<string> placasExistentes = veiculosExistentes.Select(x => x.Placa).ToList();
                List<VeiculoOferta> novosVeiculos = veiculosOferta
                    .Where(v => !placasExistentes.Contains(v.Veiculo.VeiculoTerrestre.Placa))
                    .ToList();

                veiculos.AddRange(veiculosExistentes);

                foreach (VeiculoOferta veiculo in novosVeiculos)
                {
                    if (string.IsNullOrEmpty(veiculo?.Veiculo?.VeiculoTerrestre?.Cidade?.Estado?.Abreviacao ?? ""))
                        continue;

                    string cpfCNPJ = veiculo?.Veiculo?.Proprietario?.Documentos?.FirstOrDefault(x => x.IdTipoDocumento == "8f917d47-b613-47d9-9284-89c5366e2406")?.NumeroDocumento ?? veiculo?.Veiculo?.Proprietario?.Documentos?.FirstOrDefault(x => x.IdTipoDocumento == _tipoCNPJ)?.NumeroDocumento ?? "";

                    Dominio.Entidades.Veiculo entidadeVeiculo = new Dominio.Entidades.Veiculo
                    {
                        AnoFabricacao = veiculo.Veiculo.Ano,
                        DataAtualizacao = DateTime.Now,
                        DataCadastro = DateTime.Now,
                        Placa = veiculo.Veiculo.VeiculoTerrestre.Placa,
                        Ativo = true,
                        TipoVeiculo = veiculo.Sequencia == 0 ? "0" : "1",
                        Empresa = empresa,
                        Estado = string.IsNullOrEmpty(veiculo?.Veiculo?.VeiculoTerrestre?.Cidade?.Estado?.Abreviacao ?? "") ? null : new Estado { Sigla = veiculo?.Veiculo?.VeiculoTerrestre?.Cidade?.Estado?.Abreviacao ?? "" },
                        Tara = veiculo?.Veiculo?.VeiculoTerrestre?.PesoVazio ?? 0,
                        ModeloVeicularCarga = modeloVeicularCarga,
                        AnoModelo = veiculo.Veiculo.AnoModelo,
                        TipoFrota = TipoFrota.Fixo,
                        Tipo = cpfCNPJ.ObterSomenteNumeros() == empresa.CNPJ.ToString() ? "P" : "T",
                        CapacidadeKG = Convert.ToInt32(veiculo.Veiculo?.CompartimentosCarga?.Where(x => x.UnidadesMedida?.Where(y => y.UnidadeMedida.Id == "61ff0ced-d627-477a-9758-fbb873f8fc1f").Any() ?? false).Sum(x => x.UnidadesMedida.Sum(x => x.Capacidade))),
                        CapacidadeM3 = Convert.ToInt32(veiculo.Veiculo?.CompartimentosCarga?.Where(x => x.UnidadesMedida?.Where(y => y.UnidadeMedida.Id == "c22f586c-037c-4d72-a3f8-802849cef1c8").Any() ?? false).Sum(x => x.UnidadesMedida.Sum(x => x.Capacidade))),
                        CapacidadeTanque = Convert.ToInt32(veiculo.Veiculo?.CompartimentosCarga?.Where(x => x.UnidadesMedida?.Where(y => y.UnidadeMedida.Id == "24f188df-2ed5-4f5b-b839-0dc05fa85b2e").Any() ?? false).Sum(x => x.UnidadesMedida.Sum(x => x.Capacidade))),
                        Renavam = veiculo.Veiculo.VeiculoTerrestre.NumeroIdentificacaoNacional
                    };

                    await _repositorioVeiculo.InserirAsync(entidadeVeiculo);
                    veiculos.Add(entidadeVeiculo);
                }

                return veiculos;
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Falha no CadastrarNovosVeiculos : {excecao.Message}", "HUBOfertas");
                return new List<Dominio.Entidades.Veiculo>();
            }
        }

        public async Task<Localidade> CadastrarLocalidade(Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Endereco endereco)
        {
            try
            {
                if (endereco == null)
                    return null;

                Dominio.Entidades.Localidade localidade = new Dominio.Entidades.Localidade
                {
                    Codigo = _repositorioLocalidade.BuscarPorMaiorCodigo() + 1,
                    DataAtualizacao = DateTime.Now,
                    Descricao = $"{endereco?.Cidade?.NomeCidade ?? ""} - {endereco?.Cidade?.Estado?.NomeEstado ?? ""} - {endereco.CEP}",
                    CEP = endereco.CEP,
                    Estado = string.IsNullOrEmpty(endereco?.Cidade?.Estado?.Abreviacao ?? "") ? null : new Estado { Sigla = endereco.Cidade.Estado.Abreviacao },
                    CodigoCidade = endereco.Cidade.CodigoIBGE
                };

                await _repositorioLocalidade.InserirAsync(localidade);

                return localidade;
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Falha no CadastrarEmpresa : {excecao.Message}", "HUBOfertas");
                return null;
            }
        }


        public async Task<Dominio.Entidades.Empresa> CadastrarEmpresa(Transportadora transportadora)
        {
            if (string.IsNullOrEmpty(transportadora?.Documentos?.FirstOrDefault(x => x.IdTipoDocumento == _tipoCNPJ)?.NumeroDocumento ?? ""))
                return null;

            Dominio.Entidades.Empresa empresa = await _repositorioEmpresa.BuscarPorCNPJAsync(transportadora.Documentos.FirstOrDefault(x => x.IdTipoDocumento == _tipoCNPJ).NumeroDocumento);

            if (empresa != null)
                return empresa;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Endereco endereco = transportadora.Enderecos?.FirstOrDefault(x => x.Tipo == 1) ?? null;

            string cep = endereco?.CEP ?? "";

            if (string.IsNullOrEmpty(cep))
                return null;


            try
            {
                Dominio.Entidades.Localidade localidade = await _repositorioLocalidade.BuscarPorCEPAsync(cep);

                if (localidade == null)
                    localidade = await CadastrarLocalidade(endereco);

                empresa = new Dominio.Entidades.Empresa
                {
                    Localidade = localidade,
                    CNPJ = transportadora.Documentos.FirstOrDefault(x => x.IdTipoDocumento == _tipoCNPJ).NumeroDocumento,
                    InscricaoEstadual = transportadora?.Documentos?.FirstOrDefault(x => x.IdTipoDocumento == "1d34eca9-ebe3-4722-99d0-686665fe2b64")?.NumeroDocumento ?? "",
                    EmitirTodosCTesComoSimples = false,
                    CobrarDocumentosDestinados = false,
                    ValidarMotoristaTeleriscoAoConfirmarTransportador = false,
                    NomeFantasia = transportadora.EntidadeLegal.NomeFantasia,
                    DataCadastro = DateTime.Now,
                    DataAtualizacao = DateTime.Now,
                    RazaoSocial = transportadora.EntidadeLegal.RazaoSocial
                };

                await _repositorioEmpresa.InserirAsync(empresa);

                return empresa;
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Falha no CadastrarEmpresa : {excecao.Message}", "HUBOfertas");
                return null;
            }
        }
        #endregion

    }
}
