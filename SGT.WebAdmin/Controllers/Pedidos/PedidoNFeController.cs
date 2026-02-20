//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//
//using Microsoft.AspNetCore.Mvc;

//namespace SGT.WebAdmin.Controllers.Pedidos
//{
//    [CustomAuthorize("Pedidos/EnvioNFePedido")] 
//    public class PedidoNFeController : BaseController
//    {
//        [AllowAuthenticate]
//        public async Task<IActionResult> Pesquisa()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                int cargaPedido = int.Parse(Request.Params("CodigoCargaPedido"));
//                int codCarga = 0;

//                int.TryParse(Request.Params("Carga"), out codCarga);

//                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
//                grid.header = new List<Models.Grid.Head>();
//                grid.AdicionarCabecalho("Codigo", false);
//                grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.center, true);
//                grid.AdicionarCabecalho("Chave", "Chave", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Emitente", "Emitente", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Peso", "Peso", 7, Models.Grid.Align.left, true);
//                grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.left, true);

//                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

//                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

//                bool nfAtivas = true;
//                if (codCarga > 0)
//                {
//                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
//                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
//                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
//                        nfAtivas = false;
//                }


//                if (propOrdenar == "Emitente" || propOrdenar == "Destinatario")
//                    propOrdenar += ".Nome";

//                if (propOrdenar == "Destino")
//                    propOrdenar = "Destinatario.Localidade.Descricao";

//                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotaFiscal = repPedidoXMLNotaFiscal.BuscarXMLPorCargaPedido(cargaPedido, nfAtivas, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
//                grid.setarQuantidadeTotal(repPedidoXMLNotaFiscal.ContarXMLPorCargaPedido(cargaPedido, nfAtivas));
//                var dynXmlNotaFiscal = from obj in xmlNotaFiscal
//                                       select new
//                                       {
//                                           obj.Codigo,
//                                           obj.Numero,
//                                           obj.Chave,
//                                           Emitente = obj.Emitente.Nome + "(" + obj.Emitente.CPF_CNPJ_Formatado + ")",
//                                           Destinatario = obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")",
//                                           Destino = obj.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? obj.Destinatario.Localidade.DescricaoCidadeEstado : obj.Emitente.Localidade.DescricaoCidadeEstado,
//                                           obj.Valor,
//                                           obj.Peso
//                                       };

//                grid.AdicionaRows(dynXmlNotaFiscal);
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

//        public async Task<IActionResult> Adicionar()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                unitOfWork.Start();
//                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_conexao.StringConexao);
//                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(_conexao.StringConexao);
//                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

//                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
//                string chave = Request.Params("Chave").Replace(" ", "");

//                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento)int.Parse(Request.Params("TipoDocumento"));

//                if (chave.Length == 44 || tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros)
//                {
//                    if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros || serDocumento.ValidarChave(chave))
//                    {

//                        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
//                        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
//                        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
//                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

//                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);
//                        if (cargaPedido != null)
//                        {
//                            if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
//                            {

//                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

//                                xmlNotaFiscal.TipoDocumento = tipoDocumento;

//                                if (!string.IsNullOrWhiteSpace(Request.Params("BaseCalculoICMS")))
//                                    xmlNotaFiscal.BaseCalculoICMS = decimal.Parse(Request.Params("BaseCalculoICMS"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("BaseCalculoICMS")))
//                                    xmlNotaFiscal.BaseCalculoICMS = decimal.Parse(Request.Params("BaseCalculoICMS"));

//                                xmlNotaFiscal.Chave = chave;
//                                xmlNotaFiscal.CNPJTranposrtador = cargaPedido.Carga.Empresa.CNPJ_SemFormato;
//                                xmlNotaFiscal.Empresa = cargaPedido.Carga.Empresa;

//                                if (!string.IsNullOrWhiteSpace(Request.Params("DataEmissao")))
//                                {
//                                    DateTime dataEmissao;
//                                    DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
//                                    xmlNotaFiscal.DataEmissao = dataEmissao;
//                                }
//                                else
//                                    xmlNotaFiscal.DataEmissao = DateTime.Now;

//                                double destinatario = double.Parse(Request.Params("Destinatario"));
//                                if (destinatario > 0)
//                                {
//                                    Repositorio.Cliente repCliente = new Repositorio.Cliente(_conexao.StringConexao);
//                                    xmlNotaFiscal.Destinatario = repCliente.BuscarPorCPFCNPJ(destinatario);
//                                    if (cargaPedido.Pedido.Destinatario == null)
//                                    {
//                                        cargaPedido.Pedido.Destinatario = xmlNotaFiscal.Destinatario;
//                                        cargaPedido.Destino = xmlNotaFiscal.Destinatario.Localidade;
//                                        repPedido.Atualizar(cargaPedido.Pedido);
//                                    }
//                                }
//                                else
//                                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;

