using Dominio.Excecoes.Embarcador;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 30000)]
    public class AgrupamentoCargas : LongRunningProcessBase<AgrupamentoCargas>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            AgruparCargasAutomaticamente(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _auditado);
        }

        private void AgruparCargasAutomaticamente(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                int.TryParse(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().QuantidadeSelecaoAgrupamentoCargaAutomatico, out int quantidade);
                if (quantidade == 0)
                    quantidade = 100;

                int.TryParse(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().QuantidadeCargasAgrupamentoCargaAutomatico, out int quantidadeAgrupamento);
                if (quantidadeAgrupamento == 0)
                    quantidadeAgrupamento = 10;

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = repCarga.BuscarCargasAguardandoAgrupamentoAutomatico(quantidade);
                int quantidadeTotal = repCarga.ContarCargasAguardandoAgrupamentoAutomatico();

                if (quantidadeTotal > quantidade)
                    _tempoAguardarProximaExecucao = 500;
                else
                    _tempoAguardarProximaExecucao = 30000;

                List<int> codigosFiliais = (from obj in listaCargas select obj.Filial.Codigo).Distinct().ToList();
                for (var fi = 0; fi < codigosFiliais.Count; fi++)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasFiliais = (from obj in listaCargas where obj.Filial.Codigo == codigosFiliais[fi] select obj).ToList();
                    List<int> codigosOperacoes = (from obj in cargasFiliais select obj.TipoOperacao.Codigo).Distinct().ToList();
                    for (var op = 0; op < codigosOperacoes.Count; op++)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaOperacao = (from obj in cargasFiliais where obj.TipoOperacao.Codigo == codigosOperacoes[op] select obj).ToList();
                        List<int> codigosTransportadores = (from obj in cargaOperacao select obj.Empresa.Codigo).Distinct().ToList();
                        for (var tr = 0; tr < codigosTransportadores.Count; tr++)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaTransportadores = (from obj in cargaOperacao where obj.Empresa.Codigo == codigosTransportadores[tr] select obj).ToList();

                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupar = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargaTransportadores)
                            {
                                if (cargasAgrupar.Count() == quantidadeAgrupamento)
                                {
                                    AgruparCargas(cargasAgrupar, tipoServicoMultisoftware, unitOfWork, stringConexao, auditado);

                                    cargasAgrupar = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                                }
                                else
                                    cargasAgrupar.Add(carga);
                            }
                            if (cargasAgrupar.Count() > 0)
                                AgruparCargas(cargasAgrupar, tipoServicoMultisoftware, unitOfWork, stringConexao, auditado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "AgruparCargasAutomaticamente");
            }
        }


        private void AgruparCargas(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAgrupar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (cargasAgrupar.Count() == 1)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargasAgrupar[0];

                unitOfWork.Start();

                new Servicos.Embarcador.Carga.Carga(unitOfWork).FecharCarga(carga, unitOfWork, tipoServicoMultisoftware, _clienteMultisoftware);
                carga.AgruparCargaAutomaticamente = false;
                carga.FechandoCarga = false;
                carga.CargaFechada = true;
                repCarga.Atualizar(carga);

                unitOfWork.CommitChanges();
            }
            else
            {
                try
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = new Servicos.Embarcador.Carga.CargaAgrupada(unitOfWork).AgruparCargas(null, cargasAgrupar, tipoServicoMultisoftware, _clienteMultisoftware);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaAgrupada, null, "Criada pelo agrupamento automatico das cargas " + string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList()), unitOfWork);

                    unitOfWork.CommitChanges();
                }
                catch(ServicoException excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao.Message, "AgruparCargasAutomaticamente");
                }
            }
        }


    }
}