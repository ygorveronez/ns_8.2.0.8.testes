using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.PedidosVendas
{
    [CustomAuthorize("PedidosVendas/VendaDireta")]
    public class VendaDiretaController : BaseController
    {
        #region Construtores

        public VendaDiretaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
                int.TryParse(Request.Params("Funcionario"), out int funcionario);
                int.TryParse(Request.Params("Agendador"), out int agendador);
                int.TryParse(Request.Params("FuncionarioValidador"), out int funcionarioValidador);

                List<double> CnpjPessoas = Request.GetListParam<double>("Cliente");

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataInicialVencimentoCertificado"), out DateTime vencimentoInicial);
                DateTime.TryParse(Request.Params("DataFinalVencimentoCertificado"), out DateTime vencimentoFinal);

                StatusVendaDireta statusVendaDireta = StatusVendaDireta.Todos;
                Enum.TryParse(Request.Params("Status"), out statusVendaDireta);

                StatusPedidoVendaDireta statusPedidoVendaDireta = StatusPedidoVendaDireta.Todos;
                Enum.TryParse(Request.Params("StatusPedidoVendaDireta"), out statusPedidoVendaDireta);

                ProdutoServico produtoServico = ProdutoServico.Nenhum;
                Enum.TryParse(Request.Params("ProdutoServico"), out produtoServico);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    if (!this.Usuario.UsuarioAdministrador)
                        funcionario = this.Usuario.Codigo;
                }


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", "ProdutoServico", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data Venda", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Funcionário", "Funcionario", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Agendador", "Agendador", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status Pedido", "StatusPedido", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoa", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cliente", "Cliente", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Telefone", "Telefone1", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Venc. Cert", "DataVencimentoCertificado", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("E-mail", "Email", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Pedido", "NumeroPedido", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "Status", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 10, Models.Grid.Align.right, true);

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta> informacoesFolha = repVendaDireta.Consultar(funcionarioValidador, produtoServico, codigoEmpresa, numeroInicial, numeroFinal, dataInicial, dataFinal, funcionario, CnpjPessoas, statusVendaDireta, statusPedidoVendaDireta, agendador, vencimentoInicial, vencimentoFinal, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repVendaDireta.ContarConsulta(funcionarioValidador, produtoServico, codigoEmpresa, numeroInicial, numeroFinal, dataInicial, dataFinal, funcionario, CnpjPessoas, statusVendaDireta, statusPedidoVendaDireta, agendador, vencimentoInicial, vencimentoFinal));

                var lista = (from p in informacoesFolha
                             select new
                             {
                                 p.Codigo,
                                 ProdutoServico = p.ProdutoServico.ObterDescricao(),
                                 p.Numero,
                                 p.Data,
                                 Funcionario = p.Funcionario?.Nome ?? string.Empty,
                                 Agendador = p.Agendador?.Nome ?? string.Empty,
                                 StatusPedido = p.StatusPedido.ObterDescricao(),
                                 GrupoPessoa = p.Cliente?.GrupoPessoas?.Descricao ?? string.Empty,
                                 Cliente = p.Cliente?.Nome ?? string.Empty,
                                 Telefone1 = p.Cliente?.Telefone1 ?? string.Empty,
                                 DataVencimentoCertificado = p.DataVencimentoCertificado.HasValue ? p.DataVencimentoCertificado.Value.ToString("dd/MM/yyyy") : "",
                                 Email = p.Cliente?.Email ?? string.Empty,
                                 p.NumeroPedido,
                                 Status = p.Status.ObterDescricao(),
                                 ValorTotal = p.ValorTotal.ToString("n2")
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PedidosVendas/VendaDireta");

                unitOfWork.Start();

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta = new Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta();
                Servicos.Embarcador.PedidosVendas.VendaDireta svcVendaDireta = new Servicos.Embarcador.PedidosVendas.VendaDireta(unitOfWork);

                string erro = string.Empty;

                PreencherVendaDireta(vendaDireta, unitOfWork);
                repVendaDireta.Inserir(vendaDireta, Auditado);

                SalvarItens(vendaDireta, unitOfWork);
                SalvarParcelas(vendaDireta, unitOfWork);

                if (vendaDireta.Status == StatusVendaDireta.Finalizado)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.VendaDireta_PermitirFinalizar))
                        return new JsonpResult(false, true, "Você não possui permissões para finalizar a venda.");


                    Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    if (!svcVendaDireta.AtualizarTitulos(vendaDireta, unitOfWork, TipoServicoMultisoftware, out erro, tipoAmbiente, this.Usuario, Auditado))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }

                    if (!svcVendaDireta.MovimentarEstoque(vendaDireta, unitOfWork, TipoServicoMultisoftware, out erro))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                svcVendaDireta.GerarNotificacaoVenda(vendaDireta, unitOfWork, TipoServicoMultisoftware, out erro);

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    vendaDireta.Codigo
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PedidosVendas/VendaDireta");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                string erro = string.Empty;

                Servicos.Embarcador.PedidosVendas.VendaDireta svcVendaDireta = new Servicos.Embarcador.PedidosVendas.VendaDireta(unitOfWork);

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta = repVendaDireta.BuscarPorCodigo(codigo, true);
                StatusVendaDireta statusAnterior = vendaDireta.Status;

                PreencherVendaDireta(vendaDireta, unitOfWork);
                repVendaDireta.Atualizar(vendaDireta, Auditado);

                SalvarItens(vendaDireta, unitOfWork);
                SalvarParcelas(vendaDireta, unitOfWork);
                ExcluirAnexos(unitOfWork);

                if (vendaDireta.Status == StatusVendaDireta.Finalizado && statusAnterior != StatusVendaDireta.Finalizado)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.VendaDireta_PermitirFinalizar))
                        return new JsonpResult(false, true, "Você não possui permissões para finalizar a venda.");

                    Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    if (!svcVendaDireta.AtualizarTitulos(vendaDireta, unitOfWork, TipoServicoMultisoftware, out erro, tipoAmbiente, this.Usuario, Auditado))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }

                    if (!svcVendaDireta.MovimentarEstoque(vendaDireta, unitOfWork, TipoServicoMultisoftware, out erro))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                //svcVendaDireta.GerarNotificacaoVenda(vendaDireta, unitOfWork, TipoServicoMultisoftware, out erro);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta = repVendaDireta.BuscarPorCodigo(codigo, false);

                var dynVendaDireta = new
                {
                    vendaDireta.Codigo,
                    vendaDireta.Numero,
                    Data = vendaDireta.Data.ToString("dd/MM/yyyy"),
                    Funcionario = vendaDireta.Funcionario?.Nome ?? string.Empty,
                    Cliente = vendaDireta.Cliente != null ? new { vendaDireta.Cliente.Codigo, vendaDireta.Cliente.Descricao } : null,
                    Agendador = vendaDireta.Agendador != null ? new { vendaDireta.Agendador.Codigo, vendaDireta.Agendador.Descricao } : null,
                    vendaDireta.Status,
                    vendaDireta.Observacao,
                    ValorTotal = vendaDireta.ValorTotal.ToString("n2"),
                    vendaDireta.TipoCobranca,
                    BoletoConfiguracao = vendaDireta.BoletoConfiguracao != null ? new { vendaDireta.BoletoConfiguracao.Codigo, Descricao = vendaDireta.BoletoConfiguracao.DescricaoBanco } : null,
                    vendaDireta.NumeroPedido,
                    vendaDireta.CodigoEmissao1,
                    vendaDireta.CodigoEmissao2,
                    vendaDireta.StatusPedido,
                    vendaDireta.TipoAssinatura,
                    DataAgendamento = vendaDireta.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataVencimentoCertificado = vendaDireta.DataVencimentoCertificado?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataVencimentoCobranca = vendaDireta.DataVencimentoCobranca?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataValidacao = vendaDireta.DataValidacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                    vendaDireta.GerarNF,
                    vendaDireta.TipoCobrancaVendaDireta,

                    Tipo = vendaDireta.ProdutoServico,
                    TipoCobrancaServico = vendaDireta.TipoCobrancaVendaDireta,
                    DataTreinamento = vendaDireta.DataTreinamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    FuncionarioTreinamento = vendaDireta.FuncionarioTreinamento != null ? new { vendaDireta.FuncionarioTreinamento.Codigo, vendaDireta.FuncionarioTreinamento.Descricao } : null,
                    FuncionarioValidador = vendaDireta.FuncionarioValidador != null ? new { vendaDireta.FuncionarioValidador.Codigo, vendaDireta.FuncionarioValidador.Descricao } : null,
                    vendaDireta.StatusCadastro,
                    TipoCliente = vendaDireta.TipoClienteVendaDireta,
                    vendaDireta.EmitidoDocumentos,
                    vendaDireta.Pendencia,
                    vendaDireta.Certificado,

                    Itens = (from obj in vendaDireta.Itens
                             select new
                             {
                                 CodigoItem = obj.Codigo,
                                 CodigoProduto = obj.Produto?.Codigo ?? 0,
                                 CodigoServico = obj.Servico?.Codigo ?? 0,
                                 DescricaoItem = obj.Produto?.Descricao ?? obj.Servico?.Descricao ?? string.Empty,
                                 GrupoProdutoItem = obj.Produto?.GrupoProdutoTMS?.Descricao ?? string.Empty,
                                 Quantidade = obj.Quantidade.ToString("n2"),
                                 ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                 ValorDesconto = obj.ValorDesconto.ToString("n2"),
                                 ValorTotalItem = obj.ValorTotal.ToString("n2"),
                                 CodigoTabelaPreco = obj.TabelaPrecoVenda?.Codigo ?? 0
                             }).ToList(),
                    Parcelas = (from obj in vendaDireta.Parcelas
                                select new
                                {
                                    obj.Codigo,
                                    obj.Sequencia,
                                    Parcela = obj.Sequencia,
                                    Valor = obj.Valor.ToString("n2"),
                                    Desconto = obj.Desconto.ToString("n2"),
                                    DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                    FormaTitulo = obj.Forma
                                }).ToList(),
                    ListaAnexos = (from obj in vendaDireta.Anexos
                                   select new
                                   {
                                       obj.Codigo,
                                       DescricaoAnexo = obj.Descricao,
                                       Arquivo = obj.NomeArquivo
                                   }).ToList(),
                    ListaAnexosContestacao = (from obj in vendaDireta.AnexosContestacao
                                              select new
                                              {
                                                  obj.Codigo,
                                                  DescricaoAnexo = obj.Descricao,
                                                  Arquivo = obj.NomeArquivo
                                              }).ToList()
                };

                return new JsonpResult(dynVendaDireta);
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

        public async Task<IActionResult> Estornar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PedidosVendas/VendaDireta");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unidadeDeTrabalho);
                Servicos.Embarcador.PedidosVendas.VendaDireta svcVendaDireta = new Servicos.Embarcador.PedidosVendas.VendaDireta(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta = repVendaDireta.BuscarPorCodigo(codigo, false);

                if (vendaDireta.Status != StatusVendaDireta.Finalizado)
                    return new JsonpResult(false, true, "Não é possível cancelar na situação atual da venda.");

                if (permissoesPersonalizadas != null && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.VendaDireta_PermiteCancelamento))
                    return new JsonpResult(false, true, "Não possui permissão para realizar o cancelamento.");

                unidadeDeTrabalho.Start();

                vendaDireta.Status = StatusVendaDireta.Pendente;
                repVendaDireta.Atualizar(vendaDireta);

                string erro = string.Empty;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                if (!svcVendaDireta.AtualizarTitulos(vendaDireta, unidadeDeTrabalho, TipoServicoMultisoftware, out erro, tipoAmbiente, this.Usuario, Auditado))
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                if (!svcVendaDireta.MovimentarEstoque(vendaDireta, unidadeDeTrabalho, TipoServicoMultisoftware, out erro))
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, vendaDireta, null, "Cancelou a Venda.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoVenda);

                Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela repVendaDiretaParcela = new Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repVendaDiretaParcela.BuscarBoletosPorVendaDireta(codigoVenda);

                var dynRetorno = new
                {
                    ListaTitulos = (from p in listaTitulos
                                    select new
                                    {
                                        p.Codigo,
                                        p.BoletoStatusTitulo,
                                        Pessoa = p.Pessoa.Nome,
                                        DescricaoStatusBoleto = p.BoletoStatusTitulo.ObterDescricao(),
                                        DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                                        DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                        Valor = p.ValorOriginal.ToString("n2"),
                                        p.NossoNumero,
                                        p.CaminhoBoleto
                                    }).ToList()
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailBoletos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela repVendaDiretaParcela = new Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela(unitOfWork);
                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(empresa.Codigo);

                if (email == null)
                    return new JsonpResult(false, "Não há um e-mail configurado para realizar o envio.");

                int.TryParse(Request.Params("Codigo"), out int codigoVenda);
                string mensagemDigitada = Request.Params("MensagemEmail");
                List<string> emails = new List<string>();
                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();

                Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta = repVendaDireta.BuscarPorCodigo(codigoVenda, false);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repVendaDiretaParcela.BuscarBoletosPorVendaDireta(codigoVenda);
                if (listaTitulos.Count == 0)
                    return new JsonpResult(false, "Nenhum título encontrado.");

                string assunto = "Boleto " + empresa.NomeFantasia;
                string mensagemEmail = "Olá,<br/><br/>Segue em anexo o(s) boleto(s) da empresa " + empresa.NomeFantasia + ".<br/><br/>";
                if (!string.IsNullOrWhiteSpace(mensagemDigitada))
                    mensagemEmail += mensagemDigitada + "<br/><br/>";
                mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
                string mensagemErro = "Erro ao enviar e-mail";

                //E-mails
                if (!string.IsNullOrWhiteSpace(vendaDireta.Cliente.Email))
                    emails.AddRange(vendaDireta.Cliente.Email.Split(';').ToList());

                for (int a = 0; a < vendaDireta.Cliente.Emails.Count; a++)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = vendaDireta.Cliente.Emails[a];
                    if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                        emails.Add(outroEmail.Email);
                }

                if (!string.IsNullOrWhiteSpace(empresa.Email) && empresa.StatusEmail == "A")
                    emails.AddRange(empresa.Email.Split(';').ToList());

                if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) && empresa.StatusEmailAdministrativo == "A")
                    emails.AddRange(empresa.EmailAdministrativo.Split(';').ToList());

                foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in listaTitulos)
                {
                    if (!string.IsNullOrWhiteSpace(titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                        attachments.Add(new System.Net.Mail.Attachment(titulo.CaminhoBoleto));
                }

                if (attachments.Count == 0)
                    return new JsonpResult(false, "Nenhum boleto gerado para envio.");

                Dominio.Entidades.Embarcador.Financeiro.Titulo tituloPrimeiro = listaTitulos?.FirstOrDefault();
                string urlBase = _conexao.ObterHost;

                if (tituloPrimeiro != null)
                {
                    string portalClienteCodigo = Servicos.Embarcador.Financeiro.Titulo.ObterPortalClienteCodigo(tituloPrimeiro, repTitulo);
                    string portalClienteUrl = Servicos.Embarcador.Financeiro.Titulo.ObterURLPortalClienteCodigo(urlBase, portalClienteCodigo);
                    mensagemEmail += "<br/><br/>Link de acesso aos dados da compra: " + portalClienteUrl;
                }

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                        attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, empresa.Codigo);
                    if (!sucesso)
                        return new JsonpResult(false, "Problemas ao enviar o(s) boleto(s) por e-mail: " + mensagemErro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha com o envio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVendaDireta = 0;
                int.TryParse(Request.Params("CodigoVendaDireta"), out codigoVendaDireta);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                unitOfWork.Start();

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.VendaDiretaAnexo repVendaDiretaAnexo = new Repositorio.Embarcador.PedidoVenda.VendaDiretaAnexo(unitOfWork);

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "VendaDireta");
                
                for (var i = 0; i < arquivos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaAnexo vendaDiretaAnexo = new Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaAnexo();

                    Servicos.DTO.CustomFile file = arquivos[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    vendaDiretaAnexo.CaminhoArquivo = caminho;
                    vendaDiretaAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Path.GetFileName(nomeArquivo)));
                    vendaDiretaAnexo.Descricao = descricoes[i];
                    vendaDiretaAnexo.VendaDireta = repVendaDireta.BuscarPorCodigo(codigoVendaDireta, false);

                    repVendaDiretaAnexo.Inserir(vendaDiretaAnexo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, vendaDiretaAnexo.VendaDireta, null, "Adicionou o anexo " + vendaDiretaAnexo.NomeArquivo + ".", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = 0;
                int.TryParse(Request.Params("CodigoAnexo"), out codigoAnexo);

                Repositorio.Embarcador.PedidoVenda.VendaDiretaAnexo repVendaDiretaAnexo = new Repositorio.Embarcador.PedidoVenda.VendaDiretaAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaAnexo vendaDiretaAnexo = repVendaDiretaAnexo.BuscarPorCodigo(codigoAnexo, false);

                if (vendaDiretaAnexo == null)
                    return new JsonpResult(false, "Anexo não encontrado no Banco de Dados.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(vendaDiretaAnexo.CaminhoArquivo))
                    return new JsonpResult(false, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(vendaDiretaAnexo.CaminhoArquivo), "image/jpeg", vendaDiretaAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexoContestacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = 0;
                int.TryParse(Request.Params("CodigoAnexo"), out codigoAnexo);

                Repositorio.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo repVendaDiretaAnexo = new Repositorio.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo vendaDiretaAnexo = repVendaDiretaAnexo.BuscarPorCodigo(codigoAnexo, false);

                if (vendaDiretaAnexo == null)
                    return new JsonpResult(false, "Anexo não encontrado no Banco de Dados.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(vendaDiretaAnexo.CaminhoArquivo))
                    return new JsonpResult(false, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(vendaDiretaAnexo.CaminhoArquivo), "image/jpeg", vendaDiretaAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAnexosContestacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVendaDireta = 0;
                int.TryParse(Request.Params("CodigoVendaDireta"), out codigoVendaDireta);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                unitOfWork.Start();

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo repVendaDiretaAnexo = new Repositorio.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo(unitOfWork);

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "VendaDiretaContestacao");
                
                for (var i = 0; i < arquivos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo vendaDiretaAnexo = new Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo();

                    Servicos.DTO.CustomFile file = arquivos[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    vendaDiretaAnexo.CaminhoArquivo = caminho;
                    vendaDiretaAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Path.GetFileName(nomeArquivo)));
                    vendaDiretaAnexo.Descricao = descricoes[i];
                    vendaDiretaAnexo.VendaDireta = repVendaDireta.BuscarPorCodigo(codigoVendaDireta, false);

                    repVendaDiretaAnexo.Inserir(vendaDiretaAnexo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, vendaDiretaAnexo.VendaDireta, null, "Adicionou o anexo " + vendaDiretaAnexo.NomeArquivo + ".", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarContestacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta = repVendaDireta.BuscarPorCodigo(codigo, false);

                var dynVendaDireta = new
                {
                    vendaDireta.Codigo,
                    CodigoFuncionarioContestacao = vendaDireta.FuncionarioContestacao?.Codigo ?? this.Usuario.Codigo,
                    FuncionarioContestacao = vendaDireta.FuncionarioContestacao?.Descricao ?? this.Usuario.Descricao,
                    DataContestacao = vendaDireta.DataContestacao.HasValue ? vendaDireta.DataContestacao.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"),
                    vendaDireta.ObservacaoContestacao,
                    ListaAnexosContestacao = (from obj in vendaDireta.AnexosContestacao
                                              select new
                                              {
                                                  obj.Codigo,
                                                  DescricaoAnexo = obj.Descricao,
                                                  Arquivo = obj.NomeArquivo
                                              }).ToList()
                };

                return new JsonpResult(dynVendaDireta);
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

        public async Task<IActionResult> SalvarContestacao()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PedidosVendas/VendaDireta");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                int codigoFuncionarioContestacao = Request.GetIntParam("FuncionarioContestacao");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);

                Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta = repVendaDireta.BuscarPorCodigo(codigo, true);

                vendaDireta.FuncionarioContestacao = repUsuario.BuscarPorCodigo(codigoFuncionarioContestacao);
                vendaDireta.DataContestacao = Request.GetNullableDateTimeParam("DataContestacao");
                vendaDireta.ObservacaoContestacao = Request.GetStringParam("ObservacaoContestacao");

                repVendaDireta.Atualizar(vendaDireta, Auditado);

                ExcluirAnexosContestacao(unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a contestação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private void PreencherVendaDireta(Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta, Repositorio.UnitOfWork unitOfWork)
        {
            StatusPedidoVendaDireta statusPedidoAnterior = vendaDireta.Codigo > 0 ? vendaDireta.StatusPedido : StatusPedidoVendaDireta.Todos;

            Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int.TryParse(Request.Params("BoletoConfiguracao"), out int boletoConfiguracao);
            int.TryParse(Request.Params("Agendador"), out int agendador);
            int.TryParse(Request.Params("FuncionarioTreinamento"), out int funcionarioTreinamento);
            int.TryParse(Request.Params("FuncionarioValidador"), out int funcionarioValidador);
            bool.TryParse(Request.Params("GerarNF"), out bool gerarNF);
            bool.TryParse(Request.Params("Certificado"), out bool certificado);
            double.TryParse(Request.Params("Cliente"), out double cliente);

            DateTime.TryParse(Request.Params("DataAgendamento"), out DateTime dataAgendamento);
            DateTime.TryParse(Request.Params("DataVencimentoCertificado"), out DateTime dataVencimentoCertificado);
            DateTime.TryParse(Request.Params("DataVencimentoCobranca"), out DateTime dataVencimentoCobranca);

            string observacao = Request.Params("Observacao");
            string numeroPedido = Request.Params("NumeroPedido");
            string codigoEmissao1 = Request.Params("CodigoEmissao1");
            string codigoEmissao2 = Request.Params("CodigoEmissao2");

            StatusVendaDireta statusVendaDireta;
            TipoCobranca tipoCobranca;
            StatusPedidoVendaDireta statusPedido;
            TipoAssinaturaVendaDireta tipoAssinatura;
            ProdutoServico produtoServico;
            StatusCadastro statusCadastro;
            TipoClienteVendaDireta tipoClienteVendaDireta;
            SimNao emitidoDocumentos;
            SimNao pendencia;
            Enum.TryParse(Request.Params("Status"), out statusVendaDireta);
            Enum.TryParse(Request.Params("TipoCobranca"), out tipoCobranca);
            Enum.TryParse(Request.Params("StatusPedido"), out statusPedido);
            Enum.TryParse(Request.Params("TipoAssinatura"), out tipoAssinatura);

            Enum.TryParse(Request.Params("Tipo"), out produtoServico);
            Enum.TryParse(Request.Params("StatusCadastro"), out statusCadastro);
            Enum.TryParse(Request.Params("TipoCliente"), out tipoClienteVendaDireta);
            Enum.TryParse(Request.Params("EmitidoDocumentos"), out emitidoDocumentos);
            Enum.TryParse(Request.Params("Pendencia"), out pendencia);

            if (statusPedidoAnterior == StatusPedidoVendaDireta.Todos)
            {
                if (statusPedido == StatusPedidoVendaDireta.AgendadoFora)
                    vendaDireta.DataAgendadoFora = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Baixado)
                    vendaDireta.DataBaixado = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Aprovado)
                    vendaDireta.DataAprovado = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.FaltaAgendar)
                    vendaDireta.DataFaltaAgendar = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Agendado)
                    vendaDireta.DataAgendado = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Contato1)
                    vendaDireta.DataContato1 = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Contato2)
                    vendaDireta.DataContato2 = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Contato3)
                    vendaDireta.DataContato3 = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Problema)
                    vendaDireta.DataProblema = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Reagendar)
                    vendaDireta.DataReagendar = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.ClienteBaixa)
                    vendaDireta.DataClienteBaixa = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.AguardandoVerificacaoCertisign)
                    vendaDireta.DataAguardandoVerificacao = DateTime.Now;
            }
            else
            {
                if (statusPedido == StatusPedidoVendaDireta.AgendadoFora && statusPedidoAnterior != StatusPedidoVendaDireta.AgendadoFora)
                    vendaDireta.DataAgendadoFora = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Baixado && statusPedidoAnterior != StatusPedidoVendaDireta.Baixado)
                    vendaDireta.DataBaixado = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Aprovado && statusPedidoAnterior != StatusPedidoVendaDireta.Aprovado)
                    vendaDireta.DataAprovado = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.FaltaAgendar && statusPedidoAnterior != StatusPedidoVendaDireta.FaltaAgendar)
                    vendaDireta.DataFaltaAgendar = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Agendado && statusPedidoAnterior != StatusPedidoVendaDireta.Agendado)
                    vendaDireta.DataAgendado = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Contato1 && statusPedidoAnterior != StatusPedidoVendaDireta.Contato1)
                    vendaDireta.DataContato1 = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Contato2 && statusPedidoAnterior != StatusPedidoVendaDireta.Contato2)
                    vendaDireta.DataContato2 = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Contato3 && statusPedidoAnterior != StatusPedidoVendaDireta.Contato3)
                    vendaDireta.DataContato3 = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Problema && statusPedidoAnterior != StatusPedidoVendaDireta.Problema)
                    vendaDireta.DataProblema = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.Reagendar && statusPedidoAnterior != StatusPedidoVendaDireta.Reagendar)
                    vendaDireta.DataReagendar = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.ClienteBaixa && statusPedidoAnterior != StatusPedidoVendaDireta.ClienteBaixa)
                    vendaDireta.DataClienteBaixa = DateTime.Now;
                if (statusPedido == StatusPedidoVendaDireta.AguardandoVerificacaoCertisign && statusPedidoAnterior != StatusPedidoVendaDireta.AguardandoVerificacaoCertisign)
                    vendaDireta.DataAguardandoVerificacao = DateTime.Now;
            }

            if (produtoServico == ProdutoServico.Servico)
            {
                vendaDireta.ProdutoServico = ProdutoServico.Servico;
                vendaDireta.TipoCobrancaVendaDireta = Request.GetEnumParam<TipoCobrancaVendaDireta>("TipoCobrancaServico");
                vendaDireta.DataTreinamento = Request.GetNullableDateTimeParam("DataTreinamento");
                vendaDireta.FuncionarioTreinamento = funcionarioTreinamento > 0 ? repUsuario.BuscarPorCodigo(funcionarioTreinamento) : null;
                vendaDireta.StatusCadastro = statusCadastro;
                vendaDireta.TipoClienteVendaDireta = tipoClienteVendaDireta;
                vendaDireta.EmitidoDocumentos = emitidoDocumentos;
                vendaDireta.Pendencia = pendencia;
                vendaDireta.Certificado = certificado;
            }
            else
            {
                vendaDireta.ProdutoServico = ProdutoServico.Produto;
                vendaDireta.TipoCobrancaVendaDireta = Request.GetEnumParam<TipoCobrancaVendaDireta>("TipoCobrancaVendaDireta");
                vendaDireta.FuncionarioValidador = funcionarioValidador > 0 ? repUsuario.BuscarPorCodigo(funcionarioValidador) : null;
            }

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            if (vendaDireta.Codigo == 0)
            {
                vendaDireta.Data = DateTime.Now;
                vendaDireta.Numero = repVendaDireta.ProximoNumeroVendaDireta(codigoEmpresa);
                vendaDireta.Funcionario = this.Usuario;
                vendaDireta.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            }

            vendaDireta.Status = statusVendaDireta;
            if (statusVendaDireta == StatusVendaDireta.Cancelado)
                vendaDireta.DataCancelamento = DateTime.Now;
            else if (statusVendaDireta == StatusVendaDireta.Finalizado)
                vendaDireta.DataFinalizacao = DateTime.Now;

            vendaDireta.TipoCobranca = tipoCobranca;
            vendaDireta.Observacao = observacao;
            vendaDireta.Cliente = cliente > 0 ? repCliente.BuscarPorCPFCNPJ(cliente) : null;
            vendaDireta.Agendador = agendador > 0 ? repUsuario.BuscarPorCodigo(agendador) : null;
            vendaDireta.BoletoConfiguracao = boletoConfiguracao > 0 ? repBoletoConfiguracao.BuscarPorCodigo(boletoConfiguracao) : null;

            vendaDireta.NumeroPedido = numeroPedido;
            vendaDireta.CodigoEmissao1 = codigoEmissao1;
            vendaDireta.CodigoEmissao2 = codigoEmissao2;
            vendaDireta.StatusPedido = statusPedido;
            vendaDireta.TipoAssinatura = tipoAssinatura;
            vendaDireta.GerarNF = gerarNF;

            if (dataAgendamento > DateTime.MinValue)
            {
                if (vendaDireta.DataAgendamento == null)
                {
                    vendaDireta.DataAgendamento = dataAgendamento;
                    InserirAgendaTarefa(vendaDireta, unitOfWork);
                }
                else
                    vendaDireta.DataAgendamento = dataAgendamento;
            }
            else
                vendaDireta.DataAgendamento = null;
            if (dataVencimentoCertificado > DateTime.MinValue)
                vendaDireta.DataVencimentoCertificado = dataVencimentoCertificado;
            else
                vendaDireta.DataVencimentoCertificado = null;
            if (dataVencimentoCobranca > DateTime.MinValue)
                vendaDireta.DataVencimentoCobranca = dataVencimentoCobranca;
            else
                vendaDireta.DataVencimentoCobranca = null;

            vendaDireta.DataValidacao = Request.GetNullableDateTimeParam("DataValidacao");
        }

        private void SalvarItens(Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.PedidoVenda.VendaDiretaItem repVendaDiretaItem = new Repositorio.Embarcador.PedidoVenda.VendaDiretaItem(unidadeTrabalho);
            Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda repTabelaPrecoVenda = new Repositorio.Embarcador.PedidoVenda.TabelaPrecoVenda(unidadeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unidadeTrabalho);

            dynamic itens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Itens"));
            if (vendaDireta.Itens != null && vendaDireta.Itens.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var item in itens)
                    if (item.CodigoItem != null)
                        codigos.Add((int)item.CodigoItem);

                List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem> vendaDiretaItemDeletar = (from obj in vendaDireta.Itens where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < vendaDiretaItemDeletar.Count; i++)
                    repVendaDiretaItem.Deletar(vendaDiretaItemDeletar[i]);
            }
            else
                vendaDireta.Itens = new List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem>();

            foreach (var item in itens)
            {
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem vendaDiretaItem = item.CodigoItem != null ? repVendaDiretaItem.BuscarPorCodigo((int)item.CodigoItem, true) : null;
                if (vendaDiretaItem == null)
                    vendaDiretaItem = new Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem();

                int.TryParse((string)item.CodigoProduto, out int codigoProduto);
                int.TryParse((string)item.CodigoServico, out int codigoServico);
                int.TryParse((string)item.CodigoTabelaPreco, out int codigoTabelaPreco);

                vendaDiretaItem.Quantidade = Utilidades.Decimal.Converter((string)item.Quantidade);
                vendaDiretaItem.ValorUnitario = Utilidades.Decimal.Converter((string)item.ValorUnitario);
                vendaDiretaItem.ValorDesconto = Utilidades.Decimal.Converter((string)item.ValorDesconto);
                vendaDiretaItem.ValorTotal = Utilidades.Decimal.Converter((string)item.ValorTotalItem);

                vendaDiretaItem.Produto = codigoProduto > 0 ? repProduto.BuscarPorCodigo(codigoProduto) : null;
                vendaDiretaItem.Servico = codigoServico > 0 ? repServico.BuscarPorCodigo(codigoServico) : null;
                vendaDiretaItem.TabelaPrecoVenda = codigoTabelaPreco > 0 ? repTabelaPrecoVenda.BuscarPorCodigo(codigoTabelaPreco, false) : null;
                vendaDiretaItem.VendaDireta = vendaDireta;

                if (vendaDiretaItem.Codigo > 0)
                    repVendaDiretaItem.Atualizar(vendaDiretaItem, Auditado);
                else
                    repVendaDiretaItem.Inserir(vendaDiretaItem, Auditado);
            }
        }

        private void SalvarParcelas(Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela repVendaDiretaParcela = new Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela(unidadeTrabalho);

            dynamic parcelas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parcelas"));
            if (vendaDireta.Parcelas != null && vendaDireta.Parcelas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var parcela in parcelas)
                    if (parcela.Codigo != null)
                        codigos.Add((int)parcela.Codigo);

                List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela> vendaDiretaParcelaDeletar = (from obj in vendaDireta.Parcelas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < vendaDiretaParcelaDeletar.Count; i++)
                    repVendaDiretaParcela.Deletar(vendaDiretaParcelaDeletar[i]);
            }
            else
                vendaDireta.Parcelas = new List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela>();

            foreach (var parcela in parcelas)
            {
                Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela vendaDiretaParcela = parcela.Codigo != null ? repVendaDiretaParcela.BuscarPorCodigo((int)parcela.Codigo, true) : null;
                if (vendaDiretaParcela == null)
                    vendaDiretaParcela = new Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela();

                DateTime.TryParseExact((string)parcela.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimento);

                FormaTitulo formaTitulo;
                Enum.TryParse((string)parcela.FormaTitulo, out formaTitulo);

                vendaDiretaParcela.DataVencimento = dataVencimento;
                vendaDiretaParcela.Forma = formaTitulo;
                vendaDiretaParcela.Sequencia = (int)parcela.Sequencia;
                vendaDiretaParcela.Valor = Utilidades.Decimal.Converter((string)parcela.Valor);
                vendaDiretaParcela.Desconto = Utilidades.Decimal.Converter((string)parcela.Desconto);
                vendaDiretaParcela.VendaDireta = vendaDireta;

                if (vendaDiretaParcela.Codigo > 0)
                    repVendaDiretaParcela.Atualizar(vendaDiretaParcela, Auditado);
                else
                    repVendaDiretaParcela.Inserir(vendaDiretaParcela, Auditado);
            }
        }

        private void ExcluirAnexos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.VendaDiretaAnexo repVendaDiretaAnexo = new Repositorio.Embarcador.PedidoVenda.VendaDiretaAnexo(unitOfWork);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaAnexo vendaDiretaAnexo = repVendaDiretaAnexo.BuscarPorCodigo((int)anexo.Codigo, true);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(vendaDiretaAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(vendaDiretaAnexo.CaminhoArquivo);

                    repVendaDiretaAnexo.Deletar(vendaDiretaAnexo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, vendaDiretaAnexo.VendaDireta, null, "Removeu o anexo " + vendaDiretaAnexo.NomeArquivo + ".", unitOfWork);
                }
            }
        }

        private void InserirAgendaTarefa(Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Agenda.AgendaTarefa repAgendaTarefa = new Repositorio.Embarcador.Agenda.AgendaTarefa(unidadeTrabalho);
            if (vendaDireta.DataAgendamento.HasValue && vendaDireta.Status != StatusVendaDireta.Cancelado)
            {
                Dominio.Entidades.Embarcador.Agenda.AgendaTarefa agendaTarefa = new Dominio.Entidades.Embarcador.Agenda.AgendaTarefa()
                {
                    DataInicial = vendaDireta.DataAgendamento.Value,
                    DataFinal = vendaDireta.DataAgendamento.Value.AddMinutes(30),
                    Cliente = vendaDireta.Cliente,
                    Prioridade = PrioridadeAtendimento.Normal,
                    Status = StatusAgendaTarefa.Aberto,
                    Observacao = "Venda Direta nº " + vendaDireta.Numero + (!string.IsNullOrWhiteSpace(vendaDireta.NumeroPedido) ? ", nº pedido " + vendaDireta.NumeroPedido : string.Empty) +
                        " gerada por " + vendaDireta.Funcionario.Nome + " na data " + vendaDireta.Data.ToString("dd/MM/yyyy") + ". Agendador: " + (vendaDireta.Agendador?.Nome ?? "") + ". Número do Pedido: " + (vendaDireta.NumeroPedido ?? ""),
                    Funcionario = this.Usuario,
                    Empresa = this.Usuario.Empresa,
                    TipoAssinatura = vendaDireta.TipoAssinatura
                };

                repAgendaTarefa.Inserir(agendaTarefa);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, agendaTarefa, null, "Agenda inserida automaticamente pela Venda Direta nº " + vendaDireta.Numero, unidadeTrabalho);
            }
        }

        private void ExcluirAnexosContestacao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo repVendaDiretaAnexo = new Repositorio.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo(unitOfWork);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosContestacaoExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaContestacaoAnexo vendaDiretaAnexo = repVendaDiretaAnexo.BuscarPorCodigo((int)anexo.Codigo, true);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(vendaDiretaAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(vendaDiretaAnexo.CaminhoArquivo);

                    repVendaDiretaAnexo.Deletar(vendaDiretaAnexo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, vendaDiretaAnexo.VendaDireta, null, "Removeu o anexo da contestação " + vendaDiretaAnexo.NomeArquivo + ".", unitOfWork);
                }
            }
        }


        #endregion
    }
}
