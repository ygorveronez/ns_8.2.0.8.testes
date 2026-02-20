using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/MontagemCarga", "Cargas/MontagemCargaMapa")]
    public class MontagemCargaCargaController : BaseController
    {
		#region Construtores

		public MontagemCargaCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = ObterFiltrosPesquisaCarga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = Request.GetIntParam("Inicio"),
                    LimiteRegistros = Request.GetIntParam("Limite"),
                    PropriedadeOrdenar = "Codigo"
                };
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repositorioCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = (totalRegistros > 0) ? repositorioCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var retorno = new
                {
                    Quantidade = totalRegistros,
                    Registros = (from obj in carga select servicoCarga.ObterDetalhesCargaParaCarregamento(obj, unitOfWork)).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);
                return new JsonpResult(serCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga ObterFiltrosPesquisaCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoFilial = Request.GetIntParam("Filial");
            List<int> codigosFilial = Request.GetListParam<int>("Filial");

            if ((codigoFilial > 0) && (codigosFilial.Count == 0))
                codigosFilial.Add(codigoFilial);

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoGrupoPessoas = Request.GetListParam<int>("GrupoPessoaRemetente"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoPaisDestino = Request.GetIntParam("PaisDestino"),
                codigoPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                CpfCnpjRecebedor = Request.GetDoubleParam("Recebedor"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                DataFinal = Request.GetNullableDateTimeParam("DataFim"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                OperadorLogistica = operadorLogistica,
                Ordem = Request.GetStringParam("Ordem"),
                PortoSaida = Request.GetStringParam("PortoSaida"),
                Reserva = Request.GetStringParam("Reserva"),
                SiglaEstadoDestino = Request.GetStringParam("EstadoDestino"),
                SiglaEstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                SituacaoCarga = SituacaoCarga.NaLogistica,
                SomenteComReserva = Request.GetBoolParam("SomenteComReserva"),
                SomentePermiteAgrupamento = true,
                TipoEmbarque = Request.GetStringParam("TipoEmbarque"),
                TipoOperacaoCargaCTeManual = TipoOperacaoCargaCTeManual.NovaCarga,
                TipoServicoMultisoftware = TipoServicoMultisoftware
            };

            if (filtrosPesquisa.CodigoGrupoPessoas.Count() == 0)
            {
                int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoaRemetente");
                filtrosPesquisa.CodigoGrupoPessoas.Add(codigoGrupoPessoa);
            }
            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

            return filtrosPesquisa;
        }

        #endregion
    }
}
