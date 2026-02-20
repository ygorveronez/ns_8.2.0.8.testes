using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    public class ExpedicaoController : BaseController
    {
		#region Construtores

		public ExpedicaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCarregamentoGuarita()
                {
                    DataInicialCarregamento = Request.GetDateTimeParam("DataCarregamento"),
                    DataFinalCarregamento = Request.GetDateTimeParam("DataCarregamento"),
                    Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita>("Situacao"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                    CodigoMotorista = Request.GetIntParam("Motorista"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    CentroCarregamento = Request.GetIntParam("CentroCarregamento")
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data de Carregamento", "DataCarregamento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "NumeroCarga", 17, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 11, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo Veículo", "ModeloVeiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação Guarita", "DescricaoSituacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carregado", "CarregamentoFinalizado", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoCarga", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);
                List<int> centrosCarregamento = ObterListaCodigoCentroCarregamentoPermitidosOperadorLogistica(unidadeDeTrabalho);

                filtrosPesquisa.CodigosCentrosCarregamento = centrosCarregamento;

                int rowCount = repCargaGuarita.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> listaCargaGuarita = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>();

                if (rowCount > 0)
                    listaCargaGuarita = repCargaGuarita.Consultar(filtrosPesquisa, parametrosConsulta);

                grid.setarQuantidadeTotal(rowCount);

                var retorno = (from obj in listaCargaGuarita
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   CodigoCarga = obj.Carga?.Codigo,
                                   Destinatario = obj.Carga?.DadosSumarizados?.Destinatarios,
                                   DataCarregamento = obj.CargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:00"),
                                   NumeroCarga = obj.Carga?.CodigoCargaEmbarcador + " - " + obj.Carga?.DadosSumarizados?.Destinatarios,
                                   Transportador = obj.Carga?.Empresa?.RazaoSocial,
                                   Motorista = obj.Carga?.Motoristas?.FirstOrDefault()?.Nome ?? string.Empty,
                                   Veiculo = obj.Carga?.RetornarPlacas,
                                   ModeloVeiculo = obj.Carga?.ModeloVeicularCarga?.Descricao + ObterModeloCarroceria(obj.Carga?.Veiculo),
                                   DescricaoSituacao = obj.Situacao.ObterDescricao(),
                                   CarregamentoFinalizado = obj.DataFinalCarregamento.HasValue ? "Sim" : "Não",
                                   DT_RowColor = ObterRowColor(obj),
                                   DT_FontColor = ObterFontColor(obj)
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> CarregamentoFinalizado()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaJanelaCarregamentoGuarita;
                int.TryParse(Request.Params("Codigo"), out codigoCargaJanelaCarregamentoGuarita);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repCargaGuarita.BuscarPorCodigo(codigoCargaJanelaCarregamentoGuarita);

                if (!cargaGuarita.DataFinalCarregamento.HasValue)
                {
                    cargaGuarita.DataFinalCarregamento = DateTime.Now;
                    repCargaGuarita.Atualizar(cargaGuarita);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaGuarita, null, "Informou Carregamento Finalizado.", unidadeDeTrabalho);

                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Está carga já teve seu carregamento finalizado.");
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDetalhesCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita = repCargaGuarita.BuscarPorCodigo(codigo);

                byte[] pdf = Servicos.Embarcador.Carga.Carga.GerarRelatorioDetalhesCarga(cargaJanelaCarregamentoGuarita.Carga.Codigo, unidadeTrabalho);

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o relatório de detalhes da carga. Tente novamente.");

                return Arquivo(pdf, "application/pdf", "Carga " + cargaJanelaCarregamentoGuarita.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos detalhes da carga.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarDetalhesCargaPorEmail()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string observacao = Request.Params("Observacao");

                dynamic emails = JsonConvert.DeserializeObject<dynamic>(Request.Params("Emails"));

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita = repCargaGuarita.BuscarPorCodigo(codigo);

                List<string> listaEmails = new List<string>();

                foreach (dynamic email in emails)
                    listaEmails.Add((string)email.Email);

                string mensagemErro;

                if (!Servicos.Embarcador.Carga.Carga.EnviarRelatorioDetalhesCargaPorEmail(cargaGuarita.Carga.Codigo, listaEmails, observacao, unidadeTrabalho, Usuario, out mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaGuarita, null, "Enviou detalhes da Carga por E-mail.", unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar os detalhes da carga por e-mail.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterEmailsCentroCarregamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroCarregamento;
                int.TryParse(Request.Params("CentroCarregamento"), out codigoCentroCarregamento);

                Repositorio.Embarcador.Logistica.CentroCarregamentoEmail repEmail = new Repositorio.Embarcador.Logistica.CentroCarregamentoEmail(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail> emails = repEmail.BuscarPorCentroCarregamento(codigoCentroCarregamento);

                return new JsonpResult((from obj in emails select new { Codigo = obj.Codigo, Email = obj.Email }).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os e-mails do centro de carregamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterModeloCarroceria(Dominio.Entidades.Veiculo veiculo)
        {
            string modeloCarroceria = veiculo?.ModeloCarroceria?.Descricao ?? string.Empty;

            return !string.IsNullOrWhiteSpace(modeloCarroceria) ? " - (" + modeloCarroceria + ")" : string.Empty;
        }

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            List<string> placas = new List<string>();

            if (veiculo != null)
                placas.Add(veiculo.Placa);

            placas.AddRange(veiculosVinculados.Select(o => o.Placa));

            return string.Join(", ", placas);
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataCarregamento")
                return "CargaJanelaCarregamento.InicioCarregamento";

            if (propriedadeOrdenar == "NumeroCarga")
                return "CargaJanelaCarregamento.Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Transportador")
                return "CargaJanelaCarregamento.Carga.Empresa.RazaoSocial";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenar == "CentroCarregamento")
                return "CargaJanelaCarregamento.CentroCarregamento.Descricao";

            if (propriedadeOrdenar == "CarregamentoFinalizado")
                return "DataFinalCarregamento";

            return propriedadeOrdenar;
        }

        private string ObterRowColor(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita)
        {
            // CODIGO DO LUQUINHAS QUERIDO <3
            /*if (cargaGuarita.CargaJanelaCarregamento.Carga != null && cargaGuarita.CargaJanelaCarregamento.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza;

            else if (cargaGuarita.DataLiberacaoVeiculo.HasValue && cargaGuarita.DataLiberacaoVeiculo.Value >= DateTime.Now) // O motorista se apresentou
                return "#fdc99e";

            else if (cargaGuarita.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita.AgChegadaVeiculo && !cargaGuarita.DataLiberacaoVeiculo.HasValue ) // O motorista não se apresentou
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;

            else if (cargaGuarita.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita.Liberada) // Liberou para entrar
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde;

            else // Não está liberado
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo;*/

            // CODIGO DO GOTARDO CHATO
            if (cargaGuarita.Carga != null && cargaGuarita.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza;
            else if (cargaGuarita.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita.Liberada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde;
            else if (cargaGuarita.CargaJanelaCarregamento != null && cargaGuarita.CargaJanelaCarregamento.InicioCarregamento < DateTime.Now)
                if (cargaGuarita.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita.AgChegadaVeiculo)
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Laranja;
                else
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo;


        }

        private string ObterFontColor(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaGuarita)
        {
            if ((cargaGuarita.Carga != null && cargaGuarita.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada) ||
                (cargaGuarita.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita.AguardandoLiberacao &&
                 cargaGuarita.CargaJanelaCarregamento != null && cargaGuarita.CargaJanelaCarregamento.InicioCarregamento < DateTime.Now))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco;

            return string.Empty;
        }

        #endregion
    }
}
