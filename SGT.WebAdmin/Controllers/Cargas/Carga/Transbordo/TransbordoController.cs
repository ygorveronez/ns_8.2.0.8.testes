//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.Cargas.Carga.Transbordo
//{
//    [CustomAuthorize(new string[] { "ConsultarMDFeTransbordo", "ConsultarCTesParaTransbordo" , "ConsultarCTesDoTransbordo" }, "Cargas/Transbordo")]
//    public class TransbordoController : BaseController
//    {
//        #region métodos Publicos

//        [AllowAuthenticate]
//        public async Task<IActionResult> Pesquisa()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
//                int CodigoCarga = 0;
//                int.TryParse(Request.Params("Carga"), out CodigoCarga);

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                grid.header = new List<Models.Grid.Head>();
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("Nº Transbordo", "NumeroTransbordo", 10, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Localidade do Transbordo", "LocalidadeTransbordo", 25, Models.Grid.Align.left, true);
//                if(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
//                    grid.AdicionarCabecalho("Veiculo", "Veiculo", 15, Models.Grid.Align.left, false);

//                grid.AdicionarCabecalho("Veiculo", "Veiculo", 15, Models.Grid.Align.left, false);
//                grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
//                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, false);

//                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

//                if (propOrdena == "LocalidadeTransbordo")
//                    propOrdena += ".Descricao";

//                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo situacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Todas;

//                List<Dominio.Entidades.Embarcador.Cargas.Transbordo> transbordos = repTransbordo.Consultar(CodigoCarga, situacaoTransbordo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repTransbordo.ContarConsulta(CodigoCarga, situacaoTransbordo));
//                var lista = (from p in transbordos
//                             select new
//                             {
//                                 p.Codigo,
//                                 p.NumeroTransbordo,
//                                 LocalidadeTransbordo = p.localidadeTransbordo.DescricaoCidadeEstado,
//                                 Veiculo = BuscarPlacas(p.Veiculo, p.VeiculosVinculados.ToList()),
//                                 Motorista = BuscarMotorista(p.Motoristas.ToList()),
//                                 Situacao = p.DescricaoSituacao,
//                                 DT_RowColor = (p.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.CancelamentoRejeitado || p.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Rejeicao) ? "rgba(193, 101, 101, 1)" : "",
//                                 DT_FontColor = (p.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.CancelamentoRejeitado || p.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Rejeicao) ? "#FFFFFF" : "",
//                             }).ToList();
//                grid.AdicionaRows(lista);
//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> EmitirNovamenteMDFe()
//        {

//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {

//                unitOfWork.Start();
//                int codigoMDFE, codigoEmpresa = 0;
//                int.TryParse(Request.Params("CodigoMDFE"), out codigoMDFE);
//                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

//                if (codigoMDFE > 0)
//                {
//                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
//                    Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
//                    Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
//                    Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork);
//                    Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unitOfWork);
//                    Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork);

//                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
//                    Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
//                    Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);

//                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFE, codigoEmpresa);

//                    if (mdfe != null)
//                    {
//                        if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
//                        {

//                            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
//                            DateTime dataEvento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
//                            mdfe.DataEmissao = dataEvento;
//                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;
//                            repMDFe.Atualizar(mdfe);
//                            unitOfWork.CommitChanges();

//                            bool sucesso = svcMDFe.Emitir(mdfe, mdfe.Empresa.Codigo);
//                            if (sucesso)
//                            {
//                                sucesso = serCargaMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, WebServiceConsultaCTe);
//                                if (!sucesso)
//                                {
//                                    return new JsonpResult(false, true, "Protocolo (" + mdfe.Codigo + ") não foi possível adicionar o MDF-e a fila de Envio, tente novamente.");
//                                }
//                            }
//                            else
//                            {
//                                return new JsonpResult(false, true, "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + mdfe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.");
//                            }
//                        }
//                        else
//                        {
//                            unitOfWork.Rollback();
//                            return new JsonpResult(false, true, "A atual situação do MDF-e (" + mdfe.DescricaoStatus + ") não permite sua emissão.");
//                        }
//                    }
//                    else
//                    {
//                        unitOfWork.Rollback();
//                        return new JsonpResult(false, true, "O MDF-e informado não foi localizado");
//                    }
//                }
//                return new JsonpResult(true);

//            }
//            catch (Exception ex)
//            {
//                if (unitOfWork.Transacao.IsActive)
//                    unitOfWork.Rollback();

//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o CT-e.");
//            }
//        }


//        public async Task<IActionResult> ConsultarMDFeTransbordo()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

//            try
//            {
//                Repositorio.Embarcador.Cargas.TransbordoMDFe repTransbordoMDFe = new Repositorio.Embarcador.Cargas.TransbordoMDFe(unitOfWork);
//                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);

//                int codigoTransbordo = int.Parse(Request.Params("Codigo"));

//                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigoTransbordo);

//                string statusCTe = Request.Params("Status");

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                grid.header = new List<Models.Grid.Head>();
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("CodigoMDFE", false);
//                grid.AdicionarCabecalho("CodigoEmpresa", false);
//                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);

//                grid.AdicionarCabecalho("UF Origem", "UFOrigem", 18, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("UF Destino", "UFDestino", 15, Models.Grid.Align.left, true);

//                grid.AdicionarCabecalho("Status", "Status", 15, Models.Grid.Align.left, true);

//                grid.AdicionarCabecalho("Retorno Sefaz", "MensagemRetornoSefaz", 8, Models.Grid.Align.right, true);


//                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

//                if (propOrdenacao == "UFDestino" || propOrdenacao == "UFOrigem")
//                    propOrdenacao += ".Sigla";

//                propOrdenacao = "MDFe." + propOrdenacao;

//                List<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe> mdfesTransbordo = repTransbordoMDFe.Consultar(codigoTransbordo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

//                grid.setarQuantidadeTotal(repTransbordoMDFe.ContarConsulta(codigoTransbordo));
//                var lista = (from obj in mdfesTransbordo
//                             select new
//                             {
//                                 obj.Codigo,
//                                 CodigoMDFE = obj.MDFe.Codigo,
//                                 CodigoEmpresa = obj.MDFe.Empresa.Codigo,
//                                 obj.MDFe.Numero,
//                                 Status = obj.MDFe.DescricaoStatus,
//                                 Serie = obj.MDFe.Serie.Numero,
//                                 UFOrigem = obj.MDFe.EstadoCarregamento.Sigla + " - " + obj.MDFe.EstadoCarregamento.Nome,
//                                 UFDestino = obj.MDFe.EstadoDescarregamento.Sigla + " - " + obj.MDFe.EstadoDescarregamento.Nome,
//                                 obj.MDFe.MensagemRetornoSefaz
//                             }).ToList();
//                grid.AdicionaRows(lista);

//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> ConsultarCTesParaTransbordo()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

//            try
//            {
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                string statusCTe = Request.Params("Status");

//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                string propOrdenacao = obterGridCTes(ref grid, carga);

//                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
//                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarCTes(carga.Codigo, 0, statusCTe, true, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
//                //List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarCTesParaTransbordo(carga.Codigo, statusCTe, true, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repCargaCTe.ContarConsultaCTes(carga.Codigo, 0, statusCTe, true));
//                grid.AdicionaRows(ObterListaObjetosCTe(cargaCTes));
//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> ConsultarCTesDoTransbordo()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

//            try
//            {
//                Repositorio.Embarcador.Cargas.Transbordo repCargaTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);


//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                int codigoTransbordo = int.Parse(Request.Params("Codigo"));
//                //string statusCTe = Request.Params("Status");

//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                string propOrdenacao = obterGridCTes(ref grid, carga);

//                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaTransbordo.ConsultarCTesTransbordo(codigoTransbordo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repCargaTransbordo.ContarConsultaCTesTransbordo(codigoTransbordo));
//                grid.AdicionaRows(ObterListaObjetosCTe(cargaCTes));
//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> GerarTransbordo()
//        {
//            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                unitOfWork.Start();

//                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

//                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
//                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
//                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
//                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

//                int codigoCarga, codigVeiculo, codigoLocalidade, codigoMotorista = 0;
//                int.TryParse(Request.Params("Carga"), out codigoCarga);
//                int.TryParse(Request.Params("Veiculo"), out codigVeiculo);
//                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
//                int.TryParse(Request.Params("LocalidadeTransbordo"), out codigoLocalidade);

//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
//                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigVeiculo);
//                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

//                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = new Dominio.Entidades.Embarcador.Cargas.Transbordo();

//                transbordo.Carga = carga;
//                transbordo.Veiculo = veiculo;
//                transbordo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>(veiculo.VeiculosVinculados);
//                transbordo.Motoristas = new List<Dominio.Entidades.Usuario>();
//                transbordo.Motoristas.Add(motorista);
//                transbordo.localidadeTransbordo = repLocalidade.BuscarPorCodigo(codigoLocalidade);
//                transbordo.NumeroTransbordo = repTransbordo.BuscarProximoCodigo();

//                DateTime dataTransbordo;
//                if (!DateTime.TryParseExact(Request.Params("DataTransbordo"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataTransbordo))
//                {
//                    dataTransbordo = DateTime.Now;
//                };
//                transbordo.DataTransbordo = dataTransbordo;
//                transbordo.MotivoTransbordo = Request.Params("MotivoTransbordo");
//                transbordo.SegmentoGrupoPessoas = veiculo.GrupoPessoas;
//                transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.EmEmissao;

//                dynamic dynCargaCTesTransbordados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));
//                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
//                bool selecionouTodas = bool.Parse(Request.Params("SelecionarTodos"));
//                if (!selecionouTodas)
//                {
//                    foreach (var dynCodigoCargaCTe in dynCargaCTesTransbordados)
//                    {
//                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo((int)dynCodigoCargaCTe.Codigo);
//                        cargaCTEs.Add(cargaCTe);
//                    }
//                }
//                else
//                {
//                    cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo);
//                    foreach (var dynCodigoCargaCTe in dynCargaCTesTransbordados)
//                    {
//                        int codigo = (int)dynCodigoCargaCTe.Codigo;
//                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = (from obj in cargaCTEs where obj.Codigo == codigo select obj).FirstOrDefault();
//                        if (cargaCTe != null)
//                            cargaCTEs.Remove(cargaCTe);
//                    }
//                }

//                transbordo.CargaCTesTransbordados = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>(cargaCTEs);

//                if (cargaCTEs.Count > 0)
//                {
//                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransbordo;

//                    repTransbordo.Inserir(transbordo);

//                    Servicos.Embarcador.Carga.MDFe serMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
//                    string retornoMDFe = serMDFe.EmitirMDFeTransbordo(transbordo, transbordo.CargaCTesTransbordados.ToList(), TipoServicoMultisoftware, WebServiceConsultaCTe, unitOfWork);
//                    if (retornoMDFe == "NaoPossuiMDFe")
//                    {
//                        if (transbordo.Veiculo.Tipo == "P")
//                            transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.EmTransporte;
//                        else
//                            transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.AgContratoFrete;

//                        repTransbordo.Atualizar(transbordo);
//                        unitOfWork.CommitChanges();
//                    }
//                    else
//                    {
//                        if (!string.IsNullOrWhiteSpace(retornoMDFe))
//                        {
//                            unitOfWork.Rollback();
//                            return new JsonpResult(false, true, retornoMDFe);
//                        }
//                    }

//                    serHubCarga.InformarTransbordoCargaAtualizada(transbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao, this.Usuario);
//                    return new JsonpResult(ObterTransbordo(transbordo, unitOfWork));
//                }
//                else
//                {
//                    unitOfWork.Rollback();
//                    return new JsonpResult(false, true, "É obrigatório selecionar ao menos um CT-e para o transbordo");
//                }
//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> CancelarTransbordo()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

//            try
//            {
//                unitOfWork.Start();
//                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
//                Repositorio.Embarcador.Cargas.TransbordoMDFe repTransbordoMDFe = new Repositorio.Embarcador.Cargas.TransbordoMDFe(unitOfWork);
//                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
//                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

//                Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
//                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

//                int codigo = int.Parse(Request.Params("Codigo"));
//                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
//                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigo);
//                if (transbordo.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.EmTransporte || transbordo.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.CancelamentoRejeitado)
//                {
//                    Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorTransbordo(transbordo.Codigo);
//                    if (contratoFrete != null)
//                    {
//                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorContratoFrete(contratoFrete.Codigo);

//                        if (titulo == null || titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
//                        {
//                            if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado || contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado)
//                            {
//                                contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado;
//                                repContratoFreteTerceiro.Atualizar(contratoFrete);

//                                if (titulo != null)
//                                    repTitulo.Deletar(titulo); //todo:desfazer movimentos

//                            }
//                            else
//                            {
//                                unitOfWork.Rollback();
//                                return new JsonpResult(false, true, "Não é possível cancelar o transbordo pois a atual situação do contrato não permite");
//                            }
//                        }
//                        else
//                        {
//                            unitOfWork.Rollback();
//                            return new JsonpResult(false, true, "Não é Cancelar o Transbordo, pois o título com o terceiro já foi quitado");
//                        }
//                    }


//                    List<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe> transbordoMDFes = repTransbordoMDFe.BuscarPorTransbordo(transbordo.Codigo);
//                    List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfeParaFila = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

//                    if (transbordoMDFes.Count > 0)
//                    {
//                        transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.EmCancelamento;

//                        foreach (Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe transbordoMDFe in transbordoMDFes)
//                        {
//                            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(transbordoMDFe.MDFe.Empresa.FusoHorario);
//                            DateTime dataEvento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

//                            if (transbordoMDFe.MDFe.DataAutorizacao >= dataEvento.AddDays(-1))
//                            {
//                                if (transbordoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
//                                {
//                                    if (serMDFe.Cancelar(transbordoMDFe.MDFe.Codigo, transbordo.Carga.Empresa.Codigo, "Solcitado o cancelamento do Transbordo gerado", unitOfWork, dataEvento))
//                                        mdfeParaFila.Add(transbordoMDFe.MDFe);
//                                    else
//                                    {
//                                        unitOfWork.Rollback();
//                                        return new JsonpResult(false, true, "Não foi possível cancelar o MDF-e.");
//                                    }
//                                }
//                                else
//                                {
//                                    unitOfWork.Rollback();
//                                    return new JsonpResult(false, true, "A atual situação do MDF-e não permite seu cancelamento.");
//                                }
//                            }
//                            else
//                            {
//                                if (transbordoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
//                                {
//                                    transbordoMDFe.MDFe.MunicipioEncerramento = transbordo.localidadeTransbordo;
//                                    repMDFe.Atualizar(transbordoMDFe.MDFe);

//                                    if (serMDFe.Encerrar(transbordoMDFe.MDFe.Codigo, transbordo.Carga.Empresa.Codigo, DateTime.Now, unitOfWork, dataEvento))
//                                        mdfeParaFila.Add(transbordoMDFe.MDFe);
//                                    else
//                                    {
//                                        unitOfWork.Rollback();
//                                        return new JsonpResult(false, true, "Não foi possível encerrar o MDF-e.");
//                                    }
//                                }
//                                else
//                                {
//                                    unitOfWork.Rollback();
//                                    return new JsonpResult(false, true, "A atual situação do MDF-e não permite seu encerramento.");
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Cancelado;
//                    }

//                    unitOfWork.CommitChanges();

//                    foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfeParaFila)
//                    {
//                        if (!serCargaMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, WebServiceConsultaCTe))
//                            return new JsonpResult(false, true, "Não foi possível adicionar o MDFe a fila.");
//                    }
//                    serHubCarga.InformarTransbordoCargaAtualizada(transbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao, this.Usuario);
//                    return new JsonpResult(ObterTransbordo(transbordo, unitOfWork));
//                }
//                else
//                {
//                    unitOfWork.Rollback();
//                    return new JsonpResult(false, true, "Não é possível solicitar o cancelamento do transbordo em sua atual situação");
//                }

//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha solicitar o cancelamento.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> BuscarPorCodigo()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                int codigo = int.Parse(Request.Params("Codigo"));
//                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
//                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigo);
//                return new JsonpResult(ObterTransbordo(transbordo, unitOfWork));
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }

