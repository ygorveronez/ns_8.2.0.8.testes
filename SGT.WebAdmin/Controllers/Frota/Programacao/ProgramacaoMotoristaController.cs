using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.Programacao
{
    [CustomAuthorize("Frota/ProgramacaoMotorista")]
    public class ProgramacaoMotoristaController : BaseController
    {
		#region Construtores

		public ProgramacaoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.Prop("Codigo");
                grid.Prop("Motorista").Nome("Motorista").Align(Models.Grid.Align.left).Tamanho(15);
                grid.Prop("ProgramacaoAlocacao").Nome("Alocação").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("ProgramacaoEspecialidade").Nome("Especialidade").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("ProgramacaoSituacao").Nome("Situação").Align(Models.Grid.Align.left).Tamanho(10);
                grid.Prop("Cliente").Nome("Cliente").Align(Models.Grid.Align.left).Tamanho(15);
                grid.Prop("Categoria").Nome("Cat. CNH").Align(Models.Grid.Align.left).Tamanho(8);
                grid.Prop("DataVencimentoHabilitacao").Nome("Venc. CNH").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("DataVencimentoMoop").Nome("Venc. Moop").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("DataAdmissao").Nome("Admissão").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("DataInicioFerias").Nome("Inicio Férias").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("DataFimFerias").Nome("Fim Férias").Align(Models.Grid.Align.center).Tamanho(10);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
                int.TryParse(Request.Params("ProgramacaoEspecialidade"), out int codigoProgramacaoEspecialidade);
                int.TryParse(Request.Params("ProgramacaoSituacao"), out int codigoProgramacaoSituacao);
                int.TryParse(Request.Params("ProgramacaoAlocacao"), out int codigoProgramacaoAlocacao);

                bool habilitadoPainel = false;
                bool.TryParse(Request.Params("HabilitarPainel"), out habilitadoPainel);

                int inicio = 0;
                int.TryParse(Request.Params("inicio"), out inicio);
                int limite = 0;
                int.TryParse(Request.Params("limite"), out limite);
                if (inicio <= 0)
                    inicio = grid.inicio;

                if (!habilitadoPainel)
                    limite = limite * 2;
                if (limite <= 0)
                    limite = grid.limite;
                else
                    grid.limite = limite;

                // Consulta
                List<Dominio.Entidades.Usuario> listaGrid = repUsuario.ConsultarProgramacaoMotorista(codigoMotorista, codigoProgramacaoSituacao, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade, propOrdenar, grid.dirOrdena, inicio, limite);
                int totalRegistros = repUsuario.ContarConsultarProgramacaoMotorista(codigoMotorista, codigoProgramacaoSituacao, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade);

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 Motorista = obj.Nome,
                                 ProgramacaoAlocacao = obj.ProgramacaoMotorista != null ? obj.ProgramacaoMotorista?.ProgramacaoAlocacao?.Descricao ?? string.Empty : string.Empty,
                                 ProgramacaoEspecialidade = obj.ProgramacaoMotorista != null ? obj.ProgramacaoMotorista?.ProgramacaoEspecialidade?.Descricao ?? string.Empty : string.Empty,
                                 ProgramacaoSituacao = obj.ProgramacaoMotorista != null ? obj.ProgramacaoMotorista?.ProgramacaoSituacao?.Descricao ?? string.Empty : string.Empty,
                                 Cliente = obj.ProgramacaoMotorista != null ? obj.ProgramacaoMotorista?.Cliente?.Nome ?? string.Empty : string.Empty,
                                 obj.Categoria,
                                 DataVencimentoHabilitacao = obj.DataVencimentoHabilitacao != null && obj.DataVencimentoHabilitacao.HasValue ? obj.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataVencimentoMoop = obj.DataVencimentoMoop != null && obj.DataVencimentoMoop.HasValue ? obj.DataVencimentoMoop.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataAdmissao = obj.DataAdmissao != null && obj.DataAdmissao.HasValue ? obj.DataAdmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataInicioFerias = obj.ProgramacaoMotorista != null && obj.ProgramacaoMotorista.DataInicioFerias != null && obj.ProgramacaoMotorista.DataInicioFerias.HasValue ? obj.ProgramacaoMotorista.DataInicioFerias.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DataFimFerias = obj.ProgramacaoMotorista != null && obj.ProgramacaoMotorista.DataFimFerias != null && obj.ProgramacaoMotorista.DataFimFerias.HasValue ? obj.ProgramacaoMotorista.DataFimFerias.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 DT_RowClass =  obj.ProgramacaoMotorista?.ProgramacaoSituacao != null ? obj.ProgramacaoMotorista.ProgramacaoSituacao.DescricaoCor : string.Empty,
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);

                // Valida
                if (usuario == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    usuario.Codigo,
                    Motorista = new { Codigo = usuario.Codigo, Descricao = usuario.Nome },
                    ProgramacaoAlocacao = usuario.ProgramacaoMotorista != null && usuario.ProgramacaoMotorista.ProgramacaoAlocacao != null ? new { Codigo = usuario.ProgramacaoMotorista.ProgramacaoAlocacao.Codigo, Descricao = usuario.ProgramacaoMotorista.ProgramacaoAlocacao.Descricao } : null,
                    ProgramacaoEspecialidade = usuario.ProgramacaoMotorista != null && usuario.ProgramacaoMotorista.ProgramacaoEspecialidade != null ? new { Codigo = usuario.ProgramacaoMotorista.ProgramacaoEspecialidade.Codigo, Descricao = usuario.ProgramacaoMotorista.ProgramacaoEspecialidade.Descricao } : null,
                    ProgramacaoSituacao = usuario.ProgramacaoMotorista != null && usuario.ProgramacaoMotorista.ProgramacaoSituacao != null ? new { Codigo = usuario.ProgramacaoMotorista.ProgramacaoSituacao.Codigo, Descricao = usuario.ProgramacaoMotorista.ProgramacaoSituacao.Descricao } : null,
                    Cliente = usuario.ProgramacaoMotorista != null && usuario.ProgramacaoMotorista.Cliente != null ? new { Codigo = usuario.ProgramacaoMotorista.Cliente.CPF_CNPJ, Descricao = usuario.ProgramacaoMotorista.Cliente.Descricao } : null,
                    CategoriaCNH = usuario.Categoria,
                    VencimentoCNH = usuario.DataVencimentoHabilitacao != null && usuario.DataVencimentoHabilitacao.HasValue ? usuario.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    VencimentoMoop = usuario.DataVencimentoMoop != null && usuario.DataVencimentoMoop.HasValue ? usuario.DataVencimentoMoop.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataAdmissao = usuario.DataAdmissao != null && usuario.DataAdmissao.HasValue ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataInicioFerias = usuario.ProgramacaoMotorista != null && usuario.ProgramacaoMotorista.DataInicioFerias != null && usuario.ProgramacaoMotorista.DataInicioFerias.HasValue ? usuario.ProgramacaoMotorista.DataInicioFerias.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataFimFerias = usuario.ProgramacaoMotorista != null && usuario.ProgramacaoMotorista.DataFimFerias != null && usuario.ProgramacaoMotorista.DataFimFerias.HasValue ? usuario.ProgramacaoMotorista.DataFimFerias.Value.ToString("dd/MM/yyyy") : string.Empty
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSumarizadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
                int.TryParse(Request.Params("ProgramacaoEspecialidade"), out int codigoProgramacaoEspecialidade);
                int.TryParse(Request.Params("ProgramacaoSituacao"), out int codigoProgramacaoSituacao);
                int.TryParse(Request.Params("ProgramacaoAlocacao"), out int codigoProgramacaoAlocacao);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao>();
                finalidades.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Motorista);
                finalidades.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Todos);

                // Busca informacoes
                int totalRegistros = repUsuario.ContarConsultarProgramacaoMotorista(codigoMotorista, codigoProgramacaoSituacao, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade);
                List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao> situacoesMotorista = repProgramacaoSituacao.BuscarSituacoes(finalidades);
                // Formata retorno
                var retorno = new
                {
                    Status = situacoesMotorista != null ? (from obj in situacoesMotorista
                                                           select new
                                                           {
                                                               Descricao = obj.Descricao,
                                                               Quantidade = repUsuario.ContarConsultarProgramacaoMotorista(codigoMotorista, obj.Codigo, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade).ToString("n0")
                                                           }).ToList() : null

                };
                var todos = new
                {
                    Descricao = "Todos",
                    Quantidade = totalRegistros.ToString("n0")
                };
                retorno.Status.Add(todos);

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados sumarizados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoMotorista repProgramacaoMotorista = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoMotorista(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoEspecialidade repProgramacaoEspecialidade = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoEspecialidade(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoAlocacao repProgramacaoAlocacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoAlocacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo, true);

                // Valida
                if (usuario == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                int codigoProgramacaoAlocacao = 0, codigoMotorista = 0, codigoProgramacaoEspecialidade = 0, codigoProgramacaoSituacao = 0;
                int.TryParse(Request.Params("ProgramacaoAlocacao"), out codigoProgramacaoAlocacao);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("ProgramacaoEspecialidade"), out codigoProgramacaoEspecialidade);
                int.TryParse(Request.Params("ProgramacaoSituacao"), out codigoProgramacaoSituacao);

                double cnpjCliente = 0;
                double.TryParse(Request.Params("Cliente"), out cnpjCliente);

                DateTime? dataInicioFerias = Request.GetNullableDateTimeParam("DataInicioFerias");
                DateTime? dataFimFerias = Request.GetNullableDateTimeParam("DataFimFerias");

                bool inserir = true;
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista programacao = null;
                if (usuario.ProgramacaoMotorista == null)
                    programacao = new Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista();
                else
                {
                    inserir = false;
                    programacao = repProgramacaoMotorista.BuscarPorCodigo(usuario.ProgramacaoMotorista.Codigo, true);
                }
                unitOfWork.Start();

                programacao.Cliente = cnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
                programacao.DataInicioFerias = dataInicioFerias.HasValue ? dataInicioFerias : null;
                programacao.DataFimFerias = dataFimFerias.HasValue ? dataFimFerias : null;
                programacao.Empresa = this.Usuario.Empresa;
                programacao.ProgramacaoAlocacao = codigoProgramacaoAlocacao > 0 ? repProgramacaoAlocacao.BuscarPorCodigo(codigoProgramacaoAlocacao) : null;
                programacao.ProgramacaoEspecialidade = codigoProgramacaoEspecialidade > 0 ? repProgramacaoEspecialidade.BuscarPorCodigo(codigoProgramacaoEspecialidade) : null;
                programacao.ProgramacaoSituacao = codigoProgramacaoSituacao > 0 ? repProgramacaoSituacao.BuscarPorCodigo(codigoProgramacaoSituacao) : null;

                if (inserir)
                    repProgramacaoMotorista.Inserir(programacao, Auditado);
                else
                    repProgramacaoMotorista.Atualizar(programacao, Auditado);

                usuario.ProgramacaoMotorista = programacao;
                repUsuario.Atualizar(usuario, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados
        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdenar == "Motorista") propOrdenar = "Nome";
            else if (propOrdenar == "ProgramacaoAlocacao") propOrdenar = "ProgramacaoMotorista.ProgramacaoAlocacao.Descricao";
            else if (propOrdenar == "ProgramacaoEspecialidade") propOrdenar = "ProgramacaoMotorista.ProgramacaoEspecialidade.Descricao";
            else if (propOrdenar == "ProgramacaoSituacao") propOrdenar = "ProgramacaoMotorista.ProgramacaoSituacao.Descricao";
            else if (propOrdenar == "Cliente") propOrdenar = "ProgramacaoMotorista.Cliente.Nome";
            else if (propOrdenar == "DataInicioFerias") propOrdenar = "ProgramacaoMotorista.DataInicioFerias";
            else if (propOrdenar == "DataFimFerias") propOrdenar = "ProgramacaoMotorista.DataFimFerias";
        }
        #endregion
    }
}
