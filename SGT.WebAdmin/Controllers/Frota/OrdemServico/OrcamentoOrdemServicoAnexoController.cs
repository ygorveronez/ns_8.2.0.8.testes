using SGT.WebAdmin.Controllers.Anexo;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    public class OrcamentoOrdemServicoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>
    {
		#region Construtores

		public OrcamentoOrdemServicoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> BuscarAnexosPorOS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repServicoOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = repServicoOrcamento.BuscarPorCodigo(codigo);

                if (orcamentoServico == null)
                    return new JsonpResult(false, "");

                if (orcamentoServico.Anexos.Count == 0)
                    return new JsonpResult(false, "");

                return new JsonpResult(new
                {
                    Anexos = (
                       from anexo in orcamentoServico.Anexos
                       select new
                       {
                           anexo.Codigo,
                           anexo.Descricao,
                           anexo.NomeArquivo,
                       }
                   ).ToList(),
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexoOverride()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>(unitOfWork);
                Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>(unitOfWork);
                byte[] arquivoBinario = servicoAnexo.DownloadAnexo(anexo, unitOfWork);
                repositorioAnexo.Atualizar(anexo, Auditado);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
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

        public async Task<IActionResult> DownloadTodosAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo> anexos = repositorioAnexo.BuscarPorEntidade(codigo);

                if (anexos == null || anexos.Count == 0)
                    return new JsonpResult(false, true, "Não foi encontrado anexo para este controle.");

                foreach (var anexo in anexos)
                {
                    anexo.Initialize();
                    repositorioAnexo.Atualizar(anexo, Auditado);
                }

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.OrdemServicoFrotaOrcamentoServicoAnexo, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico>(unitOfWork);
                System.IO.MemoryStream arquivo = servicoAnexo.DownloadAnexos(anexos, unitOfWork);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(anexos[0].Descricao), "_Anexos.zip"));
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

        #endregion
    }
}