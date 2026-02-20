using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CRM
{
    [CustomAuthorize("CRM/Prospeccao")]
    public class ProspeccaoController : BaseController
    {
		#region Construtores

		public ProspeccaoController(Conexao conexao) : base(conexao) { }

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
        public async Task<IActionResult> HistoricoCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridHistorico();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;

                // Instancia repositorios
                Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);

                // Dados do filtro
                int.TryParse(Request.Params("Cliente"), out int cliente);
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                // Consulta
                List<Dominio.Entidades.Embarcador.CRM.Prospeccao> listaGrid = repProspeccao.ConsultarHistorico(cliente, codigo, codigoEmpresa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                totalRegistros = repProspeccao.ContarConsultaHistorico(cliente, codigo, codigoEmpresa);

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 DataLancamento = obj.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                                 Produto = obj.ProdutoProspect != null ? obj.ProdutoProspect.Nome : String.Empty,
                                 Usuario = obj.Usuario.Nome,
                                 TipoContato = obj.DescricaoTipoContato,
                                 OrigemContato = obj.OrigemContatoClienteProspect != null ? obj.OrigemContatoClienteProspect.Nome : string.Empty,
                                 Situacao = obj.DescricaoSituacao
                             }).ToList();

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);
                Repositorio.Embarcador.CRM.ProspeccaoAnexo repProspeccaoAnexo = new Repositorio.Embarcador.CRM.ProspeccaoAnexo(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao = repProspeccao.BuscarPorCodigo(codigo);

                // Valida
                if (prospeccao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo> anexos = repProspeccaoAnexo.BuscarPorProspeccao(prospeccao.Codigo);

                // Formata retorno
                var retorno = new
                {
                    prospeccao.Codigo,
                    DataLancamento = prospeccao.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                    Nome = prospeccao.Nome,
                    Produto = prospeccao.ProdutoProspect != null ? new { prospeccao.ProdutoProspect.Codigo, Descricao = prospeccao.ProdutoProspect.Nome } : null,
                    Cliente = new { prospeccao.Cliente.Codigo, Descricao = prospeccao.Cliente.Nome },
                    CNPJ = prospeccao.CNPJ_Formatado,
                    Contato = prospeccao.Contato,
                    Email = prospeccao.Email,
                    Telefone = prospeccao.Telefone,
                    Cidade = prospeccao.Cidade != null ? new { prospeccao.Cidade.Codigo, Descricao = prospeccao.Cidade.DescricaoCidadeEstado } : null,
                    TipoContato = prospeccao.TipoContato,
                    OrigemContato = prospeccao.OrigemContatoClienteProspect != null ? new { prospeccao.OrigemContatoClienteProspect.Codigo, Descricao = prospeccao.OrigemContatoClienteProspect.Nome } : null,
                    Valor = prospeccao.Valor.ToString("n2"),
                    Situacao = prospeccao.Situacao,
                    EnumSituacao = prospeccao.Situacao,
                    Satisfacao = prospeccao.Satisfacao,
                    DataRetorno = prospeccao.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    Faturado = prospeccao.Faturado,
                    Observacao = prospeccao.Observacao,
                    Usuario = prospeccao.Usuario.Nome,
                    Anexos = (from obj in anexos
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  obj.NomeArquivo
                              }).ToList()
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

        public async Task<IActionResult> DetalheHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao = repProspeccao.BuscarPorCodigo(codigo);

                // Valida
                if (prospeccao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    prospeccao.Codigo,
                    DataLancamento = prospeccao.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                    Nome = prospeccao.Nome,
                    Produto = prospeccao.ProdutoProspect?.Nome ?? string.Empty,
                    CNPJ = prospeccao.CNPJ_Formatado,
                    Contato = prospeccao.Contato,
                    Email = prospeccao.Email,
                    Telefone = prospeccao.Telefone,
                    Cidade = prospeccao.Cidade?.DescricaoCidadeEstado ?? string.Empty,
                    TipoContato = prospeccao.DescricaoTipoContato,
                    OrigemContato = prospeccao.OrigemContatoClienteProspect?.Nome ?? string.Empty,
                    Valor = prospeccao.Valor.ToString("n2"),
                    Situacao = prospeccao.DescricaoSituacao,
                    Satisfacao = prospeccao.DescricaoSatisfacao,
                    DataRetorno = prospeccao.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    Faturado = prospeccao.DescricaoFaturado,
                    Observacao = prospeccao.Observacao
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);
                Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao = new Dominio.Entidades.Embarcador.CRM.Prospeccao();

                // Preenche entidade com dados
                PreencheEntidade(ref prospeccao, unitOfWork);

                // Cliente Prospect
                int.TryParse(Request.Params("Cliente"), out int cliente);
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect = repClienteProspect.BuscarPorCodigo(cliente, codigoEmpresa);

                if (clienteProspect == null)
                    clienteProspect = new Dominio.Entidades.Embarcador.CRM.ClienteProspect();
                else
                    clienteProspect.Initialize();

                PreencheCliente(ref clienteProspect, prospeccao, unitOfWork);
                if (clienteProspect.Codigo > 0)
                    repClienteProspect.Atualizar(clienteProspect, Auditado);
                else
                    repClienteProspect.Inserir(clienteProspect, Auditado);

                prospeccao.Cliente = clienteProspect;

                // Valida entidade
                if (!ValidaEntidade(prospeccao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repProspeccao.Inserir(prospeccao, Auditado);

                if (prospeccao.DataRetorno.HasValue && prospeccao.DataRetorno.Value > DateTime.MinValue)
                    LancarRetorno(prospeccao, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(prospeccao.Codigo);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);
                Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao = repProspeccao.BuscarPorCodigo(codigo);

                DateTime dataProspeccaoAnterior = prospeccao.DataRetorno.HasValue ? prospeccao.DataRetorno.Value : DateTime.MinValue;

                // Valida
                if (prospeccao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (prospeccao.Usuario != this.Usuario && !ConfiguracaoEmbarcador.PermiteOutrosOperadoresAlterarLancamentoProspeccao)
                    return new JsonpResult(false, true, "Registro não pode ser alterado por um usuário diferente do que efetuou o lançamento.");

                if (prospeccao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.Pendente)
                {
                    bool faturado = Request.Params("Faturado") == "1";
                    prospeccao.Faturado = faturado;
                }
                else
                {
                    // Preenche entidade com dados
                    PreencheEntidade(ref prospeccao, unitOfWork);

                    // Cliente Prospect
                    int.TryParse(Request.Params("Cliente"), out int cliente);
                    int codigoEmpresa = 0;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        codigoEmpresa = this.Usuario.Empresa.Codigo;

                    Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect = repClienteProspect.BuscarPorCodigo(cliente, codigoEmpresa);

                    if (clienteProspect == null)
                        clienteProspect = new Dominio.Entidades.Embarcador.CRM.ClienteProspect();
                    else
                        clienteProspect.Initialize();

                    PreencheCliente(ref clienteProspect, prospeccao, unitOfWork);
                    if (clienteProspect.Codigo > 0)
                        repClienteProspect.Atualizar(clienteProspect, Auditado);
                    else
                        repClienteProspect.Inserir(clienteProspect, Auditado);

                    prospeccao.Cliente = clienteProspect;
                }

                // Valida entidade
                if (!ValidaEntidade(prospeccao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repProspeccao.Atualizar(prospeccao);

                if (prospeccao.DataRetorno.HasValue && prospeccao.DataRetorno.Value > DateTime.MinValue && dataProspeccaoAnterior != prospeccao.DataRetorno.Value)
                    LancarRetorno(prospeccao, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao = repProspeccao.BuscarPorCodigo(codigo);

                // Valida
                if (prospeccao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repProspeccao.Deletar(prospeccao);
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
            grid.Prop("Descricao");
            grid.Prop("DataLancamento").Nome("Data Lançamento").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Usuario").Nome("Usuário").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Produto").Nome("Produto").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Cliente").Nome("Cliente").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("DataRetorno").Nome("Data Retorno").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Satisfacao").Nome("Satisfação").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        private Models.Grid.Grid GridHistorico()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");            
            grid.Prop("DataLancamento").Nome("Data").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Usuario").Nome("Usuário").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Produto").Nome("Produto").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("TipoContato").Nome("Forma de Contato").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);

            // Dados do filtro            
            int codigoUsuario = 0;
            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            string nomeCliente = Request.Params("Nome");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao situacaoAux))
                situacao = situacaoAux;

            DateTime.TryParseExact(Request.Params("DataLancamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataLancamento);

            // Consulta
            List<Dominio.Entidades.Embarcador.CRM.Prospeccao> listaGrid = repProspeccao.Consultar(codigoUsuario, nomeCliente, codigoEmpresa, situacao, dataLancamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProspeccao.ContarConsulta(codigoUsuario, nomeCliente, codigoEmpresa, situacao, dataLancamento);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Nome,
                            Usuario = obj.Usuario.Nome,
                            DataLancamento = obj.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                            Produto = obj.ProdutoProspect != null ? obj.ProdutoProspect.Nome : String.Empty,
                            Cliente = obj.Nome,
                            DataRetorno = obj.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                            Satisfacao = obj.DescricaoSatisfacao,
                            Situacao = obj.DescricaoSituacao,
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheCliente(ref Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect, Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao, Repositorio.UnitOfWork unitOfWork)
        {
            clienteProspect.Cidade = prospeccao.Cidade;
            clienteProspect.CNPJ = prospeccao.CNPJ;
            clienteProspect.Contato = prospeccao.Contato;
            clienteProspect.Email = prospeccao.Email;
            clienteProspect.Nome = prospeccao.Nome;
            clienteProspect.Telefone = prospeccao.Telefone;
            clienteProspect.TipoContato = prospeccao.TipoContato;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                clienteProspect.Empresa = this.Empresa;
            else
                clienteProspect.Empresa = null;
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.CRM.ProdutoProspect repProdutoProspect = new Repositorio.Embarcador.CRM.ProdutoProspect(unitOfWork);
            Repositorio.Embarcador.CRM.OrigemContatoClienteProspect repOrigemContato = new Repositorio.Embarcador.CRM.OrigemContatoClienteProspect(unitOfWork);

            // Converte valores
            //int.TryParse(Request.Params("Relacional"), out int codigoRelacional);
            //Dominio.Entidades.Embarcador.Modulo.Relacional relacional = repRelacional.BuscarPorCodigo(codigoRelacional);
            DateTime.TryParse(Request.Params("DataLancamento"), out DateTime dataLancamento);
            DateTime? dataRetorno = null;
            if (DateTime.TryParse(Request.Params("DataRetorno"), out DateTime dataRetornoAux))
                dataRetorno = dataRetornoAux;

            int.TryParse(Request.Params("Produto"), out int produto);
            int.TryParse(Request.Params("Cidade"), out int cidade);
            int.TryParse(Request.Params("OrigemContato"), out int origemContato);

            decimal.TryParse(Request.Params("Valor"), out decimal valor);

            string nome = Request.Params("Nome") ?? string.Empty;
            string cnpj = Request.Params("CNPJ") ?? string.Empty;
            string contato = Request.Params("Contato") ?? string.Empty;
            string email = Request.Params("Email") ?? string.Empty;
            string telefone = Request.Params("Telefone") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            bool faturado = Request.Params("Faturado") == "1";

            Enum.TryParse(Request.Params("TipoContato"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento tipoContato);
            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao situacao);
            Enum.TryParse(Request.Params("Satisfacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao satisfacao);

            // Vincula dados
            prospeccao.DataLancamento = dataLancamento;
            prospeccao.Usuario = this.Usuario;
            prospeccao.ProdutoProspect = repProdutoProspect.BuscarPorCodigo(produto);
            prospeccao.CNPJ = Utilidades.String.OnlyNumbers(cnpj);
            prospeccao.Contato = contato;
            prospeccao.Nome = nome;
            prospeccao.Email = email;
            prospeccao.Telefone = telefone;
            prospeccao.Cidade = cidade > 0 ? repLocalidade.BuscarPorCodigo(cidade) : null;
            prospeccao.TipoContato = tipoContato;
            prospeccao.OrigemContatoClienteProspect = origemContato > 0 ? repOrigemContato.BuscarPorCodigo(origemContato) : null;
            prospeccao.Valor = valor;
            prospeccao.Situacao = situacao;
            prospeccao.Satisfacao = satisfacao;
            prospeccao.DataRetorno = dataRetorno;
            prospeccao.Faturado = faturado;
            prospeccao.Observacao = observacao;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                prospeccao.Empresa = this.Usuario.Empresa;
            else
                prospeccao.Empresa = null;
        }

        private void LancarRetorno(Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork);

            Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccaoRetorno = new Dominio.Entidades.Embarcador.CRM.Prospeccao();

            prospeccaoRetorno.DataLancamento = prospeccao.DataRetorno.Value;
            prospeccaoRetorno.Usuario = this.Usuario;
            prospeccaoRetorno.ProdutoProspect = prospeccao.ProdutoProspect;
            prospeccaoRetorno.Cliente = prospeccao.Cliente;
            prospeccaoRetorno.CNPJ = prospeccao.CNPJ;
            prospeccaoRetorno.Contato = prospeccao.Contato;
            prospeccaoRetorno.Nome = prospeccao.Nome; ;
            prospeccaoRetorno.Email = prospeccao.Email;
            prospeccaoRetorno.Telefone = prospeccao.Telefone;
            prospeccaoRetorno.Cidade = prospeccao.Cidade;
            prospeccaoRetorno.TipoContato = prospeccao.TipoContato;
            prospeccaoRetorno.OrigemContatoClienteProspect = prospeccao.OrigemContatoClienteProspect;
            prospeccaoRetorno.Valor = prospeccao.Valor;
            prospeccaoRetorno.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.Pendente;
            prospeccaoRetorno.Satisfacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao.NaoAvaliado;
            prospeccaoRetorno.Faturado = false;
            prospeccaoRetorno.Observacao = "Lançamento automático em " + prospeccao.DataLancamento.ToString("dd/MM/yyyy HH:mm") + " referente retorno de contato.";
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                prospeccaoRetorno.Empresa = prospeccao.Empresa;
            else
                prospeccaoRetorno.Empresa = null;

            repProspeccao.Inserir(prospeccaoRetorno);
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao, out string msgErro)
        {
            msgErro = "";

            if (prospeccao.DataLancamento == DateTime.MinValue)
            {
                msgErro = "Data do Lançamento é obrigatório.";
                return false;
            }

            if (prospeccao.Cliente == null || string.IsNullOrEmpty(prospeccao.Nome))
            {
                msgErro = "Cliente é obrigatório.";
                return false;
            }

            if (prospeccao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.Vendido && string.IsNullOrEmpty(prospeccao.CNPJ))
            {
                msgErro = "CNPJ é obrigatório quando a situação for Vendido.";
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
            if (propOrdenar == "Produto") propOrdenar = "Produto.Nome";
        }
        #endregion
    }
}
