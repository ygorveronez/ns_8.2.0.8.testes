using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frota
{
    public class Frota
    {

        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;
        private readonly string _webServiceConsultaCTe;

        #endregion

        #region Construtores

        public Frota(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, string webServiceConsultaCTe)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _cliente = cliente;
            _webServiceConsultaCTe = webServiceConsultaCTe;
        }

        #endregion

        #region Métodos Públicos

        public void VincularFrotaACarga(Dominio.Entidades.Embarcador.Frota.Frota frota, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            string mensagemErro = string.Empty;
            //validar se frota ja nao esta vinculada a carga para a data de carregameento
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Frota.FrotaCarga repFrotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Frota.FrotaCarga frotaVinculada = repFrotaCarga.BuscarPorFrotaOuCargaEData(frota.Codigo, carga.Codigo, carga.DataCarregamentoCarga.Value);
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao ordemServicoVeiculoManutencao = new Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao(_unitOfWork);

            if (frotaVinculada == null)
            {
                Dominio.Entidades.Embarcador.Frota.FrotaCarga frotaCarga = new Dominio.Entidades.Embarcador.Frota.FrotaCarga
                {
                    Carga = carga,
                    DataCarregamento = carga.DataCarregamentoCarga.Value,
                    Frota = frota,
                    DataPrevistaFimViagem = carga.DadosSumarizados.DataPrevisaoEntrega.HasValue ? carga.DadosSumarizados.DataPrevisaoEntrega.Value : carga.DataCarregamentoCarga,
                    DataPrevistaInicioViagem = carga.DadosSumarizados.DataPrevisaoSaida.HasValue ? carga.DadosSumarizados.DataPrevisaoSaida : carga.DataCarregamentoCarga,
                };
                
                repFrotaCarga.Inserir(frotaCarga);

                carga.Motoristas = new List<Dominio.Entidades.Usuario>();
                carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                if (frota.Motorista != null)
                    carga.Motoristas.Add(frota.Motorista);

                if (frota.MotoristaAuxiliar != null)
                    carga.Motoristas.Add(frota.MotoristaAuxiliar);

                if (frota.Veiculo != null)
                    carga.Veiculo = frota.Veiculo;

                if (frota.Reboque1 != null)
                    carga.VeiculosVinculados.Add(frota.Reboque1);

                if (frota.Reboque2 != null)
                    carga.VeiculosVinculados.Add(frota.Reboque2);

                List<int> listaCodigosVeiculos = new List<int>();
                if (frota.Veiculo != null)
                    listaCodigosVeiculos.Add(frota.Veiculo.Codigo);

                if (frota.Reboque1 != null)
                    listaCodigosVeiculos.Add(frota.Reboque1.Codigo);

                if (frota.Reboque2 != null)
                    listaCodigosVeiculos.Add(frota.Reboque2.Codigo);

                ordemServicoVeiculoManutencao.VeiculoIndisponivelParaTransporte(carga.Pedidos.Select(o => o.Pedido).ToList(), listaCodigosVeiculos);

                auditado.Texto = "Adicionou a frota na carga atravéz do planejamento frota";
                repCarga.Atualizar(carga, auditado);

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                {
                    Carga = carga,
                    CodigoEmpresa = carga.Empresa?.Codigo ?? 0,
                    CodigoModeloVeicular = carga.ModeloVeicularCarga?.Codigo ?? 0,
                    CodigoReboque = carga.VeiculosVinculados?.Count > 0 ? carga.VeiculosVinculados.FirstOrDefault().Codigo : 0,
                    CodigoTipoCarga = carga.TipoDeCarga?.Codigo ?? 0,
                    CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                    CodigoTracao = carga.Veiculo.Codigo,
                    CodigoMotorista = carga.Motoristas?.Count > 0 ? carga.Motoristas.FirstOrDefault().Codigo : 0,
                    ListaCodigoMotorista = carga.Motoristas.Select(x => x.Codigo).ToList(),
                    SalvarDadosTransporteSemSolicitarNFes = false,

                };

                svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, auditado.Usuario, false, _tipoServicoMultisoftware, _webServiceConsultaCTe, _cliente, auditado, _unitOfWork);

                if (!string.IsNullOrEmpty(mensagemErro))
                    throw new ServicoException(mensagemErro);

            }
            else if (frotaVinculada.Carga.Codigo != carga.Codigo || frotaVinculada.Frota.Codigo != frota.Codigo)
            {
                if (frotaVinculada.Carga.Codigo != carga.Codigo)
                    throw new ServicoException($"Não foi possível finalizar esta ação, pois frota já esta vinculada a Carga { frotaVinculada.Carga.CodigoCargaEmbarcador } na data {carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? ""}");
                else if (frotaVinculada.Frota.Codigo != frota.Codigo)
                    throw new ServicoException($"Não foi possível finalizar esta ação, pois Carga já esta vinculada a outra Frota na data {carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? ""}");
            }
        }

        public void RemoverFrotaACarga(Dominio.Entidades.Embarcador.Frota.Frota frota, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Frota.FrotaCarga repFrotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Frota.FrotaCarga frotaVinculada = repFrotaCarga.BuscarPorFrotaOuCargaEData(frota.Codigo, carga.Codigo, carga.DataCarregamentoCarga.Value);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (frotaVinculada != null)
                repFrotaCarga.Deletar(frotaVinculada);

            carga.Motoristas = new List<Dominio.Entidades.Usuario>();
            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
            carga.Veiculo = null;

            auditado.Texto = "Removeu a frota da carga atravéz do planejamento frota";
            repCarga.Atualizar(carga, auditado);

        }


        public void RemoverFrotaCargaAoInativarMotorista(Dominio.Entidades.Embarcador.Frota.FrotaCarga frotaCarga, int CodigoMotoristaRemovido, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(_unitOfWork);

            Repositorio.Embarcador.Frota.FrotaCarga repFrotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            frotaCarga.Carga.Motoristas = new List<Dominio.Entidades.Usuario>();
            frotaCarga.Carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
            frotaCarga.Carga.Veiculo = null;

            if (frotaCarga.Frota.Motorista?.Codigo == CodigoMotoristaRemovido)
                frotaCarga.Frota.Motorista = null;

            if (frotaCarga.Frota.MotoristaAuxiliar?.Codigo == CodigoMotoristaRemovido)
                frotaCarga.Frota.MotoristaAuxiliar = null;

            repFrota.Atualizar(frotaCarga.Frota);
            repCarga.Atualizar(frotaCarga.Carga);
            repFrotaCarga.Deletar(frotaCarga);

            Servicos.Auditoria.Auditoria.Auditar(auditado, frotaCarga.Carga, "Inativou Motorista vinculado a programação futura", _unitOfWork);
        }

        public void InformarComprometimentoFrotaFutura(Dominio.Entidades.Embarcador.Frota.Frota frota, DateTime dataComparativo)
        {
            Repositorio.Embarcador.Frota.FrotaCarga repFrotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Frota.FrotaCarga programacaoFuturaFrota = repFrotaCarga.ExisteProgramacaoFuturaParaFrota(frota.Codigo, dataComparativo);

            if (programacaoFuturaFrota != null)
            {
                programacaoFuturaFrota.SituacaoComprometimentoFrota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprometimentoFrota.VeiculoAlteradoDeTrechosAnteriores;
                repFrotaCarga.Atualizar(programacaoFuturaFrota);
            }
        }


        public void AtualizarFrotaMotoristaVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Repositorio.Embarcador.Frota.FrotaCarga repPlanejamentoFrotaCarga = new Repositorio.Embarcador.Frota.FrotaCarga(_unitOfWork);

            List<Dominio.Entidades.Veiculo> reboques = veiculo.VeiculosVinculados?.ToList() ?? new List<Dominio.Entidades.Veiculo>();

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> veiculomotorista = repVeiculoMotorista.BuscarTodos(veiculo.Codigo);

            Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(_unitOfWork);
            Dominio.Entidades.Embarcador.Frota.Frota veiculoFrota;

            if (veiculo.TipoVeiculo == "0")
                veiculoFrota = repFrota.BuscarPorVeiculoTracao(veiculo.Codigo);
            else
                veiculoFrota = repFrota.BuscarPorVeiculoReboque(veiculo.Codigo);

            if (veiculoFrota == null)
            {
                veiculoFrota = new Dominio.Entidades.Embarcador.Frota.Frota()
                {
                    Reboque1 = veiculo.TipoVeiculo == "0" ? reboques != null && reboques.Count > 0 ? reboques[0] : null : veiculo,
                    Reboque2 = veiculo.TipoVeiculo == "0" ? reboques != null && reboques.Count > 1 ? reboques[1] : null : null,
                    VigenciaInicio = DateTime.Now,
                    Ativo = true,
                    MotoristaAuxiliar = veiculomotorista != null && veiculomotorista.Count > 0 ? veiculomotorista.Where(obj => !obj.Principal).FirstOrDefault()?.Motorista : null,
                    Motorista = veiculomotorista != null && veiculomotorista.Count > 0 ? veiculomotorista.Where(obj => obj.Principal).FirstOrDefault()?.Motorista : null,
                    Veiculo = veiculo.TipoVeiculo == "0" ? veiculo : veiculo.VeiculosTracao?.FirstOrDefault() ?? null,
                    LocalAtual = veiculo.LocalidadeAtual,
                    Longitude = veiculo.Longitude,
                    Latitude = veiculo.Latitude,
                };
            }
            else
            {
                bool troca = false;

                List<Dominio.Entidades.Veiculo> reboquesRemovidos = new List<Dominio.Entidades.Veiculo>();

                if (veiculo.Motoristas != null && veiculo.Motoristas.Count > 0)
                {
                    List<Dominio.Entidades.Usuario> motoristas = veiculo.Motoristas.Select(x => x.Motorista).ToList();

                    if (veiculoFrota.MotoristaAuxiliar != null && !motoristas.Contains(veiculoFrota.MotoristaAuxiliar))
                        troca = true;

                    if (veiculoFrota.Motorista != null && !motoristas.Contains(veiculoFrota.Motorista))
                        troca = true;

                    if (veiculo.TipoVeiculo == "0")//só tracao tem reboques..
                    {
                        if (veiculoFrota.Reboque2 != null && !reboques.Contains(veiculoFrota.Reboque2))
                        {
                            reboquesRemovidos.Add(veiculoFrota.Reboque2);
                            troca = true;
                        }

                        if (veiculoFrota.Reboque1 != null && !reboques.Contains(veiculoFrota.Reboque1))
                        {
                            reboquesRemovidos.Add(veiculoFrota.Reboque1);
                            troca = true;
                        }

                    }
                }

                if (troca)
                {
                    //validar se frota esta em um planejamento futuro.
                    Dominio.Entidades.Embarcador.Frota.FrotaCarga programacaoFuturaFrota = repPlanejamentoFrotaCarga.ExisteProgramacaoFuturaParaFrota(veiculoFrota.Codigo, DateTime.Now.Date);
                    if (programacaoFuturaFrota != null)
                        throw new ServicoException($"Não é possível efetuar essa alteração no veículo. O conjunto frota já esta vinculada uma programação futura Carga: { programacaoFuturaFrota.Carga.CodigoCargaEmbarcador } na data {programacaoFuturaFrota.DataCarregamento.ToString("dd/MM/yyyy")}");

                    veiculoFrota.VigenciaFim = DateTime.Now;
                    veiculoFrota.Ativo = false;

                    var novoveiculoFrota = new Dominio.Entidades.Embarcador.Frota.Frota()
                    {
                        Reboque1 = veiculo.TipoVeiculo == "0" ? reboques != null && reboques.Count > 0 ? reboques[0] : null : veiculo,
                        Reboque2 = veiculo.TipoVeiculo == "0" ? reboques != null && reboques.Count > 1 ? reboques[1] : null : null,
                        VigenciaInicio = DateTime.Now,
                        Ativo = true,
                        MotoristaAuxiliar = veiculomotorista != null && veiculomotorista.Count > 0 ? veiculomotorista.Where(obj => !obj.Principal).FirstOrDefault()?.Motorista : null,
                        Motorista = veiculomotorista != null && veiculomotorista.Count > 0 ? veiculomotorista.Where(obj => obj.Principal).FirstOrDefault()?.Motorista : null,
                        Veiculo = veiculo.TipoVeiculo == "0" ? veiculo : veiculo.VeiculosTracao?.FirstOrDefault() ?? null,
                        LocalAtual = veiculo.LocalidadeAtual,
                        Longitude = veiculo.Longitude,
                        Latitude = veiculo.Latitude,
                    };

                    repFrota.Inserir(novoveiculoFrota);

                    if (reboquesRemovidos.Count > 0)
                    {
                        // criar entidade frota para os reboques que foram removidos da tracao.
                        foreach (var reb in reboquesRemovidos)
                        {
                            var frotaReboqueExiste = repFrota.BuscarPorVeiculoReboque(reb.Codigo);

                            if (frotaReboqueExiste == null)
                            {
                                frotaReboqueExiste = new Dominio.Entidades.Embarcador.Frota.Frota()
                                {
                                    Reboque1 = reb,
                                    VigenciaInicio = DateTime.Now,
                                    Ativo = true,
                                    LocalAtual = veiculo.LocalidadeAtual,
                                    Longitude = veiculo.Longitude,
                                    Latitude = veiculo.Latitude,
                                };

                                repFrota.Inserir(frotaReboqueExiste);
                            }
                        }
                    }
                }
                else //atualiza
                {
                    veiculoFrota.Reboque1 = veiculo.TipoVeiculo == "0" ? reboques != null && reboques.Count > 0 ? reboques[0] : null : veiculo;
                    veiculoFrota.Reboque2 = veiculo.TipoVeiculo == "0" ? reboques != null && reboques.Count > 1 ? reboques[1] : null : null;
                    veiculoFrota.MotoristaAuxiliar = veiculomotorista != null && veiculomotorista.Count > 0 ? veiculomotorista.Where(obj => !obj.Principal).FirstOrDefault()?.Motorista : null;
                    veiculoFrota.Motorista = veiculomotorista != null && veiculomotorista.Count > 0 ? veiculomotorista.Where(obj => obj.Principal).FirstOrDefault()?.Motorista : null;

                    if (veiculo.TipoVeiculo == "0") //só atualiza se é o veiculo tracao.
                        veiculoFrota.Veiculo = veiculo;

                    veiculoFrota.LocalAtual = veiculo.LocalidadeAtual;
                    veiculoFrota.Longitude = veiculo.Longitude;
                    veiculoFrota.Latitude = veiculo.Latitude;
                }
            }

            if (veiculoFrota.Codigo == 0)
                repFrota.Inserir(veiculoFrota);
            else
                repFrota.Atualizar(veiculoFrota);


            if (veiculo.TipoVeiculo == "0" && reboques != null && reboques.Count > 0)
            {
                foreach (var reb in reboques)
                    AtualizarReboqueDaTracao(reb); //caso tenha reboques, precisamos ver se os reboques estao em frotas sem tracao e desativar.
            }

        }


        public void AtualizarReboqueDaTracao(Dominio.Entidades.Veiculo veiculo)
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;


            Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.Frota> reboquesSemTracao;

            reboquesSemTracao = repFrota.BuscarListaPorVeiculoReboqueSemTracao(veiculo.Codigo);

            foreach (var frota in reboquesSemTracao)
            {
                frota.Ativo = false;
                frota.VigenciaFim = DateTime.Now;

                repFrota.Atualizar(frota);
            }
        }

        public void VerificarReboque(Dominio.Entidades.Veiculo veiculo)
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Frota.Frota repFrota = new Repositorio.Embarcador.Frota.Frota(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.Frota> reboquesSemTracao;

            reboquesSemTracao = repFrota.BuscarListaPorVeiculoReboqueSemTracao(veiculo.Codigo);

            foreach (var frota in reboquesSemTracao)
            {
                frota.Ativo = false;
                frota.VigenciaFim = DateTime.Now;

                repFrota.Atualizar(frota);
            }
        }


        /// <summary>
        /// SÓ DEVE SER CHAMADO UMA VEZ PARA CRIAR REGISTROS NA BASE
        /// </summary>
        public void CriarFrotas()
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarVeiculosAtivosComMotoristasOuReboques();

            veiculos.Clear();
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca("GCE0D63");

            veiculos.Add(veiculo);


            foreach (var vei in veiculos)
            {
                AtualizarFrotaMotoristaVeiculo(vei);
            }


        }

            #endregion





        }
}
