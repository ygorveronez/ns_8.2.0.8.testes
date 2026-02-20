using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace SGT.WebAdmin.Controllers.Cargas.ComprovanteCarga
{
    [CustomAuthorize("Cargas/ComprovanteCarga")]
    public class ComprovanteCargaController : BaseController
    {
        #region Construtores

        public ComprovanteCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(GridPesquisa(unitOfWork));
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovanteCarga = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga ComprovanteCarga = repComprovanteCarga.BuscarPorCodigo(codigo);

                if (ComprovanteCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    Codigo = ComprovanteCarga.Codigo,
                    Carga = ComprovanteCarga.Carga.CodigoCargaEmbarcador,
                    DataEntrega = ComprovanteCarga.DataEntrega,
                    DataJustificativa = ComprovanteCarga.DataJustificativa,
                    MotivoJustificativa = ComprovanteCarga.MotivoJustificativa,
                    Remetente = ComprovanteCarga.Carga.DadosSumarizados.Remetentes,
                    Destinatario = ComprovanteCarga.Carga.DadosSumarizados.Destinatarios,
                    Origem = ComprovanteCarga.Carga.DadosSumarizados.Origens,
                    Destino = ComprovanteCarga.Carga.DadosSumarizados.Destinos,
                    Motorista = ComprovanteCarga.Carga.DadosSumarizados.Motoristas,
                    Veiculo = ComprovanteCarga.Carga.DadosSumarizados.Veiculos,
                    Situacao = ComprovanteCarga.Situacao.ObterDescricao()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarComprovante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                var codigoComprovante = Request.Params("Codigo");
                var dataComprovante = Request.Params("DataEntrega");

                int codigo = 0;
                DateTime dataEntrega;
                int.TryParse(codigoComprovante, out codigo);
                DateTime.TryParse(dataComprovante, out dataEntrega);

                string caminhoBase = "";
                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine("ComprovantesCargas", "Comprovantes");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovanteCarga = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovanteCarga = repComprovanteCarga.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoArquivo.BuscarPrimeiroRegistro();

                if (configuracaoArquivo != null)
                    caminhoBase = configuracaoArquivo.CaminhoArquivos;

                if (String.IsNullOrEmpty(caminhoBase))
                    return new JsonpResult(false, "Caminho para gravar os arquivos dos comprovantes não encontraro.");

                caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(caminhoBase, caminhoSave);

                Servicos.DTO.CustomFile file = arquivos[0];

                var nomeArquivo = file.FileName;
                var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);

                file.SaveAs(caminho);

                comprovanteCarga.Initialize();
                comprovanteCarga.DataEntrega = dataEntrega;
                comprovanteCarga.NomeArquivo = guidArquivo + extensaoArquivo;
                comprovanteCarga.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Recebido;

                repComprovanteCarga.Atualizar(comprovanteCarga, Auditado);

                unitOfWork.CommitChanges();


                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadComprovante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                string caminhoBase = "";
                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine("ComprovantesCargas", "Comprovantes");

                Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovanteCarga = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovanteCarga = repComprovanteCarga.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoArquivo.BuscarPrimeiroRegistro();

                if (configuracaoArquivo != null)
                    caminhoBase = configuracaoArquivo.CaminhoArquivos;

                if (String.IsNullOrEmpty(caminhoBase))
                    return new JsonpResult(false, "Caminho para baixar os arquivos dos comprovantes não encontraro.");

                if (comprovanteCarga == null)
                    return new JsonpResult(false, "Anexo não encontrado no Banco de Dados.");

                caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(caminhoBase, caminhoSave);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, comprovanteCarga.NomeArquivo)))
                    return new JsonpResult(false, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, comprovanteCarga.NomeArquivo)), "image/jpeg", comprovanteCarga.NomeArquivo);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o arquivo anexo do comprovante.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarComprovante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");
                string operacao = Request.GetStringParam("Operacao");

                string caminhoBase = "";
                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine("ComprovantesCargas", "Comprovantes");

                Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovanteCarga = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovanteCarga = repComprovanteCarga.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoArquivo.BuscarPrimeiroRegistro();

                if (configuracaoArquivo != null)
                    caminhoBase = configuracaoArquivo.CaminhoArquivos;

                if (String.IsNullOrEmpty(caminhoBase))
                    return new JsonpResult(false, "Caminho para gravar os arquivos dos comprovantes não encontraro.");

                caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(caminhoBase, caminhoSave);

                comprovanteCarga.Initialize();
                if (operacao == "Justificar")
                {
                    comprovanteCarga.DataJustificativa = Request.GetDateTimeParam("DataJustificativa");
                    comprovanteCarga.MotivoJustificativa = Request.GetStringParam("MotivoJustificativa");
                    comprovanteCarga.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Justificado;
                }
                else if (operacao == "Reverter")
                {
                    if (comprovanteCarga.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Recebido)
                    {
                        var arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, comprovanteCarga.NomeArquivo);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                            Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                        comprovanteCarga.DataEntrega = null;
                        comprovanteCarga.NomeArquivo = null;
                    }
                    else if (comprovanteCarga.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Justificado)
                    {
                        comprovanteCarga.DataJustificativa = null;
                        comprovanteCarga.MotivoJustificativa = null;
                    }
                    comprovanteCarga.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Pendente;
                }
                repComprovanteCarga.Atualizar(comprovanteCarga, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);
                byte[] bArquivo = grid.GerarExcel();
                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Código", "Codigo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "Carga", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data da Carga", "DataCarga", 20, Models.Grid.Align.center, true);

            Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaComprovanteCarga filtrosPesquisa = ObterFiltrosPesquisa();
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
            Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovanteCarga = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
            int totalRegistro = repComprovanteCarga.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga> comprovantesCarga =
                (totalRegistro > 0) ?
                repComprovanteCarga.Consultar(filtrosPesquisa, parametrosConsulta) :
                new List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga>();

            var tiposComprovantesRetornar = (
                from comprovanteCarga in comprovantesCarga
                select new
                {
                    Codigo = comprovanteCarga.Codigo,
                    Tipo = comprovanteCarga.TipoComprovante.Descricao,
                    Carga = comprovanteCarga.Carga.CodigoCargaEmbarcador,
                    Remetente = comprovanteCarga.Carga.DadosSumarizados.Remetentes,//Pedidos.FirstOrDefault().Pedido.Remetente.Nome,
                    Origem = comprovanteCarga.Carga.DadosSumarizados.Origens,//Pedidos.FirstOrDefault().Origem.Descricao,
                    Destinatario = comprovanteCarga.Carga.DadosSumarizados.Destinatarios,//Pedidos.FirstOrDefault().Pedido.Destinatario.Nome,
                    Destino = comprovanteCarga.Carga.DadosSumarizados.Destinos,//Pedidos.FirstOrDefault().Destino.Descricao,
                    Motorista = comprovanteCarga.Carga.DadosSumarizados.Motoristas,//Motoristas.FirstOrDefault().Nome,
                    Veiculo = comprovanteCarga.Carga.DadosSumarizados.Veiculos,// Veiculo?.Descricao ?? "",
                    Situacao = comprovanteCarga.Situacao.ObterDescricao(),
                    DT_RowClass = this.CorPorPrazo(comprovanteCarga),
                    DataCarga = comprovanteCarga.Carga.DataCriacaoCarga
                }
            ).ToList();

            grid.AdicionaRows(tiposComprovantesRetornar);
            grid.setarQuantidadeTotal(totalRegistro);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            return "Codigo";//prop;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaComprovanteCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaComprovanteCarga()
            {
                Codigo = Request.GetIntParam("Codigo"),
                Carga = Request.GetIntParam("Carga"),
                Situacao = Request.GetNullableEnumParam<SituacaoComprovanteCarga>("Situacao"),
                TipoComprovante = Request.GetIntParam("TipoComprovante"),
                MotoristaCarga = Request.GetListParam<int>("MotoristaCarga"),
                VeiculosCarga = Request.GetListParam<int>("VeiculosCarga"),
                DataCarga = Request.GetDateTimeParam("DataCarga")
            };
        }

        private string CorPorPrazo(Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovante)
        {
            if (comprovante.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Recebido)
                return ClasseCorFundo.Sucess(IntensidadeCor._100);

            if (comprovante.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Justificado)
                return ClasseCorFundo.Info(IntensidadeCor._100);

            if (comprovante.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Pendente)
                return "";

            return "";
        }

        #endregion
    }
}
