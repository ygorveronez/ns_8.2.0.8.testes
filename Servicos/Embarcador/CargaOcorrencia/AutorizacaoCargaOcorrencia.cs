using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.CargaOcorrencia
{
    public class AutorizacaoCargaOcorrencia
    {
        #region Atributos Privados Somente Leitura
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia _configuracaoOcorrencia;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoTMS;
        private readonly Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado _configuracaoChamado;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao _repositorioCargaOcorrenciaAutorizacao;

        #endregion

        #region Construtores

        public AutorizacaoCargaOcorrencia(Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS,
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
            _configuracaoOcorrencia = configuracaoOcorrencia;
            _configuracaoTMS = configuracaoTMS;
            _repositorioCargaOcorrenciaAutorizacao = repositorioCargaOcorrenciaAutorizacao;
            _configuracaoChamado = configuracaoChamado;
        }

        #endregion

        #region Métodos Publicos Estaticos
        public static List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> VerificarRegrasAutorizacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegras = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();
            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaFiltrada = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraTipoOcorrencia = repRegrasAutorizacaoOcorrencia.BuscarRegraPorTipoOcorrencia(ocorrencia.TipoOcorrencia.Codigo, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraComponenteFrete = repRegrasAutorizacaoOcorrencia.BuscarRegraPorComponenteFrete(ocorrencia.ComponenteFrete?.Codigo ?? 0, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);

            listaRegras.AddRange(listaRegraTipoOcorrencia);
            listaRegras.AddRange(listaRegraComponenteFrete);

            int codigoFilial = ocorrencia.Carga?.Filial?.Codigo ?? 0;

            if (ocorrencia.Carga?.FilialOrigem != null)
                codigoFilial = ocorrencia.Carga.FilialOrigem.Codigo;

            if (ocorrencia.Carga != null && codigoFilial > 0)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraFilial = repRegrasAutorizacaoOcorrencia.BuscarRegraPorFilial(codigoFilial, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
                listaRegras.AddRange(listaRegraFilial);
            }

            double cnpjCpfTomador = 0d;
            double cnpjCpfExpedidor = 0d;

            if (ocorrencia.Carga != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = null;

                if (!configuracao.GerarOcorrenciaParaCargaAgrupada && ocorrencia.Carga.CargaOrigemCTes != null && ocorrencia.Carga.CargaOrigemCTes.Count > 0)
                    cargaCTe = (from obj in ocorrencia.Carga.CargaOrigemCTes where obj.CTe != null && obj.CargaCTeComplementoInfo == null select obj).FirstOrDefault();
                else if (ocorrencia.Carga.CargaCTes != null && ocorrencia.Carga.CargaCTes.Count > 0)
                    cargaCTe = (from obj in ocorrencia.Carga.CargaCTes where obj.CTe != null && obj.CargaCTeComplementoInfo == null select obj).FirstOrDefault();

                if (cargaCTe != null)
                {
                    cnpjCpfExpedidor = cargaCTe.CTe.Expedidor?.Cliente?.CPF_CNPJ ?? 0d;

                    if (ocorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Remetente)
                        cnpjCpfTomador = cargaCTe.CTe.Remetente.Cliente.CPF_CNPJ;
                    else if (ocorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Destinatario)
                        cnpjCpfTomador = cargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ;
                    else if (ocorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Expedidor)
                        cnpjCpfTomador = cargaCTe.CTe.Expedidor.Cliente.CPF_CNPJ;
                    else if (ocorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Recebedor)
                        cnpjCpfTomador = cargaCTe.CTe.Recebedor.Cliente.CPF_CNPJ;
                    else
                        cnpjCpfTomador = cargaCTe.CTe.TomadorPagador.Cliente.CPF_CNPJ;

                    List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraTomador = repRegrasAutorizacaoOcorrencia.BuscarRegraPorTomador(cnpjCpfTomador, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
                    List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraExpedidor = repRegrasAutorizacaoOcorrencia.BuscarRegraPorExpedidor(cnpjCpfExpedidor, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);

                    listaRegras.AddRange(listaRegraTomador);
                    listaRegras.AddRange(listaRegraExpedidor);
                }
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraValor = repRegrasAutorizacaoOcorrencia.BuscarRegraPorValor(ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
            listaRegras.AddRange(listaRegraValor);

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraTipoOperacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();
            if (ocorrencia.Carga != null && ocorrencia.Carga.TipoOperacao != null)
            {
                listaRegraTipoOperacao = repRegrasAutorizacaoOcorrencia.BuscarRegraPorTipoOperacao(ocorrencia.Carga.TipoOperacao.Codigo, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
                listaRegras.AddRange(listaRegraTipoOperacao);
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraTipoCarga = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();
            if (ocorrencia.Carga != null && ocorrencia.Carga.TipoDeCarga != null)
            {
                listaRegraTipoOperacao = repRegrasAutorizacaoOcorrencia.BuscarRegraPorTipoCarga(ocorrencia.Carga.TipoDeCarga.Codigo, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
                listaRegras.AddRange(listaRegraTipoOperacao);
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraCanalEntrega = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();
            if (ocorrencia.Carga != null && ocorrencia.Carga.Pedidos != null && ocorrencia.Carga.Pedidos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido codigoCarga in ocorrencia.Carga.Pedidos)
                {
                    if (codigoCarga.Pedido.CanalEntrega != null)
                    {
                        listaRegraTipoOperacao = repRegrasAutorizacaoOcorrencia.BuscarRegraPorCanalEntrega(codigoCarga.Pedido.CanalEntrega.Codigo, ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
                        listaRegras.AddRange(listaRegraTipoOperacao);
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegraDiasAbertura = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();
            if (ocorrencia.Carga != null)
            {
                listaRegraDiasAbertura = repRegrasAutorizacaoOcorrencia.BuscarRegraPorDiasAbertura(ocorrencia.DataOcorrencia, etapaAutorizacaoOcorrencia);
                listaRegras.AddRange(listaRegraDiasAbertura);
            }

            if (listaRegras.Distinct().Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras.Distinct());

                RegraAutorizacao.AprovacaoAlcadaSemPadronizacao servicoAprovacao = new RegraAutorizacao.AprovacaoAlcadaSemPadronizacao();

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorTipoOcorrencia)
                    {
                        bool valido = false;
                        if (regra.RegrasTipoOcorrencia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeOcorrencia.Codigo == ocorrencia.TipoOcorrencia.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOcorrencia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeOcorrencia.Codigo == ocorrencia.TipoOcorrencia.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOcorrencia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeOcorrencia.Codigo != ocorrencia.TipoOcorrencia.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOcorrencia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeOcorrencia.Codigo != ocorrencia.TipoOcorrencia.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorComponenteFrete)
                    {
                        bool valido = false;
                        if (regra.RegrasComponenteFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.ComponenteFrete.Codigo == ocorrencia.ComponenteFrete.Codigo))
                            valido = true;
                        else if (regra.RegrasComponenteFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.ComponenteFrete.Codigo == ocorrencia.ComponenteFrete.Codigo))
                            valido = true;
                        else if (regra.RegrasComponenteFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.ComponenteFrete.Codigo != ocorrencia.ComponenteFrete.Codigo))
                            valido = true;
                        else if (regra.RegrasComponenteFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.ComponenteFrete.Codigo != ocorrencia.ComponenteFrete.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorFilialEmissao && ocorrencia.Carga != null && codigoFilial > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasFilialEmissao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilialEmissao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilialEmissao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Filial.Codigo != codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilialEmissao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Filial.Codigo != codigoFilial))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorExpedidor)
                    {
                        bool valido = false;
                        if (regra.RegrasExpedidor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Expedidor.CPF_CNPJ == cnpjCpfExpedidor))
                            valido = true;
                        else if (regra.RegrasExpedidor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Expedidor.CPF_CNPJ == cnpjCpfExpedidor))
                            valido = true;
                        else if (regra.RegrasExpedidor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Expedidor.CPF_CNPJ != cnpjCpfExpedidor))
                            valido = true;
                        else if (regra.RegrasExpedidor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Expedidor.CPF_CNPJ != cnpjCpfExpedidor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTomadorOcorrencia)
                    {
                        bool valido = false;
                        if (regra.RegrasTomadorOcorrencia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Tomador.CPF_CNPJ == cnpjCpfTomador))
                            valido = true;
                        else if (regra.RegrasTomadorOcorrencia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Tomador.CPF_CNPJ == cnpjCpfTomador))
                            valido = true;
                        else if (regra.RegrasTomadorOcorrencia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Tomador.CPF_CNPJ != cnpjCpfTomador))
                            valido = true;
                        else if (regra.RegrasTomadorOcorrencia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Tomador.CPF_CNPJ != cnpjCpfTomador))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTipoOperacao && ocorrencia.Carga != null && ocorrencia.Carga.TipoOperacao != null)
                    {
                        bool valido = false;
                        if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoOperacao.Codigo == ocorrencia.Carga.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoOperacao.Codigo == ocorrencia.Carga.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoOperacao.Codigo != ocorrencia.Carga.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoOperacao.Codigo != ocorrencia.Carga.TipoOperacao.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }


                    if (regra.RegraPorTipoCarga && ocorrencia.Carga != null && ocorrencia.Carga.TipoDeCarga != null)
                    {
                        bool valido = false;
                        if (regra.RegrasTipoCarga.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeCarga.Codigo == ocorrencia.Carga.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoCarga.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeCarga.Codigo == ocorrencia.Carga.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoCarga.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeCarga.Codigo != ocorrencia.Carga.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoCarga.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeCarga.Codigo != ocorrencia.Carga.TipoDeCarga.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }


                    if (regra.RegraPorCanalEntrega && ocorrencia.Carga != null && ocorrencia.Carga.Pedidos != null)
                    {
                        bool valido = false;
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido obj in ocorrencia.Carga.Pedidos)
                        {
                            if (regra.RegrasCanalEntrega.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.CanalEntrega.Codigo == obj.Pedido.CanalEntrega.Codigo))
                                valido = true;
                            else if (regra.RegrasCanalEntrega.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.CanalEntrega.Codigo == obj.Pedido.CanalEntrega.Codigo))
                                valido = true;
                            else if (regra.RegrasCanalEntrega.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.CanalEntrega.Codigo != obj.Pedido.CanalEntrega.Codigo))
                                valido = true;
                            else if (regra.RegrasCanalEntrega.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.CanalEntrega.Codigo != obj.Pedido.CanalEntrega.Codigo))
                                valido = true;

                        }

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }


                    if (regra.RegraPorValorOcorrencia && !servicoAprovacao.ValidarAlcadas(regra.RegrasValorOcorrencia, ocorrencia.ValorOcorrencia))
                    {
                        listaFiltrada.Remove(regra);
                        continue;
                    }

                    if (regra.RegraPorDiasAbertura && ((ocorrencia.Carga == null) || !servicoAprovacao.ValidarAlcadas(regra.RegrasDiasAbertura, (ocorrencia.DataOcorrencia.Date - (ocorrencia.Carga.DataFinalizacaoEmissao?.Date ?? ocorrencia.Carga.DataCriacaoCarga.Date)).Days)))
                    {
                        listaFiltrada.Remove(regra);
                        continue;
                    }
                }
            }

            return listaFiltrada;
        }

        public static bool LiberarProximasHierarquiasDeAprovacao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> alcadasAprovacao = repositorioAutorizacao.BuscarPendentesPorOcorrencia(ocorrencia.Codigo, etapa);

            if (alcadasAprovacao.Count > 0)
            {
                int menorPrioridadeAprovacao = alcadasAprovacao.Select(o => o.Prioridade).Min();
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia = alcadasAprovacao.Where(o => o.RegrasAutorizacaoOcorrencia != null).Select(o => o.RegrasAutorizacaoOcorrencia.EtapaAutorizacaoOcorrencia).FirstOrDefault();

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao aprovacao in alcadasAprovacao)
                {
                    if (aprovacao.Prioridade <= menorPrioridadeAprovacao)
                    {
                        aprovacao.Bloqueada = false;
                        repositorioAutorizacao.Atualizar(aprovacao);

                        foreach (Dominio.Entidades.Usuario aprovador in aprovacao.RegrasAutorizacaoOcorrencia.Aprovadores)
                            NotificarUsuario(aprovador, ocorrencia, usuario, aprovacao, tipoServicoMultisoftware, stringConexao, unitOfWork);
                    }
                }

                if (etapaAutorizacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia)
                    ocorrencia.PrioridadeAprovacaoAtualEtapaAprovacao = menorPrioridadeAprovacao;
                else
                    ocorrencia.PrioridadeAprovacaoAtualEtapaEmissao = menorPrioridadeAprovacao;

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                repositorioCargaOcorrencia.Atualizar(ocorrencia);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Cria o vinculo das regras com os aprovadores
        /// </summary>
        /// <returns>Retorna verdadeiro quando existe alguma regra para algum aprovador e falor para quando é aprovada automática</returns>
        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaFiltrada, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Usuario usuario, out List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            bool possuiRegraPendente = false;
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            int menorPrioridadeAprovacao = listaFiltrada.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia = listaFiltrada.Select(o => o.EtapaAutorizacaoOcorrencia).FirstOrDefault();
            List<DateTime> datasPrazos = new List<DateTime>();

            notificoes = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao>();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;

                    DateTime? dataAprovacaoAutomatica = null;
                    if (regra.AprovacaoAutomaticaAposDias > 0)
                    {
                        if (regra.TipoDiasAprovacao == TipoDiasAprovacao.DiasCorridos)
                            dataAprovacaoAutomatica = DateTime.Now.Date.AddDays(regra.AprovacaoAutomaticaAposDias);
                        else
                        {
                            dataAprovacaoAutomatica = DateTime.Now.Date;
                            for (int i = 1; i <= regra.AprovacaoAutomaticaAposDias; i++)
                            {
                                dataAprovacaoAutomatica = dataAprovacaoAutomatica.Value.AddDays(1);
                                if (dataAprovacaoAutomatica.Value.DayOfWeek == DayOfWeek.Monday)
                                    dataAprovacaoAutomatica = dataAprovacaoAutomatica.Value.AddDays(1);
                                else if (dataAprovacaoAutomatica.Value.DayOfWeek == DayOfWeek.Saturday)
                                    dataAprovacaoAutomatica = dataAprovacaoAutomatica.Value.AddDays(2);
                            }
                        }

                    }

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao autorizacao = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao
                        {
                            CargaOcorrencia = ocorrencia,
                            Usuario = aprovador,
                            DataPrazoAprovacaoAutomatica = dataAprovacaoAutomatica,
                            RegrasAutorizacaoOcorrencia = regra,
                            EtapaAutorizacaoOcorrencia = regra.EtapaAutorizacaoOcorrencia,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            NumeroReprovadores = regra.NumeroReprovadores,
                            PrioridadeAprovacao = regra.PrioridadeAprovacao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            DataPrazoAprovacao = regra.DiasPrazoAprovacao.HasValue ? DateTime.Today.AddDays(regra.DiasPrazoAprovacao.Value) : (DateTime?)null,
                            GuidOcorrencia = Guid.NewGuid().ToString()
                        };

                        if (autorizacao.DataPrazoAprovacao.HasValue)
                            datasPrazos.Add(autorizacao.DataPrazoAprovacao.Value);

                        repositorioAutorizacao.Inserir(autorizacao);

                        if (!autorizacao.Bloqueada)
                            notificoes.Add(new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao()
                            {
                                Aprovador = aprovador,
                                Mensagem = ObterMensagemNotificacaoAprovador(ocorrencia, usuario, configuracaoTMS.LinkUrlAcessoCliente, autorizacao)
                            });
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao autorizacao = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao
                    {
                        CargaOcorrencia = ocorrencia,
                        Usuario = null,
                        RegrasAutorizacaoOcorrencia = regra,
                        EtapaAutorizacaoOcorrencia = regra.EtapaAutorizacaoOcorrencia,
                        NumeroAprovadores = 1,
                        NumeroReprovadores = 1,
                        PrioridadeAprovacao = regra.PrioridadeAprovacao,
                        Situacao = SituacaoOcorrenciaAutorizacao.Aprovada,
                        Data = DateTime.Now,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao
                    };

                    repositorioAutorizacao.Inserir(autorizacao);
                }
            }

            if (etapaAutorizacaoOcorrencia == EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia)
                ocorrencia.PrioridadeAprovacaoAtualEtapaAprovacao = menorPrioridadeAprovacao;
            else
                ocorrencia.PrioridadeAprovacaoAtualEtapaEmissao = menorPrioridadeAprovacao;

            if (datasPrazos.Count > 0)
                ocorrencia.DataPrazoAprovacao = datasPrazos.OrderBy(o => o.Date).FirstOrDefault();

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            repositorioCargaOcorrencia.Atualizar(ocorrencia);

            return possuiRegraPendente;
        }

        public static string ObterMensagemNotificacaoAprovador(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Usuario usuarioSolicitouAprovacao, string urlAcesso, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao aprovacao = null)
        {
            string nota = string.Empty;

            if (usuarioSolicitouAprovacao == null)
            {
                nota = string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SolicitadaLiberacaoOcorrenciaValor, ocorrencia.TipoOcorrencia.Descricao, ocorrencia.NumeroOcorrencia, ocorrencia.ValorOcorrencia.ToString("n2"));
            }
            else
            {
                nota = string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.UsuarioSolicitouLiberacaoOcorrenciaValor, usuarioSolicitouAprovacao.Nome, ocorrencia.TipoOcorrencia.Descricao, ocorrencia.NumeroOcorrencia, ocorrencia.ValorOcorrencia.ToString("n2"));
            }

            if (ocorrencia.Carga != null)
            {
                nota = string.Concat(nota, $" | - {Localization.Resources.Gerais.Geral.Carga}: ", ocorrencia.Carga.CodigoCargaEmbarcador);
                nota = string.Concat(nota, $" | - {Localization.Resources.Gerais.Geral.Transportador}: ", ocorrencia.Carga.Empresa.CNPJ_Formatado, " ", ocorrencia.Carga.Empresa.RazaoSocial);
            }

            if (!string.IsNullOrWhiteSpace(ocorrencia.Observacao))
                nota = string.Concat(nota, $" | - {Localization.Resources.Gerais.Geral.Observacao}: ", ocorrencia.Observacao);

            if (!string.IsNullOrWhiteSpace(ocorrencia.Observacao))
                nota = string.Concat(nota, $" | - {Localization.Resources.Gerais.Geral.ObservacaoImpressa}: ", ocorrencia.Observacao);

            if (aprovacao != null)
                if (aprovacao.RegrasAutorizacaoOcorrencia.EnviarLinkParaAprovacaoPorEmail)
                    nota = string.Concat(string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.LinkAcessoVerificarAutorizacaoOcorrencia, $"https://{urlAcesso}/aprovacao-ocorrencia/{aprovacao.GuidOcorrencia}"));

            return nota;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarRegrasPorOcorrencias(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            return BuscarRegrasPorOcorrencias(new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> { ocorrencia }, 0);
        }

        //Verificar a possibilidade de apenas passar um objeto com Codigo E etapa da ocorrencia
        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> BuscarRegrasPorOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias, int codigoUsuario)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorenciasAutorizacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
            {
                EtapaAutorizacaoOcorrencia etapa = ocorrencia.EtapaAutorizacaoOcorrencia;
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> regras = codigoUsuario == 0 ? repCargaOcorrenciaAutorizacao.BuscarPorOcorrenciaUsuarioEtapa(ocorrencia.Codigo, 0, etapa)
                                                                                                                     : repCargaOcorrenciaAutorizacao.BuscarPendentesPorOcorrenciaUsuarioEtapa(ocorrencia.Codigo, codigoUsuario, etapa);
                ocorenciasAutorizacao.AddRange(regras);
            }

            return ocorenciasAutorizacao;
        }

        public void VerificarOcorrenciasParaAprovacaoAutomatica()
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repOcorrencia.BuscarOcorrenciasParaAprovacaoAutomatica();
            ocorrencias = ocorrencias
                .Where(o => (o.DataBaseAprovacaoAutomatica ?? o.DataOcorrencia).AddDays(o.TipoOcorrencia.DiasAprovacaoAutomatica) < DateTime.Now)
                .ToList();

            if (ocorrencias.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
                {
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes = BuscarRegrasPorOcorrencias(ocorrencia);

                    var usuarioBaseAprovacao = ocorrenciasAutorizacoes
                        .Where(o => o.Usuario?.CentrosResultado.Count > 0)
                        .OrderByDescending(o => o.OrigemRegraOcorrencia)
                        .OrderByDescending(o => o.Codigo)
                        .Select(o => o.Usuario)?
                        .FirstOrDefault();
                    var centroResultadoBase = usuarioBaseAprovacao?.CentrosResultado?.FirstOrDefault();

                    AprovarMultiplasOcorrencias(ocorrenciasAutorizacoes, utilizarJustificativaPadrao: true, centroResultado: centroResultadoBase);
                }
            }
        }

        public int AprovarMultiplasOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes, bool utilizarJustificativaPadrao, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado)
        {
            return AprovarMultiplasOcorrencias(ocorrenciasAutorizacoes, null, 0, 0m, centroResultado, utilizarJustificativaPadrao, null);
        }

        public int AprovarMultiplasOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes, Dominio.Enumeradores.TipoTomador? tomador, int quantidadeParcelas, decimal percentualJurosParcela, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento)
        {
            return AprovarMultiplasOcorrencias(ocorrenciasAutorizacoes, tomador, quantidadeParcelas, percentualJurosParcela, null, utilizarJustificativaPadrao: false, periodoPagamento);
        }

        public int ReprovarMultiplasOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes, string motivo, Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa, Dominio.Enumeradores.TipoTomador? tomador, Dominio.Entidades.Cliente clienteTomador, decimal percentualJurosParcela, int quantidadeParcelas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento)
        {
            return RejeitarMultiplasOcorrencias(ocorrenciasAutorizacoes, motivo, justificativa, tomador, clienteTomador, percentualJurosParcela, quantidadeParcelas, periodoPagamento);
        }

        public void EfetuarAprovacao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao ocorrenciaAutorizacao, Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa = null, string motivo = null, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = null, AutorizacaoOcorrenciaPagamento? pagamento = null)
        {
            Dominio.Entidades.Usuario usuario = _auditado.Usuario;

            if (ocorrenciaAutorizacao.Situacao == SituacaoOcorrenciaAutorizacao.Pendente && (usuario == null || ocorrenciaAutorizacao.Usuario.Codigo == usuario.Codigo))
            {
                int dias = ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia.DiasAprovacaoAutomatica;
                if (ocorrenciaAutorizacao.DataPrazoAprovacaoAutomatica.HasValue)
                    dias = ocorrenciaAutorizacao.RegrasAutorizacaoOcorrencia.AprovacaoAutomaticaAposDias;

                if (usuario == null)
                    motivo = $"Aprovado automaticamente pelo sistema por ficar {dias} dias aguardando aprovação";

                _repositorioCargaOcorrenciaAutorizacao.AtualizarCargaOcorrenciaAprovada(ocorrenciaAutorizacao.Codigo, motivo, justificativa?.Codigo ?? 0, centroResultado?.Codigo ?? 0);

                if (_auditado != null)
                {
                    string motivoAuditoria = $"Aprovado automaticamente pelo sistema por ficar {dias} dias aguardando aprovação";
                    if (usuario != null)
                        motivoAuditoria = "Aprovado por " + usuario.Nome;

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, ocorrenciaAutorizacao.CargaOcorrencia, null, motivoAuditoria, _unitOfWork);
                }

                if (pagamento.HasValue && _configuracaoOcorrencia.ExibirCampoInformativoPagadorAutorizacaoOcorrencia)
                    new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork).AtualizarPagamento(ocorrenciaAutorizacao.CargaOcorrencia.Codigo, pagamento.Value);

                NotificarUsuarioAlteracao(aprovada: true, ocorrenciaAutorizacao.CargaOcorrencia);
                NotificarTransportadorAlteracao(aprovada: true, ocorrenciaAutorizacao.CargaOcorrencia, ocorrenciaAutorizacao.CargaOcorrencia.ObterEmitenteOcorrencia(), _configuracaoTMS);
                Task.Factory.StartNew(() => GerarNotificacaoOcorrenciaAprovada(ocorrenciaAutorizacao.Codigo, usuario, "Ocorrência aprovada com sucesso", Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, _unitOfWork.StringConexao));
            }
        }

        public void NotificarUsuarioAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, null, _tipoServicoMultisoftware, string.Empty);

                IconesNotificacao icone = aprovada ? IconesNotificacao.confirmado : IconesNotificacao.rejeitado;

                string titulo = Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Ocorrencia;
                string mensagem = ObterMensagemNotificarAlteracao(ocorrencia, aprovada, _configuracaoTMS.LinkUrlAcessoCliente);
                serNotificacao.GerarNotificacaoEmail(ocorrencia.Usuario, _auditado.Usuario, ocorrencia.Codigo, "Ocorrencias/Ocorrencia", titulo, mensagem, icone, TipoNotificacao.credito, _tipoServicoMultisoftware, _unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public void NotificarTransportadorAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            if (transportador == null)
                return;

            Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
            {
                AssuntoEmail = Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Ocorrencia,
                CabecalhoMensagem = aprovada ? string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorrenciaAprovada, ocorrencia.NumeroOcorrencia) : string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorrenciaRejeitada, ocorrencia.NumeroOcorrencia),
                CodigoOrigemNotificacao = ocorrencia.Codigo,
                Empresa = transportador,
                Mensagem = ObterMensagemNotificarAlteracao(ocorrencia, aprovada, configuracaoTMS.LinkUrlAcessoCliente),
                TipoServicoMultisoftware = _tipoServicoMultisoftware
            };

            servicoNotificacaoEmpresa.GerarNotificacao(notificacaoEmpresa, "Ocorrencias/AutorizacaoOcorrencia", IconesNotificacao.atencao, TipoNotificacao.alerta);
            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
        }

        public void NotificarOcorrenciaDelegada(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Usuario aprovador, Dominio.Entidades.Usuario usuarioDelegou)
        {
            try
            {
                Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, null, _tipoServicoMultisoftware, string.Empty);
                string titulo = "Ocorrência";
                string mensagem = string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorrenciaValorCargaDelegadaUsuario, ocorrencia.TipoOcorrencia.Descricao, ocorrencia.ValorOcorrencia.ToString("n2"), (ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty), aprovador.Descricao);

                servicoNotificacao.GerarNotificacaoEmail(ocorrencia.Usuario, usuarioDelegou, ocorrencia.Codigo, "Ocorrencias/Ocorrencia", titulo, mensagem, IconesNotificacao.cifra, TipoNotificacao.credito, _tipoServicoMultisoftware, _unitOfWork);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        public void NotificarOcorrenciaDelegadaAoAprovador(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Usuario aprovador, Dominio.Entidades.Usuario usuarioDelegou)
        {
            try
            {
                Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, null, _tipoServicoMultisoftware, string.Empty);

                string titulo = "Ocorrência";

                string mensagem = ObterMensagemNotificacaoAprovador(ocorrencia, usuarioDelegou, _configuracaoTMS.LinkUrlAcessoCliente);

                servicoNotificacao.GerarNotificacaoEmail(aprovador, usuarioDelegou, ocorrencia.Codigo, "Ocorrencias/Ocorrencia", titulo, mensagem, IconesNotificacao.cifra, TipoNotificacao.credito, _tipoServicoMultisoftware, _unitOfWork);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        public void NotificarOcorrenciaDelegadaRejeitadaPeloAprovador(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Usuario usuarioRejeitou, Dominio.Entidades.Usuario usuarioNotificar)
        {
            if (usuarioNotificar == null)
                return;

            try
            {
                Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, null, _tipoServicoMultisoftware, string.Empty);
                string titulo = "Ocorrência";
                string mensagem = string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorrenciaValorCargaRejeitadaUsuario, ocorrencia.TipoOcorrencia.Descricao, ocorrencia.ValorOcorrencia.ToString("n2"), (ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty), usuarioRejeitou.Descricao);

                servicoNotificacao.GerarNotificacaoEmail(usuarioNotificar, usuarioRejeitou, ocorrencia.Codigo, "Ocorrencias/Ocorrencia", titulo, mensagem, IconesNotificacao.cifra, TipoNotificacao.credito, _tipoServicoMultisoftware, _unitOfWork);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        public void VerificarSituacaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Enumeradores.TipoTomador? tomador, Dominio.Entidades.Cliente tomadorOutros, int quantidadeParcelas, decimal percentualJurosParcela, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento)
        {
            if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAprovacao && ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.AgAutorizacaoEmissao)
                return;

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(_unitOfWork);

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);

            EtapaAutorizacaoOcorrencia etapa = ocorrencia.EtapaAutorizacaoOcorrencia;
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> alcadas = _repositorioCargaOcorrenciaAutorizacao.BuscarPorOcorrenciaUsuarioEtapa(ocorrencia.Codigo, 0, etapa);
            bool alcadasPossuemRegra = alcadas.Any(o => o.RegrasAutorizacaoOcorrencia != null);
            bool rejeitada = false;
            bool aprovada = true;
            bool ratearValorFrenteEntreCTesComplementares = false;
            Dominio.Entidades.Usuario usuario = _auditado.Usuario;

            if (alcadasPossuemRegra)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> regrasDesbloqueadas = (
                    from alcada in alcadas
                    where alcada.RegrasAutorizacaoOcorrencia != null
                    select alcada.RegrasAutorizacaoOcorrencia
                ).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regra in regrasDesbloqueadas)
                {
                    int prioridadeAprovacao = (
                        from alcada in alcadas
                        where alcada.RegrasAutorizacaoOcorrencia?.Codigo == regra.Codigo
                        select alcada.PrioridadeAprovacao ?? alcada.RegrasAutorizacaoOcorrencia.PrioridadeAprovacao
                    ).FirstOrDefault();

                    int aprovacoes = _repositorioCargaOcorrenciaAutorizacao.ContarAprovacoesOcorrencia(ocorrencia.Codigo, regra.Codigo, prioridadeAprovacao, etapa);
                    int rejeitadas = _repositorioCargaOcorrenciaAutorizacao.ContarRejeitadas(ocorrencia.Codigo, regra.Codigo, prioridadeAprovacao, etapa);
                    int numeroAprovacoesNecessarias = _repositorioCargaOcorrenciaAutorizacao.BuscarNumeroAprovacoesNecessarias(ocorrencia.Codigo, regra.Codigo, etapa);
                    int numeroReprovacoesNecessarias = _repositorioCargaOcorrenciaAutorizacao.BuscarNumeroReprovacoesNecessarias(ocorrencia.Codigo, regra.Codigo, etapa);

                    if (rejeitadas >= numeroReprovacoesNecessarias)
                        rejeitada = true;
                    else if (aprovacoes < numeroAprovacoesNecessarias)
                        aprovada = false;
                }
            }
            else if (alcadas.Count > 0)
            {
                int aprovacoes = _repositorioCargaOcorrenciaAutorizacao.ContarAprovacoesOcorrencia(ocorrencia.Codigo, etapa);
                int rejeitadas = _repositorioCargaOcorrenciaAutorizacao.ContarRejeitadas(ocorrencia.Codigo, etapa);
                int numeroAprovacoesNecessarias = (from alcada in alcadas select alcada.NumeroAprovadores).FirstOrDefault();
                int numeroReprovacoesNecessarias = (from alcada in alcadas select alcada.NumeroReprovadores).FirstOrDefault();

                if (rejeitadas >= numeroReprovacoesNecessarias)
                    rejeitada = true;
                else if (aprovacoes < numeroAprovacoesNecessarias)
                    aprovada = false;
            }

            SituacaoOcorrencia situacao = SituacaoOcorrencia.AgAprovacao;
            if (rejeitada && ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao)
                situacao = SituacaoOcorrencia.Rejeitada;
            else if (rejeitada && ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao)
                situacao = SituacaoOcorrencia.RejeitadaEtapaEmissao;
            else if (aprovada)
            {
                //situacao = SituacaoOcorrencia.AgEmissaoCTeComplementar;
                situacao = SituacaoOcorrencia.EmEmissaoCTeComplementar;
                aprovada = LiberarProximasHierarquiasDeAprovacao(ocorrencia, etapa, usuario, _tipoServicoMultisoftware, _unitOfWork.StringConexao, _unitOfWork);
                Servicos.Log.TratarErro(ocorrencia.Codigo.ToString() + " Avançou a etapa de aprovação em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "AprovacaoOcorrencias");
            }

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);

            if (ocorrencia.TipoOcorrencia?.ExibirParcelasNaAprovacao ?? false)
            {
                ocorrencia.PercentualJurosParcela = percentualJurosParcela;
                ocorrencia.QuantidadeParcelas = quantidadeParcelas;
                ocorrencia.PeriodoPagamento = periodoPagamento;
            }

            if (rejeitada || aprovada)
            {
                Servicos.Embarcador.Notificacao.Notificacao servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, null, _tipoServicoMultisoftware, string.Empty);

                if (ocorrencia.Carga != null)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ValidarOcorrenciaPendente(ocorrencia.Carga, _unitOfWork);

                if (repositorioCargaCTeComplementoInfo.ContarPorOcorrencia(ocorrencia.Codigo) > 0)
                {
                    // Verifica se a situacao e ag aprovacao para testar a regra de etapa ag emissao
                    if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao && !rejeitada)
                    {
                        List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaFiltrada = VerificarRegrasAutorizacaoOcorrencia(ocorrencia, EtapaAutorizacaoOcorrencia.EmissaoOcorrencia, _unitOfWork);

                        if (listaFiltrada.Count() > 0)
                        {
                            situacao = SituacaoOcorrencia.AgAutorizacaoEmissao;

                            if (!CriarRegrasAutorizacao(listaFiltrada, ocorrencia, ocorrencia.Usuario, out List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes, _tipoServicoMultisoftware, _unitOfWork.StringConexao, _unitOfWork))
                                situacao = SituacaoOcorrencia.EmEmissaoCTeComplementar; //situacao = SituacaoOcorrencia.AgEmissaoCTeComplementar;

                            foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao notificacao in notificoes)
                            {
                                string titulo = "Ocorrência";
                                servicoNotificacao.GerarNotificacaoEmail(notificacao.Aprovador, ocorrencia.Usuario, ocorrencia.Codigo, "Ocorrencias/AutorizacaoOcorrencia", titulo, notificacao.Mensagem, IconesNotificacao.cifra, TipoNotificacao.credito, _tipoServicoMultisoftware, _unitOfWork);
                            }
                        }
                        else
                            situacao = SituacaoOcorrencia.SemRegraEmissao;

                        Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentosParaProvisaoOcorrencia(ocorrencia, _tipoServicoMultisoftware, _unitOfWork, _configuracaoTMS);
                    }
                    else if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao && rejeitada)
                        Servicos.Embarcador.Escrituracao.CancelamentoProvisao.CancelarProvisaoDocumentosOcorrencia(ocorrencia, _unitOfWork);
                }

                ocorrencia.ResponsavelAutorizacao = null;
                ocorrencia.SituacaoOcorrencia = situacao;
                ocorrencia.OcorrenciaReprovada = false;

                if (situacao == SituacaoOcorrencia.AgAutorizacaoEmissao)
                    ocorrencia.DataBaseAprovacaoAutomatica = DateTime.Now;
                else if (situacao == SituacaoOcorrencia.EmEmissaoCTeComplementar)
                {
                    if (!ocorrencia.DataAprovacao.HasValue)
                        ocorrencia.DataAprovacao = DateTime.Now;

                    if ((ocorrencia.ValorOcorrencia > 0m) && (ocorrencia.PercentualJurosParcela > 0m))
                    {
                        ocorrencia.ValorOcorrencia += (ocorrencia.ValorOcorrencia * (ocorrencia.PercentualJurosParcela / 100));

                        if (ocorrencia.Moeda.HasValue && (ocorrencia.Moeda != MoedaCotacaoBancoCentral.Real))
                            ocorrencia.ValorTotalMoeda = Math.Round(ocorrencia.ValorOcorrencia / ocorrencia.ValorCotacaoMoeda.Value, 2, MidpointRounding.AwayFromZero);

                        if (ocorrencia.ComponenteFrete?.SomarComponenteFreteLiquido ?? false)
                            ocorrencia.ValorOcorrenciaLiquida = ocorrencia.ValorOcorrencia;

                        ratearValorFrenteEntreCTesComplementares = (!(ocorrencia.ComponenteFrete?.TipoComponenteFrete == TipoComponenteFrete.ICMS) && !ocorrencia.ComplementoValorFreteCarga);
                    }
                }

                if (ocorrencia.Usuario != null)
                {
                    IconesNotificacao icone = rejeitada ? IconesNotificacao.rejeitado : IconesNotificacao.confirmado;
                    string mensagem = string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorrenciaValorCargaFoi, ocorrencia.TipoOcorrencia.Descricao, ocorrencia.ValorOcorrencia.ToString("n2"), (ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty), (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitada : Localization.Resources.Gerais.Geral.Aprovada));

                    servicoNotificacao.GerarNotificacao(ocorrencia.Usuario, usuario, ocorrencia.Codigo, "Ocorrencias/Ocorrencia", mensagem, icone, TipoNotificacao.credito, _tipoServicoMultisoftware, _unitOfWork);
                }
            }
            else if (ocorrencia.ResponsavelAutorizacao != null)
                ocorrencia.ResponsavelAutorizacao = null;

            repositorioCargaOcorrencia.Atualizar(ocorrencia);

            if (tomador.HasValue && ocorrencia.TipoOcorrencia.PermiteSelecionarTomador)
            {
                ocorrencia.Responsavel = tomador;

                if (ocorrencia.Responsavel == Dominio.Enumeradores.TipoTomador.Outros && tomadorOutros != null)
                    ocorrencia.Tomador = tomadorOutros;
            }

            if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar) //(ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgEmissaoCTeComplementar)
            {
                Servicos.Embarcador.Carga.RateioCTeComplementar servicoRateioCTeComplementar = new Servicos.Embarcador.Carga.RateioCTeComplementar(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrencia.Codigo);

                if (ratearValorFrenteEntreCTesComplementares)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfoSemFilialEmissora = cargaCTesComplementoInfo.Where(o => !o.ComplementoFilialEmissora).ToList();
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfoComFilialEmissora = cargaCTesComplementoInfo.Where(o => o.ComplementoFilialEmissora).ToList();

                    if (cargaCTesComplementoInfoSemFilialEmissora.Count > 0)
                        servicoRateioCTeComplementar.RatearValorDoFrenteEntreCTesComplementares(ocorrencia.ValorOcorrencia, cargaCTesComplementoInfoSemFilialEmissora, _unitOfWork, _configuracaoTMS, _tipoServicoMultisoftware, ocorrencia.Moeda ?? MoedaCotacaoBancoCentral.Real, ocorrencia.ValorCotacaoMoeda ?? 0m, ocorrencia.ValorTotalMoeda ?? 0m, ocorrencia.TipoOcorrencia.TipoRateio);

                    if (cargaCTesComplementoInfoComFilialEmissora.Count > 0)
                        servicoRateioCTeComplementar.RatearValorDoFrenteEntreCTesComplementares(ocorrencia.ValorOcorrencia, cargaCTesComplementoInfoComFilialEmissora, _unitOfWork, _configuracaoTMS, _tipoServicoMultisoftware, MoedaCotacaoBancoCentral.Real, 0m, 0m, ocorrencia.TipoOcorrencia.TipoRateio);
                }
                else
                {
                    repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorOcorrencia(ocorrencia.Codigo);

                    for (var i = 0; i < cargaCTesComplementoInfo.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = cargaCTesComplementoInfo[i];
                        servicoRateioCTeComplementar.CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, _tipoServicoMultisoftware, _unitOfWork, _configuracaoTMS);
                    }
                }

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia.ExecutaProximoPassoOcorrencia(ocorrencia, _unitOfWork.StringConexao, _unitOfWork);
            }
        }

        public string ObterMiniatura(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos anexo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencia");
            string extensao = Path.GetExtension(anexo.NomeArquivo).ToLower();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extensao);

            List<string> extensoesPermitidas = new List<string>
            {
                ".jpg",
                ".jpeg",
                ".png"
            };

            if (!extensoesPermitidas.Contains(extensao))
            {
                Servicos.Log.TratarErro($"Anexo miniatura {anexo.Codigo} não pertence as extensões permitidas", "ObterMiniaturaLog");
                return null;
            }

            Servicos.Log.TratarErro($"Anexo miniatura {anexo.Codigo} obtendo", "ObterMiniaturaLog");

            return ObterMiniatura(nomeArquivo);
        }

        #endregion

        #region Métodos Privados

        private int AprovarMultiplasOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes, Dominio.Enumeradores.TipoTomador? tomador, int quantidadeParcelas, decimal percentualJurosParcela, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado, bool utilizarJustificativaPadrao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento)
        {
            int quantidadeAprovadas = 0;
            Stopwatch stopwatchOcorrencia = new Stopwatch();

            Stopwatch stopwatchGeral = Stopwatch.StartNew();
            int[] codigosOcorrenciasAutorizacao = ocorrenciasAutorizacoes.Select(x => x.Codigo).ToArray();

            Servicos.Log.GravarInfo("----- INÍCIO AprovarMultiplasOcorrencias -----", "TempoExecucaoAprovacaoOcorrencia");

            for (int i = 0; i < codigosOcorrenciasAutorizacao.Length; i++)
            {
                _unitOfWork.Start();
                stopwatchOcorrencia.Restart();

                try
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao ocorrenciaAutorizacao = _repositorioCargaOcorrenciaAutorizacao.BuscarPorCodigoComFetch(codigosOcorrenciasAutorizacao[i]);

                    Servicos.Log.GravarInfo($"--- Início processamento da ocorrência: {ocorrenciaAutorizacao.Codigo} ---", "TempoExecucaoAprovacaoOcorrencia");

                    if ((ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia?.InformarMotivoNaAprovacao ?? false) && ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia.JustificativaPadraoAprovacao == null)
                    {
                        Servicos.Log.GravarInfo($"Ocorrência {ocorrenciaAutorizacao.Codigo} ignorada: exige motivo mas não possui justificativa padrão.", "TempoExecucaoAprovacaoOcorrencia");
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa = utilizarJustificativaPadrao ? ocorrenciaAutorizacao.CargaOcorrencia.TipoOcorrencia.JustificativaPadraoAprovacao : null;

                    Stopwatch swEfetuarAprovacao = Stopwatch.StartNew();
                    EfetuarAprovacao(ocorrenciaAutorizacao, justificativa, null, centroResultado);
                    swEfetuarAprovacao.Stop();
                    Servicos.Log.GravarInfo($"Tempo EfetuarAprovacao: {swEfetuarAprovacao.ElapsedMilliseconds}ms | Ocorrência: {ocorrenciaAutorizacao.Codigo}", "TempoExecucaoAprovacaoOcorrencia");

                    Stopwatch swVerificarSituacao = Stopwatch.StartNew();
                    VerificarSituacaoOcorrencia(ocorrenciaAutorizacao.CargaOcorrencia, tomador, null, quantidadeParcelas, percentualJurosParcela, periodoPagamento);
                    swVerificarSituacao.Stop();
                    Servicos.Log.GravarInfo($"Tempo VerificarSituacaoOcorrencia: {swVerificarSituacao.ElapsedMilliseconds}ms | Ocorrência: {ocorrenciaAutorizacao.Codigo}", "TempoExecucaoAprovacaoOcorrencia");

                    quantidadeAprovadas++;
                    _unitOfWork.CommitChanges();

                    stopwatchOcorrencia.Stop();
                    Servicos.Log.GravarInfo($"--- Fim processamento da ocorrência: {ocorrenciaAutorizacao.Codigo} | Tempo total: {stopwatchOcorrencia.ElapsedMilliseconds}ms ---", "TempoExecucaoAprovacaoOcorrencia");
                }
                catch (ServicoException)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.GravarInfo($"Erro de serviço ao aprovar ocorrência: {codigosOcorrenciasAutorizacao[i]}", "TempoExecucaoAprovacaoOcorrencia");
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    Servicos.Log.GravarInfo($"Erro inesperado ao aprovar ocorrência: {codigosOcorrenciasAutorizacao[i]}", "TempoExecucaoAprovacaoOcorrencia");
                }
                finally
                {
                    _unitOfWork.Clear();
                }
            }

            stopwatchGeral.Stop();

            Servicos.Log.GravarInfo($"----- FIM AprovarMultiplasOcorrencias -----\n" +
                                    $"Total ocorrências aprovadas: {quantidadeAprovadas}\n" +
                                    $"Tempo total de execução: {stopwatchGeral.ElapsedMilliseconds}ms",
                                    "TempoExecucaoAprovacaoOcorrencia");

            return quantidadeAprovadas;
        }


        private int RejeitarMultiplasOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes, string motivo, Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa, Dominio.Enumeradores.TipoTomador? tomador, Dominio.Entidades.Cliente clienteTomador, decimal percentualJurosParcela, int quantidadeParcelas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoPagamento? periodoPagamento)
        {
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrenciasDelegadasNotificarRejeicao = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            // Aprova todas as regras
            for (int i = 0; i < ocorrenciasAutorizacoes.Count; i++)
            {
                try
                {
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                    Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
                    Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(_unitOfWork);
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(_unitOfWork);

                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = ocorrenciasAutorizacoes[i];

                    cargaOcorrenciaAutorizacao.Data = DateTime.Now;
                    cargaOcorrenciaAutorizacao.Situacao = SituacaoOcorrenciaAutorizacao.Rejeitada;
                    cargaOcorrenciaAutorizacao.Motivo = motivo;
                    cargaOcorrenciaAutorizacao.MotivoRejeicaoOcorrencia = justificativa;

                    repCargaOcorrenciaAutorizacao.Atualizar(cargaOcorrenciaAutorizacao);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaOcorrenciaAutorizacao.CargaOcorrencia, null, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RejeitadoPor, cargaOcorrenciaAutorizacao.Usuario?.Nome), _unitOfWork);

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(cargaOcorrenciaAutorizacao.CargaOcorrencia.Codigo);

                    VerificarSituacaoOcorrencia(cargaOcorrencia, tomador, clienteTomador, quantidadeParcelas, percentualJurosParcela, periodoPagamento);

                    if (cargaOcorrenciaAutorizacao.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Delegada)
                    {
                        cargaOcorrencia.UltimoUsuarioDelegado = cargaOcorrencia.UltimoUsuarioDelegou;
                        ocorrenciasDelegadasNotificarRejeicao.Add(cargaOcorrencia);
                    }
                    else if (cargaOcorrenciaAutorizacao.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Assumida)
                        cargaOcorrencia.UltimoUsuarioDelegado = cargaOcorrencia.UltimoUsuarioDelegou;

                    cargaOcorrencia.DataBaseAprovacaoAutomatica = DateTime.Now;
                    repCargaOcorrencia.Atualizar(cargaOcorrencia);

                    List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrencia(cargaOcorrencia.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                    {
                        if (chamado.Situacao != SituacaoChamado.LiberadaOcorrencia)
                        {
                            chamado.Situacao = SituacaoChamado.LiberadaOcorrencia;
                            repChamado.Atualizar(chamado);
                            Servicos.Auditoria.Auditoria.Auditar(_auditado, chamado, null, string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RetornadoSituacaoParaLiberadoParaOcorrenciaAoRejeitarOcorrencia, cargaOcorrencia.NumeroOcorrencia), _unitOfWork);
                        }
                    }

                    _unitOfWork.CommitChanges();
                }
                catch (Exception ex2)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                }
            }

            for (int i = 0; i < ocorrenciasDelegadasNotificarRejeicao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = ocorrenciasDelegadasNotificarRejeicao[i];
                NotificarOcorrenciaDelegadaRejeitadaPeloAprovador(cargaOcorrencia, _auditado.Usuario, cargaOcorrencia.UltimoUsuarioDelegado);
            }

            return ocorrenciasAutorizacoes.Count();

        }

        private static void NotificarUsuario(Dominio.Entidades.Usuario aprovador, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            string titulo = "Ocorrência";

            System.Text.StringBuilder st = new System.Text.StringBuilder();
            if (usuario != null)
                st.Append(string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.UsuarioSolicitouLiberacaoOcorrenciaCarga, usuario.Nome, ocorrencia.TipoOcorrencia.Descricao, ocorrencia.NumeroOcorrencia, ocorrencia.ValorOcorrencia.ToString("n2"), ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty));
            else
                st.Append(string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SolicitadaLiberacaoOcorrenciaCarga, ocorrencia.TipoOcorrencia.Descricao, ocorrencia.NumeroOcorrencia, ocorrencia.ValorOcorrencia.ToString("n2"), ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty));

            if (aprovacao.RegrasAutorizacaoOcorrencia.EnviarLinkParaAprovacaoPorEmail)
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                st.AppendLine(string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.LinkAcessoVerificarAutorizacaoOcorrencia, $"https://{configuracaoTMS.LinkUrlAcessoCliente}/aprovacao-ocorrencia/{aprovacao.GuidOcorrencia}"));
            }

            serNotificacao.GerarNotificacaoEmail(aprovador, usuario, ocorrencia.Codigo, "Ocorrencias/AutorizacaoOcorrencia", titulo, st.ToString(), IconesNotificacao.cifra, TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
        }

        private static string ObterMensagemNotificarAlteracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool aprovada, string urlAcesso)
        {
            string nota = string.Format(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.UsuarioOcorrenciaValorCarga, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou), ocorrencia.TipoOcorrencia.Descricao, ocorrencia.ValorOcorrencia.ToString("n2"), (ocorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty));

            if (ocorrencia.NumeroOcorrencia > 0)
                nota = string.Concat(nota, " | - Número ocorrência: ", ocorrencia.NumeroOcorrencia);

            if (!string.IsNullOrWhiteSpace(urlAcesso))
                nota = string.Concat(nota, " | - Link para acessar a Ocorrência: ", $"https://{urlAcesso}/#Ocorrencias/Ocorrencia?TokenAcesso={ocorrencia.Codigo}");

            return nota;
        }

        private string ObterMiniatura(string nomeArquivo)
        {
            if (Path.GetExtension(nomeArquivo).ToLower() == ".tif" || Path.GetExtension(nomeArquivo).ToLower() == ".jpg")
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo) || GetFileSize(nomeArquivo) == 0)
                    return null;

                using (System.Drawing.Image image = System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeArquivo)))
                {
                    if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(image.RawFormat))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            nomeArquivo = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                return null;

            using (Image imgPhoto = Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeArquivo)))
            using (Bitmap newImage = ResizeImage(imgPhoto, 600))
            using (MemoryStream ms = new MemoryStream())
            {
                newImage.Save(ms, imgPhoto.RawFormat);

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private Bitmap ResizeImage(Image image, int newWidth)
        {
            int newHeight = (image.Height * newWidth) / image.Width;
            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        static long GetFileSize(string FilePath)
        {
            if (Utilidades.IO.FileStorageService.Storage.Exists(FilePath))
            {
                return Utilidades.IO.FileStorageService.Storage.GetFileInfo(FilePath).Size;
            }
            return 0;
        }

        private void GerarNotificacaoOcorrenciaAprovada(int codigoCargaOcorrencia, Dominio.Entidades.Usuario usuario, string mensagemNotificacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone, string stringConexao)
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            serNotificacao.GerarNotificacao(usuario, codigoCargaOcorrencia, "Chamados/ChamadoOcorrencia", mensagemNotificacao, icone, TipoNotificacao.credito, _tipoServicoMultisoftware, unitOfWork);
        }

        #endregion
    }
}
