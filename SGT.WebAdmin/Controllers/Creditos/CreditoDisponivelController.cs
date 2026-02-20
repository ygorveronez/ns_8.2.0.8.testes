using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Creditos
{
    [CustomAuthorize("Creditos/CreditoDisponivel")]
    public class CreditoDisponivelController : BaseController
    {
		#region Construtores

		public CreditoDisponivelController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Recebedor", "Recebedor", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ValorCredito", "ValorCredito", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Saldo", "Saldo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicioCredito", 14, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFimCredito", 14, Models.Grid.Align.center, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Recebedor")
                    propOrdena += ".Nome";

                int codRecebedor = int.Parse(Request.Params("Recebedor"));
                bool somenteAtivos = bool.Parse(Request.Params("SomenteAtivos"));

                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
                List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel> listaCreditoDisponivel = repCreditoDisponivel.Consultar(this.Usuario.Codigo, codRecebedor, somenteAtivos, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCreditoDisponivel.ContarConsulta(this.Usuario.Codigo, codRecebedor, somenteAtivos));
                var lista = (from p in listaCreditoDisponivel
                             select new
                             {
                                 p.Codigo,
                                 Recebedor = p.Recebedor.Nome,
                                 ValorCredito = p.ValorCredito.ToString("n2"),
                                 Saldo = p.ValorSaldo.ToString("n2"),
                                 DataInicioCredito = p.DataInicioCredito.ToString("dd/MM/yyyy"),
                                 DataFimCredito = p.DataFimCredito.ToString("dd/MM/yyyy"),
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
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);
            Servicos.Embarcador.Hubs.ControleSaldo hubControleSaldo = new Servicos.Embarcador.Hubs.ControleSaldo();

            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);

                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivel = new Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel();
                creditoDisponivel.Recebedor = repUsuario.BuscarPorCodigo(int.Parse(Request.Params("Recebedor")));
                creditoDisponivel.Creditor = this.Usuario;

                List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado>>((string)Request.Params("CreditosUtilizados"));

                creditoDisponivel.ValorCredito = decimal.Parse(Request.Params("ValorCredito"));
                creditoDisponivel.ValorSaldo = creditoDisponivel.ValorCredito;
                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicioCredito"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                if (dataInicio != DateTime.MinValue)
                    creditoDisponivel.DataInicioCredito = dataInicio;

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFimCredito"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                if (dataFim != DateTime.MinValue)
                    creditoDisponivel.DataFimCredito = dataFim;

                if (repHierarquiaSolicitacaoCredito.BuscarPorCreditorRecebedor(creditoDisponivel.Creditor.Codigo, creditoDisponivel.Recebedor.Codigo) == null)
                    throw new ControllerException("Você não ter permissão de liberar crédito para este operador.");


                if (repCreditoDisponivel.BuscarRecebedorPossuiCreditoAtivo(creditoDisponivel.Creditor.Codigo, creditoDisponivel.Recebedor.Codigo, creditoDisponivel.DataInicioCredito, creditoDisponivel.DataFimCredito) != null)
                    throw new ControllerException("já existe um credito ativo para esse operador.");

                repCreditoDisponivel.Inserir(creditoDisponivel);

                if (creditosUtilizados != null)
                {
                    string retorno = serCreditoMovimentacao.UtilizarCreditos(creditosUtilizados, creditoDisponivel, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ControllerException(retorno);
                }

                string nota = string.Format(Localization.Resources.Credito.CreditoDisponivel.CreditorLiberouCreditoValorUtilizarDeAte, creditoDisponivel.Creditor.Nome, creditoDisponivel.ValorCredito.ToString("n2"), creditoDisponivel.DataInicioCredito.ToString("dd/MM/yyyy"), creditoDisponivel.DataFimCredito.ToString("dd/MM/yyyy"));
                serNotificacao.GerarNotificacao(creditoDisponivel.Recebedor, creditoDisponivel.Creditor, creditoDisponivel.Codigo, "Creditos/CreditoDisponivel", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor.green, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, creditoDisponivel, null, $"Disponivilização de credito. De {creditoDisponivel.Creditor.Descricao} para {creditoDisponivel.Recebedor.Descricao}({creditoDisponivel.ValorCredito}).", unitOfWork);

                hubControleSaldo.SolicitarAtualizacaoSaldo(creditoDisponivel.Recebedor, unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);
                Servicos.Embarcador.Hubs.ControleSaldo hubControleSaldo = new Servicos.Embarcador.Hubs.ControleSaldo();

                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);

                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivel = repCreditoDisponivel.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                decimal diferencaValor = decimal.Parse(Request.Params("ValorCredito")) - creditoDisponivel.ValorCredito;
                decimal saldoRestante = (creditoDisponivel.ValorSaldo + creditoDisponivel.ValorComprometido) + diferencaValor;
                creditoDisponivel.Recebedor = repUsuario.BuscarPorCodigo(int.Parse(Request.Params("Recebedor")));

                if (saldoRestante <= 0)
                {
                    decimal valorMinimo = decimal.Parse(Request.Params("ValorCredito")) + (-saldoRestante);
                    throw new ControllerException("O valor de crédito não pode ser menor que " + valorMinimo.ToString("n2") + ", já que este valor já foi utilizado ou está comprometido pelo operador.");
                }

                creditoDisponivel.Creditor = this.Usuario;
                creditoDisponivel.ValorCredito = decimal.Parse(Request.Params("ValorCredito"));

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicioCredito"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                if (dataInicio != DateTime.MinValue)
                    creditoDisponivel.DataInicioCredito = dataInicio;

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFimCredito"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);
                if (dataFim != DateTime.MinValue)
                    creditoDisponivel.DataFimCredito = dataFim;

                if (repHierarquiaSolicitacaoCredito.BuscarPorCreditorRecebedor(creditoDisponivel.Creditor.Codigo, creditoDisponivel.Recebedor.Codigo) == null)
                    throw new ControllerException("Você não ter permissão de liberar crédito para este operador.");

                if (repCreditoDisponivel.BuscarRecebedorPossuiCreditoAtivo(creditoDisponivel.Creditor.Codigo, creditoDisponivel.Recebedor.Codigo, creditoDisponivel.DataInicioCredito, creditoDisponivel.DataFimCredito) != null)
                    throw new ControllerException("Já existe um crédito liberado para esse operador no periodo informado");

                List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado>>((string)Request.Params("CreditosUtilizados"));

                if (creditosUtilizados != null)
                {
                    List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorCreditoDisponivelDestino(creditoDisponivel.Codigo);
                    serCreditoMovimentacao.ExtornarCreditos(creditosUtilizadosDestino, TipoServicoMultisoftware, unitOfWork);

                    string retorno = serCreditoMovimentacao.UtilizarCreditos(creditosUtilizados, creditoDisponivel, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno))
                        throw new ControllerException(retorno);
                }

                creditoDisponivel.ValorSaldo += diferencaValor;
                repCreditoDisponivel.Atualizar(creditoDisponivel, Auditado);

                unitOfWork.CommitChanges();

                string nota = string.Format(Localization.Resources.Credito.CreditoDisponivel.CreditorAlterouCreditoAgoraSaldoDeUtilizarDeAte, creditoDisponivel.Creditor.Nome, creditoDisponivel.ValorSaldo.ToString("n2"), creditoDisponivel.DataInicioCredito.ToString("dd/MM/yyyy"), creditoDisponivel.DataFimCredito.ToString("dd/MM/yyyy"));
                serNotificacao.GerarNotificacao(creditoDisponivel.Recebedor, creditoDisponivel.Creditor, creditoDisponivel.Codigo, "Creditos/CreditoDisponivel", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor.green, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);

                hubControleSaldo.SolicitarAtualizacaoSaldo(creditoDisponivel.Recebedor, unitOfWork);

                return new JsonpResult(true);

            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivel = repCreditoDisponivel.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizados = repCreditoDisponivelUtilizado.BuscarPorCreditoDisponivelDestino(creditoDisponivel.Codigo);

                var dynCreditoDisponivel = new
                {
                    creditoDisponivel.Codigo,
                    Creditor = new { Codigo = creditoDisponivel.Creditor.Codigo, Descricao = creditoDisponivel.Creditor.Nome },
                    Recebedor = new { Codigo = creditoDisponivel.Recebedor.Codigo, Descricao = creditoDisponivel.Recebedor.Nome },
                    creditoDisponivel.ValorCredito,
                    DataFimCredito = creditoDisponivel.DataFimCredito.ToString("dd/MM/yyyy"),
                    DataInicioCredito = creditoDisponivel.DataInicioCredito.ToString("dd/MM/yyyy"),
                    CreditosUtilizados = (from obj in creditosUtilizados
                                          select new
                                          {
                                              obj.Codigo,
                                              ValorUtilizado = obj.ValorUtilizado.ToString("n2"),
                                              Creditor = new { obj.CreditoDisponivelOrigem.Creditor.Codigo, Descricao = obj.CreditoDisponivelOrigem.Creditor.Nome }
                                          }).ToList()
                };
                return new JsonpResult(dynCreditoDisponivel);
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
                Servicos.Embarcador.Hubs.ControleSaldo hubControleSaldo = new Servicos.Embarcador.Hubs.ControleSaldo();

                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);

                unitOfWork.Start();

                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);

                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivel = repCreditoDisponivel.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizados = repCreditoDisponivelUtilizado.BuscarPorCreditoDisponivelOrigem(creditoDisponivel.Codigo);

                if (creditosUtilizados.Count == 0)
                {
                    List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosExtornados = repCreditoDisponivelUtilizado.BuscarTodosPorCreditoDisponivelOrigem(creditoDisponivel.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado creditoUtilizadoExtornados in creditosUtilizadosExtornados)
                        repCreditoDisponivelUtilizado.Deletar(creditoUtilizadoExtornados);


                    List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizadosDestino = repCreditoDisponivelUtilizado.BuscarPorCreditoDisponivelDestinoTodos(creditoDisponivel.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado creditoDisponivelUtilizado in creditosUtilizadosDestino)
                    {
                        Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoUtilizado = repCreditoDisponivel.BuscarPorCodigo(creditoDisponivelUtilizado.CreditoDisponivelOrigem.Codigo);

                        if (creditoDisponivelUtilizado.SituacaoCreditoUtilizado == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Comprometido)
                            creditoUtilizado.ValorSaldo += creditoDisponivelUtilizado.ValorComprometido;

                        if (creditoDisponivelUtilizado.SituacaoCreditoUtilizado == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado.Utilizado)
                            creditoUtilizado.ValorSaldo += creditoDisponivelUtilizado.ValorUtilizado;

                        repCreditoDisponivel.Atualizar(creditoUtilizado);
                        repCreditoDisponivelUtilizado.Deletar(creditoDisponivelUtilizado);
                    }
                    repCreditoDisponivel.Deletar(creditoDisponivel, Auditado);
                    unitOfWork.CommitChanges();


                    string nota = string.Format(Localization.Resources.Credito.CreditoDisponivel.CreditorEstornouCredito, creditoDisponivel.Creditor.Nome, creditoDisponivel.ValorCredito.ToString("n2"));
                    serNotificacao.GerarNotificacao(creditoDisponivel.Recebedor, creditoDisponivel.Creditor, creditoDisponivel.Codigo, "Creditos/CreditoDisponivel", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.estornado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor.red, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    hubControleSaldo.SolicitarAtualizacaoSaldo(creditoDisponivel.Recebedor, unitOfWork);


                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "O credito que você disponibilizou já foi utilizado por isso não é possível remove-lo, se necessário atualize o valor de credito reduzindo até " + (creditoDisponivel.ValorCredito - creditoDisponivel.ValorSaldo).ToString("n2") + " (valor já utilizado).");
                }
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


    }
}
