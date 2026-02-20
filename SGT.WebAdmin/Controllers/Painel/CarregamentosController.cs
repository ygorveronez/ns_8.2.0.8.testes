using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Painel
{
    [CustomAuthorize("Painel/Carregamentos")]
    public class CarregamentosController : BaseController
    {
        #region Construtores

        public CarregamentosController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterCarregamentos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            try
            {
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = ObterParametrosConsulta();
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = await repositorioConfiguracaoGestaoPatio.BuscarPrimeiroRegistroAsync();
                int total = await repositorioFluxoGestaoPatio.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio = total > 0 ? await repositorioFluxoGestaoPatio.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

                List<dynamic> lista = (from fluxoGestaoPatio in fluxosGestaoPatio select ObterDetalhesFluxoPatio(fluxoGestaoPatio, configuracaoEmbarcador, configuracaoGestaoPatio, unitOfWork)).ToList();

                return new JsonpResult(lista, total);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o fluxo de pátio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesFluxoPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork, configuracaoGestaoPatio);
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada etapaDescricao = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(fluxoGestaoPatio);

            var retorno = new
            {
                fluxoGestaoPatio.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(fluxoGestaoPatio.CargaBase, configuracaoEmbarcador),
                Destino = fluxoGestaoPatio.Carga != null ? ((!string.IsNullOrWhiteSpace(fluxoGestaoPatio.Carga.DadosSumarizados.CodigoIntegracaoDestinatarios) ? fluxoGestaoPatio.Carga.DadosSumarizados.CodigoIntegracaoDestinatarios + " - " : "") + fluxoGestaoPatio.Carga.DadosSumarizados.Destinos) : (fluxoGestaoPatio.PreCarga.DadosSumarizados?.Destinos ?? ""),
                ModeloVeicular = fluxoGestaoPatio.Carga != null ? ObterModeloVeicular(fluxoGestaoPatio.Carga.Veiculo, fluxoGestaoPatio.Carga.VeiculosVinculados) : fluxoGestaoPatio.PreCarga?.Veiculo?.ModeloVeicularCarga?.Descricao ?? "",
                AreaVeiculo = fluxoGestaoPatio.Carga != null ? ObterAreaVeiculo(fluxoGestaoPatio.Carga) : "",
                Hora = fluxoGestaoPatio.Carga != null ? (fluxoGestaoPatio.Carga.DataCarregamentoCarga?.ToString("HH:mm") ?? "") : (fluxoGestaoPatio.PreCarga?.DataInicioViagem?.ToString("HH:mm") ?? ""),
                Motorista = fluxoGestaoPatio.Carga != null ? (!string.IsNullOrEmpty(fluxoGestaoPatio.Carga.DadosSumarizados.Motoristas) ? fluxoGestaoPatio.Carga.DadosSumarizados.Motoristas : fluxoGestaoPatio.Carga.NomeMotoristas) : fluxoGestaoPatio.PreCarga.RetornarMotoristas,
                Placas = fluxoGestaoPatio.Carga != null ? ObterPlacas(fluxoGestaoPatio.Carga.Veiculo, fluxoGestaoPatio.Carga.VeiculosVinculados) : fluxoGestaoPatio.PreCarga.RetornarPlacas,
                Situacao = etapaDescricao?.Descricao ?? string.Empty
            };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio()
            {
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                EtapaFluxoGestaoPatio = Request.GetListEnumParam<EtapaFluxoGestaoPatio>("EtapaFluxoGestaoPatio"),
                SomenteFluxosAbertos = Request.GetBoolParam("SomenteFluxosAbertos")
            };

            int codigoFilial = Request.GetIntParam("Filial");
            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            filtrosPesquisa.CodigosFilial = codigoFilial == 0 ? codigosFilial : new List<int>() { codigoFilial };

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "desc",
                InicioRegistros = Request.GetIntParam("inicio"),
                LimiteRegistros = Request.GetIntParam("limite"),
                PropriedadeOrdenar = "Codigo"
            };
        }

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            if (veiculo != null)
            {
                List<string> placas = new List<string>() { veiculo.Placa };
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

                return string.Join(", ", placas);
            }
            else
                return "";

        }

        private string ObterModeloVeicular(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            if (veiculo != null)
            {
                List<string> modelos = new List<string>() { veiculo.ModeloVeicularCarga.Descricao };
                modelos.AddRange(veiculosVinculados.Select(o => o.ModeloVeicularCarga.Descricao));

                return string.Join(", ", modelos);
            }
            else
                return "";

        }

        private string ObterAreaVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Logistica.AreaVeiculo repAreaVeiculo = new Repositorio.Embarcador.Logistica.AreaVeiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaAreaVeiculo repCargaAreaVeiculo = new Repositorio.Embarcador.Cargas.CargaAreaVeiculo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> CargaAreaVeiculo = repCargaAreaVeiculo.BuscarPorCarga(carga.Codigo);

            if (CargaAreaVeiculo != null && CargaAreaVeiculo.Count > 0)
            {
                List<string> CargaArea = new List<string>();
                CargaArea.AddRange(CargaAreaVeiculo.Select(o => o.AreaVeiculo.Descricao));

                return string.Join(", ", CargaArea);
            }
            else
                return "";
        }

        //private string ObterSituacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        //{
        //    Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxoGestaoPatio = fluxoGestaoPatio.GetEtapas()[fluxoGestaoPatio.EtapaAtual].EtapaFluxoGestaoPatio;

        //    switch (etapaFluxoGestaoPatio)
        //    {
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.CheckList: return "Ag. CheckList";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaLoja: return "Ag. Chegada no Destinatário";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Guarita: return "Ag. Entrada do Veículo";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo: return "Ag. Chegada do Veículo";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DeslocamentoPatio: return "Ag. Deslocamento no Pátio";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca: return "Ag. Informar a Doca";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Entregas: return "Ag. Entregas";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Faturamento: return "Em Faturamento";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimCarregamento: return "Ag. Fim do Carregamento";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimHigienizacao: return "Ag. Fim da Higienização";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem: return "Ag. Fim da Viagem";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.LiberacaoChave: return "Ag. Liberação da Chave";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioCarregamento: return "Ag. Início do Carregamento";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioHigienizacao: return "Ag. Início da Higienização";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Posicao: return "Ag. Posição";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SaidaLoja: return "Ag. Saída do Destinatário";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SolicitacaoVeiculo: return "Ag. Solicitação de Veículo";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.TravamentoChave: return "Ag. Travamento da Chave";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem:
        //            if ((fluxoGestaoPatio.Carga != null) && ((fluxoGestaoPatio.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada) || fluxoGestaoPatio.Carga.CargaMDFes.All(obj => obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)))
        //                return "Encerrada";

        //            return "Em Viagem";
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Expedicao:
        //            if (!fluxoGestaoPatio.DataInicioCarregamento.HasValue && fluxoGestaoPatio.Filial.SequenciaGestaoPatio.ExpedicaoInformarInicioCarregamento)
        //                return "Ag. Início Carregamento";

        //            return (!fluxoGestaoPatio.DataFimCarregamento.HasValue) ? "Em Carregamento" : "";
        //        default: return "";
        //    }
        //}

        #endregion
    }
}
