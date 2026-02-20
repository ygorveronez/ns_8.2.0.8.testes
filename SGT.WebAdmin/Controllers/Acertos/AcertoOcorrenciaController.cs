using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize("Acertos/AcertoViagem")]
    public class AcertoOcorrenciaController : BaseController
    {
		#region Construtores

		public AcertoOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AtualizarOcorrencias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AtualizarOcorrencias " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de carga antes.");

                acertoViagem.Etapa = etapa;
                acertoViagem.Situacao = situacao;
                acertoViagem.DataAlteracao = DateTime.Now;
                acertoViagem.OcorrenciaSalvo = true;

                repAcertoViagem.Atualizar(acertoViagem, Auditado);
                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Ocorrencias, this.Usuario);

                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as ocorrências.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarOcorrencias " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarAcertoOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarAcertoOcorrencia " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Ocorrência", "NumeroOcorrencia", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data da Ocorrência", "DataOcorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "CodigoCargaEmbarcador")
                    propOrdenar = "Carga";
                else if (propOrdenar == "Valor")
                    propOrdenar = "ValorOcorrencia";

                Repositorio.Embarcador.Acerto.AcertoOcorrencia repAcertoOcorrencia = new Repositorio.Embarcador.Acerto.AcertoOcorrencia(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaGrid = repAcertoOcorrencia.Consultar(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repAcertoOcorrencia.ContarConsulta(codigoAcerto);
                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (from p in listaGrid
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroOcorrencia,
                                 DataOcorrencia = p.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                                 CodigoCargaEmbarcador = p.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                 Veiculo = p.Carga?.RetornarPlacas ?? string.Empty,
                                 TipoOcorrencia = p.TipoOcorrencia != null ? p.TipoOcorrencia.Descricao : "",
                                 Motorista = p.Carga?.NomeMotoristas ?? string.Empty,
                                 Componente = p.ComponenteFrete != null ? p.ComponenteFrete.Descricao : "Sem complemento",
                                 Valor = p.ValorOcorrencia.ToString("n2"),
                                 p.DescricaoSituacao
                             }).ToList();

                if (lista != null && lista.Count > 0)
                    grid.AdicionaRows(lista);
                else
                    grid.AdicionaRows(null);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarAcertoOcorrencia " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarOcorrenciaComissaoFuncionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoComissaoFuncionario = 0;
                int.TryParse(Request.Params("ComissaoFuncionario"), out codigoComissaoFuncionario);

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                int numeroOcorrencia = 0;
                int.TryParse(Request.Params("NumeroOcorrencia"), out numeroOcorrencia);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Ocorrência", "NumeroOcorrencia", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data da Ocorrência", "DataOcorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "CodigoCargaEmbarcador")
                    propOrdenar = "Carga";
                else if (propOrdenar == "Valor")
                    propOrdenar = "ValorOcorrencia";

                Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaGrid = repComissaoFuncionarioMotoristaDocumento.ConsultarSemComissaoMotorista(numeroOcorrencia, codigoComissaoFuncionario, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repComissaoFuncionarioMotoristaDocumento.ContarConsultaSemComissaoMotorista(numeroOcorrencia, codigoComissaoFuncionario);
                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (from p in listaGrid
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroOcorrencia,
                                 DataOcorrencia = p.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                                 CodigoCargaEmbarcador = p.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                 Veiculo = p.Carga?.RetornarPlacas ?? string.Empty,
                                 TipoOcorrencia = p.TipoOcorrencia != null ? p.TipoOcorrencia.Descricao : "",
                                 Motorista = p.Carga?.NomeMotoristas ?? string.Empty,
                                 Componente = p.ComponenteFrete != null ? p.ComponenteFrete.Descricao : "Sem complemento",
                                 Valor = p.ValorOcorrencia.ToString("n2"),
                                 p.DescricaoSituacao
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
        public async Task<IActionResult> PesquisarOcorrenciaSemAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarOcorrenciaSemAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("AcertoViagem"), out codigoAcerto);
                bool buscarTodasOcorrencias = Request.GetBoolParam("BuscarTodasOcorrencias");

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                int numeroOcorrencia = 0;
                int.TryParse(Request.Params("NumeroOcorrencia"), out numeroOcorrencia);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Ocorrência", "NumeroOcorrencia", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data da Ocorrência", "DataOcorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "CodigoCargaEmbarcador")
                    propOrdenar = "Carga";
                else if (propOrdenar == "Valor")
                    propOrdenar = "ValorOcorrencia";

                Repositorio.Embarcador.Acerto.AcertoOcorrencia repAcertoOcorrencia = new Repositorio.Embarcador.Acerto.AcertoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                if (acerto == null)
                    return new JsonpResult(false, "Favor selecione um acerto de viagem antes.");

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaGrid = repAcertoOcorrencia.ConsultarSemAcerto(numeroOcorrencia, acerto, buscarTodasOcorrencias, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repAcertoOcorrencia.ContarConsultaSemAcerto(numeroOcorrencia, acerto, buscarTodasOcorrencias);
                grid.setarQuantidadeTotal(totalRegistros);

                var lista = (from p in listaGrid
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroOcorrencia,
                                 DataOcorrencia = p.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                                 CodigoCargaEmbarcador = p.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                 Veiculo = p.Carga?.RetornarPlacas ?? string.Empty,
                                 TipoOcorrencia = p.TipoOcorrencia != null ? p.TipoOcorrencia.Descricao : "",
                                 Motorista = p.Carga?.NomeMotoristas ?? string.Empty,
                                 Componente = p.ComponenteFrete != null ? p.ComponenteFrete.Descricao : "Sem complemento",
                                 Valor = p.ValorOcorrencia.ToString("n2"),
                                 p.DescricaoSituacao
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
                Servicos.Log.TratarErro(" Fim Controller PesquisarOcorrenciaSemAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverOcorrencia " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoOcorrencia repAcertoOcorrencia = new Repositorio.Embarcador.Acerto.AcertoOcorrencia(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start();

                int codigo, codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia ocorrencia = repAcertoOcorrencia.BuscarPorOcorrenciaAcerto(codigo, codigoAcerto);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia.AcertoViagem, null, "Removida a ocorrência " + ocorrencia.Descricao + " do acerto.", unitOfWork);
                repAcertoOcorrencia.Deletar(ocorrencia, Auditado);
                servAcertoViagem.InserirLogAcerto(repAcertoViagem.BuscarPorCodigo(codigoAcerto), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.RemoveuOcorrencia, this.Usuario);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a ocorrência.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverOcorrencia " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirOcorrencia " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoOcorrencia repAcertoOcorrencia = new Repositorio.Embarcador.Acerto.AcertoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start();

                int codigo, codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia();
                ocorrencia.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                ocorrencia.CargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);
                ocorrencia.LancadoManualmente = true;

                repAcertoOcorrencia.Inserir(ocorrencia, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia.AcertoViagem, null, "Adicionada a ocorrência " + ocorrencia.Descricao + " ao acerto.", unitOfWork);
                servAcertoViagem.InserirLogAcerto(repAcertoViagem.BuscarPorCodigo(codigoAcerto), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.AdicionouOcorrencia, this.Usuario);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir a ocorrência.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirOcorrencia " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

    }
}


