using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize(new string[] { "BuscarRotaPorCargaParaPasssagensMDFe" }, "Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio")]
    public class CargaLocaisPrestacaoController : BaseController
    {
		#region Construtores

		public CargaLocaisPrestacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarRotaPorCargaParaPasssagensMDFeAsync(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codCarga = int.Parse(Request.Params("Carga"));


                Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork, cancellationToken);

                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork, cancellationToken);
                Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);




                List<Dominio.Entidades.Localidade> rotaLocalidades = new List<Dominio.Entidades.Localidade>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();

                Dominio.Entidades.Localidade fronteira = null;

                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = await repCargaLocaisPrestacao.BuscarPorCargaAsync(codCarga);
                if (cargaLocaisPrestacao.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursos = await repCargaPercurso.ConsultarPorCargaAsync(codCarga);

                    if (cargaPercursos.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoOrdenado = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in cargaPercursos)
                        {
                            rotaLocalidades.Add(cargaPercurso.Origem);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisParaOrdenacao = null;
                            if (cargaPercurso.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.carregamento)
                                cargaLocaisParaOrdenacao = (from obj in cargaLocaisPrestacao where obj.LocalidadeInicioPrestacao.Codigo == cargaPercurso.Origem.Codigo select obj).ToList();
                            else
                                cargaLocaisParaOrdenacao = (from obj in cargaLocaisPrestacao where obj.LocalidadeTerminoPrestacao.Codigo == cargaPercurso.Origem.Codigo select obj).ToList();

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalParaOrdenacao in cargaLocaisParaOrdenacao)
                            {
                                if (!cargaLocaisPrestacaoOrdenado.Contains(cargaLocalParaOrdenacao))
                                    cargaLocaisPrestacaoOrdenado.Add(cargaLocalParaOrdenacao);
                            }
                        }

                        rotaLocalidades.Add(cargaPercursos[cargaPercursos.Count - 1].Destino);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaUltimoLocaisParaOrdenacao = (from obj in cargaLocaisPrestacao where obj.LocalidadeTerminoPrestacao.Codigo == cargaPercursos[cargaPercursos.Count - 1].Destino.Codigo select obj).ToList();
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalParaOrdenacao in cargaUltimoLocaisParaOrdenacao)
                        {
                            if (!cargaLocaisPrestacaoOrdenado.Contains(cargaLocalParaOrdenacao))
                                cargaLocaisPrestacaoOrdenado.Add(cargaLocalParaOrdenacao);
                        }

                        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocaisPrestacaoPassagens = repCargaLocaisPrestacaoPassagens.BuscarPorLocaisPrestacao((from obj in cargaLocaisPrestacao select obj.Codigo).ToList());

                        var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
                        var fronteiras = serCargaFronteira.ObterFronteirasPorCargas((from o in cargaLocaisPrestacao select o.Carga).ToList());

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao in cargaLocaisPrestacao)
                        {
                            if ((cargaLocalPrestacao.LocalidadeTerminoPrestacao.CodigoIBGE == 9999999 || cargaLocalPrestacao.LocalidadeTerminoPrestacao.Estado?.Sigla == "EX")
                                && cargaLocalPrestacao.LocalidadeFronteira == null
                                && serCargaFronteira.TemFronteira(cargaLocalPrestacao.Carga, fronteiras)
                            )
                            {
                                cargaLocalPrestacao.LocalidadeFronteira = serCargaFronteira.ObterFronteiraPrincipal(cargaLocalPrestacao.Carga, fronteiras)?.Fronteira.Localidade;
                            }

                            fronteira = cargaLocalPrestacao.LocalidadeFronteira;

                            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocalPrestacaoPassagens = (from obj in cargaLocaisPrestacaoPassagens where obj.CargaLocaisPrestacao.Codigo == cargaLocalPrestacao.Codigo select obj).ToList();

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPrestacaoPassagem in cargaLocalPrestacaoPassagens)
                            {
                                if (!passagens.Exists(obj => obj.Posicao == cargaLocalPrestacaoPassagem.Posicao))
                                {
                                    passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = cargaLocalPrestacaoPassagem.EstadoDePassagem.Sigla, Posicao = cargaLocalPrestacaoPassagem.Posicao });
                                }
                            }
                        }

                        List<Dominio.Entidades.Localidade> localidadesMDFe = serCargaLocaisPrestacao.BuscarRotasParaMDFe(rotaLocalidades, fronteira);
                        List<string> ufsDestinos = (from obj in cargaLocaisPrestacao select obj.LocalidadeTerminoPrestacao.Estado.Sigla).Distinct().ToList();
                        if (passagens.Count == 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                            Dominio.Entidades.PercursoEstado percursoEstado = repPercursoEstado.BuscarPorOrigemEDestino(carga.Empresa != null ? carga.Empresa.Codigo : this.Empresa?.Codigo ?? 0, localidadesMDFe.First().Estado.Sigla, ufsDestinos);

                            if (percursoEstado == null)
                                percursoEstado = repPercursoEstado.BuscarPorOrigemEDestino(this.Empresa?.Codigo ?? 0, localidadesMDFe.First().Estado.Sigla, ufsDestinos);

                            if (percursoEstado != null)
                            {
                                List<Dominio.Entidades.PassagemPercursoEstado> passagensPercursoEstado = repPassagemPercursoEstado.Buscar(percursoEstado.Codigo);
                                foreach (Dominio.Entidades.PassagemPercursoEstado passagem in passagensPercursoEstado)
                                {
                                    passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = passagem.EstadoDePassagem.Sigla, Posicao = passagem.Ordem });
                                }
                            }
                        }
                        passagens = passagens.OrderBy(obj => obj.Posicao).ToList();

                        #region "Organiza priorizando o inicio da carga"


                        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoInicio = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
                        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoOutros = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao in cargaLocaisPrestacaoOrdenado)
                        {
                            if (rotaLocalidades.First().Codigo == cargaLocalPrestacao.LocalidadeInicioPrestacao.Codigo)
                                cargaLocaisPrestacaoInicio.Add(cargaLocalPrestacao);
                            else
                                cargaLocaisPrestacaoOutros.Add(cargaLocalPrestacao);
                        }
                        List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacaoOrdenados = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
                        cargaLocaisPrestacaoOrdenados.AddRange(cargaLocaisPrestacaoInicio);
                        cargaLocaisPrestacaoOrdenados.AddRange(cargaLocaisPrestacaoOutros);

                        #endregion

                        var retorno = new
                        {
                            EstadoOrigem = localidadesMDFe.First().Estado.Sigla,
                            EstadoDestino = localidadesMDFe.Last().Estado.Sigla,
                            EstadosOrigensDestinos = (from obj in cargaLocaisPrestacaoOrdenados
                                                      select new
                                                      {
                                                          obj.Codigo,
                                                          Origem = buscarLocalidadeMDFe(obj.LocalidadeInicioPrestacao, obj.LocalidadeFronteira),
                                                          Destino = buscarLocalidadeMDFe(obj.LocalidadeTerminoPrestacao, obj.LocalidadeFronteira)
                                                      }).ToList(),
                            passagens = passagens,
                            rotasParaMDFe = (from obj in localidadesMDFe
                                             select new
                                             {
                                                 Estado = obj.Estado.Sigla,
                                                 Cidade = obj.Descricao,
                                                 IBGE = obj.CodigoIBGE
                                             }).ToList()
                        };

                        return new JsonpResult(retorno);
                    }
                    else//quando não consegue criar as rotas retorna com Exterior para não preencher o mapa do MDF-e até que a rota seja ajustada.
                    {
                        var retorno = new
                        {
                            EstadoOrigem = "EX",
                            EstadoDestino = "EX",
                        };
                        return new JsonpResult(retorno);
                    }
                }
                else
                {
                    return new JsonpResult(false, true, "Não foi possível localizar os locais de prestação desta carga.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o percurso para passagens entre estados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarPercursoPassagemMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarPassagens) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();
                int codCarga = int.Parse(Request.Params("Carga"));

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);

                //serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);

                if (!string.IsNullOrWhiteSpace(retornoVerificarOperador))
                    throw new ControllerException(retornoVerificarOperador);

                dynamic dynCargaPercursos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CargaPercursos"));
                foreach (var dynCargaPercurso in dynCargaPercursos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao cargaLocalPrestacao = repCargaLocaisPrestacao.BuscarPorCodigo((int)dynCargaPercurso.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocaisPrestacaoPassagens = repCargaLocaisPrestacaoPassagens.BuscarPorLocalPrestacao(cargaLocalPrestacao.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPrestacaoPassagem in cargaLocaisPrestacaoPassagens)
                        repCargaLocaisPrestacaoPassagens.Deletar(cargaLocalPrestacaoPassagem);

                    foreach (var dynPassagem in dynCargaPercurso.Passagens)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPrestacaoPassagem = new Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens();
                        cargaLocalPrestacaoPassagem.CargaLocaisPrestacao = cargaLocalPrestacao;
                        cargaLocalPrestacaoPassagem.EstadoDePassagem = new Dominio.Entidades.Estado() { Sigla = (string)dynPassagem.Sigla };
                        cargaLocalPrestacaoPassagem.Posicao = (int)dynPassagem.Posicao;
                        repCargaLocaisPrestacaoPassagens.Inserir(cargaLocalPrestacaoPassagem);
                    }

                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Alterou o percurso do MDF-e", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar as passagens entre os estados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private dynamic buscarLocalidadeMDFe(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Localidade fronteira)
        {
            if (localidade != null)
            {
                if (localidade.CodigoIBGE == 9999999 || localidade.Estado?.Sigla == "EX")
                    localidade = fronteira != null ? fronteira : localidade;

                var retorno = new
                {
                    Estado = localidade.Estado.Sigla,
                    Cidade = localidade.Descricao
                };
                return retorno;
            }
            else
                return null;
        }
    }
}
