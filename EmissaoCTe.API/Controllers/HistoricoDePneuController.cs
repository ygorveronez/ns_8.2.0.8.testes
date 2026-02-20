using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class HistoricoDePneuController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("historicodepneus.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string seriePneu = Request.Params["SeriePneu"];
                string placaVeiculo = Request.Params["PlacaVeiculo"];

                Repositorio.HistoricoPneu repHistoricoPneu = new Repositorio.HistoricoPneu(unitOfWork);
                var listaHistoricoPneu = repHistoricoPneu.Consultar(this.EmpresaUsuario.Codigo, seriePneu, placaVeiculo, inicioRegistros, 50);
                int countHistoricoPneu = repHistoricoPneu.ContarConsulta(this.EmpresaUsuario.Codigo, seriePneu, placaVeiculo);

                var retorno = from obj in listaHistoricoPneu
                              select new
                              {
                                  obj.Codigo,
                                  Data = obj.Data.HasValue ? obj.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  obj.DescricaoTipo,
                                  Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                  Eixo = obj.Eixo != null ? obj.Eixo.Descricao : string.Empty,
                                  Pneu = obj.Pneu.Serie
                              };

                return Json(retorno, true, null, new string[] { "Código", "Data|12", "Tipo|12", "Veículo|12", "Eixo|34", "Pneu|20" }, countHistoricoPneu);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os históricos de pneus.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Repositorio.HistoricoPneu repHistoricoPneu = new Repositorio.HistoricoPneu(unitOfWork);
                Dominio.Entidades.HistoricoPneu historico = repHistoricoPneu.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                if (historico != null)
                {
                    var retorno = new
                    {
                        Calibragem = historico.Calibragem.ToString("n0"),
                        historico.Codigo,
                        Data = historico.Data.HasValue ? historico.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                        CodigoEixo = historico.Eixo != null ? historico.Eixo.Codigo : 0,
                        DescricaoEixo = historico.Eixo != null ? historico.Eixo.Descricao : string.Empty,
                        Kilometragem = historico.Kilometragem.ToString("n0"),
                        historico.Observacao,
                        CodigoPneu = historico.Pneu.Codigo,
                        DescricaoPneu = string.Concat(historico.Pneu.Serie, " - ", historico.Pneu.ModeloPneu.Descricao),
                        historico.Tipo,
                        CodigoVeiculo = historico.Veiculo != null ? historico.Veiculo.Codigo : 0,
                        DescricaoVeiculo = historico.Veiculo != null ? historico.Veiculo.Placa : string.Empty
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Histórico não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar o histórico de pneu.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                unidadeTrabalho.Start();
                int codigo, codigoPneu, codigoVeiculo, codigoEixo, codigoStatusPneu, km, calibragem = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoPneu"], out codigoPneu);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoEixo"], out codigoEixo);
                int.TryParse(Request.Params["CodigoStatusPneu"], out codigoStatusPneu);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["KM"]), out km);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Calibragem"]), out calibragem);
                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);
                string tipo = Request.Params["Tipo"];
                string observacao = Request.Params["Observacao"];
                if (string.IsNullOrWhiteSpace(tipo))
                    return Json<bool>(false, false, "Tipo inválido.");

                Repositorio.HistoricoPneu repHistoricoPneu = new Repositorio.HistoricoPneu(unidadeTrabalho);
                Repositorio.EixoVeiculo repEixoVeiculo = new Repositorio.EixoVeiculo(unidadeTrabalho);
                Repositorio.Pneu repPneu = new Repositorio.Pneu(unidadeTrabalho);
                Repositorio.StatusPneu repStatusPneu = new Repositorio.StatusPneu(unidadeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
                Dominio.Entidades.HistoricoPneu historicoPneu;

                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    {
                        unidadeTrabalho.Rollback();
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    }
                    historicoPneu = new Dominio.Entidades.HistoricoPneu();
                }
                else
                {
                    return Json<bool>(false, false, "Não é possível editar um histórico de pneu.");
                }

                historicoPneu.Calibragem = calibragem;
                historicoPneu.Data = data;
                if (codigoEixo > 0)
                {
                    historicoPneu.Eixo = repEixoVeiculo.BuscarPorCodigo(codigoEixo, this.EmpresaUsuario.Codigo);
                    if (historicoPneu.Eixo.Tipo != "E")
                        historicoPneu.Kilometragem = km;
                    else
                        historicoPneu.Kilometragem = 0;
                }
                else
                    historicoPneu.Kilometragem = km;
                historicoPneu.Observacao = observacao;
                historicoPneu.Pneu = repPneu.BuscarPorCodigo(codigoPneu, this.EmpresaUsuario.Codigo);
                historicoPneu.Tipo = tipo;
                if (codigoVeiculo != 0)
                    historicoPneu.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                if (historicoPneu.Tipo.Equals("E"))
                {
                    if (historicoPneu.Pneu != null && historicoPneu.Pneu.StatusPneu != null && (historicoPneu.Pneu.StatusPneu.Tipo.Equals("A") || historicoPneu.Pneu.StatusPneu.Tipo.Equals("S")))
                    {
                        historicoPneu.Pneu.StatusPneu = repStatusPneu.BuscarPorCodigo(codigoStatusPneu, this.EmpresaUsuario.Codigo);
                        if (historicoPneu.Pneu.StatusPneu == null || (!historicoPneu.Pneu.StatusPneu.Tipo.Equals("E") && !historicoPneu.Pneu.StatusPneu.Tipo.Equals("A")))
                        {
                            unidadeTrabalho.Rollback();
                            return Json<bool>(false, false, "Para registrar um historico de entrada, o status do pneu selecionado deve ser do tipo 'Entrada'.");
                        }
                        else
                        {
                            if (historicoPneu.Pneu.Veiculo != null)
                            {
                                unidadeTrabalho.Rollback();
                                return Json<bool>(false, false, "O pneu selecionado já pertence a um veículo. Não é possível dar entrada novamente.");
                            }
                            if (historicoPneu.Pneu.Eixo != null)
                            {
                                unidadeTrabalho.Rollback();
                                return Json<bool>(false, false, "O pneu selecionado já pertence a um eixo. Não é possível dar entrada novamente.");
                            }
                            if (!historicoPneu.Pneu.StatusPneu.Tipo.Equals("A"))
                            {
                                historicoPneu.Pneu.Veiculo = historicoPneu.Veiculo;
                                historicoPneu.Pneu.Eixo = historicoPneu.Eixo;
                            }
                            repPneu.Atualizar(historicoPneu.Pneu);
                        }
                    }
                    else
                    {
                        unidadeTrabalho.Rollback();
                        return Json<bool>(false, false, "Para registrar um histórico de entrada, o status atual do pneu deve ser do tipo 'Saída'.");
                    }
                }
                else if (historicoPneu.Tipo.Equals("S"))
                {
                    if (historicoPneu.Pneu != null && historicoPneu.Pneu.StatusPneu != null && historicoPneu.Pneu.StatusPneu.Tipo.Equals("E"))
                    {
                        historicoPneu.Pneu.StatusPneu = repStatusPneu.BuscarPorCodigo(codigoStatusPneu, this.EmpresaUsuario.Codigo);
                        if (historicoPneu.Pneu.StatusPneu == null || !historicoPneu.Pneu.StatusPneu.Tipo.Equals("A"))
                        {
                            unidadeTrabalho.Rollback();
                            return Json<bool>(false, false, "Para registrar um historico de saída, o status do pneu selecionado deve ser do tipo 'Saída'.");
                        }
                        else
                        {
                            if (historicoPneu.Veiculo.Codigo != historicoPneu.Pneu.Veiculo.Codigo)
                            {
                                unidadeTrabalho.Rollback();
                                return Json<bool>(false, false, "O veículo atual do pneu e o veículo selecionado não coincidem.");
                            }
                            if (historicoPneu.Eixo.Codigo != historicoPneu.Pneu.Eixo.Codigo)
                            {
                                unidadeTrabalho.Rollback();
                                return Json<bool>(false, false, "O eixo atual do pneu e o eixo selecionado não coincidem.");
                            }
                            historicoPneu.Pneu.Veiculo = null;
                            historicoPneu.Pneu.Eixo = null;
                            repPneu.Atualizar(historicoPneu.Pneu);
                        }
                    }
                    else
                    {
                        unidadeTrabalho.Rollback();
                        return Json<bool>(false, false, "Para registrar um histórico de saída, o status atual do pneu deve ser do tipo 'Entrada'.");
                    }
                }
                else if (historicoPneu.Tipo.Equals("R"))
                {
                    if (historicoPneu.Pneu != null && historicoPneu.Pneu.StatusPneu != null && historicoPneu.Pneu.StatusPneu.Tipo.Equals("E"))
                    {
                        historicoPneu.Pneu.StatusPneu = repStatusPneu.BuscarPorCodigo(codigoStatusPneu, this.EmpresaUsuario.Codigo);
                        if (historicoPneu.Pneu.StatusPneu == null || !historicoPneu.Pneu.StatusPneu.Tipo.Equals("S"))
                        {
                            unidadeTrabalho.Rollback();
                            return Json<bool>(false, false, "Para registrar um historico de rodízio, o status do pneu selecionado deve ser do tipo 'Serviço'.");
                        }
                        else
                        {
                            repPneu.Atualizar(historicoPneu.Pneu);
                        }
                    }
                    else
                    {
                        unidadeTrabalho.Rollback();
                        return Json<bool>(false, false, "Para registrar um histórico de rodízio, o status atual do pneu deve ser do tipo 'Entrada'.");
                    }
                }
                else
                {
                    unidadeTrabalho.Rollback();
                    return Json<bool>(false, false, "O tipo do histórico é inválido.");
                }

                if (codigo > 0)
                    repHistoricoPneu.Atualizar(historicoPneu);
                else
                    repHistoricoPneu.Inserir(historicoPneu);

                unidadeTrabalho.CommitChanges();
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o histórico do pneu.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
