//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosCarga
//{
//    [CustomAuthorize(new string[] {
//        "BuscarLeilaoPorCarga",
//        "BuscarTransportadoresSugeridosParaLeilao" ,
//        "BuscarTransportadoresParticipantesLeilao",
//        "BuscarLancesDoLeilao"},
//       "Cargas/Carga", "Logistica/JanelaCarregamento")]
//    public class CargaLeilaoController : BaseController
//    {
//        #region Buscar dados dos leilões

//        public async Task<IActionResult> BuscarLeilaoPorCarga()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(_conexao.StringConexao);

//                Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(codigoCarga);
//                if (cargaLeilao != null)
//                {
//                    var retorno = new
//                    {
//                        cargaLeilao.Leilao.Codigo,
//                        cargaLeilao.Leilao.SituacaoLeilao,
//                        DataInicioLeilao = cargaLeilao.Leilao.DataInicioLeilao != null ? cargaLeilao.Leilao.DataInicioLeilao.Value.ToString("dd/MM/yyyy hh:mm") : "",
//                        DataParaEncerramentoLeilao = cargaLeilao.Leilao.DataParaEncerramentoLeilao != null ? cargaLeilao.Leilao.DataParaEncerramentoLeilao.Value.ToString("dd/MM/yyyy hh:mm") : "",
//                        DataDeEncerramentoLeilao = cargaLeilao.Leilao.DataDeEncerramentoLeilao != null ? cargaLeilao.Leilao.DataDeEncerramentoLeilao.Value.ToString("dd/MM/yyyy hh:mm") : "",
//                        VencedorDoLeilao = cargaLeilao.LeilaoParticipanteEscolhido != null ? new { cargaLeilao.LeilaoParticipanteEscolhido.Empresa.Codigo, Descricao = cargaLeilao.LeilaoParticipanteEscolhido.Empresa.RazaoSocial } : null,
//                        ValorDoLanceVencedor = cargaLeilao.ValorLance
//                    };
//                    return new JsonpResult(retorno);
//                }
//                else
//                {
//                    return new JsonpResult(null);
//                }
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        public async Task<IActionResult> BuscarTransportadoresSugeridosParaLeilao()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_conexao.StringConexao);
//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                grid.header = new List<Models.Grid.Head>();
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("Razão Social", "RazaoSocial", 32, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("CNPJ", "CNPJ", 13, Models.Grid.Align.center, false);
//                grid.AdicionarCabecalho("Telefone", "Telefone", 9, Models.Grid.Align.left, false);
//                grid.AdicionarCabecalho("Localidade", "Localidade", 23, Models.Grid.Align.left, false, false, true);

//                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
//                if (propOrdenacao == "RazaoSocial")
//                {
//                    propOrdenacao = "Empresa.RazaoSocial";
//                }

//                List<Dominio.Entidades.Empresa> listaEmpresa = repCarga.BuscarEmbarcadoresSugeridosParaCarga(carga, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repCarga.ContarEmbarcadoresSugeridosParaCarga(carga));

//                dynamic lista = (from p in listaEmpresa select new { p.Codigo, p.DescricaoStatus, p.Telefone, p.RazaoSocial, CNPJ = p.CNPJ_Formatado, Localidade = p.Localidade.DescricaoCidadeEstado }).ToList();

//                grid.AdicionaRows(lista);
//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        public async Task<IActionResult> BuscarTransportadoresParticipantesLeilao()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(_conexao.StringConexao);
//                Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(codigoCarga);

//                int leilaoCodigo = 0;
//                if (cargaLeilao != null)
//                    leilaoCodigo = cargaLeilao.Leilao.Codigo;

//                Repositorio.Embarcador.Cargas.LeilaoParticipante repCargaLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(_conexao.StringConexao);
//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                grid.header = new List<Models.Grid.Head>();
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("Razão Social", "RazaoSocial", 32, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("CNPJ", "CNPJ", 13, Models.Grid.Align.center, false);
//                grid.AdicionarCabecalho("Telefone", "Telefone", 9, Models.Grid.Align.left, false);
//                grid.AdicionarCabecalho("Localidade", "Localidade", 23, Models.Grid.Align.left, false, false, true);

//                string propOrdenacao = "Empresa." + grid.header[grid.indiceColunaOrdena].data;

//                List<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante> leilaoParticipantes = repCargaLeilaoParticipante.ConsultarParticipantes(leilaoCodigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repCargaLeilaoParticipante.ContarConsultaParticipantes(leilaoCodigo));

//                dynamic lista = (from p in leilaoParticipantes
//                                 select new
//                                 {
//                                     p.Codigo,
//                                     p.Empresa.RazaoSocial,
//                                     CNPJ = p.Empresa.CNPJ_Formatado,
//                                     p.Empresa.Telefone,
//                                     Localidade = p.Empresa.Localidade.DescricaoCidadeEstado
//                                 }).ToList();

//                grid.AdicionaRows(lista);
//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        public async Task<IActionResult> BuscarLancesDoLeilao()
//        {
//            try
//            {
//                int codigoLeilao = int.Parse(Request.Params("CodigoLeilao"));
//                Repositorio.Embarcador.Cargas.Leilao repLeilao = new Repositorio.Embarcador.Cargas.Leilao(_conexao.StringConexao);
//                Repositorio.Embarcador.Cargas.LeilaoParticipante repCargaLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(_conexao.StringConexao);

//                Dominio.Entidades.Embarcador.Cargas.Leilao leilao = repLeilao.BuscarPorCodigo(codigoLeilao);

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                grid.header = new List<Models.Grid.Head>();
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("CodigoEmpresa", false);
//                grid.AdicionarCabecalho("Razão Social", "RazaoSocial", 32, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Valor do Lance", "ValorLance", 13, Models.Grid.Align.right, true);
//                grid.AdicionarCabecalho("Data do Lance", "DataLance", 9, Models.Grid.Align.center, true);

//                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
//                if (propOrdenacao == "RazaoSocial")
//                {
//                    propOrdenacao = "Empresa." + propOrdenacao;
//                }

//                List<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante> leilaoParticipantes = repCargaLeilaoParticipante.ConsultarLances(leilao.Codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repCargaLeilaoParticipante.ContarLances(leilao.Codigo));

//                dynamic lista = (from p in leilaoParticipantes
//                                select new
//                                {
//                                    p.Codigo,
//                                    CodigoEmpresa = p.Empresa.Codigo,
//                                    p.Empresa.RazaoSocial,
//                                    ValorLance = p.ValorLance.ToString("n2"),
//                                    DataLance = p.DataLance.Value.ToString("dd/MM/yyyy hh:mm")
//                                }).ToList();

//                grid.AdicionaRows(lista);
//                return new JsonpResult(grid);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        #endregion

//        #region Gerencimento dos participantes

//        public async Task<IActionResult> AdicionarTodasSugeridasNoLeilao()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_conexao.StringConexao);
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(_conexao.StringConexao);
//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);


//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {

//                    Repositorio.Embarcador.Cargas.LeilaoParticipante repCargaLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(_conexao.StringConexao);
//                    Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(carga.Codigo);
//                    if (cargaLeilao != null)
//                    {
//                        if (cargaLeilao.Leilao.SituacaoLeilao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.novo)
//                        {
//                            List<Dominio.Entidades.Empresa> empresas = repCarga.BuscarEmbarcadoresSugeridosParaCarga(carga);
//                            foreach (Dominio.Entidades.Empresa empresa in empresas)
//                            {
//                                Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante cargaParticipanteLeilao = repCargaLeilaoParticipante.BuscarPorEmpresa(cargaLeilao.Leilao.Codigo, empresa.Codigo);
//                                if (cargaParticipanteLeilao == null)
//                                {
//                                    cargaParticipanteLeilao = new Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante();
//                                    cargaParticipanteLeilao.Leilao = cargaLeilao.Leilao;
//                                    cargaParticipanteLeilao.Empresa = empresa;
//                                    repCargaLeilaoParticipante.Inserir(cargaParticipanteLeilao);
//                                }
//                            }
//                            return new JsonpResult(true);
//                        }
//                        else
//                        {
//                            return new JsonpResult(false, true, "Não é possível adicionar participantes em um leilão fechado.");
//                        }
//                    }
//                    else
//                    {
//                        return new JsonpResult(false, true, "Não existe um leilão criado.");
//                    }
//                }
//                else
//                {
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }

//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }


//        public async Task<IActionResult> AdicionarParticipanteNoLeilao()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_conexao.StringConexao);
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(_conexao.StringConexao);
//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

//                Repositorio.Embarcador.Cargas.LeilaoParticipante repCargaLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(_conexao.StringConexao);

//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {
//                    Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(carga.Codigo);
//                    if (cargaLeilao != null)
//                    {
//                        if (cargaLeilao.Leilao.SituacaoLeilao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.novo)
//                        {
//                            int empresa = int.Parse(Request.Params("Empresa"));
//                            Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante cargaParticipanteLeilao = repCargaLeilaoParticipante.BuscarPorEmpresa(cargaLeilao.Leilao.Codigo, empresa);
//                            if (cargaParticipanteLeilao == null)
//                            {
//                                cargaParticipanteLeilao = new Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante();
//                                cargaParticipanteLeilao.Leilao = cargaLeilao.Leilao;
//                                cargaParticipanteLeilao.Empresa = new Dominio.Entidades.Empresa() { Codigo = empresa };
//                                repCargaLeilaoParticipante.Inserir(cargaParticipanteLeilao);
//                                return new JsonpResult(true);
//                            }
//                            else
//                            {
//                                return new JsonpResult(false, true, "O Transportador " + cargaParticipanteLeilao.Empresa.RazaoSocial + " já está participando deste leilão.");
//                            }
//                        }
//                        else
//                        {
//                            return new JsonpResult(false, true, "Não é possível adicionar participantes em um leilão fechado.");
//                        }
//                    }
//                    else
//                    {
//                        return new JsonpResult(false, true, "Não existe um leilão criado.");
//                    }
//                }
//                else
//                {
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }

//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }


//        public async Task<IActionResult> RemoverParticipanteNoLeilao()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_conexao.StringConexao);
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(_conexao.StringConexao);
//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
//                Repositorio.Embarcador.Cargas.LeilaoParticipante repCargaLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(_conexao.StringConexao);

//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {
//                    Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(carga.Codigo);
//                    if (cargaLeilao != null)
//                    {
//                        if (cargaLeilao.Leilao.SituacaoLeilao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.novo)
//                        {
//                            int codigoCargaLeilaoParticipante = int.Parse(Request.Params("CodigoCargaLeilaoParticipante"));
//                            Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante cargaParticipanteLeilao = repCargaLeilaoParticipante.BuscarPorCodigo(codigoCargaLeilaoParticipante);
//                            if (cargaParticipanteLeilao != null)
//                            {
//                                repCargaLeilaoParticipante.Deletar(cargaParticipanteLeilao);
//                                return new JsonpResult(true);
//                            }
//                            else
//                            {
//                                return new JsonpResult(false, true, "O Transportador não foi localizado no leilão.");
//                            }
//                        }
//                        else
//                        {
//                            return new JsonpResult(false, true, "Não é remover os participantes em um leilão fechado.");
//                        }
//                    }
//                    else
//                    {
//                        return new JsonpResult(false, true, "Não existe um leilão criado.");
//                    }
//                }
//                else
//                {
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }

//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        #endregion

//        #region Gerenciamento do Leilao


//        public async Task<IActionResult> CriarLeilao()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                unitOfWork.Start();
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(unitOfWork);
//                Repositorio.Embarcador.Cargas.Leilao repLeilao = new Repositorio.Embarcador.Cargas.Leilao(unitOfWork);
//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
//                Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(carga.Codigo);

//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {

//                    if (cargaLeilao == null)
//                    {
//                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
//                        Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao repLeilaoTipoOperacaoConfig = new Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao(unitOfWork);

//                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCarga(carga.Codigo).FirstOrDefault();
//                        Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao leilaoTipoOperacaoConfig = repLeilaoTipoOperacaoConfig.BuscarPorTipoOperacao(cargaPedido.Pedido.TipoOperacaoEmissao);
//                        if (leilaoTipoOperacaoConfig != null && leilaoTipoOperacaoConfig.PermiteLeilao)
//                        {
//                            Dominio.Entidades.Embarcador.Cargas.Leilao leilao = new Dominio.Entidades.Embarcador.Cargas.Leilao();
//                            leilao.ValorInicial = carga.ValorFreteAPagar;
//                            leilao.SituacaoLeilao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.novo;

//                            repLeilao.Inserir(leilao);
//                            cargaLeilao = new Dominio.Entidades.Embarcador.Cargas.CargaLeilao();
//                            cargaLeilao.Carga = carga;

//                            cargaLeilao.Leilao = leilao;
//                            repCargaLeilao.Inserir(cargaLeilao);
//                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmLeilao;
//                            carga.Operador = this.Usuario;
//                            repCarga.Atualizar(carga);
//                            unitOfWork.CommitChanges();
//                            return new JsonpResult(true);
//                        }
//                        else
//                        {
//                            unitOfWork.Rollback();
//                            return new JsonpResult(false, true, "O tipo de operação do pedido não permite leilão");
//                        }
//                    }
//                    else
//                    {
//                        unitOfWork.Rollback();
//                        return new JsonpResult(false, true, "Já existe um leilão criado para essa carga");
//                    }
//                }
//                else
//                {
//                    unitOfWork.Rollback();
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }
//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        public async Task<IActionResult> IniciarLeilao()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(_conexao.StringConexao);
//                Repositorio.Embarcador.Cargas.Leilao repLeilao = new Repositorio.Embarcador.Cargas.Leilao(_conexao.StringConexao);
//                Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(codigoCarga);

//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, cargaLeilao.Carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {
//                    cargaLeilao.Leilao.DataInicioLeilao = DateTime.Now;
//                    cargaLeilao.Leilao.MenorLance = 0;
//                    cargaLeilao.Leilao.SituacaoLeilao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.iniciado;