//        }

//        public async Task<IActionResult> SalvarFreteTerceiroTransbordo()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                int codigo = int.Parse(Request.Params("Transbordo"));
//                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
//                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

//                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigo);

//                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorTransbordo(transbordo.Codigo);

//                bool inserir = false;
//                if (contratoFrete == null)
//                {
//                    inserir = true;
//                    contratoFrete = new Dominio.Entidades.Embarcador.Terceiros.ContratoFrete(unitOfWork);
//                    contratoFrete.Carga = transbordo.Carga;
//                    contratoFrete.NumeroContrato = repContratoFreteTerceiro.BuscarProximoCodigo();
//                    contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao;
//                }

//                contratoFrete.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
//                contratoFrete.DataEmissaoContrato = DateTime.Now;
//                contratoFrete.TransportadorTerceiro = transbordo.Veiculo.Proprietario;
//                contratoFrete.Usuario = this.Usuario;
//                contratoFrete.Transbordo = transbordo;
//                decimal descontos, percentualAdiantamentoFretesTerceiro = 0, valorOutrosAdiantamento = 0, valorFreteSubcontratacao = 0;
//                decimal.TryParse(Request.Params("PercentualAdiantamento"), out percentualAdiantamentoFretesTerceiro);
//                decimal.TryParse(Request.Params("ValorOutrosAdiantamento"), out valorOutrosAdiantamento);
//                decimal.TryParse(Request.Params("ValorFreteSubcontratacao"), out valorFreteSubcontratacao);

