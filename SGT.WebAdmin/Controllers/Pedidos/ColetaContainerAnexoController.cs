using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using Dominio.ObjetosDeValor.OCR;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Cargas/Carga")]
    public class ColetaContainerAnexoController : BaseController
    {
        #region Construtores

        public ColetaContainerAnexoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarRicPelaColetaContainer()
        {
            int codigoColetaContainer = Request.GetIntParam("CodigoColetaContainer");
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var repositorioRic = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic(unitOfWork);
                var repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                var coletaContainer = repositorioColetaContainer.BuscarPorCodigo(codigoColetaContainer, auditavel: false);
                string container = coletaContainer?.Container?.Numero?.Length == 11 ? coletaContainer?.Container?.Numero : coletaContainer?.Container?.Descricao;
                if (string.IsNullOrEmpty(container) || container.Length != 11)
                    return new JsonpResult(new
                    {
                        Ric = false
                    });

                var ric = repositorioRic.BuscarPorNumeroContainer(container);

                if (ric != null)
                    return new JsonpResult(new
                    {
                        Ric = ric.ConverterEmDTO()
                    });
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return new JsonpResult(new
            {
                Ric = false
            });
        }

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                bool ocrRic = Request.GetBoolParam("OcrRic");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");

                ObjetoRicRetorno objetoRicRetorno = null;
                Dominio.Entidades.Embarcador.Pedidos.Container containerDoRic = null;
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic entidadeReciboDeContainer = null;

                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repositorioCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCodigo(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculo = repositorioCargaVeiculoContainer.BuscarPorCargaEVeiculo(codigoCarga, carga.Veiculo?.Codigo ?? 0);

                if (ocrRic && arquivos.Count > 0)
                {
                    objetoRicRetorno = ProcessarImagemRicOCR(arquivos[0].InputStream, unitOfWork);

                    if (objetoRicRetorno != null && !string.IsNullOrEmpty(objetoRicRetorno.Erro))
                        return new JsonpResult(false, false, objetoRicRetorno.Erro);

                    if (objetoRicRetorno != null && !string.IsNullOrEmpty(objetoRicRetorno.Container) && objetoRicRetorno.Container.Length == 11)
                    {
                        Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                        containerDoRic = repositorioContainer.BuscarPorNumero(objetoRicRetorno.Container);
                        if (containerDoRic == null)
                        {
                            var servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                            containerDoRic = servicoColetaContainer.CadastrarContainer(objetoRicRetorno.ConverterEmObjetoDeValor(), Auditado);
                        }
                    }
                    if (objetoRicRetorno != null)
                    {
                        if (containerDoRic == null)
                            containerDoRic = coletaContainer?.Container;

                        if (containerDoRic != null)
                        {
                            objetoRicRetorno.CodigoContainer = containerDoRic.Codigo;
                            objetoRicRetorno.Container = containerDoRic.Numero;
                            if (containerDoRic.Codigo > 0)
                            {
                                var objValor = objetoRicRetorno.ConverterEmObjetoDeValor();
                                entidadeReciboDeContainer = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic()
                                {
                                    ArmadorBooking = objValor.ArmadorBooking,
                                    Container = containerDoRic,
                                    ContainerDescricao = objValor.Container,
                                    DataDeColeta = objValor.DataDeColeta,
                                    Motorista = objValor.Motorista,
                                    Placa = objValor.Placa,
                                    TaraContainer = objValor.TaraContainer,
                                    TipoContainer = objValor.TipoContainer,
                                    Transportadora = objValor.Transportadora
                                };
                            }
                        }
                    }
                }

                if (coletaContainer == null && carga == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o registro.");

                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, false, "Nenhum arquivo selecionado para envio.");

                Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic repositorioRic = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic(unitOfWork);
                string caminho = ObterCaminhoArquivos(unitOfWork);

                for (int i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile arquivo = arquivos[i];
                    string extensaoArquivo = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                    arquivo.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"));

                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo anexo = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo()
                    {
                        ColetaContainer = coletaContainer != null ? coletaContainer : null,
                        Carga = carga,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.FileName)))
                    };

                    if (entidadeReciboDeContainer != null)
                    {
                        repositorioRic.Inserir(entidadeReciboDeContainer, Auditado);
                        if (entidadeReciboDeContainer.Codigo > 0)
                            anexo.ColetaContainerAnexoRic = entidadeReciboDeContainer;
                    }

                    repositorioColetaContainerAnexo.Inserir(anexo, Auditado);

                    if (cargaVeiculo != null)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaVeiculo, null, $"Adicionou o arquivo {anexo.NomeArquivo}.", unitOfWork);
                    else
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Adicionou o arquivo {anexo.NomeArquivo}.", unitOfWork);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> anexos = repositorioColetaContainerAnexo.BuscarPorColetaContainerECarga(codigo, codigoCarga);

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo
                    }
                ).ToList();

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos,
                    Ric = objetoRicRetorno
                });
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private ObjetoRicRetorno ProcessarImagemRicOCR(Stream imagem, Repositorio.UnitOfWork unitOfWork)
        {
            MemoryStream ms = new MemoryStream();
            imagem.CopyTo(ms);
            var servicoOcr = new Servicos.Global.ServicoOCR(unitOfWork);
            var conversorRic = new Servicos.Global.ConversorDeTextoOcrEmRIC(servicoOcr, ms.ToArray());
            Servicos.Global.IObjetoModeloRic modeloRicOcr = null;
            try
            {
                modeloRicOcr = conversorRic.ExecutarConversao();
            }
            catch (Exception ex)
            {
                return new ObjetoRicRetorno { Erro = ex.Message };
            }

            if (modeloRicOcr == null)
                return null;

            var dto = conversorRic.ConverterEmDTO(modeloRicOcr);
            return dto;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repositorioCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);


                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo coletaContainerAnexo = repositorioColetaContainer.BuscarPorCodigo(codigo, auditavel: false);
                int codigoCarga = coletaContainerAnexo?.Carga?.Codigo ?? 0;
                int codigoVeiculo = coletaContainerAnexo?.Carga?.Veiculo?.Codigo ?? 0;

                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculo = repositorioCargaVeiculoContainer.BuscarPorCargaEVeiculo(codigoCarga, codigoVeiculo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (coletaContainerAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Pedido.ColetaContainerAnexo servicoColetaContainerAnexo = new Servicos.Embarcador.Pedido.ColetaContainerAnexo(unitOfWork);
                byte[] arquivoBinario = servicoColetaContainerAnexo.DownloadAnexo(coletaContainerAnexo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Realizou o download do arquivo {coletaContainerAnexo.NomeArquivo}.", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", coletaContainerAnexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaVeiculoContainer repositorioCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo coletaContainerAnexo = repositorioColetaContainer.BuscarPorCodigo(codigo, auditavel: false);
                int codigoCarga = coletaContainerAnexo?.Carga?.Codigo ?? 0;
                int codigoVeiculo = coletaContainerAnexo?.Carga?.Veiculo?.Codigo ?? 0;

                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculo = repositorioCargaVeiculoContainer.BuscarPorCargaEVeiculo(codigoCarga, codigoVeiculo);

                if (coletaContainerAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Pedido.ColetaContainerAnexo servicoColetaContainerAnexo = new Servicos.Embarcador.Pedido.ColetaContainerAnexo(unitOfWork);

                servicoColetaContainerAnexo.ExcluirAnexo(coletaContainerAnexo);

                if (cargaVeiculo != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaVeiculo, null, $"Excluiu o arquivo {coletaContainerAnexo.NomeArquivo}.", unitOfWork);
                else if (coletaContainerAnexo.Carga != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, coletaContainerAnexo.Carga, null, $"Excluiu o arquivo {coletaContainerAnexo.NomeArquivo}.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ObterAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> anexos;

                if (codigo > 0)
                {
                    Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(unitOfWork);
                    anexos = repositorioColetaContainerAnexo.BuscarPorColetaContainerECarga(codigo);
                }
                else
                    anexos = new List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo>();

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }



        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAnexo()
        {
            //ATENÇÃO BUSCA UTILIZADA EM: 
            //RetiradaContainerAnexo (onde nao envia e nao possui coletaContainer) e ColetaContainerAnexo (envia codigo coletaContainer), mas tudo deve se basear NA COLETACONTAINER.

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repositorioColetaContainerHistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, false);

                // Dados do filtro
                int codigo = Request.GetIntParam("Codigo");

                //int codigoContainer = Request.GetIntParam("CodigoContainer");
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> anexos = new List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo>();

                if (codigo == 0 && codigoCarga > 0)
                //nao veio o codigoColetaContainer.
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(codigoCarga);
                    if (coletaContainer == null)
                        coletaContainer = repositorioColetaContainer.BuscarPorCargaDeColeta(codigoCarga);

                    if (coletaContainer != null)
                        codigo = coletaContainer.Codigo;
                }

                if (codigoCarga > 0)
                    anexos = repColetaContainerAnexo.BuscarPorColetaContainerECarga(codigo, codigoCarga);

                if (anexos.Count <= 0 && codigo > 0)
                    anexos = repColetaContainerAnexo.BuscarPorColetaContainerECarga(codigo);

                int totalRegistros = anexos.Count;
                var lista = from obj in anexos
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.NomeArquivo
                            };

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


        #endregion

        #region Métodos Protegidos

        protected string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo).Name })}";
        }

        #endregion
    }
}
