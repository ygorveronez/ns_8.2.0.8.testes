using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize("Acertos/TabelaComissaoMotorista")]
    public class TabelaComissaoMotoristaController : BaseController
    {
		#region Construtores

		public TabelaComissaoMotoristaController(Conexao conexao) : base(conexao) { }

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
                unitOfWork.Start();

                Repositorio.Embarcador.Acerto.TabelaComissaoMotorista repTabelaComissaoMotorista = new Repositorio.Embarcador.Acerto.TabelaComissaoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissaoMotorista = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista();

                PreencherTabelaComissaoMotorista(tabelaComissaoMotorista, unitOfWork);

                repTabelaComissaoMotorista.Inserir(tabelaComissaoMotorista, Auditado);

                SalvarModelos(tabelaComissaoMotorista, unitOfWork);
                SalvarSegmentos(tabelaComissaoMotorista, unitOfWork);
                SalvarTiposOperacao(tabelaComissaoMotorista, unitOfWork);
                SalvarListasEntidades(tabelaComissaoMotorista, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Acerto.TabelaComissaoMotorista repTabelaComissaoMotorista = new Repositorio.Embarcador.Acerto.TabelaComissaoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissaoMotorista = repTabelaComissaoMotorista.BuscarPorCodigo(codigo, true);

                if (tabelaComissaoMotorista == null)
                    return new JsonpResult(false, true, "Tabela não encontrada.");

                PreencherTabelaComissaoMotorista(tabelaComissaoMotorista, unitOfWork);

                repTabelaComissaoMotorista.Atualizar(tabelaComissaoMotorista, Auditado);

                SalvarModelos(tabelaComissaoMotorista, unitOfWork);
                SalvarSegmentos(tabelaComissaoMotorista, unitOfWork);
                SalvarTiposOperacao(tabelaComissaoMotorista, unitOfWork);
                SalvarListasEntidades(tabelaComissaoMotorista, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Acerto.TabelaComissaoMotorista repTabelaComissaoMotorista = new Repositorio.Embarcador.Acerto.TabelaComissaoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissao = repTabelaComissaoMotorista.BuscarPorCodigo(codigo);

                var dynTabelaComissaoMotorista = new
                {
                    tabelaComissao.Codigo,
                    tabelaComissao.Descricao,
                    tabelaComissao.PercentualComissaoPadrao,
                    DataVigencia = tabelaComissao.DataVigencia.Value.ToString("dd/MM/yyyy"),
                    Status = tabelaComissao.Ativo,
                    tabelaComissao.AtivarBonificacaoMediaCombustivel,
                    tabelaComissao.AtivarBonificacaoRotaFrete,
                    tabelaComissao.AtivarBonificacaoRepresentacaoCombustivel,
                    tabelaComissao.AtivarBonificacaoFaturamentoDia,

                    Segmentos = (from obj in tabelaComissao.Segmentos
                                 orderby obj.SegmentoVeiculo.Descricao
                                 select new
                                 {
                                     Segmento = new
                                     {
                                         Codigo = obj.SegmentoVeiculo.Codigo,
                                         Descricao = obj.SegmentoVeiculo.Descricao
                                     }
                                 }).ToList(),
                    Modelos = (from obj in tabelaComissao.Modelos
                               orderby obj.Modelo.Descricao
                               select new
                               {
                                   Modelo = new
                                   {
                                       Codigo = obj.Modelo.Codigo,
                                       Descricao = obj.Modelo.Descricao
                                   }
                               }).ToList(),
                    TiposOperacao = (from obj in tabelaComissao.TiposOperacao
                                     orderby obj.TipoOperacao.Descricao
                                     select new
                                     {
                                         TipoOperacao = new
                                         {
                                             Codigo = obj.TipoOperacao.Codigo,
                                             Descricao = obj.TipoOperacao.Descricao
                                         }
                                     }).ToList(),
                    Medias = (from obj in tabelaComissao.Medias
                              select new
                              {
                                  obj.Codigo,
                                  MediaInicial = obj.MediaInicial.ToString("n2"),
                                  MediaFinal = obj.MediaFinal.ToString("n2"),
                                  PercentualAcrescimoComissaoMedia = obj.PercentualAcrescimoComissao.ToString("n2"),
                                  ValorBonificacaoMedia = obj.ValorBonificacao.ToString("n2"),
                                  CodigoJustificativaBonificacaoMedia = obj.JustificativaBonificacao?.Codigo ?? 0,
                                  DescricaoJustificativaBonificacaoMedia = obj.JustificativaBonificacao?.Descricao ?? string.Empty
                              }).ToList(),
                    Representacaos = (from obj in tabelaComissao.Representacaos
                                      select new
                                      {
                                          obj.Codigo,
                                          PercentualRepresentacao = obj.PercentualRepresentacao.ToString("n2"),
                                          PercentualAcrescimoComissaoRepresentacao = obj.PercentualAcrescimoComissao.ToString("n2"),
                                          ValorBonificacaoRepresentacao = obj.ValorBonificacao.ToString("n2"),
                                          CodigoJustificativaBonificacaoRepresentacao = obj.JustificativaBonificacao?.Codigo ?? 0,
                                          DescricaoJustificativaBonificacaoRepresentacao = obj.JustificativaBonificacao?.Descricao ?? string.Empty
                                      }).ToList(),
                    FaturamentoDia = (from obj in tabelaComissao.FaturamentoDia
                                      select new
                                      {
                                          obj.Codigo,
                                          FaturamentoInicial = obj.FaturamentoInicial.ToString("n4"),
                                          FaturamentoFinal = obj.FaturamentoFinal.ToString("n4"),
                                          PercentualAcrescimoComissaoFaturamentoDia = obj.PercentualAcrescimoComissao.ToString("n2")
                                      }).ToList(),
                    RotasFretes = (from obj in tabelaComissao.RotasFretes
                                   select new
                                   {
                                       obj.Codigo,
                                       ValorBonificacaoRotaFrete = obj.ValorBonificacao.ToString("n2"),
                                       CodigoJustificativaBonificacaoRotaFrete = obj.JustificativaBonificacao?.Codigo ?? 0,
                                       DescricaoJustificativaBonificacaoRotaFrete = obj.JustificativaBonificacao?.Descricao ?? string.Empty,
                                       CodigoRotaFrete = obj.RotaFrete?.Codigo ?? 0,
                                       DescricaoRotaFrete = obj.RotaFrete?.Descricao ?? string.Empty
                                   }).ToList(),
                };

                return new JsonpResult(dynTabelaComissaoMotorista);
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

        #endregion

        #region Métodos Privados

        private void PreencherTabelaComissaoMotorista(Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissaoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao");

            bool.TryParse(Request.Params("Status"), out bool ativo);
            bool.TryParse(Request.Params("AtivarBonificacaoMediaCombustivel"), out bool ativarBonificacaoMediaCombustivel);
            bool.TryParse(Request.Params("AtivarBonificacaoRotaFrete"), out bool ativarBonificacaoRotaFrete);
            bool.TryParse(Request.Params("AtivarBonificacaoFaturamentoDia"), out bool ativarBonificacaoFaturamentoDia);
            bool.TryParse(Request.Params("AtivarBonificacaoRepresentacaoCombustivel"), out bool ativarBonificacaoRepresentacaoCombustivel);

            decimal.TryParse(Request.Params("PercentualComissaoPadrao"), out decimal percentualComissaoPadrao);

            DateTime.TryParse(Request.Params("DataVigencia"), out DateTime dataVigencia);

            tabelaComissaoMotorista.AtivarBonificacaoMediaCombustivel = ativarBonificacaoMediaCombustivel;
            tabelaComissaoMotorista.AtivarBonificacaoRotaFrete = ativarBonificacaoRotaFrete;
            tabelaComissaoMotorista.AtivarBonificacaoFaturamentoDia = ativarBonificacaoFaturamentoDia;
            tabelaComissaoMotorista.AtivarBonificacaoRepresentacaoCombustivel = ativarBonificacaoRepresentacaoCombustivel;
            tabelaComissaoMotorista.Ativo = ativo;
            tabelaComissaoMotorista.DataVigencia = dataVigencia;
            tabelaComissaoMotorista.Descricao = descricao;
            tabelaComissaoMotorista.PercentualComissaoPadrao = percentualComissaoPadrao;
        }

        private void SalvarModelos(Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissaoMotorista, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo repTabelaComissaoMotoristaModeloVeiculo = new Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo(unidadeTrabalho);

            dynamic modelos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Modelos"));

            List<int> codigos = new List<int>();

            foreach (dynamic mod in modelos)
                codigos.Add((int)mod.Modelo.Codigo);

            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo> modelosDeletar = tabelaComissaoMotorista.Modelos != null && tabelaComissaoMotorista.Modelos.Count > 0 ? tabelaComissaoMotorista.Modelos.Where(o => !codigos.Contains(o.Modelo.Codigo)).ToList() : null;

            if (modelosDeletar != null && modelosDeletar.Count > 0)
                foreach (Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo modDeletar in modelosDeletar)
                    repTabelaComissaoMotoristaModeloVeiculo.Deletar(modDeletar);

            foreach (var mod in modelos)
            {
                int codigoModelo = (int)mod.Modelo.Codigo;
                if (tabelaComissaoMotorista.Modelos == null || tabelaComissaoMotorista.Modelos.Count == 0 || (tabelaComissaoMotorista.Modelos.Count > 0 && !tabelaComissaoMotorista.Modelos.Any(o => o.Modelo.Codigo == codigoModelo)))
                {
                    Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo tabelaModelo = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo()
                    {
                        Modelo = repModeloVeiculo.BuscarPorCodigo(codigoModelo, 0),
                        TabelaComissaoMotorista = tabelaComissaoMotorista
                    };
                    repTabelaComissaoMotoristaModeloVeiculo.Inserir(tabelaModelo);
                }
            }
        }

        private void SalvarSegmentos(Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissaoMotorista, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaSegmento repTabelaComissaoMotoristaSegmento = new Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaSegmento(unidadeTrabalho);

            dynamic segmentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Segmentos"));

            List<int> codigos = new List<int>();

            foreach (dynamic mod in segmentos)
                codigos.Add((int)mod.Segmento.Codigo);

            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento> segmentosDeletar = tabelaComissaoMotorista.Segmentos != null && tabelaComissaoMotorista.Segmentos.Count > 0 ? tabelaComissaoMotorista.Segmentos.Where(o => !codigos.Contains(o.SegmentoVeiculo.Codigo)).ToList() : null;

            if (segmentosDeletar != null && segmentosDeletar.Count > 0)
                foreach (Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento segDeletar in segmentosDeletar)
                    repTabelaComissaoMotoristaSegmento.Deletar(segDeletar);

            foreach (var mod in segmentos)
            {
                int codigoModelo = (int)mod.Segmento.Codigo;
                if (tabelaComissaoMotorista.Segmentos == null || tabelaComissaoMotorista.Segmentos.Count == 0 || (tabelaComissaoMotorista.Segmentos.Count > 0 && !tabelaComissaoMotorista.Segmentos.Any(o => o.SegmentoVeiculo.Codigo == codigoModelo)))
                {
                    Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento tabelaSegmento = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento()
                    {
                        SegmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codigoModelo),
                        TabelaComissaoMotorista = tabelaComissaoMotorista
                    };
                    repTabelaComissaoMotoristaSegmento.Inserir(tabelaSegmento);
                }
            }
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissaoMotorista, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao repTabelaComissaoMotoristaTipoOperacao = new Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao(unidadeTrabalho);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            List<int> codigos = new List<int>();

            foreach (dynamic tipoOperacao in tiposOperacao)
                codigos.Add((int)tipoOperacao.TipoOperacao.Codigo);

            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao> entidadesDeletar = tabelaComissaoMotorista.TiposOperacao != null && tabelaComissaoMotorista.TiposOperacao.Count > 0 ? tabelaComissaoMotorista.TiposOperacao.Where(o => !codigos.Contains(o.TipoOperacao.Codigo)).ToList() : null;

            if (entidadesDeletar != null && entidadesDeletar.Count > 0)
                foreach (Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao entidade in entidadesDeletar)
                    repTabelaComissaoMotoristaTipoOperacao.Deletar(entidade);

            foreach (var tipoOperacao in tiposOperacao)
            {
                int codigoTipoOperacao = (int)tipoOperacao.TipoOperacao.Codigo;
                if (tabelaComissaoMotorista.TiposOperacao == null || tabelaComissaoMotorista.TiposOperacao.Count == 0 || (tabelaComissaoMotorista.TiposOperacao.Count > 0 && !tabelaComissaoMotorista.TiposOperacao.Any(o => o.TipoOperacao.Codigo == codigoTipoOperacao)))
                {
                    Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao tabelaTipoOperacao = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao()
                    {
                        TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao),
                        TabelaComissaoMotorista = tabelaComissaoMotorista
                    };
                    repTabelaComissaoMotoristaTipoOperacao.Inserir(tabelaTipoOperacao);
                }
            }
        }

        private void SalvarListasEntidades(Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissaoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao repTabelaComissaoMotoristaRepresentacao = new Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao(unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaMedia repTabelaComissaoMotoristaMedia = new Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaMedia(unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete repTabelaComissaoMotoristaRotaFrete = new Repositorio.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaComissaoFaturamentoDia repTabelaComissaoFaturamentoDia = new Repositorio.Embarcador.Acerto.TabelaComissaoFaturamentoDia(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagemTabelaComissao repAcertoViagemTabelaComissao = new Repositorio.Embarcador.Acerto.AcertoViagemTabelaComissao(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            //Lista das medias
            dynamic dynMedias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Medias"));
            List<int> codigosMedia = new List<int>();
            if (dynMedias.Count > 0)
            {
                foreach (var med in dynMedias)
                {
                    int.TryParse((string)med.Codigo, out int codigoMedia);
                    Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaMedia media;
                    if (codigoMedia > 0)
                        media = repTabelaComissaoMotoristaMedia.BuscarPorCodigo(codigoMedia);
                    else
                        media = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaMedia();

                    media.TabelaComissaoMotorista = tabelaComissaoMotorista;

                    decimal.TryParse((string)med.MediaInicial, out decimal mediaInicial);
                    decimal.TryParse((string)med.MediaFinal, out decimal mediaFinal);
                    decimal.TryParse((string)med.PercentualAcrescimoComissaoMedia, out decimal percentualAcrescimoComissaoMedia);
                    decimal.TryParse((string)med.ValorBonificacaoMedia, out decimal valorBonificacaoMedia);

                    media.MediaInicial = mediaInicial;
                    media.MediaFinal = mediaFinal;
                    media.PercentualAcrescimoComissao = percentualAcrescimoComissaoMedia;
                    media.ValorBonificacao = valorBonificacaoMedia;

                    int.TryParse((string)med.CodigoJustificativaBonificacaoMedia, out int codigoJustificativaBonificacaoMedia);
                    if (codigoJustificativaBonificacaoMedia > 0)
                        media.JustificativaBonificacao = repJustificativa.BuscarPorCodigo(codigoJustificativaBonificacaoMedia);
                    else
                        media.JustificativaBonificacao = null;

                    if (codigoMedia > 0)
                        repTabelaComissaoMotoristaMedia.Atualizar(media);
                    else
                    {
                        repTabelaComissaoMotoristaMedia.Inserir(media);
                    }

                    codigosMedia.Add(media.Codigo);
                }
            }
            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaMedia> medias = repTabelaComissaoMotoristaMedia.BuscarPorTabela(tabelaComissaoMotorista.Codigo);
            foreach (var med in medias)
            {
                if (!codigosMedia.Contains(med.Codigo))
                {
                    //  Inicialmente eu ia só deletar, mas visto que essa entidade tem como alternativa as representações, decidi só remover a referência.

                    //  Se o AcertoViagem estiver em andamento:
                    //      A função RetornaObjetoReceitaViagem() já tem uma tratativa caso as médias forem nulas ou caso as médias E as representações forem nulas.
                    //  Se não estiver:
                    //      Só vai afetar o percentual de comissão, mas isso parece proposital.
                    foreach (var acerto in repAcertoViagemTabelaComissao.BuscarPorMedia(med.Codigo))
                    {
                        acerto.TabelaComissaoMotoristaMedia = null;
                        repAcertoViagemTabelaComissao.Atualizar(acerto);
                    }

                    //  O sistema não permite deletar algumas médias porque elas podem estar vinculadas a um "AcertoViagemTabelaComissao". Tratativa acima.
                    repTabelaComissaoMotoristaMedia.Deletar(med);
                }
            }

            //Lista das rotasFretes
            dynamic dynRotasFretes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("RotasFretes"));
            List<int> codigosRotaFrete = new List<int>();
            if (dynRotasFretes.Count > 0)
            {
                foreach (var rot in dynRotasFretes)
                {
                    int.TryParse((string)rot.Codigo, out int codigoRotaFrete);
                    Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete rotaFrete;
                    if (codigoRotaFrete > 0)
                        rotaFrete = repTabelaComissaoMotoristaRotaFrete.BuscarPorCodigo(codigoRotaFrete);
                    else
                        rotaFrete = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete();

                    rotaFrete.TabelaComissaoMotorista = tabelaComissaoMotorista;

                    decimal.TryParse((string)rot.ValorBonificacaoRotaFrete, out decimal valorBonificacaoRotaFrete);
                    rotaFrete.ValorBonificacao = valorBonificacaoRotaFrete;

                    int.TryParse((string)rot.CodigoJustificativaBonificacaoRotaFrete, out int codigoJustificativaBonificacaoRotaFrete);
                    if (codigoJustificativaBonificacaoRotaFrete > 0)
                        rotaFrete.JustificativaBonificacao = repJustificativa.BuscarPorCodigo(codigoJustificativaBonificacaoRotaFrete);
                    else
                        rotaFrete.JustificativaBonificacao = null;

                    int.TryParse((string)rot.CodigoRotaFrete, out int codRotaFrete);
                    if (codRotaFrete > 0)
                        rotaFrete.RotaFrete = repRotaFrete.BuscarPorCodigo(codRotaFrete);
                    else
                        rotaFrete.RotaFrete = null;

                    if (codigoRotaFrete > 0)
                        repTabelaComissaoMotoristaRotaFrete.Atualizar(rotaFrete);
                    else
                    {
                        repTabelaComissaoMotoristaRotaFrete.Inserir(rotaFrete);
                    }

                    codigosRotaFrete.Add(rotaFrete.Codigo);
                }
            }
            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete> rotasFretes = repTabelaComissaoMotoristaRotaFrete.BuscarPorTabela(tabelaComissaoMotorista.Codigo);
            foreach (var rot in rotasFretes)
            {
                if (!codigosRotaFrete.Contains(rot.Codigo))
                    repTabelaComissaoMotoristaRotaFrete.Deletar(rot);
            }

            //Lista das representacaos
            dynamic dynRepresentacaos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Representacoes"));
            List<int> codigosRepresentacao = new List<int>();
            if (dynRepresentacaos.Count > 0)
            {
                foreach (var med in dynRepresentacaos)
                {
                    int.TryParse((string)med.Codigo, out int codigoRepresentacao);
                    Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao representacao;
                    if (codigoRepresentacao > 0)
                        representacao = repTabelaComissaoMotoristaRepresentacao.BuscarPorCodigo(codigoRepresentacao);
                    else
                        representacao = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao();

                    representacao.TabelaComissaoMotorista = tabelaComissaoMotorista;

                    decimal.TryParse((string)med.PercentualRepresentacao, out decimal percentualRepresentacao);
                    decimal.TryParse((string)med.PercentualAcrescimoComissaoRepresentacao, out decimal percentualAcrescimoComissaoRepresentacao);
                    decimal.TryParse((string)med.ValorBonificacaoRepresentacao, out decimal valorBonificacaoRepresentacao);

                    representacao.PercentualRepresentacao = percentualRepresentacao;
                    representacao.PercentualAcrescimoComissao = percentualAcrescimoComissaoRepresentacao;
                    representacao.ValorBonificacao = valorBonificacaoRepresentacao;

                    int.TryParse((string)med.CodigoJustificativaBonificacaoRepresentacao, out int codigoJustificativaBonificacaoRepresentacao);
                    if (codigoJustificativaBonificacaoRepresentacao > 0)
                        representacao.JustificativaBonificacao = repJustificativa.BuscarPorCodigo(codigoJustificativaBonificacaoRepresentacao);
                    else
                        representacao.JustificativaBonificacao = null;

                    if (codigoRepresentacao > 0)
                        repTabelaComissaoMotoristaRepresentacao.Atualizar(representacao);
                    else
                    {
                        repTabelaComissaoMotoristaRepresentacao.Inserir(representacao);
                    }

                    codigosRepresentacao.Add(representacao.Codigo);
                }
            }
            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao> representacaos = repTabelaComissaoMotoristaRepresentacao.BuscarPorTabela(tabelaComissaoMotorista.Codigo);
            foreach (var med in representacaos)
            {
                if (!codigosRepresentacao.Contains(med.Codigo))
                    repTabelaComissaoMotoristaRepresentacao.Deletar(med);
            }

            //Lista do faturamento de dia
            dynamic dynFaturamentoDia = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FaturamentoDia"));
            List<int> codigosFaturamentoDia = new List<int>();
            if (dynFaturamentoDia.Count > 0)
            {
                foreach (var faturamento in dynFaturamentoDia)
                {
                    int.TryParse((string)faturamento.Codigo, out int codigoFaturamento);
                    Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia faturamentoDia;
                    if (codigoFaturamento > 0)
                        faturamentoDia = repTabelaComissaoFaturamentoDia.BuscarPorCodigo(codigoFaturamento);
                    else
                        faturamentoDia = new Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia();

                    faturamentoDia.TabelaComissaoMotorista = tabelaComissaoMotorista;

                    decimal.TryParse((string)faturamento.FaturamentoInicial, out decimal faturamentoInicial);
                    decimal.TryParse((string)faturamento.FaturamentoFinal, out decimal faturamentoFinal);
                    decimal.TryParse((string)faturamento.PercentualAcrescimoComissaoFaturamentoDia, out decimal percentualAcrescimoComissao);

                    faturamentoDia.FaturamentoFinal = faturamentoFinal;
                    faturamentoDia.FaturamentoInicial = faturamentoInicial;
                    faturamentoDia.PercentualAcrescimoComissao = percentualAcrescimoComissao;

                    if (codigoFaturamento > 0)
                        repTabelaComissaoFaturamentoDia.Atualizar(faturamentoDia);
                    else
                        repTabelaComissaoFaturamentoDia.Inserir(faturamentoDia);

                    codigosFaturamentoDia.Add(faturamentoDia.Codigo);
                }
            }
            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia> faturamentosDias = repTabelaComissaoFaturamentoDia.BuscarPorTabela(tabelaComissaoMotorista.Codigo);
            foreach (var fat in faturamentosDias)
            {
                if (!codigosFaturamentoDia.Contains(fat.Codigo))
                    repTabelaComissaoFaturamentoDia.Deletar(fat);
            }

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
            grid.Prop("Descricao").Nome("Descrição").Tamanho(35).Align(Models.Grid.Align.left);
            grid.Prop("Ativo").Nome("Status").Tamanho(25).Align(Models.Grid.Align.left);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Acerto.TabelaComissaoMotorista repTabelaComissaoMotorista = new Repositorio.Embarcador.Acerto.TabelaComissaoMotorista(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista> listaGrid = repTabelaComissaoMotorista.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTabelaComissaoMotorista.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
        }

        #endregion
    }
}


