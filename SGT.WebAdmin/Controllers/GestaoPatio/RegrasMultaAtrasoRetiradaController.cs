using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    public class RegrasMultaAtrasoRetiradaController : BaseController
    {
		#region Construtores

		public RegrasMultaAtrasoRetiradaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRegrasMultaAtrasoRetirada filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada repRegrasMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada> regrasMultaAtrasoRetirada = repRegrasMultaAtrasoRetirada.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repRegrasMultaAtrasoRetirada.ContarConsulta(filtrosPesquisa));

                var lista = (from p in regrasMultaAtrasoRetirada
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo,
                                 Filial = p.Filial?.Descricao ?? string.Empty,
                                 TipoOcorrencia = p.TipoOcorrencia?.Descricao ?? string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada repRegrasMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada = new Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada();

                unitOfWork.Start();

                PreencherRegrasMultaAtrasoRetirada(regrasMultaAtrasoRetirada, unitOfWork);

                SetarTransportadores(regrasMultaAtrasoRetirada, unitOfWork);
                SetarEstados(regrasMultaAtrasoRetirada, unitOfWork);
                SetarLocalidades(regrasMultaAtrasoRetirada, unitOfWork);
                SetarTiposOperacoes(regrasMultaAtrasoRetirada, unitOfWork);
                SetarClientes(regrasMultaAtrasoRetirada, unitOfWork);
                SalvarPeriodosCarregamentos(regrasMultaAtrasoRetirada, unitOfWork);

                repRegrasMultaAtrasoRetirada.Inserir(regrasMultaAtrasoRetirada, Auditado);

                SalvarCEPs(regrasMultaAtrasoRetirada, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada repRegrasMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada = repRegrasMultaAtrasoRetirada.BuscarPorCodigo(codigo, true);

                if (regrasMultaAtrasoRetirada == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                PreencherRegrasMultaAtrasoRetirada(regrasMultaAtrasoRetirada, unitOfWork);

                SetarTransportadores(regrasMultaAtrasoRetirada, unitOfWork);
                SetarEstados(regrasMultaAtrasoRetirada, unitOfWork);
                SetarLocalidades(regrasMultaAtrasoRetirada, unitOfWork);
                SetarTiposOperacoes(regrasMultaAtrasoRetirada, unitOfWork);
                SalvarCEPs(regrasMultaAtrasoRetirada, unitOfWork);
                SetarClientes(regrasMultaAtrasoRetirada, unitOfWork);
                SalvarPeriodosCarregamentos(regrasMultaAtrasoRetirada, unitOfWork);

                repRegrasMultaAtrasoRetirada.Atualizar(regrasMultaAtrasoRetirada, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada repRegrasMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento repRegrasMultaAtrasoRetiradaPeriodoCarregamento = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada = repRegrasMultaAtrasoRetirada.BuscarPorCodigo(codigo, true);

                if (regrasMultaAtrasoRetirada == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> periodosCarregamento = repRegrasMultaAtrasoRetiradaPeriodoCarregamento.BuscarPorRegrasMultaAtrasoRetirada(regrasMultaAtrasoRetirada.Codigo);

                var dynRegrasMultaAtrasoRetirada = new
                {
                    regrasMultaAtrasoRetirada.Codigo,
                    regrasMultaAtrasoRetirada.Descricao,
                    regrasMultaAtrasoRetirada.Ativo,
                    Filial = new { Codigo = regrasMultaAtrasoRetirada.Filial?.Codigo ?? 0, Descricao = regrasMultaAtrasoRetirada.Filial?.Descricao ?? string.Empty },
                    regrasMultaAtrasoRetirada.PercentualInclusao,
                    Ocorrencia = new { Codigo = regrasMultaAtrasoRetirada.TipoOcorrencia?.Codigo ?? 0, Descricao = regrasMultaAtrasoRetirada.TipoOcorrencia?.Descricao ?? string.Empty },
                    RegrasMultaAtrasoRetiradaTransportadores = ObterTransportadores(regrasMultaAtrasoRetirada),
                    RegrasMultaAtrasoRetiradaEstados = ObterEstados(regrasMultaAtrasoRetirada),
                    RegrasMultaAtrasoRetiradaLocalidades = ObterLocalidades(regrasMultaAtrasoRetirada),
                    RegrasMultaAtrasoRetiradaTiposOperacoes = ObterTiposOperacoes(regrasMultaAtrasoRetirada),
                    CEPs = ObterCEPs(regrasMultaAtrasoRetirada),
                    RegrasMultaAtrasoRetiradaClientes = ObterClientes(regrasMultaAtrasoRetirada),
                    PeriodosCarregamento = ObterPeriodosCarregamento(periodosCarregamento)

                };

                return new JsonpResult(dynRegrasMultaAtrasoRetirada);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada repRegrasMultaAtrasoRetirada = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento repRegrasMultaAtrasoRetiradaPeriodoCarregamento = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada = repRegrasMultaAtrasoRetirada.BuscarPorCodigo(codigo, true);
                List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> periodosCarregamento = repRegrasMultaAtrasoRetiradaPeriodoCarregamento.BuscarPorRegrasMultaAtrasoRetirada(regrasMultaAtrasoRetirada.Codigo);

                foreach (var deletar in periodosCarregamento)
                    repRegrasMultaAtrasoRetiradaPeriodoCarregamento.Deletar(deletar);

                regrasMultaAtrasoRetirada.Transportadores = null;
                regrasMultaAtrasoRetirada.Estados = null;
                regrasMultaAtrasoRetirada.Cidades = null;
                regrasMultaAtrasoRetirada.TipoOperacoes = null;
                regrasMultaAtrasoRetirada.CEPs = null;
                regrasMultaAtrasoRetirada.Clientes = null;

                repRegrasMultaAtrasoRetirada.Deletar(regrasMultaAtrasoRetirada, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherRegrasMultaAtrasoRetirada(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrenciaCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoDeOcorrenciaDeCTe = Request.GetIntParam("Ocorrencia");

            regrasMultaAtrasoRetirada.Descricao = Request.GetStringParam("Descricao");
            regrasMultaAtrasoRetirada.TipoOcorrencia = codigoTipoDeOcorrenciaDeCTe > 0 ? repositorioTipoOcorrenciaCTe.BuscarPorCodigo(codigoTipoDeOcorrenciaDeCTe) : null;
            regrasMultaAtrasoRetirada.Filial = codigoFilial > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilial) : null;
            regrasMultaAtrasoRetirada.PercentualInclusao = Request.GetDecimalParam("PercentualInclusao");
            regrasMultaAtrasoRetirada.Ativo = Request.GetBoolParam("Ativo");
        }

        private dynamic ObterTransportadores(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada)
        {

            return (from obj in regrasMultaAtrasoRetirada.Transportadores
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterEstados(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada)
        {

            return (from obj in regrasMultaAtrasoRetirada.Estados
                    select new
                    {
                        Codigo = obj.Sigla,
                        Descricao = obj.Nome
                    }).ToList();
        }

        private dynamic ObterLocalidades(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada)
        {

            return (from obj in regrasMultaAtrasoRetirada.Cidades
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.DescricaoCidadeEstado
                    }).ToList();
        }

        private dynamic ObterTiposOperacoes(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada)
        {

            return (from obj in regrasMultaAtrasoRetirada.TipoOperacoes
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterCEPs(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada)
        {

            return (from obj in regrasMultaAtrasoRetirada.CEPs
                    select new
                    {
                        Codigo = obj.Codigo.ToString(),
                        CEPInicial = string.Format(@"{0:00\.000\-000}", obj.CEPInicial),
                        CEPFinal = string.Format(@"{0:00\.000\-000}", obj.CEPFinal)
                    }).ToList();
        }

        private dynamic ObterClientes(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada)
        {

            return (from obj in regrasMultaAtrasoRetirada.Clientes
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterPeriodosCarregamento(List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> periodosCarregamento)
        {

            return (from obj in periodosCarregamento
                    select new
                    {
                        obj.Codigo,
                        DiaSemana = obj.Dia,
                        HoraInicio = string.Format("{0:00}:{1:00}", obj.HoraInicio.Hours, obj.HoraInicio.Minutes),
                        HoraTermino = string.Format("{0:00}:{1:00}", obj.HoraTermino.Hours, obj.HoraTermino.Minutes),
                        QuantidadeHorasContrato = obj.QuantidadeHorasContrato,
                        obj.QuantidadeCargas,
                    }).ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRegrasMultaAtrasoRetirada ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRegrasMultaAtrasoRetirada()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTipoOcorrencia = Request.GetIntParam("Ocorrencia")
            };
        }

        private void SetarTransportadores(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            dynamic transportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegrasMultaAtrasoRetiradaTransportadores"));

            regrasMultaAtrasoRetirada.Transportadores = new List<Dominio.Entidades.Empresa>();

            foreach (dynamic transportadorDinamico in transportadores)
            {
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo((int)transportadorDinamico.Codigo);

                regrasMultaAtrasoRetirada.Transportadores.Add(transportador);
            }
        }

        private void SetarEstados(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            dynamic estados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegrasMultaAtrasoRetiradaEstados"));

            regrasMultaAtrasoRetirada.Estados = new List<Dominio.Entidades.Estado>();

            foreach (dynamic estadoDinamico in estados)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)estadoDinamico.Codigo);

                regrasMultaAtrasoRetirada.Estados.Add(estado);
            }
        }

        private void SetarLocalidades(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            dynamic localidades = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegrasMultaAtrasoRetiradaLocalidades"));

            regrasMultaAtrasoRetirada.Cidades = new List<Dominio.Entidades.Localidade>();

            foreach (dynamic localidadeDinamico in localidades)
            {
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo((int)localidadeDinamico.Codigo);

                regrasMultaAtrasoRetirada.Cidades.Add(localidade);
            }
        }

        private void SetarTiposOperacoes(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            dynamic tiposOperacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegrasMultaAtrasoRetiradaTiposOperacoes"));

            regrasMultaAtrasoRetirada.TipoOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            foreach (var tipoOperacaoDinamico in tiposOperacoes)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo((int)tipoOperacaoDinamico.Codigo);

                regrasMultaAtrasoRetirada.TipoOperacoes.Add(tipoOperacao);
            }
        }

        private void SalvarCEPs(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP repRegrasMultaAtrasoRetiradaCEP = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP(unitOfWork);

            dynamic ceps = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCEPs"));

            if (regrasMultaAtrasoRetirada.CEPs != null && regrasMultaAtrasoRetirada.CEPs.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var cepDinamico in ceps)
                {
                    int codigo = 0;

                    if (int.TryParse((string)cepDinamico.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP> cepsDeletar = (from obj in regrasMultaAtrasoRetirada.CEPs where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < cepsDeletar.Count; i++)
                    repRegrasMultaAtrasoRetiradaCEP.Deletar(cepsDeletar[i]);
            }

            foreach (var cepDinamico in ceps)
            {
                int codigo = 0;

                Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP cep = null;

                if (cepDinamico.Codigo != null && int.TryParse((string)cepDinamico.Codigo, out codigo))
                    cep = repRegrasMultaAtrasoRetiradaCEP.BuscarPorCodigo(codigo, false);

                if (cep == null)
                    cep = new Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaCEP();

                string cepInicial = (string)cepDinamico.CEPInicial;
                string cepFinal = (string)cepDinamico.CEPFinal;

                cep.RegrasMultaAtrasoRetirada = regrasMultaAtrasoRetirada;
                cep.CEPInicial = int.Parse(Utilidades.String.OnlyNumbers(cepInicial));
                cep.CEPFinal = int.Parse(Utilidades.String.OnlyNumbers(cepFinal));

                if (cep.Codigo > 0)
                    repRegrasMultaAtrasoRetiradaCEP.Atualizar(cep);
                else
                    repRegrasMultaAtrasoRetiradaCEP.Inserir(cep);
            }
        }

        private void SetarClientes(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic clientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegrasMultaAtrasoRetiradaClientes"));

            regrasMultaAtrasoRetirada.Clientes = new List<Dominio.Entidades.Cliente>();

            foreach (var clienteDinamico in clientes)
            {
                double.TryParse(Utilidades.String.OnlyNumbers((string)clienteDinamico.Codigo), out double cpfCnpjCliente);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                regrasMultaAtrasoRetirada.Clientes.Add(cliente);
            }
        }

        private void SalvarPeriodosCarregamentos(Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada regrasMultaAtrasoRetirada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento repRegrasMultaAtrasoRetiradaPeriodoCarregamento = new Repositorio.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento(unitOfWork);

            dynamic dynPeriodosCarregamentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PeriodosCarregamento"));

            List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> listaRegrasMultaAtrasoRetiradaPeriodoCarregamento = repRegrasMultaAtrasoRetiradaPeriodoCarregamento.BuscarPorRegrasMultaAtrasoRetirada(regrasMultaAtrasoRetirada.Codigo);

            if (listaRegrasMultaAtrasoRetiradaPeriodoCarregamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic periodoCarregamento in dynPeriodosCarregamentos)
                {
                    int codigo = ((string)periodoCarregamento.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> listaDeletar = (from obj in listaRegrasMultaAtrasoRetiradaPeriodoCarregamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (var deletar in listaDeletar)
                {
                    repRegrasMultaAtrasoRetiradaPeriodoCarregamento.Deletar(deletar);
                }
            }

            foreach (dynamic periodoCarregamento in dynPeriodosCarregamentos)
            {
                int codigo = ((string)periodoCarregamento.Codigo).ToInt();

                Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento regrasMultaAtrasoRetiradaPeriodoCarregamento = codigo > 0 ? repRegrasMultaAtrasoRetiradaPeriodoCarregamento.BuscarPorCodigo(codigo, false) : null;

                if (regrasMultaAtrasoRetiradaPeriodoCarregamento == null)
                    regrasMultaAtrasoRetiradaPeriodoCarregamento = new Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento();

                regrasMultaAtrasoRetiradaPeriodoCarregamento.RegrasMultaAtrasoRetirada = regrasMultaAtrasoRetirada;
                regrasMultaAtrasoRetiradaPeriodoCarregamento.Dia = (DiaSemana)periodoCarregamento.DiaSemana;
                regrasMultaAtrasoRetiradaPeriodoCarregamento.HoraInicio = TimeSpan.ParseExact((string)periodoCarregamento.HoraInicio, "g", null, System.Globalization.TimeSpanStyles.None);
                regrasMultaAtrasoRetiradaPeriodoCarregamento.HoraTermino = TimeSpan.ParseExact((string)periodoCarregamento.HoraTermino, "g", null, System.Globalization.TimeSpanStyles.None);
                regrasMultaAtrasoRetiradaPeriodoCarregamento.QuantidadeHorasContrato = (int)periodoCarregamento.QuantidadeHorasContrato;
                regrasMultaAtrasoRetiradaPeriodoCarregamento.QuantidadeCargas = ((string)periodoCarregamento.QuantidadeCargas).ToInt();

                if (regrasMultaAtrasoRetiradaPeriodoCarregamento.Codigo > 0)
                    repRegrasMultaAtrasoRetiradaPeriodoCarregamento.Atualizar(regrasMultaAtrasoRetiradaPeriodoCarregamento);
                else
                    repRegrasMultaAtrasoRetiradaPeriodoCarregamento.Inserir(regrasMultaAtrasoRetiradaPeriodoCarregamento);

            }
        }

        #endregion
    }
}
