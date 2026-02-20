using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/ContratoNotaFiscal")]
    public class ContratoNotaFiscalController : BaseController
    {
        #region Construtores

        public ContratoNotaFiscalController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaContratoNotaFiscal filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Contrato", "Contrato", 55, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.EmpresaContrato repContratoNotaFiscal = new Repositorio.EmpresaContrato(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.EmpresaContrato> listaContratoNotaFiscal = repContratoNotaFiscal.ConsultarTMS(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repContratoNotaFiscal.ContarConsultaTMS(filtrosPesquisa));

                var lista = (from p in listaContratoNotaFiscal
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Contrato = p.ContratoFormatado,
                                 p.DescricaoAtivo
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


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.EmpresaContrato repContratoNotaFiscal = new Repositorio.EmpresaContrato(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    Dominio.Entidades.EmpresaContrato contratoAntigo = repContratoNotaFiscal.BuscarPorEmpresaTMS(Empresa.Codigo);
                    if (contratoAntigo != null)
                        return new JsonpResult(false, true, "Não é permitido adicionar mais que um contrato!");
                }

                Dominio.Entidades.EmpresaContrato contratoNotaFiscal = new Dominio.Entidades.EmpresaContrato();

                PreencherContratoNotaFiscal(contratoNotaFiscal);

                SalvarConfiguracoes(contratoNotaFiscal);

                repContratoNotaFiscal.Inserir(contratoNotaFiscal, Auditado);

                SalvarTransportadores(contratoNotaFiscal, unitOfWork);

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    contratoNotaFiscal.Codigo
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.EmpresaContrato repContratoNotaFiscal = new Repositorio.EmpresaContrato(unitOfWork);
                Repositorio.EmpresaContratoAnexo repContratoNotaFiscalAnexo = new Repositorio.EmpresaContratoAnexo(unitOfWork);

                Dominio.Entidades.EmpresaContrato contratoNotaFiscal = repContratoNotaFiscal.BuscarPorCodigo(codigo, true);

                if (contratoNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherContratoNotaFiscal(contratoNotaFiscal);

                SalvarConfiguracoes(contratoNotaFiscal);
                SalvarTransportadores(contratoNotaFiscal, unitOfWork);

                repContratoNotaFiscal.Atualizar(contratoNotaFiscal, Auditado);

                Dominio.Entidades.EmpresaContratoAnexo empresaContratoAnexo = repContratoNotaFiscalAnexo.BuscarPorCodigo(Request.GetIntParam("CodigoAnexoRemovido"), true);
                if (empresaContratoAnexo != null)
                {
                    if (Utilidades.IO.FileStorageService.Storage.Exists(empresaContratoAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(empresaContratoAnexo.CaminhoArquivo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresaContratoAnexo.EmpresaContrato, null, "Removeu anexo.", unitOfWork);

                    repContratoNotaFiscalAnexo.Deletar(empresaContratoAnexo);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.EmpresaContrato repContratoNotaFiscal = new Repositorio.EmpresaContrato(unitOfWork);
                Repositorio.EmpresaContratoTransportador repEmpresaContratoTransportador = new Repositorio.EmpresaContratoTransportador(unitOfWork);

                Dominio.Entidades.EmpresaContrato contratoNotaFiscal = repContratoNotaFiscal.BuscarPorCodigo(codigo);

                if (contratoNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.EmpresaContratoTransportador> transportadores = repEmpresaContratoTransportador.BuscarPorContrato(codigo);

                var dynContratoNotaFiscal = new
                {
                    contratoNotaFiscal.Codigo,
                    contratoNotaFiscal.Descricao,
                    contratoNotaFiscal.Ativo,
                    contratoNotaFiscal.Contrato,
                    CodigoAnexo = contratoNotaFiscal.Anexos?.Count > 0 ? contratoNotaFiscal.Anexos[0].Codigo : 0,
                    Configuracoes = new
                    {
                        contratoNotaFiscal.RecorrenciaEmDias,
                        contratoNotaFiscal.NotificarPorEmail
                    },
                    Transportadores = (from obj in transportadores
                                       select new
                                       {
                                           obj.Empresa.Codigo,
                                           obj.Empresa.Descricao
                                       }).ToList()
                };

                return new JsonpResult(dynContratoNotaFiscal);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.EmpresaContrato repContratoNotaFiscal = new Repositorio.EmpresaContrato(unitOfWork);
                Repositorio.EmpresaContratoAnexo repContratoNotaFiscalAnexo = new Repositorio.EmpresaContratoAnexo(unitOfWork);
                Repositorio.EmpresaContratoTransportador repEmpresaContratoTransportador = new Repositorio.EmpresaContratoTransportador(unitOfWork);

                Dominio.Entidades.EmpresaContrato contratoNotaFiscal = repContratoNotaFiscal.BuscarPorCodigo(codigo, true);

                if (contratoNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                foreach (Dominio.Entidades.EmpresaContratoAnexo anexo in contratoNotaFiscal.Anexos)
                {
                    if (Utilidades.IO.FileStorageService.Storage.Exists(anexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(anexo.CaminhoArquivo);

                    repContratoNotaFiscalAnexo.Deletar(anexo);
                }

                List<Dominio.Entidades.EmpresaContratoTransportador> transportadores = repEmpresaContratoTransportador.BuscarPorContrato(codigo);
                foreach (Dominio.Entidades.EmpresaContratoTransportador transportador in transportadores)
                    repEmpresaContratoTransportador.Deletar(transportador);

                repContratoNotaFiscal.Deletar(contratoNotaFiscal, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                Repositorio.EmpresaContratoAnexo repContratoNotaFiscalAnexo = new Repositorio.EmpresaContratoAnexo(unitOfWork);
                Repositorio.EmpresaContrato repContratoNotaFiscal = new Repositorio.EmpresaContrato(unitOfWork);

                int.TryParse(Request.Params("CodigoContrato"), out int codigoContrato);

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "EmpresaContrato");
                
                for (var i = 0; i < files.Count; i++)
                {
                    Dominio.Entidades.EmpresaContratoAnexo contratoAnexo = new Dominio.Entidades.EmpresaContratoAnexo();

                    Servicos.DTO.CustomFile file = files[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    contratoAnexo.CaminhoArquivo = caminho;
                    contratoAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Path.GetFileName(nomeArquivo)));
                    contratoAnexo.Descricao = "Termo de Uso";
                    contratoAnexo.EmpresaContrato = repContratoNotaFiscal.BuscarPorCodigo(codigoContrato, true);

                    repContratoNotaFiscalAnexo.Inserir(contratoAnexo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoAnexo.EmpresaContrato, null, "Adicionou um anexo.", unitOfWork);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.EmpresaContrato repositorioContratoNotaFiscal = new Repositorio.EmpresaContrato(unitOfWork);
                Repositorio.EmpresaContratoAnexo repositorioContratoNotaFiscalAnexo = new Repositorio.EmpresaContratoAnexo(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.EmpresaContrato contratoNotaFiscal = repositorioContratoNotaFiscal.BuscarPorCodigo(codigo);

                if (contratoNotaFiscal == null || contratoNotaFiscal?.Anexos?.Count <= 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.EmpresaContratoAnexo anexo = repositorioContratoNotaFiscalAnexo.BuscarPorCodigo(contratoNotaFiscal.Anexos[0].Codigo, false);

                byte[] arquivoBinario = Utilidades.File.LerArquivo(anexo.CaminhoArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoNotaFiscal, null, $"Realizou o download do arquivo {anexo.NomeArquivo}.", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu um falha ao relizar o download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherContratoNotaFiscal(Dominio.Entidades.EmpresaContrato contratoNotaFiscal)
        {
            contratoNotaFiscal.Contrato = Request.Params("Contrato");
            contratoNotaFiscal.Descricao = Request.GetStringParam("Descricao");
            contratoNotaFiscal.Ativo = Request.GetBoolParam("Ativo");
            contratoNotaFiscal.DataUltimaAlteracao = DateTime.Now;

            if (contratoNotaFiscal.Codigo == 0)
            {
                contratoNotaFiscal.Empresa = Empresa;
                contratoNotaFiscal.DataCadastro = DateTime.Now;
            }
        }

        private void SalvarConfiguracoes(Dominio.Entidades.EmpresaContrato contratoNotaFiscal)
        {
            dynamic dynConfiguracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Configuracoes"));
            if (dynConfiguracoes == null)
                return;

            contratoNotaFiscal.RecorrenciaEmDias = ((string)dynConfiguracoes.RecorrenciaEmDias).ToInt();
            contratoNotaFiscal.NotificarPorEmail = ((string)dynConfiguracoes.NotificarPorEmail).ToBool();
        }

        private void SalvarTransportadores(Dominio.Entidades.EmpresaContrato contratoNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.EmpresaContratoTransportador repEmpresaContratoTransportador = new Repositorio.EmpresaContratoTransportador(unitOfWork);
            Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            dynamic dynContratoTransportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transportadores"));

            List<Dominio.Entidades.EmpresaContratoTransportador> empresaContratoTransportadores = repEmpresaContratoTransportador.BuscarPorContrato(contratoNotaFiscal.Codigo);

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (empresaContratoTransportadores.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic dynContratoTransportador in dynContratoTransportadores)
                {
                    int codigo = ((string)dynContratoTransportador.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.EmpresaContratoTransportador> listaDeletar = (from obj in empresaContratoTransportadores where !codigos.Contains(obj.Empresa.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.EmpresaContratoTransportador deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Transportador",
                        De = $"{deletar.Empresa.Descricao}",
                        Para = ""
                    });

                    repEmpresaContratoTransportador.Deletar(deletar);
                }
            }

            List<int> codigoNovosTransportadores = new List<int>();
            List<int> codigoTransportadores = new List<int>();
            foreach (dynamic dynContratoTransportador in dynContratoTransportadores)
            {
                int codigoTransportador = ((string)dynContratoTransportador.Codigo).ToInt();

                Dominio.Entidades.EmpresaContratoTransportador empresaContratoTransportador = codigoTransportador > 0 ? repEmpresaContratoTransportador.BuscarPorContratoETransportador(contratoNotaFiscal.Codigo, codigoTransportador) : null;

                if (empresaContratoTransportador == null)
                {
                    empresaContratoTransportador = new Dominio.Entidades.EmpresaContratoTransportador();

                    empresaContratoTransportador.Contrato = contratoNotaFiscal;
                    empresaContratoTransportador.Empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                    repEmpresaContratoTransportador.Inserir(empresaContratoTransportador);

                    codigoNovosTransportadores.Add(codigoTransportador);
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Transportador",
                        De = "",
                        Para = $"{empresaContratoTransportador.Empresa.Descricao}"
                    });
                }

                codigoTransportadores.Add(codigoTransportador);
            }

            if (codigoNovosTransportadores.Count > 0)
                if (repEmpresaContratoTransportador.PossuiTransportadorEmOutroContrato(contratoNotaFiscal.Codigo, codigoNovosTransportadores))
                    throw new ControllerException("Há transportador adicionado em outro contrato, não sendo possível continuar!");

            if (codigoTransportadores.Count == 0)
                if (repEmpresaContrato.PossuiOutroContratoSemTransportador(contratoNotaFiscal.Codigo))
                    throw new ControllerException("Não é permitido ter mais que um contrato sem transportador!");

            contratoNotaFiscal.SetExternalChanges(alteracoes);
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaContratoNotaFiscal ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaContratoNotaFiscal()
            {
                Contrato = Request.GetStringParam("Contrato"),
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin ? Empresa.Codigo : 0
            };
        }

        #endregion
    }
}
