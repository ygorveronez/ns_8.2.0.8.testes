using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/FichaCliente")]
    public class FichaClienteController : BaseController
    {
		#region Construtores

		public FichaClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nome do Cliente", "NomeCliente", 30, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Saldo Atual", "SaldoAtual", 20, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaFichaCliente filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Financeiro.FichaCliente repFichaCliente = new Repositorio.Embarcador.Financeiro.FichaCliente(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.FichaCliente> listaFichaCliente = repFichaCliente.Consultar(filtrosPesquisa, parametroConsulta);

                var lista = (from o in listaFichaCliente
                             select new
                             {
                                 o.Codigo,
                                 NomeCliente = o.Cliente.Nome,
                                 o.SaldoAtual,
                             }).ToList();

                int totalRegistros = repFichaCliente.ContarConsulta(filtrosPesquisa);
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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
                Repositorio.Embarcador.Financeiro.FichaCliente repFichaCliente = new Repositorio.Embarcador.Financeiro.FichaCliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.FichaClienteLancamento repFichaClienteLancamento = new Repositorio.Embarcador.Financeiro.FichaClienteLancamento(unitOfWork);

                int codigoFichaCliente = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Financeiro.FichaCliente fichaCliente = repFichaCliente.BuscarPorCodigo(codigoFichaCliente);
                List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaFichaClienteLancamentos = repFichaClienteLancamento.BuscarPorFichaCliente(codigoFichaCliente);

                return new JsonpResult(new
                {

                    fichaCliente.Codigo,
                    fichaCliente.SaldoAtual,

                    Lancamentos = (from l in listaFichaClienteLancamentos
                                   select new
                                   {
                                       l.Codigo,
                                       l.Tipo,
                                       Valor = l.Valor.ToString("n2"),
                                       Data = l.Data?.ToString("dd/MM/yyyy")
                                   }).ToList(),
                    Cliente = new
                    {
                        fichaCliente.Cliente.Codigo,
                        fichaCliente.Cliente.Descricao
                    }

                });
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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
                unitOfWork.Start();

                double CpfCnpjCliente = Request.GetDoubleParam("Cliente");

                Repositorio.Embarcador.Financeiro.FichaCliente repFichaCliente = new Repositorio.Embarcador.Financeiro.FichaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.FichaCliente fichaCliente = repFichaCliente.BuscarPorCpfCnpjCliente(CpfCnpjCliente);
                if (fichaCliente != null)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Já existe uma ficha cadastrada para este cliente");
                }
                fichaCliente = new Dominio.Entidades.Embarcador.Financeiro.FichaCliente();
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(CpfCnpjCliente);
                PreencherFichaCliente(fichaCliente, cliente);
                Repositorio.Embarcador.Financeiro.FichaClienteLancamento repFichaClienteLancamento = new Repositorio.Embarcador.Financeiro.FichaClienteLancamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaFichaClienteLancamentosFront = PreencherFichaClienteLancamentos(fichaCliente);

                repFichaCliente.Inserir(fichaCliente, Auditado);

                foreach (var lancamento in listaFichaClienteLancamentosFront)
                {
                    repFichaClienteLancamento.Inserir(lancamento, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                unitOfWork.Start();

                int codigoFichaCliente = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.FichaCliente repFichaCliente = new Repositorio.Embarcador.Financeiro.FichaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.FichaCliente fichaCliente = codigoFichaCliente > 0 ? repFichaCliente.BuscarPorCodigo(codigoFichaCliente) : throw new Exception("Não foi possível encontrar a Ficha do Cliente");
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(fichaCliente.Cliente.CPF_CNPJ);
                PreencherFichaCliente(fichaCliente, cliente);
                Repositorio.Embarcador.Financeiro.FichaClienteLancamento repFichaClienteLancamento = new Repositorio.Embarcador.Financeiro.FichaClienteLancamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaFichaClienteLancamentosBack = repFichaClienteLancamento.BuscarPorFichaCliente(codigoFichaCliente);
                List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaFichaClienteLancamentosFront = PreencherFichaClienteLancamentos(fichaCliente);
                List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaDeletar = ObterListaDeletar(fichaCliente, listaFichaClienteLancamentosBack, listaFichaClienteLancamentosFront);

                AtualizarListaBackLocal(listaFichaClienteLancamentosBack, listaFichaClienteLancamentosFront, listaDeletar);

                foreach (var lancamento in listaDeletar)
                {
                    repFichaClienteLancamento.Deletar(lancamento, Auditado);
                }
                foreach (var lancamento in listaFichaClienteLancamentosBack)
                {
                    if (lancamento.Codigo > 0)
                    {
                        repFichaClienteLancamento.Atualizar(lancamento, Auditado);
                    }

                    else
                    {
                        repFichaClienteLancamento.Inserir(lancamento, Auditado);
                    }
                }
                repFichaCliente.Atualizar(fichaCliente, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados
        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaFichaCliente ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaFichaCliente()
            {
                CpfCnpjCliente = Request.GetDoubleParam("Cliente")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> PreencherFichaClienteLancamentos(Dominio.Entidades.Embarcador.Financeiro.FichaCliente fichaCliente)
        {
            List<dynamic> listaLancamentosFrontJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("ListaLancamentos"));
            return ConverterListaFront(fichaCliente, listaLancamentosFrontJSON);
        }

        private void PreencherFichaCliente(Dominio.Entidades.Embarcador.Financeiro.FichaCliente fichaCliente, Dominio.Entidades.Cliente cliente)
        {
            fichaCliente.Cliente = cliente;
            fichaCliente.Codigo = Request.GetIntParam("Codigo");
            fichaCliente.SaldoAtual = Request.GetDecimalParam("SaldoAtual");
        }

        private List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> ObterListaDeletar(Dominio.Entidades.Embarcador.Financeiro.FichaCliente fichaCliente, List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaLancamentosBack, List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaLancamentosFront)
        {
            List<int> codigosFront = new List<int>();
            foreach (var l in listaLancamentosFront)
            {
                codigosFront.Add(l.Codigo);
            }
            List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaDeletar = listaLancamentosBack.FindAll(x => !codigosFront.Contains(x.Codigo)).ToList();

            return listaDeletar;
        }

        private List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> ConverterListaFront(Dominio.Entidades.Embarcador.Financeiro.FichaCliente fichaCliente, List<dynamic> listaLancamentosFrontJSON)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaLancamentosFront = new List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento>();

            foreach (var lancamentoFrontJSON in listaLancamentosFrontJSON)
            {
                Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento lancamentoFront = new Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento();

                lancamentoFront.Codigo = ((string)lancamentoFrontJSON.Codigo).Any(x => char.IsLetter(x)) ? 0 : (int)lancamentoFrontJSON.Codigo;
                lancamentoFront.Data = Convert.ToDateTime(lancamentoFrontJSON.Data);
                lancamentoFront.Valor = Convert.ToDecimal(lancamentoFrontJSON.Valor.ToString().Replace(".", ""));
                lancamentoFront.FichaCliente = fichaCliente;
                lancamentoFront.Tipo = (string)lancamentoFrontJSON.Tipo == "Entrada" ? TipoEntradaSaida.Entrada : TipoEntradaSaida.Saida;

                listaLancamentosFront.Add(lancamentoFront);
            }

            return listaLancamentosFront;
        }

        private void AtualizarListaBackLocal(List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaFichaClienteLancamentosBack, List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaFichaClienteLancamentosFront, List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> listaDeletar)
        {
            listaFichaClienteLancamentosBack.RemoveAll(x => listaDeletar.Contains(x));

            foreach (var lancamento in listaFichaClienteLancamentosFront)
            {
                if (listaFichaClienteLancamentosBack.Any(x => x.Codigo == lancamento.Codigo && (x.Data != lancamento.Data || x.Tipo != lancamento.Tipo || x.Valor != lancamento.Valor)))
                {
                    lancamento.CopyProperties(listaFichaClienteLancamentosBack.Find(x => x.Codigo == lancamento.Codigo));
                }
                else if (lancamento.Codigo == 0)
                {
                    listaFichaClienteLancamentosBack.Add(lancamento);
                }
            }
        }

        #endregion
    }
}
