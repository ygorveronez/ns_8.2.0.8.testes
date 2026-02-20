using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("Cargas/CargaControleExpedicao")]
    public class ConferenciaSeparacaoVolumeController : BaseController
    {
		#region Construtores

		public ConferenciaSeparacaoVolumeController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridProdutosConferidos();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);
                if (lista != null && lista.Count <= 0)
                {
                    unitOfWork.Start();
                    InserirVolumesExpedicao(unitOfWork);
                    unitOfWork.CommitChanges();
                    totalRegistros = 0;
                    lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);
                }

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

        public async Task<IActionResult> AutorizarVolumesFaltantes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Expedicao"), out int codigoExpedicao);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CargaControleExpedicao");
                if (!(permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.WMS_Autorizar_Volumes_Faltantes) || this.Usuario.UsuarioAdministrador))
                {
                    return new JsonpResult(false, "O usuário logado não possui permissão para autorizar os volumes faltantes.");
                }
                else if (codigoExpedicao == 0)
                {
                    return new JsonpResult(false, "Favor selecione um recebimento.");
                }

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao expedicao = repCargaControleExpedicao.BuscarPorCodigo(codigoExpedicao);

                // Inicia transacao
                unitOfWork.Start();

                expedicao.AutorizadoProdutosFaltantes = true;

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

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Expedicao"), out int codigoExpedicao);


                // Inicia transacao
                unitOfWork.Start();

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao expedicao = repCargaControleExpedicao.BuscarPorCodigo(codigoExpedicao);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(expedicao.Carga.Codigo);

                if (!ValidaConferencia(carga.Codigo, out string erro, unitOfWork, expedicao.AutorizadoProdutosFaltantes))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                expedicao.SituacaoCargaControleExpedicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaControleExpedicao.Liberada;
                expedicao.DataConfirmacao = DateTime.Now;
                expedicao.Usuario = this.Usuario;
                carga.SeparacaoConferida = true;

                repCargaControleExpedicao.Atualizar(expedicao);
                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, expedicao, null, "Finalizou a conferência", unitOfWork);

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);

                // Converte valores
                int.TryParse(Request.Params("Carga"), out int carga);
                string codigoBarras = Request.Params("CodigoBarras") ?? string.Empty;
                int.TryParse(Request.Params("Expedicao"), out int expedicao);

                string numeroNS = "", volume = "", cnpjRemetente = "", numeroNota = "", serieNota = "";
                string codigoBarrasSalvar = "";
                bool validarCodigoBarras = false;
                if (codigoBarras.Length == 13)
                {
                    numeroNS = codigoBarras.Substring(0, 10);
                    volume = codigoBarras.Substring(10, 3);
                    codigoBarrasSalvar = numeroNS.ToUpper();
                }
                else if (codigoBarras.Length == 31)
                {
                    cnpjRemetente = codigoBarras.Substring(0, 14);

                    numeroNota = codigoBarras.Substring(14, 9);
                    numeroNota = numeroNota.TrimStart('0');

                    volume = codigoBarras.Substring(23, 4);

                    serieNota = codigoBarras.Substring(27, 2);
                    serieNota = serieNota.TrimStart('0');
                    codigoBarrasSalvar = codigoBarras.ToUpper();

                    validarCodigoBarras = true;
                }
                else if (codigoBarras.Length == 33)
                {
                    cnpjRemetente = codigoBarras.Substring(0, 14);

                    numeroNota = codigoBarras.Substring(14, 9);
                    numeroNota = numeroNota.TrimStart('0');

                    volume = codigoBarras.Substring(23, 4);

                    serieNota = codigoBarras.Substring(27, 3);
                    serieNota = serieNota.TrimStart('0');

                    codigoBarrasSalvar = codigoBarras.ToUpper();

                    validarCodigoBarras = true;
                }
                else if (codigoBarras.Length == 34)
                {
                    cnpjRemetente = codigoBarras.Substring(0, 14);
                    cnpjRemetente = cnpjRemetente.TrimStart('0');

                    numeroNota = codigoBarras.Substring(14, 6);
                    numeroNota = numeroNota.TrimStart('0');

                    serieNota = codigoBarras.Substring(20, 2);
                    serieNota = serieNota.TrimStart('0');

                    volume = codigoBarras.Substring(22, 4);

                    codigoBarrasSalvar = codigoBarras.ToUpper();
                }
                else
                {
                    var retornoErro = new
                    {
                        JaConferido = false,
                        Conferido = 0,
                        Quantidade = 0,
                        CodigoBarrasValido = false
                    };

                    return new JsonpResult(retornoErro);
                }
                codigoBarras = codigoBarras.ToUpper();
                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao conferencia = repConferenciaSeparacao.BuscarPorCargaECodigoBarras(carga, codigoBarrasSalvar, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume);
                if (conferencia == null && validarCodigoBarras)
                {
                    conferencia = repConferenciaSeparacao.BuscarPorCargaECodigoValido(carga, codigoBarrasSalvar, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume);
                    if (conferencia != null)
                    {
                        if (conferencia.QuantidadeFaltante <= 0)
                            return new JsonpResult(false, true, "Todos os volumes já foram conferidos.");
                        validarCodigoBarras = false;
                    }
                }

                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume> dadosRelacaoSeparacaoVolume = repCargaPedidoDocumentoCTe.RelatorioRelacaoSeparacaoVolume(carga, numeroNS, cnpjRemetente, numeroNota, serieNota);
                if (dadosRelacaoSeparacaoVolume == null || dadosRelacaoSeparacaoVolume.Count == 0)
                {
                    var retornoErro = new
                    {
                        JaConferido = false,
                        Conferido = 0,
                        Quantidade = 0,
                        CodigoBarrasValido = false
                    };

                    return new JsonpResult(retornoErro);
                }
                int posicaoDadosRelacao = 0;
                if (dadosRelacaoSeparacaoVolume[posicaoDadosRelacao].Volumes <= 0 && dadosRelacaoSeparacaoVolume.Count > 1)
                {
                    for (int i = 0; i < dadosRelacaoSeparacaoVolume.Count; i++)
                    {
                        if (dadosRelacaoSeparacaoVolume[i].Volumes > 0 && posicaoDadosRelacao == 0)
                            posicaoDadosRelacao = i;
                    }
                }
                // Busca informacoes
                if (conferencia == null)
                {
                    conferencia = new Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao()
                    {
                        CodigoBarras = codigoBarrasSalvar,
                        TipoRecebimentoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume,
                        Expedicao = repCargaControleExpedicao.BuscarPorCodigo(expedicao)
                    };
                }
                else if (validarCodigoBarras)
                {
                    var retornoErro = new
                    {
                        JaConferido = true,
                        Conferido = 0,
                        Quantidade = 0,
                        CodigoBarrasValido = true
                    };

                    return new JsonpResult(retornoErro);
                }

                //Validar código de barras passado duas vezes
                if (conferencia.CodigosValidados != null && conferencia.CodigosValidados != "")
                {
                    if (conferencia.CodigosValidados.Contains(codigoBarras))
                    {
                        Servicos.Log.TratarErro(codigoBarras + " - O código de barras já foi conferido.");
                        return new JsonpResult(false, true, "O código de barras " + codigoBarras + " já foi conferido.");
                    }
                }

                // Vincula dados
                conferencia.Quantidade += 1;
                conferencia.TipoRecebimentoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume;
                conferencia.VolumeCarga = dadosRelacaoSeparacaoVolume[posicaoDadosRelacao].Volumes;
                conferencia.QuantidadeFaltante = conferencia.VolumeCarga - conferencia.Quantidade;
                if (string.IsNullOrWhiteSpace(conferencia.CodigosValidados))
                    conferencia.CodigosValidados = codigoBarras;
                else
                    conferencia.CodigosValidados = conferencia.CodigosValidados + "|" + codigoBarras;
                if (string.IsNullOrWhiteSpace(conferencia.Numero))
                    conferencia.Numero = !string.IsNullOrWhiteSpace(numeroNota) ? numeroNota : !string.IsNullOrWhiteSpace(numeroNS) ? numeroNS : string.Empty;
                else
                {
                    string numeroSalvar = (!string.IsNullOrWhiteSpace(numeroNota) ? numeroNota : !string.IsNullOrWhiteSpace(numeroNS) ? numeroNS : string.Empty);
                    if (!string.IsNullOrEmpty(numeroSalvar) && !conferencia.Numero.Contains(numeroSalvar))
                        conferencia.Numero = conferencia.Numero + "|" + numeroSalvar;
                }

                // Valida entidade
                if (!ValidaEntidade(conferencia, out string erro))
                    return new JsonpResult(false, true, erro);

                if (dadosRelacaoSeparacaoVolume[posicaoDadosRelacao].Volumes < conferencia.Quantidade)
                    return new JsonpResult(false, true, "A conferência ultrapassa a quantia máxima de " + dadosRelacaoSeparacaoVolume[posicaoDadosRelacao].Volumes.ToString("n3") + ".");

                // Persiste dados
                if (conferencia.Codigo > 0)
                    repConferenciaSeparacao.Atualizar(conferencia);
                else
                    repConferenciaSeparacao.Inserir(conferencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, conferencia, null, "Adicionou " + 1.ToString("n3") + " a conferência.", unitOfWork);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    JaConferido = false,
                    Conferido = conferencia?.Quantidade + 1 ?? 0,
                    Quantidade = dadosRelacaoSeparacaoVolume != null && dadosRelacaoSeparacaoVolume.Count > 0 ? dadosRelacaoSeparacaoVolume[posicaoDadosRelacao].Volumes : 0,
                    CodigoBarrasValido = dadosRelacaoSeparacaoVolume != null && dadosRelacaoSeparacaoVolume.Count > 0 ? true : false
                };
                return new JsonpResult(retorno);
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

        #region Métodos Privados

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao conferencia, out string msgErro)
        {
            msgErro = "";

            if (conferencia.CodigoBarras.Length == 0)
            {
                msgErro = "Código de Barras é obrigatório.";
                return false;
            }

            if (conferencia.Quantidade == 0)
            {
                msgErro = "Quantidade é obrigatório.";
                return false;
            }

            if (conferencia.Expedicao == null)
            {
                throw new Exception("Expedição null.");
            }

            return true;
        }


        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaConferencia(int carga, out string msgErro, Repositorio.UnitOfWork unitOfWork, bool autorizadoProdutosFaltantes)
        {
            msgErro = "";

            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

            // Conferencia zerada
            if (repConferenciaSeparacao.QuantidadeConferidaPorCarga(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume) == 0)
            {
                msgErro = "Nenhuma conferência registrada.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> conferencia = repConferenciaSeparacao.BuscarPorCarga(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume);
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume> dadosRelacaoSeparacaoVolume = repCargaPedidoDocumentoCTe.RelatorioRelacaoSeparacaoVolume(carga, "", "", "", "");

            for (int i = 0; i < conferencia.Count; i++)
            {
                if (conferencia[i].QuantidadeFaltante > 0 && !autorizadoProdutosFaltantes)
                {
                    msgErro = "Existem volumes faltantes a serem conferidos.";
                    return false;
                }
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                int somaVolumes = dadosRelacaoSeparacaoVolume.Sum(c => c.Volumes);
                decimal somaConferencia = conferencia.Sum(c => c.Quantidade);
                if (somaVolumes > 0 && (decimal)somaVolumes != somaConferencia)
                {
                    msgErro = "Não foi realizado a conferencia de todos os volumes.";
                    return false;
                }
            }

            if (dadosRelacaoSeparacaoVolume.Count != conferencia.Count && !autorizadoProdutosFaltantes)
            {
                msgErro = "Existem volumes que não foram conferidos, verifique a Relação de Separação de Volumes.";
                return false;
            }

            for (int i = 0; i < conferencia.Count; i++)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote = repProdutoEmbarcadorLote.BuscarPorCodigoBarras(conferencia[i].CodigoBarras, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume);
                if (lote != null && lote.QuantidadeAtual > 0)
                {
                    lote.Initialize();
                    lote.QuantidadeAtual -= conferencia[i].Quantidade;
                    repProdutoEmbarcadorLote.Atualizar(lote, Auditado);
                }
            }

            return true;
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridProdutosConferidos()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Carga").Nome("Carga").Tamanho(15);
            grid.Prop("Numero").Nome("Número").Tamanho(15);
            grid.Prop("CodigoBarras");
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("QuantidadeFaltante").Nome("Qtd. Faltante").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("VolumeCarga").Nome("Volume Carga").Tamanho(10).Align(Models.Grid.Align.right);

            return grid;
        }


        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Carga") propOrdenar = "Expedicao.Carga.CodigoCargaEmbarcador";
        }


        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Expedicao"), out int codigoExpedicao);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao expedicao = repCargaControleExpedicao.BuscarPorCodigo(codigoExpedicao);

            // Consulta
            List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> listaGrid = repConferenciaSeparacao.Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume, expedicao.Carga.Codigo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repConferenciaSeparacao.ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume, expedicao.Carga.Codigo);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Carga = obj.Expedicao.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                            obj.Numero,
                            CodigoBarras = obj.CodigoBarras,
                            Quantidade = obj.Quantidade.ToString("n3"),
                            QuantidadeFaltante = obj.QuantidadeFaltante.ToString("n3"),
                            VolumeCarga = obj.VolumeCarga.ToString("n3")
                        };

            return lista.ToList();
        }

        private void InserirVolumesExpedicao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);

            int.TryParse(Request.Params("Expedicao"), out int codigoExpedicao);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao expedicao = repCargaControleExpedicao.BuscarPorCodigo(codigoExpedicao);
            if (expedicao != null && expedicao.Carga != null)
            {
                Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume()
                {
                    CodigoCarga = expedicao.Carga.Codigo
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedadeAgrupamento = new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento()
                {
                    Propriedade = "NumeroSolicitacao",
                    CodigoDinamico = 0
                };
                agrupamentos.Add(propriedadeAgrupamento);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedadeAgrupamento1 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento()
                {
                    Propriedade = "CodigoBarras",
                    CodigoDinamico = 0
                };
                agrupamentos.Add(propriedadeAgrupamento1);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedadeAgrupamento2 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento()
                {
                    Propriedade = "Volumes",
                    CodigoDinamico = 0
                };
                agrupamentos.Add(propriedadeAgrupamento2);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedadeAgrupamento3 = new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento()
                {
                    Propriedade = "NumeroNota",
                    CodigoDinamico = 0
                };
                agrupamentos.Add(propriedadeAgrupamento3);

                IList<Dominio.Relatorios.Embarcador.DataSource.WMS.ExpedicaoVolume> listaExpedicaoVolume = repRecebimentoMercadoria.RelatorioExpedicaoVolume(filtrosPesquisa, agrupamentos, parametrosConsulta);
                if (listaExpedicaoVolume != null && listaExpedicaoVolume.Count > 0)
                {
                    foreach (var volume in listaExpedicaoVolume)
                    {
                        Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao conferencia = new Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao()
                        {
                            Quantidade = 0,
                            CodigoBarras = string.IsNullOrWhiteSpace(volume.NumeroSolicitacao) ? volume.CodigoBarras : volume.NumeroSolicitacao,
                            Expedicao = expedicao,
                            TipoRecebimentoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume,
                            QuantidadeFaltante = volume.Volumes,
                            VolumeCarga = volume.Volumes,
                            CodigosValidados = string.Empty,
                            Numero = string.IsNullOrWhiteSpace(volume.NumeroSolicitacao) ? volume.NumeroNota : volume.NumeroSolicitacao
                        };

                        repConferenciaSeparacao.Inserir(conferencia);
                    }
                }
            }
        }

        #endregion
    }
}