//                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_conexao.StringConexao);
//                    Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao repLeilaoTipoOperacaoConfig = new Repositorio.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao(_conexao.StringConexao);

//                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCarga(cargaLeilao.Carga.Codigo).FirstOrDefault();
//                    Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao leilaoTipoOperacaoConfig = repLeilaoTipoOperacaoConfig.BuscarPorTipoOperacao(cargaPedido.Pedido.TipoOperacaoEmissao);

//                    if (leilaoTipoOperacaoConfig.LimiteTempoLeilaoEmHoras > 0)
//                    {
//                        cargaLeilao.Leilao.DataParaEncerramentoLeilao = DateTime.Now.AddHours(leilaoTipoOperacaoConfig.LimiteTempoLeilaoEmHoras);
//                    }

//                    repLeilao.Atualizar(cargaLeilao.Leilao);

//                    var retorno = new
//                    {
//                        cargaLeilao.Leilao.Codigo,
//                        DataInicioLeilao = cargaLeilao.Leilao.DataInicioLeilao.Value.ToString("dd/MM/yyyy hh:mm"),
//                        DataParaEncerramentoLeilao = cargaLeilao.Leilao.DataParaEncerramentoLeilao != null ? cargaLeilao.Leilao.DataParaEncerramentoLeilao.Value.ToString("dd/MM/yyyy hh:mm") : ""
//                    };

//                    return new JsonpResult(retorno);
//                }
//                else
//                {
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }

//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }



//        public async Task<IActionResult> CancelarLeilao()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                unitOfWork.Start();
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.Leilao repLeilao = new Repositorio.Embarcador.Cargas.Leilao(unitOfWork);
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(unitOfWork);
//                Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(codigoCarga);
//                cargaLeilao.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
//                cargaLeilao.Leilao.SituacaoLeilao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.cancelado;

//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, cargaLeilao.Carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {
//                    repCarga.Atualizar(cargaLeilao.Carga);
//                    repLeilao.Atualizar(cargaLeilao.Leilao);
//                    unitOfWork.CommitChanges();
//                    return new JsonpResult(true);
//                }
//                else
//                {
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }
//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        public async Task<IActionResult> FinalizarLeilao()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                unitOfWork.Start();

//                int codigoLeilao = int.Parse(Request.Params("CodigoLeilao"));
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                int codLance = int.Parse(Request.Params("Lance"));
//                Repositorio.Embarcador.Cargas.Leilao repLeilao = new Repositorio.Embarcador.Cargas.Leilao(unitOfWork);
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
//                Repositorio.Embarcador.Cargas.LeilaoParticipante repLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(unitOfWork);
//                Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(unitOfWork);
//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {

//                    Dominio.Entidades.Embarcador.Cargas.Leilao leilao = repLeilao.BuscarPorCodigo(codigoLeilao);
//                    Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante leilaoParticipante = repLeilaoParticipante.BuscarPorCodigo(codLance);

//                    if (leilaoParticipante != null && leilaoParticipante.ValorLance > 0)
//                    {
//                        Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorCarga(codigoCarga);
//                        cargaLeilao.LeilaoParticipanteEscolhido = leilaoParticipante;
//                        cargaLeilao.ValorLance = leilaoParticipante.ValorLance;
//                        Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(_conexao.StringConexao, TipoServicoMultisoftware);
//                        carga = serFrete.RecalcularFrete(carga, TipoServicoMultisoftware, cargaLeilao.ValorLance, unitOfWork);
//                        carga.Empresa = leilaoParticipante.Empresa;
//                        carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Leilao;
//                        carga.ValorFreteLeilao = carga.ValorFreteAPagar;
//                        repCargaLeilao.Atualizar(cargaLeilao);
//                    }

//                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
//                    leilao.SituacaoLeilao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.encerrado;
//                    leilao.DataDeEncerramentoLeilao = DateTime.Now;
//                    repCarga.Atualizar(carga);
//                    repLeilao.Atualizar(leilao);
//                    unitOfWork.CommitChanges();
//                    return new JsonpResult(true);
//                }
//                else
//                {
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }

//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        public async Task<IActionResult> NaoUsarLeilao()
//        {
//            try
//            {
//                int codigoCarga = int.Parse(Request.Params("Carga"));
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_conexao.StringConexao);
//                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
//                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;

//                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
//                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
//                if (string.IsNullOrWhiteSpace(retornoVerificarOperador))
//                {
//                    carga.Operador = this.Usuario;
//                    repCarga.Atualizar(carga);
//                    return new JsonpResult(true);
//                }
//                else
//                {
//                    return new JsonpResult(false, true, retornoVerificarOperador);
//                }
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
//            }
//        }

//        #endregion

//    }
//}