//                                xmlNotaFiscal.Filial = cargaPedido.Pedido.Filial;
//                                xmlNotaFiscal.Modelo = Request.Params("Modelo");
//                                xmlNotaFiscal.NaturezaOP = "";
//                                xmlNotaFiscal.nfAtiva = true;
//                                xmlNotaFiscal.Numero = int.Parse(Request.Params("Numero"));
//                                xmlNotaFiscal.Peso = decimal.Parse(Request.Params("Peso"));


//                                if (!string.IsNullOrWhiteSpace(Request.Params("PesoLiquido")))
//                                    xmlNotaFiscal.PesoLiquido = decimal.Parse(Request.Params("PesoLiquido"));

//                                xmlNotaFiscal.PlacaVeiculoNotaFiscal = cargaPedido.Carga.Veiculo.Placa;
//                                if (cargaPedido.Pedido.Remetente != null)
//                                    xmlNotaFiscal.Emitente = cargaPedido.Pedido.Remetente;
//                                else
//                                {
//                                    unitOfWork.Rollback();
//                                    return new JsonpResult(false, true, "Para informar uma nota manualmente é necessário informar o rementete do pedido");
//                                }

//                                xmlNotaFiscal.Serie = Request.Params("Serie");

//                                xmlNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
//                                xmlNotaFiscal.TipoOperacaoNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal)int.Parse(Request.Params("Tipo"));

//                                xmlNotaFiscal.Descricao = Request.Params("Descricao");

//                                xmlNotaFiscal.Valor = decimal.Parse(Request.Params("Valor"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorCOFINS")))
//                                    xmlNotaFiscal.ValorCOFINS = decimal.Parse(Request.Params("ValorCOFINS"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorDesconto")))
//                                    xmlNotaFiscal.ValorDesconto = decimal.Parse(Request.Params("ValorDesconto"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorICMS")))
//                                    xmlNotaFiscal.ValorICMS = decimal.Parse(Request.Params("ValorICMS"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorIPI")))
//                                    xmlNotaFiscal.ValorIPI = decimal.Parse(Request.Params("ValorIPI"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorPIS")))
//                                    xmlNotaFiscal.ValorPIS = decimal.Parse(Request.Params("ValorPIS"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorST")))
//                                    xmlNotaFiscal.ValorST = decimal.Parse(Request.Params("ValorST"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorTotalProdutos")))
//                                    xmlNotaFiscal.ValorTotalProdutos = decimal.Parse(Request.Params("ValorTotalProdutos"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("Volumes")))
//                                    xmlNotaFiscal.Volumes = int.Parse(Request.Params("Volumes"));

//                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorFrete")))
//                                    xmlNotaFiscal.ValorFrete = decimal.Parse(Request.Params("ValorFrete"));

//                                xmlNotaFiscal.XML = "";

//                                repXmlNotaFiscal.Inserir(xmlNotaFiscal);
//                                serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, cargaPedido, cargaPedido.Carga.FreteDeTerceiro ? cargaPedido.Carga.Veiculo.Proprietario : null, cargaPedido.Carga.Motoristas.ToList(), TipoServicoMultisoftware, unitOfWork);


//                                string retorno = tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe ? serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, unitOfWork) : "";
//                                if (string.IsNullOrEmpty(retorno))
//                                {
//                                    serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, unitOfWork);
//                                    unitOfWork.CommitChanges();
//                                    return new JsonpResult(true);
//                                }
//                                else
//                                {
//                                    unitOfWork.Rollback();
//                                    return new JsonpResult(false, true, retorno);
//                                }
//                            }
//                            else
//                            {
//                                unitOfWork.Rollback();
//                                return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite o envio de notas fiscais");
//                            }
//                        }
//                        else
//                        {
//                            unitOfWork.Rollback();
//                            return new JsonpResult(false, true, "Pedido não encontrado");
//                        }
//                    }
//                    else
//                    {
//                        unitOfWork.Rollback();
//                        return new JsonpResult(false, true, "A chave informada é inválida, por favor, verifique e tente novamente.");
//                    }
//                }
//                else
//                {
//                    unitOfWork.Rollback();
//                    return new JsonpResult(false, true, "A chave informada não contem 44 caracteres");
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

