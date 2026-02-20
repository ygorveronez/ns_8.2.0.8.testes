using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/AutorizacaoPagamentoCIOT")]
    public class AutorizacaoPagamentoCIOTController : BaseController
    {
		#region Construtores

		public AutorizacaoPagamentoCIOTController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AutorizarPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracoes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracoes = repConfiguracoes.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros = ObterFiltrosPesquisa();

                filtros.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;

                List<int> codigosCIOTs = repCIOT.ObterCodigosConsulta(filtros);

                if (codigosCIOTs.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CIOT's encerrados com os filtros realizados para autorizar o pagamento.");

                if (!ValidarApenasCanhotosRecebidosFisicamenteEDigitalizados(codigosCIOTs, out List<string> listaStringRetorno, unitOfWork) && (configuracoes?.PermitirAutorizarPagamentoCIOTComCanhotosRecebidos ?? false))
                    return new JsonpResult(new { ExibirModal = true, Mensagem = listaStringRetorno}, true, "Não foi possível autorizar o pagamento");

                foreach (int codigoCIOT in codigosCIOTs)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                    if (Servicos.Embarcador.CIOT.CIOT.IntegrarAutorizacaoPagamento(out string mensagemErro, ciot, Usuario, Auditado, TipoServicoMultisoftware, unitOfWork))
                    {
                        unitOfWork.Start();

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in ciot.CargaCIOT)
                        {
                            cargaCIOT.ContratoFrete.DataAutorizacaoPagamentoAdiantamento = ciot.DataAutorizacaoPagamento;
                            cargaCIOT.ContratoFrete.DataAutorizacaoPagamentoSaldo = ciot.DataAutorizacaoPagamento;
                            cargaCIOT.ContratoFrete.DataAutorizacaoPagamento = ciot.DataAutorizacaoPagamento;

                            repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                        }

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Start();

                        ciot.Mensagem = mensagemErro;

                        repCIOT.Atualizar(ciot);

                        unitOfWork.CommitChanges();
                    }

                    unitOfWork.FlushAndClear();
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao autorizar o pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros = ObterFiltrosPesquisa();

                filtros.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem;

                List<int> codigosCIOTs = repCIOT.ObterCodigosConsulta(filtros);

                if (codigosCIOTs.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CIOT's aguardando liberação de viagem com os filtros realizados.");

                foreach (int codigoCIOT in codigosCIOTs)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                    if (Servicos.Embarcador.CIOT.CIOT.IntegrarLiberacaoViagem(out string mensagemErro, ciot, Usuario, Auditado, TipoServicoMultisoftware, unitOfWork))
                    {
                        unitOfWork.Start();

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in ciot.CargaCIOT)
                        {
                            cargaCIOT.ContratoFrete.DataLiberacaoViagem = ciot.DataLiberacaoViagem;

                            repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                        }

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Rollback();
                    }

                    unitOfWork.FlushAndClear();
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao autorizar o pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT()
            {
                CPFCNPJTransportador = Request.GetNullableDoubleParam("Transportador"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>("Situacao"),
                SelecionarTodos = Request.GetNullableBoolParam("SelecionarTodos"),
                ListaCodigosCIOT = Request.GetNullableListParam<int>("ListaCodigosCIOT"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador")
            };
        }

        private Models.Grid.Grid ObterConfiguracaoGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cód. Verificador", "CodigoVerificador", 6, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Protocolo", "ProtocoloAutorizacao", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Final da Viagem", "DataFinalViagem", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nº Ped. Embarcador", "NumeroPedidoEmbarcador", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cargas", "Cargas", 6, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 19, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Acréscimos", "Acrescimos", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Descontos", "Descontos", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Saldo", "Saldo", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 19, Models.Grid.Align.left, false);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterConfiguracaoGrid();
                Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                int totalRegistros = repCIOT.ContarConsulta(filtros);

                List<Dominio.Entidades.Embarcador.Documentos.CIOT> listaCIOT = totalRegistros > 0 ? repCIOT.Consultar(filtros, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Documentos.CIOT>();

                var retorno = (from obj in listaCIOT
                               select new
                               {
                                   obj.Codigo,
                                   Numero = obj.Numero,
                                   CodigoVerificador = obj.CodigoVerificador,
                                   obj.ProtocoloAutorizacao,
                                   DataFinalViagem = obj.DataFinalViagem.ToString("dd/MM/yyyy"),
                                   Transportador = obj.Transportador?.Descricao,
                                   Acrescimos = obj.CargaCIOT?.Sum(o => o.ContratoFrete?.ValoresAdicionais.Where(va => va.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Sum(va => va.Valor)),
                                   Descontos = obj.CargaCIOT?.Sum(o => o.ContratoFrete?.ValoresAdicionais.Where(va => va.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Sum(va => va.Valor)),
                                   Saldo = obj.CargaCIOT?.Sum(o => o.ContratoFrete?.SaldoAReceber)?.ToString("n2"),
                                   Situacao = obj.DescricaoSituacao,
                                   NumeroPedidoEmbarcador = string.Join(", ", obj.CargaCIOT?.Select(o => string.Join(", ", o.Carga.Pedidos.Select(p => p.Pedido.NumeroPedidoEmbarcador)))),
                                   Cargas = string.Join(", ", obj.CargaCIOT?.Select(o => o.Carga.CodigoCargaEmbarcador)),
                                   Mensagem = obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private bool ValidarApenasCanhotosRecebidosFisicamenteEDigitalizados(List<int> codigosCIOTs, out List<string> stringRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            stringRetorno = new List<string>();

            IList<Dominio.ObjetosDeValor.CIOT.RetornoCanhotosCIOT> retorno = repCIOT.BuscarCanhotosRecebidosFisicamenteEDigitalizados(codigosCIOTs);

            if (retorno.Count == 0)
                return true;

            List<string> cargas = retorno.Select(o => o.Carga).Distinct().ToList();

            foreach (string carga in cargas)
                stringRetorno.Add($@"A carga {carga} possui canhotos [{string.Join(", ", retorno.Where(o => o.Carga == carga).Select(o => o.Canhoto.ToString()).ToList())}], ");
            
            return false;
        }

        #endregion
    }
}