//                decimal.TryParse(Request.Params("Descontos"), out descontos);
//                contratoFrete.ValorFreteSubcontratacao = valorFreteSubcontratacao;

//                contratoFrete.Descontos = descontos;
//                contratoFrete.PercentualAdiantamento = percentualAdiantamentoFretesTerceiro;
//                decimal valorTotal = contratoFrete.ValorFreteSubcontratacao + contratoFrete.ValorPedagio - contratoFrete.Descontos;
//                contratoFrete.ValorAdiantamento = (valorTotal * percentualAdiantamentoFretesTerceiro) / 100;

//                contratoFrete.ValorOutrosAdiantamento = valorOutrosAdiantamento;
//                contratoFrete.Observacao = Request.Params("Observacao");

//                if (inserir)
//                    repContratoFreteTerceiro.Inserir(contratoFrete);
//                else
//                    repContratoFreteTerceiro.Atualizar(contratoFrete);

//                return new JsonpResult(ObterTransbordo(transbordo, unitOfWork));
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        #endregion

//        #region Métodos Privados

//        private string obterGridCTes(ref Models.Grid.Grid grid, Dominio.Entidades.Embarcador.Cargas.Carga carga)
//        {

//            grid.header = new List<Models.Grid.Head>();
//            grid.AdicionarCabecalho("Codigo", false);
//            grid.AdicionarCabecalho("SituacaoCTe", false);
//            grid.AdicionarCabecalho("CodigoCTE", false);
//            grid.AdicionarCabecalho("CodigoEmpresa", false);
//            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
//            grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
//            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
//            {
//                grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
//            }

