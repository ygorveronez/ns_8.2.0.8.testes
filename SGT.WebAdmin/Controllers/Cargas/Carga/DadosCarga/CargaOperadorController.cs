using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosCarga
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio")]
    public class CargaOperadorController : BaseController
    {
		#region Construtores

		public CargaOperadorController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AlterarOperadorAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarCarga);

                int codigoOperador = Request.GetIntParam("Operador");
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork, cancellationToken);
                Dominio.Entidades.Usuario operador = await repositorioUsuario.BuscarPorCodigoAsync(codigoOperador);

                if (operador == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarOperador);

                Repositorio.Embarcador.Cargas.LogCargaOperador repositorioLogCargaOperador = new Repositorio.Embarcador.Cargas.LogCargaOperador(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.LogCargaOperador logCargaOperador = new Dominio.Entidades.Embarcador.Cargas.LogCargaOperador
                {
                    Carga = carga,
                    Usuario = this.Usuario,
                    DataRegistroLog = DateTime.Now,
                    Justificativa = Request.Params("Justificativa"),
                    OperadorLogisticaAnterior = carga.Operador,
                    OperadorLogisticaAtual = operador
                };

                await repositorioLogCargaOperador.InserirAsync(logCargaOperador);

                if ((carga.OperadorContratouCarga != null) && (carga.OperadorContratouCarga.Codigo != operador.Codigo))
                    carga.OperadorContratouCarga = null;

                new Servicos.Embarcador.Carga.CargaOperador(unitOfWork).Atualizar(carga, operador, TipoServicoMultisoftware);

                await repositorioCarga.AtualizarAsync(carga);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, Localization.Resources.Cargas.Carga.AlterouOperadorPara + " " + operador.Nome, unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarOperadorDaCarga);
            }
        }
    }
}
