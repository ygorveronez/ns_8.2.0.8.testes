using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CRM
{
    [CustomAuthorize("CRM/ClienteProspect")]
    public class ClienteProspectController : BaseController
    {
		#region Construtores

		public ClienteProspectController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios                
                Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect = new Dominio.Entidades.Embarcador.CRM.ClienteProspect();

                // Preenche entidade com dados
                PreencheEntidade(ref clienteProspect, unitOfWork);

                // Persiste dados
                repClienteProspect.Inserir(clienteProspect, Auditado);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(clienteProspect.Codigo);
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
                Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect = repClienteProspect.BuscarPorCodigo(codigo, true);

                // Valida
                if (clienteProspect == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref clienteProspect, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(clienteProspect, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repClienteProspect.Atualizar(clienteProspect, Auditado);

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
                Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect = repClienteProspect.BuscarPorCodigo(codigo, true);

                // Valida
                if (clienteProspect == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repClienteProspect.Deletar(clienteProspect, Auditado);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Empresa.Codigo;

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.ClienteProspect cliente = repClienteProspect.BuscarPorCodigo(codigo, codigoEmpresa);

                // Valida
                if (cliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    cliente.Codigo,
                    cliente.Nome,
                    cliente.CNPJ,
                    cliente.Contato,
                    cliente.Email,
                    cliente.Telefone,
                    Cidade = cliente.Cidade != null ? new { cliente.Cidade.Codigo, Descricao = cliente.Cidade.DescricaoCidadeEstado } : null,
                    cliente.TipoContato
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

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            int.TryParse(Request.Params("Cidade"), out int cidade);
            string nome = Request.Params("Nome") ?? string.Empty;
            string cnpj = Request.Params("CNPJ") ?? string.Empty;
            string contato = Request.Params("Contato") ?? string.Empty;
            string email = Request.Params("Email") ?? string.Empty;
            string telefone = Request.Params("Telefone") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;
            Enum.TryParse(Request.Params("TipoContato"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento tipoContato);

            clienteProspect.Cidade = cidade > 0 ? repLocalidade.BuscarPorCodigo(cidade) : null;
            clienteProspect.CNPJ = Utilidades.String.OnlyNumbers(cnpj);
            clienteProspect.Contato = contato;
            clienteProspect.Email = email;
            clienteProspect.Nome = nome;
            clienteProspect.Telefone = telefone;
            clienteProspect.TipoContato = tipoContato;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                clienteProspect.Empresa = this.Empresa;
            else
                clienteProspect.Empresa = null;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteProspect, out string msgErro)
        {
            msgErro = "";

            return true;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Contato");
            grid.Prop("Email");
            grid.Prop("Telefone");
            grid.Prop("CodigoCidade");
            grid.Prop("Descricao").Nome("Nome").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("CNPJ").Nome("CNPJ").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Cidade").Nome("Cidade").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(unitOfWork);

            // Dados do filtro
            //int.TryParse(Request.Params("Filtro"), out int codigoFiltro);
            string descricao = Request.Params("Descricao") ?? string.Empty;
            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.CRM.ClienteProspect> listaGrid = repClienteProspect.Consultar(descricao, codigoEmpresa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repClienteProspect.ContarConsulta(descricao, codigoEmpresa);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,

                            obj.Contato,
                            obj.Email,
                            obj.Telefone,
                            CodigoCidade = obj.Cidade?.Codigo ?? 0,

                            Descricao = obj.Nome,
                            CNPJ = obj.CNPJ_Formatado,
                            Cidade = obj.Cidade?.DescricaoCidadeEstado ?? string.Empty,
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Descricao") propOrdenar = "Nome";
        }

        #endregion
    }
}