//            if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
//            {
//                grid.AdicionarCabecalho("T. Serviço", "DescricaoTipoServico", 10, Models.Grid.Align.center, true);
//            }

//            grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
//            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
//            grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
//            grid.AdicionarCabecalho("Alíquota", "Aliquota", 5, Models.Grid.Align.right, false);

//            string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

//            if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
//                propOrdenacao += ".Nome";
//            if (propOrdenacao == "Destino")
//                propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

//            if (propOrdenacao == "DescricaoTipoPagamento")
//                propOrdenacao = "TipoPagamento";

//            if (propOrdenacao == "DescricaoTipoServico")
//                propOrdenacao = "TipoServico";

//            propOrdenacao = "CTe." + propOrdenacao;

//            return propOrdenacao;
//        }

//        private dynamic ObterListaObjetosCTe(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes)
//        {
//            var lista = (from obj in cargaCTes
//                         select new
//                         {
//                             obj.Codigo,
//                             CodigoCTE = obj.CTe.Codigo,
//                             obj.CTe.DescricaoTipoServico,
//                             CodigoEmpresa = obj.CTe.Empresa.Codigo,
//                             obj.CTe.Numero,
//                             SituacaoCTe = obj.CTe.Status,
//                             Serie = obj.CTe.Serie.Numero,
//                             obj.CTe.DescricaoTipoPagamento,
//                             Remetente = obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")",
//                             Destinatario = obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")",
//                             Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
//                             ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
//                             Aliquota = obj.CTe.AliquotaICMS.ToString("n2"),
//                         }).ToList();

