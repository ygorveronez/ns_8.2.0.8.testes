using Dominio.Entidades.Embarcador.Chamados;
using Dominio.Excecoes.Embarcador;
using SGT.WebAdmin.Controllers.Anexo;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ChamadoOcorrencia")]
    public class ChamadoNivelAtendimentoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Chamados.ChamadoNivelAtendimentoAnexo, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>
    {
		#region Construtores

		public ChamadoNivelAtendimentoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		protected override void PreecherInformacoesAdicionais(ChamadoNivelAtendimentoAnexo anexo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoAnalise repositorioChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
            Repositorio.Embarcador.Chamados.NivelAtendimento repositorioNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(unitOfWork);

            int codigoChamadoAnalise = Request.GetIntParam("Codigo");

            Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = repositorioChamadoAnalise.BuscarPorCodigo(codigoChamadoAnalise);
            List<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento> nivelAtendimento = repositorioNivelAtendimento.BuscarPorChamado(chamadoAnalise.Codigo);

            if (nivelAtendimento != null)
            {
                anexo.Nivel = nivelAtendimento.LastOrDefault().Nivel;
                repositorioChamadoAnalise.Atualizar(chamadoAnalise);

            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoNivelAtendimentoAnexo> anexos;

                if (codigo > 0)
                {
                    Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Chamados.ChamadoNivelAtendimentoAnexo, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Chamados.ChamadoNivelAtendimentoAnexo, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>(unitOfWork);
                    anexos = repositorioAnexo.BuscarPorEntidade(codigo);
                }
                else
                    anexos = new List<Dominio.Entidades.Embarcador.Chamados.ChamadoNivelAtendimentoAnexo>();

                var listaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                        anexo.Nivel
                    }
                ).ToList();

                return new JsonpResult(listaAnexos);
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
    }
}