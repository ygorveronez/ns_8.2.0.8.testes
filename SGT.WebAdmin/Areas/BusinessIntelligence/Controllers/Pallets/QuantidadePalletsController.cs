using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Pallets
{
	[Area("BusinessIntelligence")]
	[CustomAuthorize("BusinessIntelligence/Pallets/QuantidadePallets")]
    public class QuantidadePalletsController : BaseController
    {
		#region Construtores

		public QuantidadePalletsController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterQuantidadesGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRota, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial;
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Filial"), out codigoFilial);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicialCarregamento, dataFinalCarregamento;
                DateTime.TryParseExact(Request.Params("DataInicialCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCarregamento);
                DateTime.TryParseExact(Request.Params("DataFinalCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCarregamento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuantidadeCarga? tipoPai = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuantidadeCarga tipoPaiAux;
                if (Enum.TryParse(Request.Params("TipoPai"), out tipoPaiAux))
                    tipoPai = tipoPaiAux;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoQuantidadeCarga> quantidades = repCarga.BuscarQuantidadePallets(codigoTransportador, codigoVeiculo, codigoFilial, codigoCentroCarregamento, codigoRota, dataInicialCarregamento, dataFinalCarregamento, cpfCnpjDestinatario, tipoPai);

                quantidades = quantidades.Where(o => o.Quantidade > 0).ToList();

                return new JsonpResult(quantidades);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as quantidades de pallets.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterCargasPorTipo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRota, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial;
                int.TryParse(Request.Params("Rota"), out codigoRota);
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Filial"), out codigoFilial);

                double cpfCnpjDestinatario;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out cpfCnpjDestinatario);

                DateTime dataInicialCarregamento, dataFinalCarregamento;
                DateTime.TryParseExact(Request.Params("DataInicialCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCarregamento);
                DateTime.TryParseExact(Request.Params("DataFinalCarregamento"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCarregamento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuantidadeCarga tipo;
                Enum.TryParse(Request.Params("Tipo"), out tipo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuantidadeCarga? tipoPai = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuantidadeCarga tipoPaiAux;
                if (Enum.TryParse(Request.Params("TipoPai"), out tipoPaiAux))
                    tipoPai = tipoPaiAux;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoQuantidadeCarga> quantidades = repCarga.BuscarCargasPorTipo(codigoTransportador, codigoVeiculo, codigoFilial, codigoCentroCarregamento, codigoRota, dataInicialCarregamento, dataFinalCarregamento, cpfCnpjDestinatario, tipo, tipoPai);

                quantidades = quantidades.Where(o => o.Quantidade > 0).ToList();

                return new JsonpResult(quantidades);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as quantidades de cargas.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterDestinatariosPorCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoQuantidadeCarga tipo;
                Enum.TryParse(Request.Params("Tipo"), out tipo);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoQuantidadeCarga> quantidades = repCarga.BuscarDestinatariosPorCarga(codigoCarga, tipo);

                quantidades = quantidades.Where(o => o.Quantidade > 0).ToList();

                return new JsonpResult(quantidades);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os destinatários da carga.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
