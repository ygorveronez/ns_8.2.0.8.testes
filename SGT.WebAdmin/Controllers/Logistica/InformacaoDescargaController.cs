using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/InformacaoDescarga")]
    public class InformacaoDescargaController : BaseController
    {
		#region Construtores

		public InformacaoDescargaController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Logistica.InformacaoDescarga repInformacaoDescarga = new Repositorio.Embarcador.Logistica.InformacaoDescarga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga informacaoDescarga = repInformacaoDescarga.BuscarPorCodigo(codigo);

                // Valida
                if (informacaoDescarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    informacaoDescarga.Codigo,
                    Data = informacaoDescarga.Data.ToString("dd/MM/yyyy"),
                    Hora = informacaoDescarga.Hora.ToString(""),
                    informacaoDescarga.NotaFiscal,
                    PesoDescarga = informacaoDescarga.PesoDescarga.ToString("n4"),
                    informacaoDescarga.Placa,
                    informacaoDescarga.Serie,
                    informacaoDescarga.Empresa,
                    Carga = new { Codigo = informacaoDescarga.Carga?.Codigo ?? 0, Descricao = informacaoDescarga.Carga?.Descricao ?? string.Empty }
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
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Logistica.InformacaoDescarga repInformacaoDescarga = new Repositorio.Embarcador.Logistica.InformacaoDescarga(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga informacaoDescarga = new Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga();

                // Preenche entidade com dados
                PreencheEntidade(ref informacaoDescarga, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(informacaoDescarga, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repInformacaoDescarga.Inserir(informacaoDescarga, Auditado);
                unitOfWork.CommitChanges();

                if (informacaoDescarga.Carga != null && informacaoDescarga.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                    EncerrarCarga(informacaoDescarga.Carga.Codigo, unitOfWork);

                // Retorna sucesso
                return new JsonpResult(true);
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
                Repositorio.Embarcador.Logistica.InformacaoDescarga repInformacaoDescarga = new Repositorio.Embarcador.Logistica.InformacaoDescarga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga informacaoDescarga = repInformacaoDescarga.BuscarPorCodigo(codigo);

                // Valida
                if (informacaoDescarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref informacaoDescarga, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(informacaoDescarga, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repInformacaoDescarga.Atualizar(informacaoDescarga);
                unitOfWork.CommitChanges();

                if (informacaoDescarga.Carga != null && informacaoDescarga.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                    EncerrarCarga(informacaoDescarga.Carga.Codigo, unitOfWork);

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
                Repositorio.Embarcador.Logistica.InformacaoDescarga repInformacaoDescarga = new Repositorio.Embarcador.Logistica.InformacaoDescarga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga informacaoDescarga = repInformacaoDescarga.BuscarPorCodigo(codigo);

                // Valida
                if (informacaoDescarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repInformacaoDescarga.Deletar(informacaoDescarga, Auditado);
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


        #region Importação

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoProduto()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Data", Propriedade = "Data", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Hora", Propriedade = "Hora", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Empresa", Propriedade = "Empresa", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Nota Fiscal", Propriedade = "NotaFiscal", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Série", Propriedade = "Serie", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Placa", Propriedade = "Placa", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Peso Descarga", Propriedade = "PesoDescarga", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProduto();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoProduto();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.InformacaoDescarga repInformacaoDescarga = new Repositorio.Embarcador.Logistica.InformacaoDescarga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga informacaoDescarga = new Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colData = (from obj in linha.Colunas where obj.NomeCampo == "Data" select obj).FirstOrDefault();
                        if (colData != null)
                        {
                            DateTime data;
                            DateTime.TryParse(colData.Valor, out data);
                            informacaoDescarga.Data = data;
                        }
                        else
                            retorno = "Data é obrigatória";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHora = (from obj in linha.Colunas where obj.NomeCampo == "Hora" select obj).FirstOrDefault();
                        if (colHora != null)
                        {
                            DateTime hora;
                            DateTime.TryParse(colHora.Valor, out hora);
                            informacaoDescarga.Hora = hora.TimeOfDay;
                        }
                        else
                            retorno = "Hora é obrigatória";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmpresa = (from obj in linha.Colunas where obj.NomeCampo == "Empresa" select obj).FirstOrDefault();
                        if (colEmpresa != null)
                            informacaoDescarga.Empresa = colEmpresa.Valor.Trim();
                        else
                            retorno = "Empresa é obrigatório";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPlaca = (from obj in linha.Colunas where obj.NomeCampo == "Placa" select obj).FirstOrDefault();
                        if (colPlaca != null)
                            informacaoDescarga.Placa = colPlaca.Valor.Trim();
                        else
                            retorno = "Placa é obrigatório";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNotaFiscal = (from obj in linha.Colunas where obj.NomeCampo == "NotaFiscal" select obj).FirstOrDefault();
                        if (colNotaFiscal != null)
                        {
                            int notaFiscal;
                            int.TryParse(colNotaFiscal.Valor, out notaFiscal);
                            informacaoDescarga.NotaFiscal = notaFiscal;
                        }
                        else
                            retorno = "Nota Fiscal é obrigatória";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerie = (from obj in linha.Colunas where obj.NomeCampo == "Serie" select obj).FirstOrDefault();
                        if (colSerie != null)
                        {
                            int serie;
                            int.TryParse(colSerie.Valor, out serie);
                            informacaoDescarga.Serie = serie;
                        }
                        else
                            retorno = "Série é obrigatória";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoDescarga = (from obj in linha.Colunas where obj.NomeCampo == "PesoDescarga" select obj).FirstOrDefault();
                        if (colPesoDescarga != null)
                        {
                            decimal pesoDescarga;
                            decimal.TryParse(colPesoDescarga.Valor, out pesoDescarga);
                            informacaoDescarga.PesoDescarga = pesoDescarga;
                        }
                        else
                            retorno = "Peso da Descarga é obrigatória";


                        informacaoDescarga.DataImportacao = DateTime.Now;
                        informacaoDescarga.Usuario = this.Usuario;
                        if (informacaoDescarga.Carga == null && informacaoDescarga.NotaFiscal > 0 && informacaoDescarga.Serie > 0 && !string.IsNullOrWhiteSpace(informacaoDescarga.Placa))
                        {
                            string serie = Utilidades.String.OnlyNumbers(informacaoDescarga.Serie.ToString("n0"));
                            informacaoDescarga.Carga = repCarga.BuscaCargPorVeiculoNotaFiscal(informacaoDescarga.Placa, informacaoDescarga.NotaFiscal, serie);
                        }

                        repInformacaoDescarga.Inserir(informacaoDescarga, Auditado);

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();

                            if (informacaoDescarga.Carga != null && informacaoDescarga.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                                EncerrarCarga(informacaoDescarga.Carga.Codigo, unitOfWork);
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
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
            grid.Prop("Data").Nome("Data").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Empresa").Nome("Empresa").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("NotaFiscal").Nome("Nota Fiscal").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Placa").Nome("Placa").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("PesoDescarga").Nome("Peso Descarregado").Tamanho(10).Align(Models.Grid.Align.right);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Logistica.InformacaoDescarga repInformacaoDescarga = new Repositorio.Embarcador.Logistica.InformacaoDescarga(unitOfWork);

            // Dados do filtro
            DateTime data = Request.GetDateTimeParam("Data");
            int notaFiscal = Request.GetIntParam("NotaFiscal");
            string placa = Request.GetStringParam("Placa");
            int codigoCarga = Request.GetIntParam("Carga");

            // Consulta
            List<Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga> listaGrid = repInformacaoDescarga.Consultar(data, notaFiscal, placa, codigoCarga, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repInformacaoDescarga.ContarConsulta(data, notaFiscal, placa, codigoCarga);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Data = obj.Data.ToString("dd/MM/yyyy"),
                            obj.Empresa,
                            obj.NotaFiscal,
                            obj.Placa,
                            PesoDescarga = obj.PesoDescarga.ToString("n4")
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga informacaoDescarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            // Vincula dados
            informacaoDescarga.Carga = Request.GetIntParam("Carga") > 0 ? repCarga.BuscarPorCodigo(Request.GetIntParam("Carga")) : null;
            informacaoDescarga.Data = Request.GetDateTimeParam("Data");
            informacaoDescarga.Empresa = Request.GetStringParam("Empresa");
            informacaoDescarga.Hora = Request.GetTimeParam("Hora");
            informacaoDescarga.NotaFiscal = Request.GetIntParam("NotaFiscal");
            informacaoDescarga.PesoDescarga = Request.GetDecimalParam("PesoDescarga");
            informacaoDescarga.Placa = Request.GetStringParam("Placa");
            informacaoDescarga.Serie = Request.GetIntParam("Serie");

            if (informacaoDescarga.Codigo == 0)
            {
                informacaoDescarga.DataImportacao = DateTime.Now;
                informacaoDescarga.Usuario = this.Usuario;
            }

            if (informacaoDescarga.Carga == null && informacaoDescarga.NotaFiscal > 0 && informacaoDescarga.Serie > 0 && !string.IsNullOrWhiteSpace(informacaoDescarga.Placa))
            {
                string serie = Utilidades.String.OnlyNumbers(informacaoDescarga.Serie.ToString("n0"));
                informacaoDescarga.Carga = repCarga.BuscaCargPorVeiculoNotaFiscal(informacaoDescarga.Placa, informacaoDescarga.NotaFiscal, serie);
            }
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga informacaoDescarga, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(informacaoDescarga.Placa))
            {
                msgErro = "Placa é obrigatório.";
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
        }

        private void EncerrarCarga(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFE = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFE.BuscarPorCarga(carga.Codigo);                

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte && !carga.CargaEmitidaParcialmente)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Solicitou encerramento da Carga", unitOfWork);

                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    if (cargaMDFEs.Any(obj => obj.MDFe != null && (ConfiguracaoEmbarcador.PermiteEncerrarMDFeEmitidoNoEmbarcador || obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe) && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao))
                    {
                        serCarga.SolicitarEncerramentoMDFeAutomaticamente(carga, unitOfWork, TipoServicoMultisoftware, WebServiceConsultaCTe, Auditado, this.Usuario);
                        //return new JsonpResult(true);
                    }
                    else
                    {
                        Repositorio.Embarcador.Cargas.CargaNFS repCargaNFS = new Repositorio.Embarcador.Cargas.CargaNFS(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> cargaNFSs = repCargaNFS.BuscarPorCarga(carga.Codigo);

                        if (cargaNFSs.Count == 0 || !cargaNFSs.Exists(obj => obj.NotaFiscalServico == null))
                        {

                            serCarga.LiberarSituacaoDeCargaFinalizada(carga, unitOfWork, TipoServicoMultisoftware, Auditado, this.Usuario);
                            unitOfWork.CommitChanges();
                            //return new JsonpResult(true);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            //return new JsonpResult(false, true, "Não é possível encerrar a carga, pois, existe(m) NFS(s) não informada(s).");
                        }
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    //if (!carga.CargaEmitidaParcialmente)
                    //    return new JsonpResult(false, true, "Não é possível encerrar a carga na atual situação (" + carga.DescricaoSituacaoCarga + ").");
                    //else
                    //    return new JsonpResult(false, true, "Não é possível encerrar uma carga que esteja parcialmente emitida.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                //return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados para encerramento do MDF-e.");
            }
        }

        #endregion

    }
}