//        public async Task<IActionResult> Atualizar()
//        {
//            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

//            try
//            {
//                int codigo;
//                int.TryParse(Request.Params("Codigo"), out codigo);

//                decimal peso;
//                decimal.TryParse(Request.Params("Peso"), out peso);

//                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeTrabalho);

//                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(codigo);

//                if (xmlNotaFiscal == null)
//                    return new JsonpResult(false, true, "Nota fiscal não encontrada.");

//                xmlNotaFiscal.Peso = peso;

//                repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

//                return new JsonpResult(true);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);

//                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a nota fiscal.");
//            }
//            finally
//            {
//                unidadeTrabalho.Dispose();
//            }
//        }

//        public async Task<IActionResult> Excluir()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                unitOfWork.Start();

//                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_conexao.StringConexao);

//                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
//                int codXMLNotaFiscal = int.Parse(Request.Params("Codigo"));

//                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
//                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
//                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
//                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

//                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
//                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

//                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
//                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

//                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);

//                if (cargaPedido != null)
//                {
//                    if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
//                    {
//                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXmlNotaFiscal.BuscarPorCodigo(codXMLNotaFiscal);
//                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLsNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(codXMLNotaFiscal);

//                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoxmlNotaFiscal in pedidoXMLsNotasFiscais)
//                        {
//                            repPedidoXMLNotaFiscal.Deletar(pedidoxmlNotaFiscal);
//                        }

//                        serCanhoto.ExcluirCanhotoDaNotaFiscal(xmlNotaFiscal, unitOfWork);

//                        repXmlNotaFiscal.Deletar(xmlNotaFiscal);

//                        if (cargaPedido.Carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
//                        {
//                            int numeroNotas = repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo);
//                            if (numeroNotas == 0)
//                            {
//                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorPedido(cargaPedido.Pedido.Codigo);
//                                if (pedidosCTeParaSubContratacao.Count > 0)
//                                {
//                                    cargaPedido.Carga.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
//                                    cargaPedido.Pedido.PedidoSubContratado = true;
//                                    cargaPedido.Tomador = pedidosCTeParaSubContratacao.First().CTeTerceiro.TransportadorTerceiro;
//                                    cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
//                                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
//                                    repPedido.Atualizar(cargaPedido.Pedido);
//                                    repCarga.Atualizar(cargaPedido.Carga);
//                                    serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);
//                                }
//                                else
//                                {
//                                    cargaPedido.Carga.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
//                                    repCarga.Atualizar(cargaPedido.Carga);
//                                }
//                            }
//                        }
//                        unitOfWork.CommitChanges();
//                        return new JsonpResult(cargaPedido.Carga.TipoContratacaoCarga);
//                    }
//                    else
//                    {
//                        unitOfWork.Rollback();
//                        return new JsonpResult(false, true, "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite a exclusão de notas fiscais");
//                    }
//                }
//                else
//                {
//                    unitOfWork.Rollback();
//                    return new JsonpResult(false, true, "Pedido não encontrado");
//                }

//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();
//                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
//                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
//                else
//                {
//                    Servicos.Log.TratarErro(ex);
//                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
//                }
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        public async Task<IActionResult> ExcluirNotasFiscais()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

//            try
//            {
//                int codigoCargaPedido, codigoCarga;
//                int.TryParse(Request.Params("CargaPedido"), out codigoCargaPedido);
//                int.TryParse(Request.Params("Carga"), out codigoCarga);

//                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
//                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
//                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

//                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
//                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

//                if (cargaPedido == null)
//                    return new JsonpResult(true, false, "Pedido não encontrado.");

//                if (cargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
//                    return new JsonpResult(true, false, "Não é possível excluir as notas fiscais na situação atual da carga.");

//                unitOfWork.Start();

//                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in cargaPedido.Pedido.NotasFiscais)
//                {
//                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;

//                    repPedidoXMLNotaFiscal.Deletar(pedidoXMLNotaFiscal);

//                    serCanhoto.ExcluirCanhotoDaNotaFiscal(xmlNotaFiscal, unitOfWork);

//                    repXMLNotaFiscal.Deletar(xmlNotaFiscal);
//                }

//                unitOfWork.CommitChanges();

//                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
//                serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

//                return new JsonpResult(true);
//            }
//            catch (Exception ex)
//            {
//                unitOfWork.Rollback();

//                Servicos.Log.TratarErro(ex);

//                return new JsonpResult(false, "Ocorreu uma falha ao excluir as notas fiscais.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }
//    }
//}
