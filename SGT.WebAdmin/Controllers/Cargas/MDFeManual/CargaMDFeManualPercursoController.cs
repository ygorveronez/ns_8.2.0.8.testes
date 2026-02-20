using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MDFeManual
{
    [CustomAuthorize("Cargas/CargaMDFeManual", "Cargas/CargaMDFeAquaviarioManual")]
    public class CargaMDFeManualPercursoController : BaseController
    {
		#region Construtores

		public CargaMDFeManualPercursoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarRotaParaMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrigem, codigoDestino;
                int.TryParse(Request.Params("Origem"), out codigoOrigem);

                bool usaDadosCTe = false;
                bool.TryParse(Request.Params("UsaDadosCTe"), out usaDadosCTe);

                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unidadeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
                Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unidadeTrabalho);

                List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();

                List<Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao> ordemDestinos = new List<Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao>();
                if (usaDadosCTe)
                {
                    var dynDestinos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinos"));

                    foreach (var dynDestino in dynDestinos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao ordemDestino = new Dominio.ObjetosDeValor.Embarcador.MDFeManual.MDFeManualDestinoPosicao();
                        ordemDestino.destino = repLocalidade.BuscarPorCodigo((int)dynDestino.Codigo);
                        ordemDestino.Posicao = (int)dynDestino.Posicao;
                        ordemDestinos.Add(ordemDestino);
                    }

                    ordemDestinos = ordemDestinos.OrderBy(obj => obj.Posicao).ToList();
                    foreach (var ordemDestino in ordemDestinos)
                        destinos.Add(ordemDestino.destino);
                }
                else
                {
                    int.TryParse(Request.Params("Destino"), out codigoDestino);
                    Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigo(codigoDestino);
                    destinos.Add(destino);
                }

                Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigo(codigoOrigem);


                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();

                if (origem == null || destinos.Count == 0)
                    return new JsonpResult(false, true, "Origem/Destino inválidos para obter o percurso.");

                Dominio.Entidades.PercursoEstado percursoEstado = repPercursoEstado.BuscarPorOrigemEDestino(0, origem.Estado.Sigla, (from obj in destinos select obj.Estado.Sigla).Distinct().ToList());

                List<Dominio.Entidades.Localidade> localidadesMDFe = new List<Dominio.Entidades.Localidade>();
                localidadesMDFe.Add(origem);

                foreach (Dominio.Entidades.Localidade destino in destinos)
                    localidadesMDFe.Add(destino);

                if (percursoEstado != null)
                {
                    List<Dominio.Entidades.PassagemPercursoEstado> passagensPercursoEstado = repPassagemPercursoEstado.Buscar(percursoEstado.Codigo);

                    passagens = (from passagem in passagensPercursoEstado orderby passagem.Ordem select new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = passagem.EstadoDePassagem.Sigla, Posicao = passagem.Ordem }).ToList();
                }

                var retorno = new
                {
                    EstadoOrigem = origem.Estado.Sigla,
                    EstadoDestino = destinos.LastOrDefault().Estado.Sigla,
                    EstadosOrigensDestinos = (from obj in destinos
                                              select new
                                              {
                                                  Codigo = 0,
                                                  Origem = new { Estado = origem.Estado.Sigla, Cidade = origem.Descricao },
                                                  Destino = new { Estado = obj.Estado.Sigla, Cidade = obj.Descricao }
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
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o percurso para o MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

    }
}

