using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGT.Italac.Thread
{
    public abstract class AbstractThreadProcessamento
    {

        #region Atributos privados

        private readonly string Thread = "ThreadItalac";

        #endregion

        #region Métodos protegidos

        protected void IniciarThread(string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Log("Inicio");

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    if (TemRegistrosPendentes(unitOfWork))
                        Executar(unitOfWork, stringConexao, tipoServicoMultisoftware, clienteMultisoftware);
                    else
                        Log("Não tem registros para executar");
                }

                Log("Fim");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        protected abstract void Executar(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware);

        protected abstract bool TemRegistrosPendentes(Repositorio.UnitOfWork unitOfWork);

        protected void Log(string msg)
        {
            Servicos.Log.TratarErro(msg, Thread);
        }

        #endregion

    }
}