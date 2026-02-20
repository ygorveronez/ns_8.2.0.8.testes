using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/PercursosEntreEstados")]
    public class PercursosEntreEstadosController : BaseController
    {
		#region Construtores

		public PercursosEntreEstadosController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                //Servicos.Embarcador.Logistica.PassagemEntreEstados serPassagemEntreEstados = new Servicos.Embarcador.Logistica.PassagemEntreEstados(_conexao.StringConexao);
                //serPassagemEntreEstados.AdicionarPassagensEntreTodosOsEstadosPadrao(this.Empresa, unitOfWork);

                string EstadoOrigem = Request.Params("EstadoOrigem");
                string EstadoDestino = Request.Params("EstadoDestino");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);

                grid.AdicionarCabecalho("Estado de Origem", "EstadoOrigem", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Estado de Destino", "EstadoDestino", 30, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Empresa", 30, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 30, Models.Grid.Align.left, true);


                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "EstadoOrigem" || propOrdenar == "EstadoDestino")
                    propOrdenar += ".Nome";
                else if (propOrdenar == "Empresa")
                    propOrdenar += ".RazaoSocial";


                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
                List<Dominio.Entidades.PercursoEstado> listaPercursoEstado = repPercursoEstado.Consultar(codigoEmpresa, EstadoOrigem, EstadoDestino, true, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPercursoEstado.ContarConsulta(codigoEmpresa, EstadoOrigem, EstadoDestino, true);

                grid.recordsTotal = totalRegistros;
                grid.recordsFiltered = totalRegistros;

                dynamic lista = (from p in listaPercursoEstado
                                 select new
                                 {
                                     Codigo = p.Codigo,
                                     EstadoOrigem = p.EstadoOrigem.Nome,
                                     EstadoDestino = p.DescricaoDestino,
                                     Empresa = this.Empresa.Codigo == p.Empresa.Codigo ? "" : p.Empresa.Descricao,
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
                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
                Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                if (codigoEmpresa == 0)
                    codigoEmpresa = this.Empresa.Codigo;

                Dominio.Entidades.PercursoEstado percursoEstado = new Dominio.Entidades.PercursoEstado
                {
                    EstadoOrigem = repEstado.BuscarPorSigla(Request.Params("EstadoOrigem")),
                    Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa)
                };
                SetarEstadosDestino(ref percursoEstado, unitOfWork);
                Dominio.Entidades.PercursoEstado percursoEstadoExistente = repPercursoEstado.Buscar(percursoEstado.Empresa.Codigo, percursoEstado.EstadoOrigem.Sigla, (from obj in percursoEstado.EstadosDestino select obj.Sigla).Distinct().ToList());
                percursoEstado.DescricaoDestino = Utilidades.String.Left(string.Join(", ", (from obj in percursoEstado.EstadosDestino select obj.Nome.Trim()).ToList()), 500);
                percursoEstado.EstadoDestino = percursoEstado.EstadosDestino.LastOrDefault();
                if (percursoEstadoExistente == null)
                {
                    repPercursoEstado.Inserir(percursoEstado, Auditado);
                    dynamic passagensPercursoEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PassagemPercursoEstado"));
                    foreach (var dynPassagemPercursoEstado in passagensPercursoEstado)
                    {
                        Dominio.Entidades.PassagemPercursoEstado passagem = new Dominio.Entidades.PassagemPercursoEstado();
                        passagem.Ordem = (int)dynPassagemPercursoEstado.Ordem;
                        passagem.Percurso = percursoEstado;
                        passagem.EstadoDePassagem = new Dominio.Entidades.Estado() { Sigla = (string)dynPassagemPercursoEstado.EstadoDePassagem };
                        repPassagemPercursoEstado.Inserir(passagem);
                    }

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe um percurso entre " + percursoEstadoExistente.EstadoOrigem.Nome + " e " + percursoEstado.DescricaoDestino);
                }

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

        private void SetarEstadosDestino(ref Dominio.Entidades.PercursoEstado percursoEstado, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeTrabalho);

            dynamic estadosDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosDestino"));

            percursoEstado.EstadosDestino = new List<Dominio.Entidades.Estado>();

            foreach (var estadoDestino in estadosDestino)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)estadoDestino.Tipo.Codigo);
                percursoEstado.EstadosDestino.Add(estado);
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
                Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                if (codigoEmpresa == 0)
                    codigoEmpresa = this.Empresa.Codigo;


                Dominio.Entidades.PercursoEstado percursoEstado = repPercursoEstado.BuscarPorCodigo(codigo);
                percursoEstado.Initialize();
                percursoEstado.EstadoOrigem = repEstado.BuscarPorSigla(Request.Params("EstadoOrigem"));
                SetarEstadosDestino(ref percursoEstado, unitOfWork);

                if (codigoEmpresa > 0)
                    percursoEstado.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                else
                    percursoEstado.Empresa = this.Empresa;

                percursoEstado.DescricaoDestino = Utilidades.String.Left(string.Join(", ", (from obj in percursoEstado.EstadosDestino select obj.Nome.Trim()).ToList()), 500);

                Dominio.Entidades.PercursoEstado percursoEstadoExistente = repPercursoEstado.Buscar(percursoEstado.Empresa.Codigo, percursoEstado.EstadoOrigem.Sigla, (from obj in percursoEstado.EstadosDestino select obj.Sigla).Distinct().ToList());

                if (percursoEstadoExistente == null || percursoEstadoExistente.Codigo == percursoEstado.Codigo)
                {
                    percursoEstado.EstadoDestino = percursoEstado.EstadosDestino.LastOrDefault();
                    repPercursoEstado.Atualizar(percursoEstado, Auditado);
                    dynamic passagensPercursoEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PassagemPercursoEstado"));

                    List<Dominio.Entidades.PassagemPercursoEstado> passagensAtivos = new List<Dominio.Entidades.PassagemPercursoEstado>();
                    foreach (var dynPassagemPercursoEstado in passagensPercursoEstado)
                    {
                        Dominio.Entidades.PassagemPercursoEstado passagem = repPassagemPercursoEstado.Buscar(percursoEstado.Empresa.Codigo, (int)dynPassagemPercursoEstado.Codigo);
                        if (passagem == null)
                        {
                            passagem = new Dominio.Entidades.PassagemPercursoEstado
                            {
                                Ordem = (int)dynPassagemPercursoEstado.Ordem,
                                Percurso = percursoEstado,
                                EstadoDePassagem = repEstado.BuscarPorSigla((string)dynPassagemPercursoEstado.EstadoDePassagem)
                            };
                            repPassagemPercursoEstado.Inserir(passagem);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, percursoEstado, null, "Adicionou Passagem por " + passagem.EstadoDePassagem.Nome + ".", unitOfWork);
                        }
                        else
                        {
                            passagem.Ordem = (int)dynPassagemPercursoEstado.Ordem;
                            passagem.Percurso = percursoEstado;
                            passagem.EstadoDePassagem = new Dominio.Entidades.Estado() { Sigla = (string)dynPassagemPercursoEstado.EstadoDePassagem };
                            repPassagemPercursoEstado.Atualizar(passagem);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, percursoEstado, null, "Alterou a Passagem por " + passagem.EstadoDePassagem.Nome + ".", unitOfWork);
                        }
                        passagensAtivos.Add(passagem);
                    }

                    List<Dominio.Entidades.PassagemPercursoEstado> passagensPercursosEstadosSalvosNoBanco = repPassagemPercursoEstado.Buscar(percursoEstado.Codigo);
                    foreach (Dominio.Entidades.PassagemPercursoEstado percursoSalvoNoBanco in passagensPercursosEstadosSalvosNoBanco)
                    {
                        if (!passagensAtivos.Exists(obj => obj.Codigo == percursoSalvoNoBanco.Codigo))
                        {
                            repPassagemPercursoEstado.Deletar(percursoSalvoNoBanco);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, percursoEstado, null, "Removeu a Passagem por " + percursoSalvoNoBanco.EstadoDePassagem.Nome + ".", unitOfWork);
                        }
                    }
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe um percurso entre " + percursoEstadoExistente.EstadoOrigem.Nome + " e " + percursoEstado.DescricaoDestino);
                }
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
                Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork);

                Dominio.Entidades.PercursoEstado percursoEstado = repPercursoEstado.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.PassagemPercursoEstado> passagensPercursoEstado = repPassagemPercursoEstado.Buscar(codigo);

                List<Dominio.Entidades.Estado> destinos = percursoEstado.EstadosDestino.ToList();
                destinos.Remove(percursoEstado.EstadoDestino);
                destinos.Add(percursoEstado.EstadoDestino);

                var entidade = new
                {
                    percursoEstado.Codigo,
                    EstadoOrigem = percursoEstado.EstadoOrigem.Sigla,
                    Empresa = this.Empresa.Codigo == percursoEstado.Empresa.Codigo ? new { Codigo = 0, Descricao = "" } : new { Codigo = percursoEstado.Empresa.Codigo, Descricao = percursoEstado.Empresa.Descricao },
                    PassagemPercursoEstado = (from obj in passagensPercursoEstado
                                              select new
                                              {
                                                  obj.Codigo,
                                                  EstadoDePassagem = obj.EstadoDePassagem.Sigla, //new { Sigla = obj.EstadoDePassagem.Sigla, Descricao = obj.EstadoDePassagem.Sigla },
                                                  obj.Ordem
                                              }).ToList(),
                    EstadosDestino = (from obj in destinos
                                      select new
                                      {
                                          Codigo = obj.Sigla,
                                          Descricao = obj.Nome
                                      }).ToList(),
                };

                return new JsonpResult(entidade);
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
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
                Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork);

                Dominio.Entidades.PercursoEstado percursoEstado = repPercursoEstado.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.PassagemPercursoEstado> passagensPercursoEstado = repPassagemPercursoEstado.Buscar(codigo);
                foreach (Dominio.Entidades.PassagemPercursoEstado passagem in passagensPercursoEstado)
                {
                    repPassagemPercursoEstado.Deletar(passagem);
                }

                repPercursoEstado.Deletar(percursoEstado, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #endregion

    }
}
