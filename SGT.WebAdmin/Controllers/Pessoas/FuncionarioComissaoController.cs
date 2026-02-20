using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize(new string[] { "PesquisaAutorizacoes", "DetalhesAutorizacao" }, "Pessoas/FuncionarioComissao")]
    public class FuncionarioComissaoController : BaseController
    {
		#region Construtores

		public FuncionarioComissaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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


        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTitulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo repFuncionarioComissaoTitulo = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo(unitOfWork);
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaTitulo();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "DataEmissao") propOrdenar = "Titulo.DataEmissao";
                else if (propOrdenar == "CodigoTitulo") propOrdenar = "Titulo.Codigo";
                else if (propOrdenar == "Pessoa") propOrdenar = "Titulo.Pessoa.Nome";
                else if (propOrdenar == "CPFCNPJ") propOrdenar = "Titulo.Pessoa.CPF_CNPJ";
                else if (propOrdenar == "DataVencimento") propOrdenar = "Titulo.DataVencimento";
                else if (propOrdenar == "DataLiquidacao") propOrdenar = "Titulo.DataLiquidacao";
                else if (propOrdenar == "ValorOriginal") propOrdenar = "Titulo.ValorTituloOriginal";
                else if (propOrdenar == "ValorPago") propOrdenar = "Titulo.ValorPago";

                // Busca Dados
                int.TryParse(Request.Params("Codigo"), out int funcionarioComissao);
                int totalRegistros = repFuncionarioComissaoTitulo.ContarConsultaPorFuncionarioComissao(funcionarioComissao);

                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo> listaGrid = repFuncionarioComissaoTitulo.ConsultarPorFuncionarioComissao(funcionarioComissao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                CodigoTitulo = obj.Titulo.Codigo,
                                Pessoa = obj.Titulo.Pessoa.Nome,
                                CPFCNPJ = obj.Titulo.Pessoa.CPF_CNPJ_Formatado,
                                NumeroFatura = obj.Titulo.FaturaParcela?.Fatura?.Numero.ToString("n0") ?? string.Empty,
                                DataEmissao = obj.Titulo.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                DataVencimento = obj.Titulo.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                DataLiquidacao = obj.Titulo.DataLiquidacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                ValorOriginal = obj.Titulo.ValorTituloOriginal.ToString("n2"),
                                ValorPago = obj.Titulo.ValorPago.ToString("n2"),
                                ValorISS = obj.ValorISS.ToString("n2"),
                                ValorICMS = obj.ValorICMS.ToString("n2"),
                                ValorLiquido = obj.ValorLiquido.ToString("n2"),
                                PercentualImpostoFederal = obj.PercentualImpostoFederal.ToString("n2"),
                                ValorFinal = obj.ValorFinal.ToString("n2")
                            };

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Busca
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao> listaAutorizacao = repAprovacaoAlcadaFuncionarioComissao.ConsultarAutorizacoesPorFuncionarioComissao(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repAprovacaoAlcadaFuncionarioComissao.ContarConsultaAutorizacoesPorFuncionarioComissao(codigo);

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegraHelper.ObterDescricao(obj.Situacao),
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = obj.RegraFuncionarioComissao.Descricao,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 DT_RowColor = CorAprovacao(obj.Situacao)
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao = repFuncionarioComissao.BuscarPorCodigo(codigo);

                // Valida
                if (funcionarioComissao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    funcionarioComissao.Codigo,
                    funcionarioComissao.Situacao,
                    funcionarioComissao.Numero,
                    Funcionario = funcionarioComissao.Funcionario != null ? new { funcionarioComissao.Funcionario.Codigo, Descricao = funcionarioComissao.Funcionario.Nome } : null,
                    DataInicial = funcionarioComissao.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = funcionarioComissao.DataFinal.ToString("dd/MM/yyyy"),
                    funcionarioComissao.Observacao,
                    funcionarioComissao.QuantidadeTitulos,
                    ValorTotalFinal = funcionarioComissao.ValorTotalFinal.ToString("n2"),

                    Resumo = ResumoAutorizacao(funcionarioComissao, unitOfWork)
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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
        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao autorizacao = repAprovacaoAlcadaFuncionarioComissao.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Delegada ? "(Delegada)" : autorizacao.RegraFuncionarioComissao.Descricao,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegraHelper.ObterDescricao(autorizacao.Situacao),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,

                    PodeAprovar = autorizacao.Usuario != null && autorizacao.Usuario.Codigo == this.Usuario.Codigo && autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,

                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(autorizacao.Motivo) ? autorizacao.Motivo : string.Empty,
                };

                return new JsonpResult(retorno);
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
                // Instancia repositorios
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao = new Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao();

                // Preenche entidade com dados
                PreencheEntidade(ref funcionarioComissao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(funcionarioComissao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repFuncionarioComissao.Inserir(funcionarioComissao, Auditado);
                SalvarTitulos(funcionarioComissao, unitOfWork, Auditado, null);
                CalcularComissoes(funcionarioComissao, unitOfWork);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Servicos.Embarcador.Pessoa.FuncionarioComissao.EtapaAprovacao(ref funcionarioComissao, unitOfWork, TipoServicoMultisoftware, tipoAmbiente, _conexao.StringConexao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao = repFuncionarioComissao.BuscarPorCodigo(codigo, true);

                // Valida
                if (funcionarioComissao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (funcionarioComissao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                // Busca as regras
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Servicos.Embarcador.Pessoa.FuncionarioComissao.EtapaAprovacao(ref funcionarioComissao, unitOfWork, TipoServicoMultisoftware, tipoAmbiente, _conexao.StringConexao, Auditado);
                repFuncionarioComissao.Atualizar(funcionarioComissao);
                unitOfWork.CommitChanges();

                bool possuiRegra = funcionarioComissao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.SemRegra;

                // Retorna sucesso
                return new JsonpResult(possuiRegra);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
                Servicos.Embarcador.Pessoa.FuncionarioComissao svcFuncionarioComissao = new Servicos.Embarcador.Pessoa.FuncionarioComissao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao = repFuncionarioComissao.BuscarPorCodigo(codigo);

                // Valida
                if (funcionarioComissao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                unitOfWork.Start();

                string erro = string.Empty;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                if (!svcFuncionarioComissao.AtualizarTitulo(funcionarioComissao, unitOfWork, TipoServicoMultisoftware, out erro, tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Cancelado))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarTitulosPorFuncionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo repFuncionarioComissaoTitulo = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo(unitOfWork);

                int.TryParse(Request.Params("CodigoFuncionario"), out int codigoFuncionario);
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                decimal percentualImpostoFederal = ConfiguracaoEmbarcador.PercentualImpostoFederal;

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repFuncionarioComissaoTitulo.BuscarTitulosPorVendedorPessoa(codigoFuncionario, dataInicial, dataFinal, codigoEmpresa);
                var listaTit = (from obj in listaTitulos
                                select new
                                {
                                    obj.Codigo,
                                    CodigoTitulo = obj.Codigo,
                                    Pessoa = obj.Pessoa.Nome,
                                    CPFCNPJ = obj.Pessoa.CPF_CNPJ_Formatado,
                                    NumeroFatura = obj.FaturaParcela?.Fatura?.Numero.ToString("n0") ?? string.Empty,
                                    DataEmissao = obj.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                    DataVencimento = obj.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                    DataLiquidacao = obj.DataLiquidacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                    ValorOriginal = obj.ValorTituloOriginal.ToString("n2"),
                                    ValorPago = obj.ValorPago.ToString("n2"),
                                    ValorISS = this.RetornaValorISS(obj, unitOfWork, obj.ValorPago),
                                    ValorICMS = this.RetornaValorICMS(obj, unitOfWork, obj.ValorPago),
                                    ValorLiquido = obj.ValorPago,
                                    PercentualImpostoFederal = percentualImpostoFederal,
                                    ValorFinal = (obj.ValorPago).ToString("n2")
                                }).ToList();

                var retorno = new
                {
                    Titulos = (from obj in listaTit
                               select new
                               {
                                   obj.Codigo,
                                   CodigoTitulo = obj.Codigo,
                                   Pessoa = obj.Pessoa,
                                   CPFCNPJ = obj.CPFCNPJ,
                                   NumeroFatura = obj.NumeroFatura,
                                   DataEmissao = obj.DataEmissao,
                                   DataVencimento = obj.DataVencimento,
                                   DataLiquidacao = obj.DataLiquidacao,
                                   ValorOriginal = obj.ValorOriginal,
                                   ValorPago = obj.ValorPago,
                                   ValorISS = obj.ValorISS.ToString("n2"),
                                   ValorICMS = obj.ValorICMS.ToString("n2"),
                                   ValorLiquido = (obj.ValorLiquido - obj.ValorISS - obj.ValorICMS).ToString("n2"),
                                   PercentualImpostoFederal = obj.PercentualImpostoFederal.ToString("n2"),
                                   ValorFinal = (obj.PercentualImpostoFederal > 0 && (obj.ValorLiquido - obj.ValorISS - obj.ValorICMS) > 0 ? ((obj.ValorLiquido - obj.ValorISS - obj.ValorICMS) * (obj.PercentualImpostoFederal / 100)) : (obj.ValorLiquido - obj.ValorISS - obj.ValorICMS)).ToString("n2")
                               }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os títulos do Vendedor");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Funcionario").Nome("Funcionário").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("DataInicial").Nome("Data Inicial").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("DataFinal").Nome("Data Final").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.center).Ord(false);
            grid.Prop("QuantidadeTitulos").Nome("Qtd. Titulos").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("ValorTotalFinal").Nome("Valor Total Final").Tamanho(10).Align(Models.Grid.Align.right).Ord(false);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaTitulo()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("CodigoTitulo").Nome("Nº Título").Tamanho(6).Align(Models.Grid.Align.right);
            grid.Prop("Pessoa").Nome("Pessoa").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("CPFCNPJ").Nome("CPF/CNPJ").Tamanho(8).Align(Models.Grid.Align.center);
            grid.Prop("NumeroFatura").Nome("Nº Fatura").Tamanho(6).Align(Models.Grid.Align.right).Ord(false);
            grid.Prop("DataEmissao").Nome("Emissão").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("DataVencimento").Nome("Vencimento").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("DataLiquidacao").Nome("Liquidação").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("ValorOriginal").Nome("Vlr. Original").Tamanho(6).Align(Models.Grid.Align.right);
            grid.Prop("ValorPago").Nome("Vlr Pago").Tamanho(6).Align(Models.Grid.Align.right);
            grid.Prop("ValorISS").Nome("Vlr ISS").Tamanho(6).Align(Models.Grid.Align.right);
            grid.Prop("ValorICMS").Nome("Vlr ICMS").Tamanho(6).Align(Models.Grid.Align.right);
            grid.Prop("ValorLiquido").Nome("Vlr Líquido").Tamanho(7).Align(Models.Grid.Align.right);
            grid.Prop("PercentualImpostoFederal").Nome("% Imp. Federal").Tamanho(6).Align(Models.Grid.Align.right);
            grid.Prop("ValorFinal").Nome("Valor Final").Tamanho(7).Align(Models.Grid.Align.right);

            return grid;
        }

        /* ExecutaPesquisa
        * Converte os dados recebidos e executa a busca
        * Retorna um dynamic pronto para adicionar ao grid
        */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Funcionario"), out int funcionario);

            DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicio);
            DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFim);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao situacaoAux))
                situacao = situacaoAux;

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> listaGrid = repFuncionarioComissao.Consultar(codigoEmpresa, dataInicio, dataFim, funcionario, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repFuncionarioComissao.ContarConsulta(codigoEmpresa, dataInicio, dataFim, funcionario, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            Funcionario = obj.Funcionario.Nome,
                            DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"),
                            DataFinal = obj.DataFinal.ToString("dd/MM/yyyy"),
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissaoHelper.ObterDescricao(obj.Situacao),
                            obj.QuantidadeTitulos,
                            ValorTotalFinal = obj.ValorTotalFinal.ToString("n2")
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
        * Recebe uma instancia da entidade
        * Converte parametros recebido por request
        * Atribui a entidade
        */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            int.TryParse(Request.Params("Funcionario"), out int funcionario);

            DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

            string observacao = Request.Params("Observacao") ?? string.Empty;

            // Vincula dados
            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;
            funcionarioComissao.Numero = repFuncionarioComissao.BuscarProximoNumero(codigoEmpresa);
            funcionarioComissao.Operador = this.Usuario;
            funcionarioComissao.DataGeracao = DateTime.Now;

            funcionarioComissao.DataInicial = dataInicial;
            funcionarioComissao.DataFinal = dataFinal;
            funcionarioComissao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.AgAprovacao;
            funcionarioComissao.Observacao = observacao;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                funcionarioComissao.Empresa = this.Usuario.Empresa;
            if (funcionario > 0)
                funcionarioComissao.Funcionario = repUsuario.BuscarPorCodigo(funcionario);
            else
                funcionarioComissao.Funcionario = null;
        }

        /* ValidaEntidade
        * Recebe uma instancia da entidade
        * Valida informacoes
        * Retorna de entidade e valida ou nao e retorna erro (se tiver)
        */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, out string msgErro)
        {
            msgErro = "";

            if (funcionarioComissao.Funcionario == null)
            {
                msgErro = "Funcionário é obrigatório.";
                return false;
            }

            if (funcionarioComissao.DataInicial == DateTime.MinValue || funcionarioComissao.DataFinal == DateTime.MinValue)
            {
                msgErro = "Datas são obrigatória.";
                return false;
            }

            return true;
        }

        /* PropOrdena
        * Recebe o campo ordenado na grid
        * Retorna o elemento especifico da entidade para ordenacao
        */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Funcionario") propOrdenar = "Funcionario.Nome";
        }


        private void SalvarTitulos(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo repFuncionarioComissaoTitulo = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            List<dynamic> titulosFuncionarioComissao = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Titulos"));
            if (titulosFuncionarioComissao == null) return;

            List<int> codigosTitulos = new List<int>();
            foreach (dynamic codigo in titulosFuncionarioComissao)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
            }
            codigosTitulos = codigosTitulos.Where(o => o > 0).Distinct().ToList();

            List<int> codigosExcluir = repFuncionarioComissaoTitulo.BuscarNaoPresentesNaLista(funcionarioComissao.Codigo, codigosTitulos);

            foreach (dynamic dynTitulo in titulosFuncionarioComissao)
            {
                int.TryParse((string)dynTitulo.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo funcionarioComissaoTitulo = repFuncionarioComissaoTitulo.BuscarPorFuncionarioComissaoETitulo(funcionarioComissao.Codigo, codigo);

                if (funcionarioComissaoTitulo == null)
                    funcionarioComissaoTitulo = new Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo();
                else funcionarioComissaoTitulo.Initialize();

                int.TryParse((string)dynTitulo.CodigoTitulo, out int codigoTitulo);

                funcionarioComissaoTitulo.PercentualImpostoFederal = Utilidades.Decimal.Converter((string)dynTitulo.PercentualImpostoFederal);
                funcionarioComissaoTitulo.ValorISS = Utilidades.Decimal.Converter((string)dynTitulo.ValorISS);
                funcionarioComissaoTitulo.ValorICMS = Utilidades.Decimal.Converter((string)dynTitulo.ValorICMS);
                funcionarioComissaoTitulo.ValorLiquido = Utilidades.Decimal.Converter((string)dynTitulo.ValorLiquido);
                funcionarioComissaoTitulo.ValorFinal = Utilidades.Decimal.Converter((string)dynTitulo.ValorFinal);

                funcionarioComissaoTitulo.Titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
                funcionarioComissaoTitulo.FuncionarioComissao = funcionarioComissao;

                if (funcionarioComissaoTitulo.Codigo == 0)
                    repFuncionarioComissaoTitulo.Inserir(funcionarioComissaoTitulo, auditado, historicoPai);
                else
                    repFuncionarioComissaoTitulo.Atualizar(funcionarioComissaoTitulo, auditado, historicoPai);
            }

            foreach (int excluir in codigosExcluir)
            {
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo objParExcluir = repFuncionarioComissaoTitulo.BuscarPorFuncionarioComissaoETitulo(funcionarioComissao.Codigo, excluir);
                if (objParExcluir != null) repFuncionarioComissaoTitulo.Deletar(objParExcluir, auditado, historicoPai);
            }
        }

        private dynamic ResumoAutorizacao(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

            return new
            {
                funcionarioComissao.QuantidadeTitulos,
                ValorFinal = funcionarioComissao.ValorTotalFinal.ToString("n2"),
                Operador = funcionarioComissao.Operador?.Nome ?? string.Empty,
                DataGeracao = funcionarioComissao.DataGeracao.ToString("dd/MM/yyyy") ?? string.Empty,
                Funcionario = funcionarioComissao.Funcionario?.Nome ?? string.Empty,
                PercentualComissao = funcionarioComissao.PercentualComissao.ToString("n2"),
                PercentualComissaoAcrescimo = funcionarioComissao.PercentualComissaoAcrescimo.ToString("n2"),
                PercentualComissaoTotal = funcionarioComissao.PercentualComissaoTotal.ToString("n2"),
                ValorComissao = funcionarioComissao.ValorComissao.ToString("n2"),
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissaoHelper.ObterDescricao(funcionarioComissao.Situacao),
            };
        }

        private string CorAprovacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra situacao)
        {
            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private void CalcularComissoes(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo repFuncionarioComissaoTitulo = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo(unitOfWork);
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta repFuncionarioMeta = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta(unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaFuncionario repPessoaFuncionario = new Repositorio.Embarcador.Pessoas.PessoaFuncionario(unitOfWork);

            List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo> funcionarioComissaoTitulos = repFuncionarioComissaoTitulo.BuscarPorFuncionarioComissao(funcionarioComissao.Codigo);
            if (funcionarioComissaoTitulos.Count > 0)
            {
                decimal percentualComissao = 0;
                decimal percentualComissaoAcrescimo = 0;
                decimal valorTotalTitulos = 0;

                foreach (Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo titulo in funcionarioComissaoTitulos)
                {
                    Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario pessoaFuncionario = repPessoaFuncionario.BuscarPorFuncionarioEPessoa(funcionarioComissao.Funcionario.Codigo, titulo.Titulo.Pessoa.CPF_CNPJ);
                    percentualComissao += pessoaFuncionario?.PercentualComissao ?? 0;
                    valorTotalTitulos += titulo.ValorFinal;
                }

                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta funcionarioMeta = repFuncionarioMeta.BuscarPorFuncionario(funcionarioComissao.Funcionario.Codigo, funcionarioComissao.Empresa?.Codigo ?? 0);
                if (funcionarioMeta != null && funcionarioMeta.Metas != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaValor valor in funcionarioMeta.Metas)
                    {
                        if (valor.Ano == DateTime.Now.Year && valor.Mes == DateTime.Now.Month)
                        {
                            if (valorTotalTitulos > valor.Valor)
                                percentualComissaoAcrescimo = valor.Percentual;
                        }
                    }
                }

                funcionarioComissao.PercentualComissao = percentualComissao / funcionarioComissaoTitulos.Count;
                funcionarioComissao.PercentualComissaoAcrescimo = percentualComissaoAcrescimo;
                funcionarioComissao.PercentualComissaoTotal = funcionarioComissao.PercentualComissao + funcionarioComissao.PercentualComissaoAcrescimo;
                funcionarioComissao.ValorComissao = Math.Round(valorTotalTitulos * (funcionarioComissao.PercentualComissaoTotal / 100), 2);

                repFuncionarioComissao.Atualizar(funcionarioComissao);
            }
        }

        private decimal RetornaValorISS(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork, decimal valorBase)
        {
            decimal aliquotaISS = 0;
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> documentos = repTituloDocumento.BuscarPorTitulo(titulo.Codigo);
            if (documentos != null && documentos.Count > 0)
            {
                aliquotaISS = documentos.Where(o => o.CTe.ValorISS > 0).Average(o => (decimal?)o.CTe.AliquotaISS) ?? 0m;
            }
            if (aliquotaISS > 0 && valorBase > 0)
                return (valorBase * (aliquotaISS / 100));
            else
                return 0m;
        }

        private decimal RetornaValorICMS(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork, decimal valorBase)
        {
            decimal aliquotaICMS = 0;
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> documentos = repTituloDocumento.BuscarPorTitulo(titulo.Codigo);
            if (documentos != null && documentos.Count > 0)
            {
                aliquotaICMS = documentos.Where(o => o.CTe.ValorICMS > 0 &&
                    (o.CTe.CSTICMS == Dominio.Enumeradores.TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90 ||
                    o.CTe.CSTICMS == Dominio.Enumeradores.TipoICMS.ICMS_Outras_Situacoes_90 ||
                    o.CTe.CSTICMS == Dominio.Enumeradores.TipoICMS.ICMS_Normal_00 ||
                    o.CTe.CSTICMS == Dominio.Enumeradores.TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60 ||
                    o.CTe.CSTICMS == Dominio.Enumeradores.TipoICMS.ICMS_Reducao_Base_Calculo_20)).Average(o => (decimal?)o.CTe.AliquotaICMS) ?? 0m;
            }
            if (aliquotaICMS > 0 && valorBase > 0)
                return (valorBase * (aliquotaICMS / 100));
            else
                return 0m;
        }

        #endregion
    }
}
