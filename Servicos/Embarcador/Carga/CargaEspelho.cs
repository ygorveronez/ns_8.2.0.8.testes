using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga
{
    public class CargaEspelho
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public CargaEspelho(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void GerarCargaEspelho()
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
                List<int> codigoCargasPendenteGerarCargaEspelho = repositorioCarga.BuscarCargasPendentesGerarCargaEspelho(10);

                for (int i = 0; i < codigoCargasPendenteGerarCargaEspelho.Count; i++)
                {
                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCargasPendenteGerarCargaEspelho[i]);

                    if (carga != null)
                    {
                        Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                        servicoCarga.GerarCargaEspelho(carga, _unitOfWork, _tipoServicoMultisoftware, configuracao, null);

                        carga.PendenteGerarCargaEspelho = false;
                        repositorioCarga.Atualizar(carga);
                    }

                    _unitOfWork.CommitChanges();
                }

            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "GerarCargaEspelhoThread");
            }
        }

        #endregion

    }
}