//            return lista;
//        }

//        private dynamic ObterTransbordo(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Repositorio.UnitOfWork unitOfWork)
//        {
//            Repositorio.Embarcador.Cargas.TransbordoMDFe repTransbordoMDFe = new Repositorio.Embarcador.Cargas.TransbordoMDFe(unitOfWork);
//            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
//            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

//            int numeroMDFe = repTransbordoMDFe.ContarConsulta(transbordo.Codigo);
//            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorTransbordo(transbordo.Codigo);

//            var dynTransbordo = new
//            {
//                PossuiContrato = transbordo.Veiculo.Tipo == "T" ? true : false,
//                ContratoFrete = serContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unitOfWork),
//                transbordo.Codigo,
//                DataTransbordo = transbordo.DataTransbordo.ToString("dd/MM/yyyy HH:mm:ss"),
//                transbordo.NumeroTransbordo,
//                Motoristas = (from obj in transbordo.Motoristas.ToList()
//                              select new
//                              {
//                                  Codigo = obj.Codigo,
//                                  Descricao = obj.Nome
//                              }).ToList(),
//                Motorista = new { Codigo = transbordo.Motoristas.FirstOrDefault().Codigo, Descricao = BuscarMotorista(transbordo.Motoristas.ToList()) },
//                Veiculo = new { Codigo = transbordo.Veiculo.Codigo, Descricao = BuscarPlacas(transbordo.Veiculo, transbordo.VeiculosVinculados.ToList()) },
//                transbordo.DescricaoSituacao,
//                LocalidadeTransbordo = new { Codigo = transbordo.localidadeTransbordo.Codigo, Descricao = transbordo.localidadeTransbordo.DescricaoCidadeEstado },
//                transbordo.MotivoTransbordo,
//                transbordo.SituacaoTransbordo,
//                CargaCTesTransbordados = (from obj in transbordo.CargaCTesTransbordados
//                                          select new
//                                          {
//                                              obj.Codigo
//                                          }).ToList(),
//                Carga = transbordo.Carga.Codigo,
//                PossuiMDFe = numeroMDFe > 0 ? true : false
//            };

//            return dynTransbordo;
//        }

//        private string BuscarMotorista(List<Dominio.Entidades.Usuario> motoristas)
//        {
//            Dominio.Entidades.Usuario ultimoMotorista = motoristas.Last();
//            string strMotorista = "";
//            foreach (Dominio.Entidades.Usuario motorista in motoristas)
//            {
//                strMotorista += motorista.Nome;
//                if (ultimoMotorista.Codigo != motorista.Codigo)
//                {
//                    strMotorista += ",";
//                }
//            }
//            return strMotorista;
//        }

//        private string BuscarPlacas(Dominio.Entidades.Veiculo tracao, List<Dominio.Entidades.Veiculo> reboques)
//        {
//            string strPlacas = tracao.Placa;
//            if (reboques.Count > 0)
//            {
//                Dominio.Entidades.Veiculo ultimoReboque = reboques.Last();
//                foreach (Dominio.Entidades.Veiculo reboque in reboques)
//                {
//                    strPlacas += "/";
//                    strPlacas += reboque.Placa;
//                    if (ultimoReboque.Codigo != reboque.Codigo)
//                    {
//                        strPlacas += "/";
//                    }
//                }
//            }

//            return strPlacas;
//        }

//        #endregion

//    }
//}
