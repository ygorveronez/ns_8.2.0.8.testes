using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Planejamentos
{
    [CustomAuthorize("Planejamentos/PlanejamentoFrota")]
    public class PlanejamentoFrotaController : BaseController
    {
		#region Construtores

		public PlanejamentoFrotaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCargasPorData()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrotaCarga filtrosPesquisa = ObterFiltrosPesquisaCarga(unitOfWork);
                int inicio = Request.GetIntParam("Inicio");
                int limite = Request.GetIntParam("Limite");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Pessoa.GrupoPessoa servicoGrupoPessoa = new Servicos.Embarcador.Pessoa.GrupoPessoa();

                int total = repositorioCarga.ContarConsultaCargasPlanejamentoFrota(filtrosPesquisa);
                IList<Dominio.Relatorios.Embarcador.DataSource.Planejamentos.PlanejamentoFrotaCarga> listaCargas = total > 0 ? repositorioCarga.ConsultarCargasPlanejamentoFrota(filtrosPesquisa, "CodigoCargaEmbarcador", "desc", inicio, limite) : new List<Dominio.Relatorios.Embarcador.DataSource.Planejamentos.PlanejamentoFrotaCarga>();

                for (int i = 0; i < listaCargas.Count; i++)
                    listaCargas[i].LogoGrupoPessoas = servicoGrupoPessoa.ObterLogoBase64(listaCargas[i].CodigoGrupoPessoas, unitOfWork);

                var retorno = new
                {
                    Total = total,
                    Cargas = listaCargas
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(unitOfWork);
                Repositorio.Embarcador.Frota.FrotaCarga repFrotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrota filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                    InicioRegistros = Request.GetIntParam("inicio"),
                    LimiteRegistros = Request.GetIntParam("limite"),
                    PropriedadeOrdenar = "PlacaVeiculoTracao"
                };

                int codigoCarga = Request.GetIntParam("codigoCarga");
                Dominio.Entidades.Embarcador.Cargas.Carga Carga = null;

                if (codigoCarga > 0)
                {
                    Carga = repCarga.BuscarPorCodigo(codigoCarga);
                    if (Carga == null)
                        return new JsonpResult(false, "Carga não encontrada.");

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargaPedido = repCargaPedido.BuscarPorCarga(Carga.Codigo);
                    Dominio.Entidades.Localidade localOrigem = CargaPedido.Select(x => x.Pedido.Expedidor != null ? x.Pedido.Expedidor.Localidade : x.Pedido.Remetente.Localidade).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Frota.FrotaCarga frotaVinculada = repFrotaCarga.BuscarPorFrotaOuCargaEData(0, Carga.Codigo, Carga.DataCarregamentoCarga.Value);

                    if (!localOrigem.Latitude.HasValue && !localOrigem.Longitude.HasValue)
                        return new JsonpResult(false, "Localidade de origem da carga não possui coordenadas, impossível ordenar veículos prioritários .");

                    filtrosPesquisa.latitudeOrigem = Math.Round(localOrigem.Latitude.Value, 5);
                    filtrosPesquisa.longitudeOrigem = Math.Round(localOrigem.Longitude.Value, 5);
                    if (frotaVinculada != null)
                        filtrosPesquisa.CodigoFrota = frotaVinculada.Frota.Codigo;
                }

                int totalRegistros = repFrota.ContarConsultarPlanejamentoFrota(filtrosPesquisa);

                if (totalRegistros == 0)
                    return new JsonpResult(new List<dynamic>(), totalRegistros);

                IList<Dominio.ObjetosDeValor.Embarcador.Frota.PlanejamentoFrota> lista = repFrota.ConsultarPlanejamentoFrota(filtrosPesquisa, parametrosConsulta);

                foreach (var frota in lista)
                {
                    frota.DescricaoStatus = frota.ObterStatusFrota(frota.Planejamento, filtrosPesquisa.DataConsultaVigencia);
                    frota.PossuiProgramacaoFutura = frota.VerificarProgramacaoFutura(frota.Planejamento, filtrosPesquisa.DataConsultaVigencia);
                    frota.LocalPlanejamentoFormatado = frota.BuscarLocalVeiculo(frota.Planejamento, filtrosPesquisa.DataConsultaVigencia);
                    frota.PaisDestino = frota.BuscarPaisDestinoVeiculo(frota.Planejamento, filtrosPesquisa.DataConsultaVigencia);
                    frota.ManutencaoExpirada = frota.ExpirouManutencao(filtrosPesquisa.DataConsultaVigencia);
                    frota.ExisteManutencaoProxima = frota.ManutencaoProxima(filtrosPesquisa.DataConsultaVigencia);

                    if (Carga != null)
                        frota.DataDisponivelCarregamento = frota.TempoPercorrerDistancia(Carga.DataCarregamentoCarga.Value); ;
                }

                return new JsonpResult(new
                {
                    Frotas = lista
                }, totalRegistros);
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

        public async Task<IActionResult> VincularFrotaACarga()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Frota.Frota servicoFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

            try
            {
                unitOfWork.Start();

                int codFrota = Request.GetIntParam("Frota");
                int codCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Frota.Frota Frota = repFrota.BuscarPorCodigo(codFrota, true);

                if (Frota != null)
                {
                    if (Frota.Veiculo == null)
                        return new JsonpResult(false, true, "Não é possível vincular a frota na carga pois a frota nao possuí uma tração");


                    Dominio.Entidades.Embarcador.Cargas.Carga Carga = repCarga.BuscarPorCodigoFetch(codCarga);
                    if (Carga != null)
                    {
                        if (Carga.DataCarregamentoCarga.HasValue)
                            servicoFrota.VincularFrotaACarga(Frota, Carga, Auditado);
                        else
                            return new JsonpResult(false, true, "A Carga não possuí data de carregamento");

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true, "Registro vinculado com sucesso");
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Carga não encontrada");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Frota não encontrada");
                }
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao vincular a frota na carga.");
            }
        }

        public async Task<IActionResult> RemoverFrotaACarga()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Frota.Frota servicoFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

            try
            {
                unitOfWork.Start();

                int codFrota = Request.GetIntParam("Frota");
                int codCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Frota.Frota Frota = repFrota.BuscarPorCodigo(codFrota, true);

                if (Frota != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga Carga = repCarga.BuscarPorCodigo(codCarga, true);
                    if (Carga != null)
                    {
                        servicoFrota.RemoverFrotaACarga(Frota, Carga, Auditado);
                        servicoFrota.InformarComprometimentoFrotaFutura(Frota, Carga.DataCarregamentoCarga.Value);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true, "Registro removido com sucesso");
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Carga não encontrada");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Frota não encontrada");
                }
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao vincular a frota na carga.");
            }
        }


        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrota ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrota()
            {
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                SituacaoDaFrota = Request.GetEnumParam<SituacaoFrota>("SituacaoDaFrota"),
                SituacaoDoConjunto = Request.GetEnumParam<SituacaoDoConjuntoFrota>("SituacaoDoConjunto"),
                VeiculoNecessitaManutencao = Request.GetBoolParam("VeiculoNecessitaManutencao"),
                VeiculoComCarga = Request.GetBoolParam("VeiculoComCarga"),
                MotoristaNecessitaIrCasa = Request.GetBoolParam("MotoristaNecessitaIrCasa"),
                DataConsultaVigencia = Request.GetDateTimeParam("dataPesquisa")
            };
        }


        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrotaCarga ObterFiltrosPesquisaCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrotaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrotaCarga()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                CodigosExpedidores = Request.GetListParam<long>("Expedidor"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestinos = Request.GetListParam<int>("Destino"),
                CodigoClienteDestino = Request.GetListParam<double>("ClienteDestino"),
                CodigoClienteOrigem = Request.GetListParam<double>("ClienteOrigem"),
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                CodigosResponsavelVeiculo = Request.GetListParam<int>("ResponsavelVeiculo"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosFronteiraRotaFrete = Request.GetListParam<double>("FronteiraRotaFrete"),
                CodigosPaisDestino = Request.GetListParam<int>("PaisDestino"),
                CodigosPaisOrigem = Request.GetListParam<int>("PaisOrigem"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                Data = Request.GetDateTimeParam("Data"),
                CodigosTipoOperacaoDiferenteDe = Request.GetListParam<int>("TipoOperacaoDiferenteDe")
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

            if (filtrosPesquisa.CodigosTipoOperacao == null || filtrosPesquisa.CodigosTipoOperacao.Count == 0)
                filtrosPesquisa.CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork); //codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

            return filtrosPesquisa;
        }

        #endregion
    }
}
