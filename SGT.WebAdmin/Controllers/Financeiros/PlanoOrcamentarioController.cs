using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/PlanoOrcamentario", "Financeiros/TituloFinanceiro", "Pessoas/Pessoa", "Financeiros/LancamentoConta")]
    public class PlanoOrcamentarioController : BaseController
    {
		#region Construtores

		public PlanoOrcamentarioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCentroResultado;
                int.TryParse(Request.Params("CentroResultado"), out codigoCentroResultado);

                DateTime dataBase;
                DateTime.TryParse(Request.Params("DataBase"), out dataBase);

                int empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Código", "Codigo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Base", "DataBase", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, true);

                Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario> listaPlanoOrcamentario = repPlanoOrcamentario.Consulta(codigoCentroResultado, dataBase, empresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPlanoOrcamentario.ContaConsulta(codigoCentroResultado, dataBase, empresa));

                var lista = (from p in listaPlanoOrcamentario
                             select new
                             {
                                 p.Codigo,
                                 CentroResultado = p.CentroResultado.Descricao,
                                 Descricao = p.Observacao,
                                 DataBase = p.DataBase.ToString("dd/MM/yyyy"),
                                 Valor = p.Valor.ToString("n2")
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
                unitOfWork.Start();

                int codigoCentroResultado, numeroOcorrencia = 0;
                int.TryParse(Request.Params("CentroResultado"), out codigoCentroResultado);
                int.TryParse(Request.Params("NumeroOcorrencia"), out numeroOcorrencia);

                decimal valor;
                decimal.TryParse(Request.Params("Valor"), out valor);

                bool repetir, dividir;
                bool.TryParse(Request.Params("Repetir"), out repetir);
                bool.TryParse(Request.Params("Dividir"), out dividir);

                DateTime dataBase;
                DateTime.TryParse(Request.Params("DataBase"), out dataBase);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade periodicidade;
                Enum.TryParse(Request.Params("Periodicidade"), out periodicidade);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentario = new Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario();

                planoOrcamentario.DataBase = dataBase;
                planoOrcamentario.Periodicidade = periodicidade;
                planoOrcamentario.Observacao = observacao;
                planoOrcamentario.Valor = valor;
                planoOrcamentario.Repetir = repetir;
                planoOrcamentario.Dividir = dividir;
                planoOrcamentario.NumeroOcorrencia = numeroOcorrencia;

                planoOrcamentario.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);
                planoOrcamentario.Empresa = this.Usuario.Empresa;

                var codigoPlano = ValidarCentroResultadoPlanos(planoOrcamentario, unitOfWork);
                if (codigoPlano == 0)
                    repPlanoOrcamentario.Inserir(planoOrcamentario);
                else
                    return new JsonpResult(false, "Plano orçamentário de código " + codigoPlano + " já está cadastrado com esse Centro de Resultado no mesmo Mês e Ano.");

                List<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta> planoOrcamentarioContas = SalvarListaContas(planoOrcamentario, unitOfWork);
                string retorno = ReplicarPlanoOrcamentario(unitOfWork, planoOrcamentario, planoOrcamentarioContas);
                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);

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
                unitOfWork.Start();

                int codigo, codigoCentroResultado, numeroOcorrencia = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CentroResultado"), out codigoCentroResultado);
                int.TryParse(Request.Params("NumeroOcorrencia"), out numeroOcorrencia);

                decimal valor;
                decimal.TryParse(Request.Params("Valor"), out valor);

                DateTime dataBase;
                DateTime.TryParse(Request.Params("DataBase"), out dataBase);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentario = repPlanoOrcamentario.BuscarPorCodigo(codigo);

                planoOrcamentario.DataBase = dataBase;
                planoOrcamentario.Observacao = observacao;
                planoOrcamentario.Valor = valor;
                planoOrcamentario.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);

                var codigoPlano = ValidarCentroResultadoPlanos(planoOrcamentario, unitOfWork);
                if (codigoPlano == 0)
                    repPlanoOrcamentario.Atualizar(planoOrcamentario);
                else
                    return new JsonpResult(false, "Plano orçamentário de código " + codigoPlano + " já está cadastrado com esse Centro de Resultado no mesmo Mês e Ano.");

                SalvarListaContas(planoOrcamentario, unitOfWork);
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
                Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentario = repPlanoOrcamentario.BuscarPorCodigo(codigo);
                object retorno = null;

                if (planoOrcamentario != null)
                {
                    retorno = new
                    {
                        planoOrcamentario.Codigo,
                        DataBase = planoOrcamentario.DataBase.ToString("dd/MM/yyyy"),
                        CentroResultado = planoOrcamentario.CentroResultado != null ? new { Codigo = planoOrcamentario.CentroResultado.Codigo, Descricao = planoOrcamentario.CentroResultado.Descricao } : null,
                        planoOrcamentario.Observacao,
                        Valor = planoOrcamentario.Valor.ToString("n2"),
                        planoOrcamentario.Repetir,
                        planoOrcamentario.Dividir,
                        planoOrcamentario.Periodicidade,
                        planoOrcamentario.NumeroOcorrencia,
                        ListaConta = planoOrcamentario.Contas != null ? (from obj in planoOrcamentario.Contas
                                                                         select new
                                                                         {
                                                                             Codigo = obj.Codigo,
                                                                             CodigoPlanoConta = obj.PlanoConta != null ? obj.PlanoConta.Codigo : 0,
                                                                             PlanoConta = obj.PlanoConta != null ? new { Codigo = obj.PlanoConta.Codigo, Descricao = obj.PlanoConta.Plano + " - " + obj.PlanoConta.Descricao } : null,
                                                                             Percentual = obj.Percentual.ToString("n2"),
                                                                             Valor = obj.Valor.ToString("n2")
                                                                         }).ToList() : null
                    };

                    return new JsonpResult(retorno);
                }
                else
                    return new JsonpResult(false, "Plano Orçamentário não encontrado.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentario = repPlanoOrcamentario.BuscarPorCodigo(codigo);
                repPlanoOrcamentario.Deletar(planoOrcamentario);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");
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

        [AllowAuthenticate]
        public async Task<IActionResult> ValidaPlanoOrcamentarioEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTipoMovimento = 0, codigoBaixaTitulo = 0, codigoNaturezaOperacao = 0, codigoNaturezaOperacaoNFSe;
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                int.TryParse(Request.Params("BaixaTitulo"), out codigoBaixaTitulo);
                int.TryParse(Request.Params("NaturezaOperacao"), out codigoNaturezaOperacao);
                int.TryParse(Request.Params("NaturezaOperacaoServico"), out codigoNaturezaOperacaoNFSe);

                DateTime dataEmissao;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);

                bool nfce = false;
                bool.TryParse(Request.Params("NFCe"), out nfce);

                if (dataEmissao == DateTime.MinValue)
                    dataEmissao = DateTime.Now;

                String retornoExistePlanoOrcamentario = "";
                retornoExistePlanoOrcamentario = Servicos.Embarcador.Financeiro.PlanoOrcamentario.ObterConfiguracaoPlanoOrcamentario(unitOfWork, this.Usuario?.Empresa, dataEmissao, codigoTipoMovimento, codigoBaixaTitulo, codigoNaturezaOperacao, codigoNaturezaOperacaoNFSe, nfce);

                if (!String.IsNullOrWhiteSpace(retornoExistePlanoOrcamentario))
                {
                    object retorno = new
                    {
                        this.Usuario.Empresa.TipoLancamentoFinanceiroSemOrcamento,
                        Mensagem = retornoExistePlanoOrcamentario
                    };

                    return new JsonpResult(retorno, true, retornoExistePlanoOrcamentario);
                }
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar plano orçamentário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta> SalvarListaContas(Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.PlanoOrcamentarioConta repPlanoOrcamentarioConta = new Repositorio.Embarcador.Financeiro.PlanoOrcamentarioConta(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);
            repPlanoOrcamentarioConta.DeletarPorPlanoOrcamentario(planoOrcamentario.Codigo);

            List<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta> contas = new List<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta>();

            dynamic listaConta = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConta"));
            if (listaConta != null)
            {
                foreach (var contaPlanoConta in listaConta)
                {
                    int codigoPlanoConta;
                    int.TryParse((string)contaPlanoConta.CodigoPlanoConta, out codigoPlanoConta);

                    decimal valor, percentual;
                    valor = Utilidades.Decimal.Converter((string)contaPlanoConta.Valor);
                    percentual = Utilidades.Decimal.Converter((string)contaPlanoConta.Percentual);

                    Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta conta = new Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta
                    {
                        Valor = valor,
                        Percentual = percentual,
                        PlanoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta),
                        PlanoOrcamentario = planoOrcamentario
                    };
                    repPlanoOrcamentarioConta.Inserir(conta);

                    contas.Add(conta);
                }
            }

            return contas;
        }

        private int ValidarCentroResultadoPlanos(Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unidadeDeTrabalho);

            var codigoExistente = repPlanoOrcamentario.ExisteCentroResultadoMesAno(planoOrcamentario.Codigo, planoOrcamentario.CentroResultado.Codigo, planoOrcamentario.DataBase);

            return codigoExistente;
        }

        private string ReplicarPlanoOrcamentario(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentario, List<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta> planoOrcamentarioContas)
        {
            Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoOrcamentarioConta repPlanoOrcamentarioConta = new Repositorio.Embarcador.Financeiro.PlanoOrcamentarioConta(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeDeTrabalho);

            if (planoOrcamentario.Dividir || planoOrcamentario.Repetir)
            {
                if (planoOrcamentario.Periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal)
                    return "Não é permitido replicar semanalmente, favor utilizar outra periodicidade!";

                decimal valorNovoPlano = 0;
                DateTime dataBaseNovoPlano = planoOrcamentario.DataBase;

                if (planoOrcamentario.Repetir)
                    valorNovoPlano = planoOrcamentario.Valor;
                else
                    valorNovoPlano = planoOrcamentario.Valor / planoOrcamentario.NumeroOcorrencia;

                for (int i = 0; i < planoOrcamentario.NumeroOcorrencia; i++)
                {
                    if (planoOrcamentario.Periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal)
                        dataBaseNovoPlano = dataBaseNovoPlano.AddMonths(1);
                    else if (planoOrcamentario.Periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Bimestral)
                        dataBaseNovoPlano = dataBaseNovoPlano.AddMonths(2);
                    else if (planoOrcamentario.Periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Trimestral)
                        dataBaseNovoPlano = dataBaseNovoPlano.AddMonths(3);
                    else if (planoOrcamentario.Periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semestral)
                        dataBaseNovoPlano = dataBaseNovoPlano.AddMonths(6);
                    else if (planoOrcamentario.Periodicidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Anual)
                        dataBaseNovoPlano = dataBaseNovoPlano.AddYears(1);

                    Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario planoOrcamentarioNovo = new Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario();
                    planoOrcamentarioNovo.DataBase = dataBaseNovoPlano;
                    planoOrcamentarioNovo.Periodicidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal;
                    planoOrcamentarioNovo.Observacao = planoOrcamentario.Observacao;
                    planoOrcamentarioNovo.Valor = valorNovoPlano;
                    planoOrcamentarioNovo.Repetir = false;
                    planoOrcamentarioNovo.Dividir = false;
                    planoOrcamentarioNovo.NumeroOcorrencia = 0;

                    planoOrcamentarioNovo.CentroResultado = planoOrcamentario.CentroResultado;
                    planoOrcamentarioNovo.Empresa = planoOrcamentario.Empresa;

                    var codigoPlano = ValidarCentroResultadoPlanos(planoOrcamentarioNovo, unidadeDeTrabalho);
                    if (codigoPlano == 0)
                        repPlanoOrcamentario.Inserir(planoOrcamentarioNovo);
                    else
                        return "Plano orçamentário de código " + codigoPlano + " já está cadastrado com esse Centro de Resultado no mesmo Mês e Ano do novo plano a ser gerado.";

                    foreach (var conta in planoOrcamentarioContas)
                    {
                        decimal valorNovoPlanoConta = 0;
                        if (planoOrcamentario.Repetir)
                            valorNovoPlanoConta = conta.Valor;
                        else
                            valorNovoPlanoConta = valorNovoPlano * (conta.Percentual / 100);

                        Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta contaNovoPlano = new Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentarioConta
                        {
                            Valor = valorNovoPlanoConta,
                            Percentual = conta.Percentual,
                            PlanoConta = conta.PlanoConta,
                            PlanoOrcamentario = planoOrcamentarioNovo
                        };
                        repPlanoOrcamentarioConta.Inserir(contaNovoPlano);
                    }
                }
            }

            return string.Empty;
        }

        #endregion
    }
}
