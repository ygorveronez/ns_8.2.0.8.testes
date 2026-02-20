using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.SAC
{
    [CustomAuthorize(new string[] { "Imprimir", "FaturasDocumento", "OcorrenciasDocumento", "CargasDocumento" }, "SAC/AtendimentoCliente")]
    public class AtendimentoClienteController : BaseController
    {
		#region Construtores

		public AtendimentoClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.Prop("Codigo");
                grid.Prop("CodigoCTe");
                grid.Prop("Numero").Nome("Número").Align(Models.Grid.Align.right).Tamanho(10);
                grid.Prop("Serie").Nome("Série").Align(Models.Grid.Align.right).Tamanho(8);
                grid.Prop("ModeloDocumentoFiscal").Nome("Mód. Doc.").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("NumeroNotas").Nome("Nota(s) Fiscai(s)").Tamanho(15).Ord(false);
                grid.Prop("NumerosSolicitacoes").Nome("Nº Solicitação(ões)").Tamanho(15).Ord(false);
                grid.Prop("DescricaoTipoPagamento").Nome("T. Pagamento").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("Empresa").Nome("Transportadora").Tamanho(15);
                grid.Prop("Remetente").Nome("Remetente").Tamanho(20);
                grid.Prop("Destinatario").Nome("Destinatário").Tamanho(20);
                grid.Prop("LocalidadeTerminoPrestacao").Nome("Destino").Tamanho(10);
                grid.Prop("ValorAReceber").Nome("Valor a Receber").Align(Models.Grid.Align.right).Tamanho(10).Ord(false);
                grid.Prop("AliquotaICMS").Nome("Alíquota").Align(Models.Grid.Align.right).Tamanho(8).Ord(false);
                grid.Prop("DescricaoStatus").Nome("Status").Align(Models.Grid.Align.center).Tamanho(10).Ord(false).Visibilidade(true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                DateTime dataEmissaoNotaDe, dataEmissaoNotaAte, dataEmissaoCTeDe, dataEmissaoCTeAte;
                DateTime.TryParse(Request.Params("DataEmissaoNotaDe"), out dataEmissaoNotaDe);
                DateTime.TryParse(Request.Params("DataEmissaoNotaAte"), out dataEmissaoNotaAte);
                DateTime.TryParse(Request.Params("DataEmissaoCTeDe"), out dataEmissaoCTeDe);
                DateTime.TryParse(Request.Params("DataEmissaoCTeAte"), out dataEmissaoCTeAte);

                int serieNota = 0, numeroCTeDe = 0, numeroCTeAte = 0, serieCTe = 0, numeroFaturaDe = 0, numeroFaturaAte = 0, numeroPreFatura = 0;
                //int.TryParse(Request.Params("NumeroNotaDe"), out numeroNotaDe);
                //int.TryParse(Request.Params("NumeroNotaAte"), out numeroNotaAte);
                int.TryParse(Request.Params("SerieNota"), out serieNota);
                int.TryParse(Request.Params("NumeroCTeDe"), out numeroCTeDe);
                int.TryParse(Request.Params("NumeroCTeAte"), out numeroCTeAte);
                int.TryParse(Request.Params("SerieCTe"), out serieCTe);
                int.TryParse(Request.Params("NumeroFaturaDe"), out numeroFaturaDe);
                int.TryParse(Request.Params("NumeroFaturaAte"), out numeroFaturaAte);
                int.TryParse(Request.Params("NumeroPreFatura"), out numeroPreFatura);

                string numeroNotaDe = Request.Params("NumeroNotaDe");
                string numeroSolicitacao = Request.Params("NumeroSolicitacao");
                string numeroPedidoCF = Request.Params("NumeroPedidoCF");
                string numeroCarga = Request.Params("NumeroCarga");
                string numeroPedidoEmbarcador = Request.Params("numeroPedidoEmbarcador");
                int numeroPedido = 0;
                int.TryParse(Request.Params("NumeroPedidoDe"), out numeroPedido);

                int codigoEmpresaOrigem = 0, codigoEmpresaDestino = 0, codigoCidadeOrigem = 0, codigoCidadeDestino = 0, codigoGrupoPessoa = 0, codigoTipoOperacao = 0, codigoVeiculo = 0, codigoMotorista = 0, codigoTipoCarga = 0;
                int.TryParse(Request.Params("EmpresaOrigem"), out codigoEmpresaOrigem);
                int.TryParse(Request.Params("EmpresaDestino"), out codigoEmpresaDestino);
                int.TryParse(Request.Params("CidadeOrigem"), out codigoCidadeOrigem);
                int.TryParse(Request.Params("CidadeDestino"), out codigoCidadeDestino);
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                int.TryParse(Request.Params("TipoOperacao"), out codigoTipoOperacao);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("TipoCarga"), out codigoTipoCarga);

                double codigoCliente = 0, codigoRemetente = 0, codigoDestinatario = 0, codigoRecebedor = 0;
                double.TryParse(Request.Params("Cliente"), out codigoCliente);
                double.TryParse(Request.Params("Remetente"), out codigoRemetente);
                double.TryParse(Request.Params("Destinatario"), out codigoDestinatario);
                double.TryParse(Request.Params("Recebedor"), out codigoRecebedor);

                double cnpjClienteUsuario = 0;
                if (this.Usuario.AssociarUsuarioCliente && this.Usuario.Cliente != null)
                    cnpjClienteUsuario = this.Usuario.Cliente.CPF_CNPJ;

                // Consulta
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaGrid = repCargaCTe.ConsultarSAC(numeroPedidoEmbarcador, codigoRecebedor, numeroPedidoCF, cnpjClienteUsuario, numeroSolicitacao, dataEmissaoNotaDe, dataEmissaoNotaAte, dataEmissaoCTeDe, dataEmissaoCTeAte,
                    numeroNotaDe, serieNota, numeroCTeDe, numeroCTeAte, serieCTe, numeroFaturaDe, numeroFaturaAte, numeroPreFatura,
                    numeroCarga, numeroPedido,
                    codigoEmpresaOrigem, codigoEmpresaDestino, codigoCidadeOrigem, codigoCidadeDestino, codigoGrupoPessoa, codigoTipoOperacao, codigoVeiculo, codigoMotorista, codigoTipoCarga,
                    codigoCliente, codigoRemetente, codigoDestinatario, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repCargaCTe.ContarConsultaSAC(numeroPedidoEmbarcador, codigoRecebedor, numeroPedidoCF, cnpjClienteUsuario, numeroSolicitacao, dataEmissaoNotaDe, dataEmissaoNotaAte, dataEmissaoCTeDe, dataEmissaoCTeAte,
                    numeroNotaDe, serieNota, numeroCTeDe, numeroCTeAte, serieCTe, numeroFaturaDe, numeroFaturaAte, numeroPreFatura,
                    numeroCarga, numeroPedido,
                    codigoEmpresaOrigem, codigoEmpresaDestino, codigoCidadeOrigem, codigoCidadeDestino, codigoGrupoPessoa, codigoTipoOperacao, codigoVeiculo, codigoMotorista, codigoTipoCarga,
                    codigoCliente, codigoRemetente, codigoDestinatario);

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 CodigoCTe = obj.Codigo,
                                 Numero = obj.Numero.ToString("n0"),
                                 Serie = obj.Serie != null ? obj.Serie.Numero.ToString("n0") : string.Empty,
                                 ModeloDocumentoFiscal = obj.ModeloDocumentoFiscal != null ? obj.ModeloDocumentoFiscal.Abreviacao : string.Empty,
                                 obj.NumeroNotas,
                                 obj.NumerosSolicitacoes,
                                 obj.DescricaoTipoPagamento,
                                 Empresa = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.Localidade.Estado.Sigla + ")" : string.Empty,
                                 Remetente = obj.Remetente != null ? obj.Remetente.Nome + " (" + obj.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome + " (" + obj.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 LocalidadeTerminoPrestacao = obj.LocalidadeTerminoPrestacao != null ? obj.LocalidadeTerminoPrestacao.Descricao : string.Empty,
                                 ValorAReceber = obj.ValorAReceber.ToString("n2"),
                                 AliquotaICMS = obj.AliquotaICMS.ToString("n2"),
                                 obj.DescricaoStatus,
                                 DT_RowColor = obj.Status == "C" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho : ""
                             }).ToList();

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                // Valida
                if (cte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasCTE = repXMLNotaFiscal.BuscarPorCTe(cte);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe ultimaCarga = repCargaCTe.BuscarUltimaCargaDoCTe(codigo);

                // Formata retorno
                var retorno = new
                {
                    cte.Codigo,
                    Numero = cte.Numero.ToString("n0"),
                    TipoDocumento = cte.ModeloDocumentoFiscal != null ? cte.ModeloDocumentoFiscal.Abreviacao : string.Empty,
                    cte.NumeroNotas,
                    cte.NumerosSolicitacoes,
                    DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataPrevisao = cte.DataPrevistaEntrega.HasValue ? cte.DataPrevistaEntrega.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Remetente = cte.Remetente != null ? cte.Remetente.Nome + " (" + cte.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                    Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome + " (" + cte.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                    Transportador = cte.Empresa.RazaoSocial,
                    Observacao = cte.ObservacoesGerais,
                    CodigoCarga = ultimaCarga != null ? ultimaCarga.Carga.Codigo : 0,
                    CodigoCargaEmbarcador = ultimaCarga != null ? ultimaCarga.Carga.CodigoCargaEmbarcador : string.Empty,
                    RecebedorNF = notasCTE != null && notasCTE.Count > 0 ? string.Join(", ", notasCTE.Where(o => o.Recebedor != null).Select(o => o.Recebedor.NomeCNPJ)) : string.Empty,
                    DestinatarioNF = notasCTE != null && notasCTE.Count > 0 ? string.Join(", ", notasCTE.Where(o => o.Destinatario != null).Select(o => o.Destinatario.NomeCNPJ).Distinct()) : string.Empty,
                    NumeroPedidoCF = cte.Documentos != null && cte.Documentos.Count > 0 && cte.Documentos.Where(o => !string.IsNullOrWhiteSpace(o.NumeroPedido)).Count() > 0 ? string.Join(", ", cte.Documentos.Select(o => o.NumeroPedido)) : string.Empty
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> FaturasDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("PossuiPermissaoFatura");
                grid.Prop("Numero").Nome("Número").Align(Models.Grid.Align.right).Tamanho(10);
                grid.Prop("NumeroPreFatura").Nome("Pré-Fatura").Align(Models.Grid.Align.right).Tamanho(10);
                grid.Prop("NumeroTitulos").Nome("Título(s)").Tamanho(20).Ord(false);
                grid.Prop("Total").Nome("Total").Align(Models.Grid.Align.right).Tamanho(10);
                grid.Prop("PeriodoVencimento").Nome("Data Vencimento").Tamanho(30).Ord(false);
                grid.Prop("SituacaoTitulos").Nome("Situação").Tamanho(30).Ord(false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "TipoOcorrencia")
                    propOrdenar = "Fatura.TipoOcorrencia.Descricao";
                else if (propOrdenar == "Numero")
                    propOrdenar = "Fatura.Numero";
                else if (propOrdenar == "NumeroPreFatura")
                    propOrdenar = "Fatura.NumeroPreFatura";
                else if (propOrdenar == "Total")
                    propOrdenar = "Fatura.Total";

                // Dados do filtro
                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                // Consulta
                List<Dominio.Entidades.Embarcador.Fatura.Fatura> listaGrid = repFatura.ConsultarFaturaConhecimento(codigoCTe, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repFatura.ContarConsultarFaturaConhecimento(codigoCTe);
                bool possuiPermissaoFatura = this.UsuarioPossuiPermissao("Faturas/Fatura");

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 PossuiPermissaoFatura = possuiPermissaoFatura,
                                 Numero = obj.Numero.ToString("n0"),
                                 NumeroPreFatura = obj.NumeroPreFatura.ToString("n0"),
                                 obj.NumeroTitulos,
                                 Total = obj.Total.ToString("n2"),
                                 obj.PeriodoVencimento,
                                 obj.SituacaoTitulos
                             }).ToList();

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        [AllowAuthenticate]
        public async Task<IActionResult> OcorrenciasEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("TipoDeOcorrencia").Nome("Ocorrência").Tamanho(85);
                grid.Prop("DataOcorrencia").Nome("Data").Align(Models.Grid.Align.center).Tamanho(15);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "TipoDeOcorrencia")
                    propOrdenar = "TipoDeOcorrencia.Descricao";

                // Dados do filtro
                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                // Consulta
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> listaGrid = repPedidoOcorrenciaColetaEntrega.ConsultarOcorrenciaConhecimento(codigoCTe, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPedidoOcorrenciaColetaEntrega.ContarConsultarOcorrenciaConhecimento(codigoCTe);
                bool possuiPermissaoOcorrencia = this.UsuarioPossuiPermissao("Ocorrencias/Ocorrencia");

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 TipoDeOcorrencia = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(obj.TipoDeOcorrencia, obj.Carga, obj.Alvo, obj.Pedido.Remetente, true) + " " + obj.Observacao,
                                 DataOcorrencia = obj.DataOcorrencia.ToString("dd/MM/yyyy HH:mm")
                             }).ToList();

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        [AllowAuthenticate]
        public async Task<IActionResult> OcorrenciasDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("PossuiPermissaoOcorrencia");
                grid.Prop("TipoOcorrencia").Nome("Ocorrência").Tamanho(30);
                grid.Prop("Observacao").Nome("Observação").Tamanho(40);
                grid.Prop("DataOcorrencia").Nome("Data").Align(Models.Grid.Align.center).Tamanho(15);
                grid.Prop("DescricaoSituacao").Nome("Situação").Tamanho(15);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "TipoOcorrencia")
                    propOrdenar = "CargaOcorrencia.TipoOcorrencia.Descricao";
                else if (propOrdenar == "Observacao")
                    propOrdenar = "CargaOcorrencia.Observacao";
                else if (propOrdenar == "DataOcorrencia")
                    propOrdenar = "CargaOcorrencia.DataOcorrencia";

                // Dados do filtro
                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                // Consulta
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaGrid = repCargaOcorrencia.ConsultarOcorrenciaConhecimento(codigoCTe, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCargaOcorrencia.ContarConsultarOcorrenciaConhecimento(codigoCTe);
                bool possuiPermissaoOcorrencia = this.UsuarioPossuiPermissao("Ocorrencias/Ocorrencia");

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 PossuiPermissaoOcorrencia = possuiPermissaoOcorrencia,
                                 TipoOcorrencia = obj.TipoOcorrencia?.Descricao ?? "",
                                 obj.Observacao,
                                 DataOcorrencia = obj.DataOcorrencia.ToString("dd/MM/yyyy"),
                                 obj.DescricaoSituacao
                             }).ToList();

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        [AllowAuthenticate]
        public async Task<IActionResult> ManifestosDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("Numero").Nome("Número").Tamanho(12);
                grid.Prop("DataEmissao").Nome("Data").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("Veiculos").Nome("Veículo(s)").Tamanho(25).Ord(false);
                grid.Prop("Motoristas").Nome("Motorista(s)").Tamanho(25).Ord(false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Numero")
                    propOrdenar = "MunicipioDescarregamento.MDFe.Numero";
                else if (propOrdenar == "DataEmissao")
                    propOrdenar = "obj.MunicipioDescarregamento.MDFe.DataEmissao";
                // Dados do filtro
                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                // Consulta
                List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> listaGrid = repDocumentoMunicipioDescarregamentoMDFe.ConsultarCargaConhecimento(codigoCTe, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repDocumentoMunicipioDescarregamentoMDFe.ContarConsultarCargaConhecimento(codigoCTe);

                var lista = (from obj in listaGrid
                             where obj.MunicipioDescarregamento != null && obj.MunicipioDescarregamento.MDFe != null
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 DataEmissao = obj.MunicipioDescarregamento.MDFe.DataEmissao.HasValue ? obj.MunicipioDescarregamento.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Numero = obj.MunicipioDescarregamento.MDFe.Numero.ToString("n0"),
                                 Veiculos = obj.MunicipioDescarregamento.MDFe.Veiculos != null && obj.MunicipioDescarregamento.MDFe.Veiculos.Count > 0 ? string.Join(", ", obj.MunicipioDescarregamento.MDFe.Veiculos.Select(o => o.Placa_Formatada)) : string.Empty,
                                 Motoristas = obj.MunicipioDescarregamento.MDFe.Motoristas != null && obj.MunicipioDescarregamento.MDFe.Motoristas.Count > 0 ? string.Join(", ", obj.MunicipioDescarregamento.MDFe.Motoristas.Select(o => o.Nome)) : string.Empty
                             }).ToList();

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        [AllowAuthenticate]
        public async Task<IActionResult> CargasDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("PossuiPermissaoCarga");
                grid.Prop("Situacao");
                grid.Prop("CodigoTipoCarga");
                grid.Prop("CodigoModeloVeiculo");
                grid.Prop("CodigoCargaEmbarcador").Nome("Número").Tamanho(12);
                grid.Prop("DataCriacaoCarga").Nome("Data").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("TipoDeCarga").Nome("Tip. Carga").Tamanho(12);
                grid.Prop("TipoOperacao").Nome("Operação").Tamanho(12);
                grid.Prop("NomeMotoristas").Nome("Motorista(s)").Tamanho(15).Ord(false);
                grid.Prop("DescricaoSituacaoCarga").Nome("Situação").Tamanho(8);
                grid.Prop("Veiculo").Nome("Veículo").Tamanho(8);
                grid.Prop("DataCarregamentoCarga").Nome("Carregamento").Align(Models.Grid.Align.center).Tamanho(10);
                grid.Prop("Origem").Nome("Origem").Tamanho(20).Ord(false);
                grid.Prop("Destino").Nome("Destino").Tamanho(20).Ord(false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "TipoDeCarga")
                    propOrdenar = "Carga.TipoDeCarga.Descricao";
                else if (propOrdenar == "TipoOperacao")
                    propOrdenar = "Carga.TipoOperacao.Descricao";
                else if (propOrdenar == "DescricaoSituacaoCarga")
                    propOrdenar = "Carga.SituacaoCarga";
                else if (propOrdenar == "Veiculo")
                    propOrdenar = "Carga.Veiculo.Placa";
                else if (propOrdenar == "CodigoCargaEmbarcador")
                    propOrdenar = "Carga.CodigoCargaEmbarcador";
                else if (propOrdenar == "DataCriacaoCarga")
                    propOrdenar = "Carga.DataCriacaoCarga";
                else if (propOrdenar == "DataCarregamentoCarga")
                    propOrdenar = "Carga.DataCarregamentoCarga";

                // Dados do filtro
                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                // Consulta
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaGrid = repCarga.ConsultarCargaConhecimento(codigoCTe, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCarga.ContarConsultarCargaConhecimento(codigoCTe);
                bool possuiPermissaoCarga = this.UsuarioPossuiPermissao("Cargas/Carga");

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 PossuiPermissaoCarga = possuiPermissaoCarga,
                                 Situacao = obj.SituacaoCarga,
                                 CodigoTipoCarga = obj.TipoDeCarga != null ? obj.TipoDeCarga.Codigo : 0,
                                 CodigoModeloVeiculo = obj.ModeloVeicularCarga != null ? obj.ModeloVeicularCarga.Codigo : 0,
                                 obj.CodigoCargaEmbarcador,
                                 DataCriacaoCarga = obj.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                                 TipoDeCarga = obj.TipoDeCarga != null ? obj.TipoDeCarga.Descricao : string.Empty,
                                 TipoOperacao = obj.TipoOperacao != null ? obj.TipoOperacao.Descricao : string.Empty,
                                 obj.NomeMotoristas,
                                 obj.DescricaoSituacaoCarga,
                                 Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa_Formatada : string.Empty,
                                 DataCarregamentoCarga = obj.DataCarregamentoCarga.HasValue ? obj.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Origem = obj.DadosSumarizados != null ? obj.DadosSumarizados.Origens : string.Empty,
                                 Destino = obj.DadosSumarizados != null ? obj.DadosSumarizados.Destinos : string.Empty
                             }).ToList();

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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
        [AllowAuthenticate]
        public async Task<IActionResult> CanhotosDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCanhoto", false);
                grid.AdicionarCabecalho("GuidNomeArquivo", false);
                grid.AdicionarCabecalho("Data Histórico", "DataHistorico", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Operador", "Operador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Observacao", 35, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Digitalização", "DescricaoDigitalizacao", 15, Models.Grid.Align.center, false);


                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> canhotosNotaFiscalHistoricos = repCanhotoHistorico.ConsultarPorCTe(codigoCTe, "DataHistorico", "desc", grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCanhotoHistorico.ContarConsultaPorCTe(codigoCTe));
                var lista = (from p in canhotosNotaFiscalHistoricos
                             select new
                             {
                                 p.Codigo,
                                 CodigoCanhoto = p.Canhoto.Codigo,
                                 GuidNomeArquivo = p.Canhoto.GuidNomeArquivo,
                                 DataHistorico = p.DataHistorico.ToString("dd/MM/yyyy HH:mm"),
                                 Operador = p.Usuario != null ? p.Usuario.Nome : "",
                                 Observacao = p.Observacao + (p.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente && p.LocalArmazenamentoCanhoto != null ? (" (" + p.LocalArmazenamentoCanhoto.Descricao + (p.PacoteArmazenado > 0 ? " (Pacote " + p.PacoteArmazenado.ToString() + "/Posição " + p.PosicaoNoPacote.ToString() + ")" : string.Empty) + ")") : string.Empty),
                                 p.DescricaoSituacao,
                                 p.DescricaoDigitalizacao
                             }).ToList();
                grid.AdicionaRows(lista);
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
        [AllowAuthenticate]
        public async Task<IActionResult> GuaritasDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "DataSaidaEntrada", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("KM", "KMAtual", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Situação", "DescricaoTipoEntradaSaida", 15, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS> listaGuaritaTMS = repGuaritaTMS.Consultar(codigoCTe, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repGuaritaTMS.ContarConsulta(codigoCTe));

                var lista = (from p in listaGuaritaTMS
                             select new
                             {
                                 p.Codigo,
                                 Veiculo = p.Veiculo.Placa,
                                 Carga = p.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                 DataSaidaEntrada = p.DataSaidaEntrada.ToString("dd/MM/yyyy"),
                                 KMAtual = p.KMAtual.ToString("n0"),
                                 DescricaoTipoEntradaSaida = p.TipoEntradaSaida.ObterDescricao()
                             }).ToList();

                grid.AdicionaRows(lista);

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
        [AllowAuthenticate]
        public async Task<IActionResult> TitulosDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoCTe);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Código", "Codigo", 6, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Doc.", "NumeroDocumentoTituloOriginal", 6, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Fatura", "Fatura", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Seq.", "Sequencia", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Desconto", "ValorDesconto", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 6, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor Pago", "ValorPago", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Saldo", "ValorSaldo", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Nº Boleto", "NossoNumero", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remessa", "Remessa", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 8, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisa = ObterFiltrosPesquisa(codigoCTe);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.Consultar(filtrosPesquisa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaTitulo
                             select new
                             {
                                 p.Codigo,
                                 Fatura = p.FaturaParcela?.Fatura?.Numero.ToString("n0") ?? string.Empty,
                                 p.NumeroDocumentoTituloOriginal,
                                 DescricaoTipo = p.TipoTitulo.ObterDescricao(),
                                 DescricaoSituacao = p.StatusTitulo.ObterDescricao(),
                                 p.Sequencia,
                                 Pessoa = p.Pessoa != null ? p.Pessoa.Nome + " (" + p.Pessoa.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Valor = p.ValorOriginal.ToString("n2"),
                                 DataEmissao = p.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                 DataVencimento = p.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                 ValorDesconto = p.Desconto.ToString("n2"),
                                 ValorAcrescimo = p.Acrescimo.ToString("n2"),
                                 ValorPago = p.ValorPago.ToString("n2"),
                                 ValorSaldo = p.StatusTitulo == StatusTitulo.Quitada ? 0.ToString("n2") : (p.ValorOriginal - p.Desconto + p.Acrescimo).ToString("n2"),
                                 p.NossoNumero,
                                 p.Observacao,
                                 Remessa = p.BoletoRemessa?.NumeroSequencial.ToString("n0") ?? string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);

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
        [AllowAuthenticate]
        public async Task<IActionResult> Imprimir()
        {
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                var pdf  =  ReportRequest.WithType(ReportType.AtendimentoCliente)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo.ToString())
                    .CallReport()
                    .GetContentFile();
               return Arquivo(pdf, "application/pdf", "Atendimento Cliente.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
        }

        #region Métodos Privados
        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdenar == "Numero") propOrdenar = "Numero";
            else if (propOrdenar == "Serie") propOrdenar = "Serie.Numero";
            else if (propOrdenar == "ModeloDocumentoFiscal") propOrdenar = "ModeloDocumentoFiscal.Abreviacao";
            else if (propOrdenar == "DescricaoTipoPagamento") propOrdenar = "TipoPagamento";
            else if (propOrdenar == "Empresa") propOrdenar = "Empresa.RazaoSocial";
            else if (propOrdenar == "Remetente") propOrdenar = "Remetente.Nome";
            else if (propOrdenar == "Destinatario") propOrdenar = "Destinatario.Nome";
            else if (propOrdenar == "LocalidadeTerminoPrestacao") propOrdenar = "LocalidadeTerminoPrestacao.Descricao";
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro ObterFiltrosPesquisa(int codigoCTe)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisaTituloFinanceiro = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro()
            {
                CodigoCTe = codigoCTe
            };

            if (filtrosPesquisaTituloFinanceiro.CodigoTitulo == 0)
                filtrosPesquisaTituloFinanceiro.CodigoTitulo = Request.GetIntParam("Descricao");

            return filtrosPesquisaTituloFinanceiro;
        }
        #endregion
    }
}
