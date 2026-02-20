using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Servicos.DTO;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize("Canhotos/VincularCanhotoManual")]
    public class VincularCanhotoManualController : BaseController
    {
        #region Construtores

        public VincularCanhotoManualController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> Renderizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            MemoryStream stream = new MemoryStream();
            string nome = "Canhoto.pdf";

            try
            {

                int codigo = Request.GetIntParam("Codigo");
                int codigoCanhoto = (codigo == 0) ? Request.GetIntParam("Canhoto") : 0;
                Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repositorioControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto canhoto = null;
                List<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto> controleCanhotos = (codigo + codigoCanhoto) > 0 ? repositorioControleLeituraImagemCanhoto.BuscarPorTodosCodigoOuCanhoto(codigo, codigoCanhoto) : new List<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto>();

                if (controleCanhotos.Count == 1)
                    canhoto = controleCanhotos.FirstOrDefault();
                else if (controleCanhotos.Count > 1)
                {
                    canhoto = controleCanhotos.Where(o => o.SituacaoleituraImagemCanhoto != SituacaoleituraImagemCanhoto.Descartada && o.SituacaoleituraImagemCanhoto != SituacaoleituraImagemCanhoto.FalhaProcessamento).OrderByDescending(o => o.Codigo).FirstOrDefault();

                    if (canhoto == null)
                        canhoto = controleCanhotos.OrderByDescending(o => o.Codigo).FirstOrDefault();
                }


                if (canhoto != null)
                {
                    Servicos.Embarcador.Canhotos.LeitorOCR servicoLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);
                    nome = canhoto.NomeArquivo;
                    stream = servicoLeitorOCR.ObterStremingPDF(canhoto, unitOfWork);
                }

                if ((stream == null || stream.Length <= 0) && codigoCanhoto > 0)
                {
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhotoCanhoto = repositorioCanhoto.BuscarPorCodigo(codigoCanhoto);

                    if (canhotoCanhoto != null)
                    {
                        Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                        nome = canhotoCanhoto.NomeArquivo;
                        stream = servicoCanhoto.ObterStremingPDFCanhoto(canhotoCanhoto, unitOfWork);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return File(stream, "application/pdf", nome);
        }

        public async Task<IActionResult> UploadImagens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Canhotos.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);
                string cnpj = "";
                string strDataEntrega = "";

                double cpfCnpj = Request.GetDoubleParam("Cliente");
                DateTime dataEntrega = Request.GetDateTimeParam("DataEntrega");

                if (cpfCnpj > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
                    cnpj = cliente.CPF_CNPJ_SemFormato;
                }
                else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                    Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                    if ((operadorLogistica != null) && (operadorLogistica.Filiais.Count == 1))
                        filial = operadorLogistica.Filiais.First().Filial;

                    if (filial == null)
                        return new JsonpResult(false, "O usuário não está vinculado a uma filial para enviar os canhotos.");

                    cnpj = filial.CNPJ;
                }

                if (string.IsNullOrEmpty(cnpj))
                    return new JsonpResult(false, "O emissor das notas fiscais deve ser informado.");

                if (dataEntrega != DateTime.MinValue)
                    strDataEntrega = dataEntrega.ToString("ddMMyyyyHHmmss");

                if (dataEntrega != DateTime.MinValue && dataEntrega > DateTime.Now)
                    return new JsonpResult(false, true, "Data de entrega não pode ser maior que a data atual.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Imagem").ToList();
                List<string> erros = new List<string>();
                List<string> extensoesValidas = new List<string>() {
                    ".jpg", ".tif", ".pdf"
                };
                int adicionados = 0;

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];

                    string extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!extensoesValidas.Contains(extensaoArquivo))
                    {
                        erros.Add("Extensão " + extensaoArquivo + " não permitida.");
                        continue;
                    }
                    try
                    {
                        string caminhoRaiz = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            srvLeitorOCR.AdicionarArquivoNaPastaProcessados(unitOfWork, file, caminhoRaiz, cnpj, this.Usuario, strDataEntrega);
                        else
                            srvLeitorOCR.AdicionarArquivoNaPastaEnviados(unitOfWork, file, caminhoRaiz, cnpj, this.Usuario, strDataEntrega);

                        adicionados++;
                    }
                    catch (Exception e)
                    {
                        erros.Add("Erro ao processar arquivo " + file.FileName + ".");
                        Servicos.Log.TratarErro(e);
                    }
                }

                return new JsonpResult(new
                {
                    Adicionados = adicionados,
                    Erros = erros
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo a ocorrência.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repositorioControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto = repositorioControleLeituraImagemCanhoto.BuscarPorCodigo(codigo);

                if (controleLeituraImagemCanhoto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Canhotos.LeitorOCR servcoLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);

                var retorno = new
                {
                    controleLeituraImagemCanhoto.Codigo,
                    Situacao = controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto,
                    controleLeituraImagemCanhoto.Extensao,
                    Canhoto = controleLeituraImagemCanhoto.Canhoto != null ? new { controleLeituraImagemCanhoto.Canhoto.Codigo, controleLeituraImagemCanhoto.Canhoto.Descricao } : null,
                    Imagem = controleLeituraImagemCanhoto.Extensao != ExtensaoArquivo.PDF ? servcoLeitorOCR.ObterBase64DaImagem(controleLeituraImagemCanhoto, unitOfWork) : null
                };
                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Confirmar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                // Parametros
                int codigo = Request.GetIntParam("Codigo");
                int codigoCanhoto = Request.GetIntParam("Canhoto");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto imagemCanhoto = repControleLeituraImagemCanhoto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);

                // Valida
                if (imagemCanhoto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (imagemCanhoto.Canhoto != null)
                    return new JsonpResult(false, true, "Já existe um canhoto vinculado.");

                if (canhoto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                unitOfWork.Start();

                string caminhoNaoReconhecido = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoCanhotos, "NaoReconhecidos");
                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoNaoReconhecido, imagemCanhoto.GuidArquivo);

                DateTime? dataEntregaNotaCliente = Request.GetNullableDateTimeParam("DataEntrega");

                if (configuracaoEmbarcador.ExigirDataEntregaNotaClienteCanhotos && !canhoto.DataEntregaNotaCliente.HasValue)
                {
                    if (!dataEntregaNotaCliente.HasValue)
                        return new JsonpResult(false, true, "É obrigatório informar uma data de entrega.");

                    if (canhoto.DataEntregaNotaCliente.HasValue && dataEntregaNotaCliente.HasValue)
                        return new JsonpResult(false, true, "Data de entrega já foi informada.");

                    canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente;
                }

                srvLeitorOCR.VincularCanhotoDaDigitalizacao(canhoto, caminhoCompleto, imagemCanhoto, TipoLeituraImagemCanhoto.Automatico, unitOfWork, this.Usuario, TipoServicoMultisoftware);

                repCanhoto.Atualizar(canhoto, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Descartar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto imagemCanhoto = repControleLeituraImagemCanhoto.BuscarPorCodigo(codigo);

                // Valida
                if (imagemCanhoto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (imagemCanhoto.Canhoto != null)
                    return new JsonpResult(false, true, "Já existe um canhoto vinculado.");

                // Persiste dados
                unitOfWork.Start();
                string caminhoNaoReconhecido = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoCanhotos, "NaoReconhecidos");
                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoNaoReconhecido, imagemCanhoto.GuidArquivo);
                srvLeitorOCR.DescartarDigitalizacao(imagemCanhoto, caminhoCompleto, unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroDocumento").Nome("Número Documento").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("NomeArquivo").Nome("Nome Arquivo").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Data").Nome("Data").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("MensagemRetorno").Nome("Retorno").Tamanho(30).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);

            // Dados do filtro
            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto>("Situacao");
            string numeroDocumento = Request.Params("NumeroDocumento") ?? string.Empty;
            int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : 0;
            // Consulta
            List<Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto> listaGrid = repControleLeituraImagemCanhoto.Consultar(dataInicio, dataFim, situacao, numeroDocumento, codigoEmpresa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repControleLeituraImagemCanhoto.ContarConsulta(dataInicio, dataFim, situacao, numeroDocumento, codigoEmpresa);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            NumeroDocumento = obj.NumeroDocumento ?? string.Empty,
                            obj.NomeArquivo,
                            Data = obj.Data.ToString("dd/MM/yyyy HH:mm"),
                            Situacao = obj.DescricaoSituacao,
                            obj.MensagemRetorno
                        };

            return lista.ToList();
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Situacao") propOrdenar = "SituacaoleituraImagemCanhoto";
        }
        #endregion
    }
}
