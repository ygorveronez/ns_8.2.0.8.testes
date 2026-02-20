using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize(new string[] { "BuscarRotaPorCarga" }, "Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio")]
    public class CargaPercursoController : BaseController
    {
        #region Construtores

        public CargaPercursoController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> BuscarRotaPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codCarga = int.Parse(Request.Params("Carga"));


                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> cargaPercursos = repCargaPercurso.ConsultarPorCarga(carga.Codigo);

                var lista = (from obj in cargaPercursos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Posicao,
                                 TipoRota = obj.TipoRota,
                                 obj.DescricaoTipoRota,
                                 obj.DistanciaKM,
                                 Origem = new { obj.Origem.Codigo, Descricao = obj.Origem.DescricaoCidadeEstado },
                                 Destino = new { obj.Destino.Codigo, Descricao = obj.Destino.DescricaoCidadeEstado }
                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarPercursoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarPercurso) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();
                int codCarga = int.Parse(Request.Params("Carga"));


                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                bool podeCalcular = carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete || (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador);

                if (!podeCalcular)
                    throw new ControllerException("Não é possível informar o frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
                {
                    serCargaRota.DeletarPercursoDestinosCarga(carga, unitOfWork);

                    dynamic dynCargaPercursos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CargaPercursos"));

                    bool todasAsRotas = true;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> novaCargaPercursos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso>();
                    foreach (var dynCargaPercurso in dynCargaPercursos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPercurso cp = new Dominio.Entidades.Embarcador.Cargas.CargaPercurso();
                        cp.Carga = carga;
                        cp.Origem = repLocalidade.BuscarPorCodigo((int)dynCargaPercurso.Origem.Codigo);
                        cp.Destino = repLocalidade.BuscarPorCodigo((int)dynCargaPercurso.Destino.Codigo);
                        cp.Posicao = (int)dynCargaPercurso.Posicao;
                        cp.TipoRota = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota)dynCargaPercurso.TipoRota;
                        novaCargaPercursos.Add(cp);
                    }

                    novaCargaPercursos = novaCargaPercursos.OrderBy(obj => obj.Posicao).ToList();

                    if (novaCargaPercursos[novaCargaPercursos.Count - 1].TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento)
                    {

                        Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercursoAnterior = null;

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cp in novaCargaPercursos)
                        {

                            if (cargaPercursoAnterior == null)
                            {
                                Dominio.Entidades.Localidade localidadeOrigem = null;
                                if (cp.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRota.descarregamento)
                                    localidadeOrigem = serCargaRota.BuscarLocalidadeOrigem(repCargaPedido.BuscarPorCarga(carga.Codigo).FirstOrDefault()); //todo:temporario até todas as cargas percursos terem a informação da carga Pedido
                                else
                                    localidadeOrigem = serCargaRota.BuscarLocalidadeOrigem((from obj in repCargaPedido.BuscarPorCarga(carga.Codigo) where obj.InicioDaCarga select obj).FirstOrDefault());

                                cargaPercursoAnterior = adicionarCargaPercurso(cp, localidadeOrigem, cp.Destino, unitOfWork);

                                if (cargaPercursoAnterior == null)
                                {
                                    todasAsRotas = false;
                                    break;
                                }

                            }
                            else
                            {
                                cargaPercursoAnterior = adicionarCargaPercurso(cp, cargaPercursoAnterior.Destino, cp.Destino, unitOfWork);
                            }
                        }

                        if (todasAsRotas)
                        {
                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                                serCargaPedido.SetarDestinatarioFinalCarga(carga, cargaPedidos, unitOfWork, configuracaoPedido);
                            }

                            Repositorio.Embarcador.Cargas.CargaTabelaFreteRota repCargaTabelaFreteRota = new Repositorio.Embarcador.Cargas.CargaTabelaFreteRota(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota> cargaTabelasFreteRota = repCargaTabelaFreteRota.BuscarPorCarga(carga.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteRota cargaTabelaFreteRota in cargaTabelasFreteRota)
                            {
                                repCargaTabelaFreteRota.Deletar(cargaTabelaFreteRota);
                            }

                            //Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(_conexao.StringConexao, TipoServicoMultisoftware);
                            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete(); //serFrete.CalcularFreteSemExtornarComplementos(ref carga, unitOfWork, ConfiguracaoEmbarcador);
                            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete;
                            carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                            carga.PossuiPendencia = false;
                            carga.CalcularFreteSemEstornarComplemento = true;
                            carga.MotivoPendencia = "";
                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.CalculandoFrete = true;
                            if (carga.CargaAgrupada)
                                carga.AgIntegracaoAgrupamentoCarga = true;

                            new Servicos.Embarcador.Carga.CargaOperador(unitOfWork).Atualizar(carga, Usuario, TipoServicoMultisoftware);

                            repCarga.Atualizar(carga);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Alterou o Percurso da Carga", unitOfWork);

                            unitOfWork.CommitChanges();
                            return new JsonpResult(retorno);
                        }
                        else
                            throw new ControllerException("Não foi possível montar a rota informada, por favor tente novamente.");
                    }
                    else
                        throw new ControllerException("O ultimo percurso não pode ser para carregamento da carga");
                }
                else
                    throw new ControllerException(retornoVerificarOperador);
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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o percurso da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaPercurso adicionarCargaPercurso(Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.Rota rota = buscarRota(origem, destino, unitOfWork);
            if (rota != null)
            {
                cargaPercurso.Origem = rota.Origem;
                cargaPercurso.Destino = rota.Destino;
                cargaPercurso.DistanciaKM = rota.DistanciaKM;
                repCargaPercurso.Inserir(cargaPercurso);
                return cargaPercurso;
            }
            else
            {
                return null;
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.Rota buscarRota(Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Servicos.Embarcador.Logistica.MapRequestApi(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorOrigemDestino(origem.Codigo, destino.Codigo);
            if (rota == null)
            {
                rota = serMapRequestAPI.CriarRota(origem, destino, unitOfWork);
            }
            return rota;
        }
    }
}
