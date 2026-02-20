using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using System.Text;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/Cheque", "Financeiros/ChequeAnexo")]
    public class ChequeController : BaseController
    {
		#region Construtores

		public ChequeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Financeiro.Cheque cheque = new Dominio.Entidades.Embarcador.Financeiro.Cheque();

                try
                {
                    PreencherCheque(cheque, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Cheque repositorio = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);

                cheque.DataCadastro = DateTime.Now;
                cheque.Numero = repositorio.BuscarProximoNumero();

                if (cheque.DataCompensacao.HasValue && cheque.DataCompensacao.Value > DateTime.MinValue)
                {
                    string erro = "";
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipoDocumentoMovimento = TipoDocumentoMovimento.Manual;
                    if (cheque.Tipo == TipoCheque.Caucao)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Outros;
                    else if (cheque.Tipo == TipoCheque.Recebido || cheque.Tipo == TipoCheque.Repassado)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Recebimento;
                    else if (cheque.Tipo == TipoCheque.Emitido || cheque.Tipo == TipoCheque.Pago)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Pagamento;

                    if (!servProcessoMovimento.GerarMovimentacao(out erro, cheque.TipoMovimentoCompensacao, cheque.DataCompensacao.Value, cheque.Valor, cheque.NumeroCheque, "COMPENSAÇÃO DO CHEQUE Nº " + cheque.NumeroCheque + " " + cheque.Observacao, unitOfWork, tipoDocumentoMovimento, TipoServicoMultisoftware, 0, null, null, 0, null, cheque.Pessoa, cheque.Pessoa?.GrupoPessoas ?? null))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                if (repositorio.ChequeDuplicado(0, cheque.NumeroCheque, cheque.DataVencimento, cheque.Banco?.Codigo ?? 0, cheque.Empresa?.Codigo ?? 0, cheque.Pessoa?.CPF_CNPJ ?? 0, cheque.Tipo, cheque.NumeroAgencia, cheque.NumeroConta))
                    return new JsonpResult(false, true, "Já existe um cheque lançado com este número, agência e conta. Favor verificar.");

                repositorio.Inserir(cheque, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    cheque.Codigo
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.Cheque repositorio = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Cheque cheque = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (cheque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                StatusCheque statusAnterior = StatusCheque.Normal;
                StatusCheque novoStatus = StatusCheque.Normal;
                try
                {
                    statusAnterior = cheque.Status;
                    //ValidarPermissaoEdicaoCheque(cheque, unitOfWork); Permite editar alguns campos agora mesmo com baixas
                    PreencherCheque(cheque, unitOfWork);
                    novoStatus = cheque.Status;
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                if (novoStatus == StatusCheque.Compensado && statusAnterior != StatusCheque.Compensado && cheque.TipoMovimentoCompensacao != null && cheque.DataCompensacao.HasValue && cheque.DataCompensacao.Value > DateTime.MinValue)
                {
                    string erro = "";
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipoDocumentoMovimento = TipoDocumentoMovimento.Manual;
                    if (cheque.Tipo == TipoCheque.Caucao)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Outros;
                    else if (cheque.Tipo == TipoCheque.Recebido || cheque.Tipo == TipoCheque.Repassado)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Recebimento;
                    else if (cheque.Tipo == TipoCheque.Emitido || cheque.Tipo == TipoCheque.Pago)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Pagamento;
                    if (!servProcessoMovimento.GerarMovimentacao(out erro, cheque.TipoMovimentoCompensacao, cheque.DataCompensacao.Value, cheque.Valor, cheque.NumeroCheque, "COMPENSAÇÃO DO CHEQUE Nº " + cheque.NumeroCheque + " " + cheque.Observacao, unitOfWork, tipoDocumentoMovimento, TipoServicoMultisoftware, 0, null, null, 0, null, cheque.Pessoa, cheque.Pessoa?.GrupoPessoas ?? null))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                if (repositorio.ChequeDuplicado(cheque.Codigo, cheque.NumeroCheque, cheque.DataVencimento, cheque.Banco?.Codigo ?? 0, cheque.Empresa?.Codigo ?? 0, cheque.Pessoa?.CPF_CNPJ ?? 0, cheque.Tipo, cheque.NumeroAgencia, cheque.NumeroConta))
                    return new JsonpResult(false, true, "Já existe um cheque lançado com este número, agência e conta. Favor verificar.");

                repositorio.Atualizar(cheque, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Financeiro.Cheque repositorio = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Cheque cheque = repositorio.BuscarPorCodigo(codigo);

                if (cheque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorioTituloBaixaCheque = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> listaTituloBaixaCheque = repositorioTituloBaixaCheque.BuscarPorChequeESituacaoTituloBaixaNaoCancelada(codigo);

                return new JsonpResult(new
                {
                    cheque.Codigo,
                    Banco = new { cheque.Banco.Codigo, cheque.Banco.Descricao },
                    DataCompensacao = cheque.DataCompensacao?.ToString("dd/MM/yyyy") ?? "",
                    DataTransacao = cheque.DataTransacao.ToString("dd/MM/yyyy"),
                    DataVencimento = cheque.DataVencimento.ToString("dd/MM/yyyy"),
                    cheque.DigitoAgencia,
                    Empresa = new { cheque.Empresa.Codigo, cheque.Empresa.Descricao },
                    cheque.Numero,
                    cheque.NumeroAgencia,
                    cheque.NumeroCheque,
                    cheque.NumeroConta,
                    cheque.Observacao,
                    Pessoa = new { cheque.Pessoa.Codigo, cheque.Pessoa.Descricao },
                    PessoaRepasse = cheque.PessoaRepasse != null ? new { Codigo = cheque.PessoaRepasse?.Codigo ?? 0, Descricao = cheque.PessoaRepasse?.Descricao ?? "" } : null,
                    cheque.Status,
                    cheque.Tipo,
                    TipoMovimentoCompensacao = new { Codigo = cheque.TipoMovimentoCompensacao?.Codigo ?? 0, Descricao = cheque.TipoMovimentoCompensacao?.Descricao ?? "" },
                    cheque.Valor,
                    BaixasTitulo = (
                        from tituloBaixaCheque in listaTituloBaixaCheque
                        select new
                        {
                            Descricao = $"Baixa de titulo {tituloBaixaCheque.TituloBaixa.TipoBaixaTitulo.ObterDescricao().ToLower()}: {tituloBaixaCheque.TituloBaixa.Codigo}"
                        }
                    ).ToList(),
                    Anexos = (
                        from anexo in cheque.Anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.Cheque repositorio = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Cheque cheque = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (cheque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(cheque, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                {
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, deve ser alterado o status para Cancelado.");
                }
                else
                {
                    Servicos.Log.TratarErro(excecao);
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> Extornar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.Cheque repositorio = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Cheque cheque = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (cheque == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cheque.Status != StatusCheque.Compensado)
                    return new JsonpResult(false, true, "O status do cheque não permite realizar o estorno.");

                unitOfWork.Start();

                if (cheque.TipoMovimentoCompensacao != null && cheque.TipoMovimentoCompensacao.PlanoDeContaDebito != null && cheque.TipoMovimentoCompensacao.PlanoDeContaCredito != null && cheque.DataCompensacao.HasValue && cheque.DataCompensacao.Value > DateTime.MinValue)
                {
                    string erro = "";
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipoDocumentoMovimento = TipoDocumentoMovimento.Manual;
                    if (cheque.Tipo == TipoCheque.Caucao)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Outros;
                    else if (cheque.Tipo == TipoCheque.Recebido || cheque.Tipo == TipoCheque.Repassado)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Recebimento;
                    else if (cheque.Tipo == TipoCheque.Emitido || cheque.Tipo == TipoCheque.Pago)
                        tipoDocumentoMovimento = TipoDocumentoMovimento.Pagamento;
                    if (!servProcessoMovimento.GerarMovimentacao(out erro, null, cheque.DataCompensacao.Value, cheque.Valor, cheque.NumeroCheque, "REVERSÃO DA COMPENSAÇÃO DO CHEQUE Nº " + cheque.NumeroCheque + " " + cheque.Observacao, unitOfWork, tipoDocumentoMovimento, TipoServicoMultisoftware, 0, cheque.TipoMovimentoCompensacao.PlanoDeContaDebito, cheque.TipoMovimentoCompensacao.PlanoDeContaCredito, 0, null, cheque.Pessoa, cheque.Pessoa?.GrupoPessoas ?? null))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }
                }

                cheque.DataCompensacao = null;
                cheque.Status = StatusCheque.Normal;
                cheque.TipoMovimentoCompensacao = null;

                repositorio.Atualizar(cheque);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cheque, "Estornou o cheque", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao estornar o cheque.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherCheque(Dominio.Entidades.Embarcador.Financeiro.Cheque cheque, Repositorio.UnitOfWork unitOfWork)
        {
            cheque.Banco = ObterBanco(unitOfWork);
            cheque.DataTransacao = Request.GetDateTimeParam("DataTransacao");
            cheque.DataVencimento = Request.GetDateTimeParam("DataVencimento");
            cheque.DigitoAgencia = Request.GetNullableStringParam("DigitoAgencia");
            cheque.Empresa = ObterEmpresa(unitOfWork);
            cheque.NumeroAgencia = Request.GetNullableStringParam("NumeroAgencia");
            cheque.NumeroCheque = Request.GetNullableStringParam("NumeroCheque") ?? throw new ControllerException("O número do cheque deve ser informado.");
            cheque.NumeroConta = Request.GetNullableStringParam("NumeroConta");
            cheque.Observacao = Request.GetNullableStringParam("Observacao");
            cheque.Pessoa = ObterPessoa(unitOfWork);
            cheque.PessoaRepasse = ObterPessoaRepasse(unitOfWork);
            cheque.Status = Request.GetEnumParam<StatusCheque>("Status");
            cheque.Tipo = Request.GetEnumParam<TipoCheque>("Tipo");
            cheque.Valor = Request.GetDecimalParam("Valor");

            if (cheque.Status == StatusCheque.Compensado)
            {
                cheque.DataCompensacao = Request.GetNullableDateTimeParam("DataCompensacao") ?? throw new ControllerException("A data de compensação deve ser informada.");
                cheque.TipoMovimentoCompensacao = ObterTipoMovimentoCompensacao(unitOfWork);

                if (cheque.TipoMovimentoCompensacao.Banco != null && cheque.TipoMovimentoCompensacao.Banco.Codigo != cheque.Banco.Codigo)
                    throw new ControllerException("Banco informado diverge do configurado para o Tipo Movimento de Compensação.");
            }
            else
            {
                cheque.DataCompensacao = null;
                cheque.TipoMovimentoCompensacao = null;
            }
        }

        private Dominio.Entidades.Banco ObterBanco(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoBanco = Request.GetIntParam("Banco");
            Repositorio.Banco repositorio = new Repositorio.Banco(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoBanco) ?? throw new ControllerException("Banco não encontrado.");
        }

        private Dominio.Entidades.Empresa ObterEmpresa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoEmpresa = Request.GetIntParam("Empresa");
            Repositorio.Empresa repositorio = new Repositorio.Empresa(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            return repositorio.BuscarPorCodigo(codigoEmpresa) ?? throw new ControllerException("Empresa não encontrada.");
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCheque ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCheque()
            {
                CpfCnpjPessoa = Request.GetDoubleParam("Pessoa"),
                NumeroCheque = Request.GetStringParam("NumeroCheque"),
                Status = Request.GetNullableEnumParam<StatusCheque>("Status"),
                Tipo = Request.GetNullableEnumParam<TipoCheque>("Tipo"),
                Tipos = Request.GetListEnumParam<TipoCheque>("Tipos"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "NumeroCheque", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Banco", "Banco", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "Status", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 12, Models.Grid.Align.right, false);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCheque filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Financeiro.Cheque repositorio = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Financeiro.Cheque> listaCheque = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Financeiro.Cheque>();

                var listaChequeRetornar = (
                    from cheque in listaCheque
                    select new
                    {
                        cheque.Codigo,
                        cheque.NumeroCheque,
                        Banco = cheque.Banco.Descricao,
                        Pessoa = cheque.Pessoa.Descricao,
                        Status = cheque.Status.ObterDescricao(),
                        Tipo = cheque.Tipo.ObterDescricao(),
                        Valor = cheque.Valor.ToString("n2")
                    }
                ).ToList();

                grid.AdicionaRows(listaChequeRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.Entidades.Cliente ObterPessoa(Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpjPessoa = Request.GetDoubleParam("Pessoa");
            Repositorio.Cliente repositorio = new Repositorio.Cliente(unitOfWork);

            return repositorio.BuscarPorCPFCNPJ(cpfCnpjPessoa) ?? throw new ControllerException("Pessoa não encontrada.");
        }

        private Dominio.Entidades.Cliente ObterPessoaRepasse(Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpjPessoa = Request.GetDoubleParam("PessoaRepasse");
            Repositorio.Cliente repositorio = new Repositorio.Cliente(unitOfWork);

            return cpfCnpjPessoa > 0 ? repositorio.BuscarPorCPFCNPJ(cpfCnpjPessoa) ?? null : null;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Banco")
                return "Banco.Descricao";

            return propriedadeOrdenar;
        }

        private Dominio.Entidades.Embarcador.Financeiro.TipoMovimento ObterTipoMovimentoCompensacao(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoMovimentoCompensacao = Request.GetIntParam("TipoMovimentoCompensacao");
            Repositorio.Embarcador.Financeiro.TipoMovimento repositorio = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoMovimentoCompensacao) ?? throw new ControllerException("Tipo do movimento de compensação não encontrado.");
        }

        private void ValidarPermissaoEdicaoCheque(Dominio.Entidades.Embarcador.Financeiro.Cheque cheque, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloBaixaCheque repositorioTituloBaixaCheque = new Repositorio.Embarcador.Financeiro.TituloBaixaCheque(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> listaTituloBaixaCheque = repositorioTituloBaixaCheque.BuscarPorChequeESituacaoTituloBaixaNaoCancelada(cheque.Codigo);

            if (listaTituloBaixaCheque.Count > 0)
            {
                if (listaTituloBaixaCheque.Count > 1)
                {
                    StringBuilder mensagem = new StringBuilder($"O cheque está vinculado a {listaTituloBaixaCheque.Count} baixas, edição não permitida.");

                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque tituloBaixaCheque in listaTituloBaixaCheque)
                    {
                        mensagem.Append($" Baixa de título {tituloBaixaCheque.TituloBaixa.TipoBaixaTitulo.ObterDescricao().ToLower()}: {tituloBaixaCheque.TituloBaixa.Codigo}.");
                    }

                    throw new ControllerException(mensagem.ToString());
                }
                else
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = listaTituloBaixaCheque[0].TituloBaixa;

                    throw new ControllerException($"O cheque está vinculado a uma baixa, edição não permitida. Baixa de titulo {tituloBaixa.TipoBaixaTitulo.ObterDescricao().ToLower()}: {tituloBaixa.Codigo}");
                }
            }
        }

        #endregion
    }
}
