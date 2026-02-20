using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public class AutorizacaoAjusteTabelaFrete
    {
        #region Métodos Privados

        private static void NotificarUsuario(Dominio.Entidades.Usuario aprovador, Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            System.Text.StringBuilder st = new System.Text.StringBuilder();
            st.Append(string.Format(Localization.Resources.Fretes.TabelaFrete.SolicitouLiberacaoAlteracaoTabelaFrete, usuario?.Nome ?? string.Empty));

            if (aprovacao.RegrasAutorizacaoTabelaFrete.EnviarLinkParaAprovacaoPorEmail)
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                st.AppendLine(string.Format(Localization.Resources.Fretes.TabelaFrete.LinkAcessoVerificarAutorizacaoTabelaFrete, $"https://{configuracaoTMS.LinkUrlAcessoCliente}/aprovacao-tabelafrete/{aprovacao.GuidTabelaFrete}"));
            }

            if (usuario != null)
                serNotificacao.GerarNotificacao(aprovador, usuario, ajuste.Codigo, "Frete/AjusteTabelaFrete", st.ToString(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
        }

        #endregion

        #region Métodos Públicos

        public static List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> VerificarRegrasAutorizacaoAjuste(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete repRegrasAutorizacaoTabelaFrete = new Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegras = new List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete>();
            List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaFiltrada = new List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete>();

            DateTime dataAjuste = ajuste.DataAjuste.HasValue ? ajuste.DataAjuste.Value : ajuste.DataCriacao;
            List<int> listaCodigoDestino = repTabelaFreteCliente.BuscarDestinoTabelasPorAjusteTabelaFrete(ajuste.Codigo);
            List<int> listaCodigoFiliais = ajuste.TabelaFrete.Filiais.Select(obj => obj.Codigo).ToList();
            List<int> listaCodigoOrigem = repTabelaFreteCliente.BuscarOrigemTabelasPorAjusteTabelaFrete(ajuste.Codigo);
            List<int> listaCodigoTipoOperacao = ajuste.TabelaFrete.TiposOperacao.Select(obj => obj.Codigo).ToList();
            List<int> listaCodigoTransportadores = ajuste.TabelaFrete.Transportadores.Select(obj => obj.Codigo).ToList();

            if (ajuste.MotivoReajuste != null)
            {
                List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegraMotivoReajuste = repRegrasAutorizacaoTabelaFrete.BuscarRegraPorMotivoReajuste(ajuste.TabelaFrete.Codigo, ajuste.MotivoReajuste.Codigo, dataAjuste, etapa);
                listaRegras.AddRange(listaRegraMotivoReajuste);
            }

            if (listaCodigoOrigem.Count > 0 && listaCodigoOrigem.Count < 300)
            {
                List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegraOrigem = repRegrasAutorizacaoTabelaFrete.BuscarRegraPorOrigemFrete(ajuste.TabelaFrete.Codigo, listaCodigoOrigem, dataAjuste, etapa);
                listaRegras.AddRange(listaRegraOrigem);
            }

            if (listaCodigoDestino.Count > 0 && listaCodigoDestino.Count < 300)
            {
                List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegraDestino = repRegrasAutorizacaoTabelaFrete.BuscarRegraPorDestinoFrete(ajuste.TabelaFrete.Codigo, listaCodigoDestino, dataAjuste, etapa);
                listaRegras.AddRange(listaRegraDestino);
            }

            if (listaCodigoTransportadores.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegraTransportador = repRegrasAutorizacaoTabelaFrete.BuscarRegraPorTransportadora(ajuste.TabelaFrete.Codigo, listaCodigoTransportadores, dataAjuste, etapa);
                listaRegras.AddRange(listaRegraTransportador);
            }

            if (listaCodigoTipoOperacao.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegraTipoOperacao = repRegrasAutorizacaoTabelaFrete.BuscarRegraPorTipoOperacao(ajuste.TabelaFrete.Codigo, listaCodigoTipoOperacao, dataAjuste, etapa);
                listaRegras.AddRange(listaRegraTipoOperacao);
            }

            if (listaCodigoFiliais.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegraTransportador = repRegrasAutorizacaoTabelaFrete.BuscarRegraPorFilial(ajuste.TabelaFrete.Codigo, listaCodigoFiliais, dataAjuste, etapa);
                listaRegras.AddRange(listaRegraTransportador);
            }

            IEnumerable<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaDistintas = listaRegras.Distinct();

            if (listaDistintas.Count() > 0)
            {
                listaFiltrada.AddRange(listaDistintas);

                foreach (Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regra in listaDistintas)
                {
                    if (regra.RegraPorMotivoReajuste)
                    {
                        bool valido = false;
                        if (regra.RegrasMotivoReajuste.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && o.MotivoReajuste.Codigo == ajuste.MotivoReajuste.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoReajuste.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && o.MotivoReajuste.Codigo == ajuste.MotivoReajuste.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoReajuste.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && o.MotivoReajuste.Codigo != ajuste.MotivoReajuste.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoReajuste.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && o.MotivoReajuste.Codigo != ajuste.MotivoReajuste.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorOrigemFrete)
                    {
                        bool valido = false;
                        if (regra.RegrasOrigemFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && listaCodigoOrigem.Contains(o.OrigemFrete.Codigo)))
                            valido = true;
                        else if (regra.RegrasOrigemFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && listaCodigoOrigem.Contains(o.OrigemFrete.Codigo)))
                            valido = true;
                        else if (regra.RegrasOrigemFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && !listaCodigoOrigem.Contains(o.OrigemFrete.Codigo)))
                            valido = true;
                        else if (regra.RegrasOrigemFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && !listaCodigoOrigem.Contains(o.OrigemFrete.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorDestinoFrete)
                    {
                        bool valido = false;
                        if (regra.RegrasDestinoFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && listaCodigoDestino.Contains(o.DestinoFrete.Codigo)))
                            valido = true;
                        else if (regra.RegrasDestinoFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && listaCodigoDestino.Contains(o.DestinoFrete.Codigo)))
                            valido = true;
                        else if (regra.RegrasDestinoFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && !listaCodigoDestino.Contains(o.DestinoFrete.Codigo)))
                            valido = true;
                        else if (regra.RegrasDestinoFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && !listaCodigoDestino.Contains(o.DestinoFrete.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTransportador)
                    {
                        bool valido = false;
                        if (regra.RegrasTransportador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && listaCodigoTransportadores.Contains(o.Transportador.Codigo)))
                            valido = true;
                        else if (regra.RegrasTransportador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && listaCodigoTransportadores.Contains(o.Transportador.Codigo)))
                            valido = true;
                        else if (regra.RegrasTransportador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && !listaCodigoTransportadores.Contains(o.Transportador.Codigo)))
                            valido = true;
                        else if (regra.RegrasTransportador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && !listaCodigoTransportadores.Contains(o.Transportador.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTipoOperacao)
                    {
                        bool valido = false;
                        if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && listaCodigoTipoOperacao.Contains(o.TipoOperacao.Codigo)))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && listaCodigoTipoOperacao.Contains(o.TipoOperacao.Codigo)))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && !listaCodigoTipoOperacao.Contains(o.TipoOperacao.Codigo)))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && !listaCodigoTipoOperacao.Contains(o.TipoOperacao.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorFilial)
                    {
                        bool valido = false;
                        if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && listaCodigoFiliais.Contains(o.Filial.Codigo)))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && listaCodigoFiliais.Contains(o.Filial.Codigo)))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.Ou && !listaCodigoFiliais.Contains(o.Filial.Codigo)))
                            valido = true;
                        else if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete.E && !listaCodigoFiliais.Contains(o.Filial.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaRegraTabelaFrete = repRegrasAutorizacaoTabelaFrete.BuscarRegraPorTipoTabelaFrete(ajuste.TabelaFrete.Codigo, dataAjuste, etapa);
            listaFiltrada.AddRange(listaRegraTabelaFrete.Distinct().ToList());

            return listaFiltrada;
        }

        public static bool LiberarProximasHierarquiasDeAprovacao(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repositorioAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> alcadasAprovacao = repositorioAutorizacao.BuscarPendentesBloqueadas(ajuste.Codigo);

            if (alcadasAprovacao.Count > 0)
            {
                int menorPrioridadeAprovacao = alcadasAprovacao.Select(obj => obj.RegrasAutorizacaoTabelaFrete.PrioridadeAprovacao).Min();

                foreach (Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao aprovacao in alcadasAprovacao)
                {
                    if (aprovacao.RegrasAutorizacaoTabelaFrete.PrioridadeAprovacao <= menorPrioridadeAprovacao)
                    {
                        aprovacao.Bloqueada = false;
                        repositorioAutorizacao.Atualizar(aprovacao);

                        NotificarUsuario(aprovacao.Usuario, ajuste, usuario, aprovacao, tipoServicoMultisoftware, stringConexao, unitOfWork);
                    }
                }

                return false;
            }

            return true;
        }

        public static void CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaFiltrada, Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool existeRegraSemAprovacao = false;
            Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
            List<Dominio.Entidades.Usuario> aprovadoresPorTransportador;
            int menorPrioridadeAprovacao = listaFiltrada.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            if (listaFiltrada.Any(regra => regra.TipoAprovadorRegra == TipoAprovadorRegra.Transportador))
                aprovadoresPorTransportador = new Repositorio.Usuario(unitOfWork).BuscarUsuariosAcessoTransportador(ajuste.Empresas.FirstOrDefault()?.Codigo ?? 0);
            else
                aprovadoresPorTransportador = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;
                    List<Dominio.Entidades.Usuario> aprovadores;

                    if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
                        aprovadores = regra.Aprovadores.ToList();
                    else
                        aprovadores = aprovadoresPorTransportador;

                    foreach (Dominio.Entidades.Usuario aprovador in aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao autorizacao = new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao()
                        {
                            AjusteTabelaFrete = ajuste,
                            Usuario = aprovador,
                            RegrasAutorizacaoTabelaFrete = regra,
                            EtapaAutorizacaoTabelaFrete = regra.EtapaAutorizacaoTabelaFrete,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            GuidTabelaFrete = Guid.NewGuid().ToString(),
                            TipoAprovadorRegra = regra.TipoAprovadorRegra
                        };

                        repAutorizacao.Inserir(autorizacao);

                        if (!autorizacao.Bloqueada)
                            NotificarUsuario(aprovador, ajuste, usuario, autorizacao, tipoServicoMultisoftware, stringConexao, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao autorizacao = new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao()
                    {
                        AjusteTabelaFrete = ajuste,
                        Usuario = null,
                        RegrasAutorizacaoTabelaFrete = regra,
                        EtapaAutorizacaoTabelaFrete = regra.EtapaAutorizacaoTabelaFrete,
                        Situacao = SituacaoAjusteTabelaFreteAutorizacao.Aprovada,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        Data = DateTime.Now,
                        TipoAprovadorRegra = regra.TipoAprovadorRegra
                    };

                    repAutorizacao.Inserir(autorizacao);
                }
            }

            if (!existeRegraSemAprovacao)
            {
                ajuste.Situacao = SituacaoAjusteTabelaFrete.EmProcessamento;
                ajuste.SituacaoAposProcessamento = SituacaoAjusteTabelaFrete.Finalizado;
                ajuste.UsuarioAprovador = null;
            }
            else
                ajuste.Situacao = SituacaoAjusteTabelaFrete.AgAprovacao;

            repositorioAjusteTabelaFrete.Atualizar(ajuste);
        }

        #endregion
    }
}