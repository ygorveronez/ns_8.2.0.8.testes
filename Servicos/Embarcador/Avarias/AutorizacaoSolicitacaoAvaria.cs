using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Avarias
{
    public class AutorizacaoSolicitacaoAvaria
    {
        #region Métodos Públicos

        public static List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> VerificarRegrasAutorizacaoAvaria(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria repRegrasAutorizacaoAvaria = new Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegras = new List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada = new List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();

            //Regra pormotivo da avaria
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegraMotivoAvaria = repRegrasAutorizacaoAvaria.BuscarRegraPorMotivoAvaria(solicitacao.MotivoAvaria.Codigo, solicitacao.DataAvaria, etapa);
            listaRegras.AddRange(listaRegraMotivoAvaria);

            //Regra por origem
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegraOrigem = repRegrasAutorizacaoAvaria.BuscarRegraPorOrigem((from o in solicitacao.Carga.Percursos select o.Origem.Codigo).ToList(), solicitacao.DataAvaria, etapa);
            listaRegras.AddRange(listaRegraOrigem);

            //Regra por destino
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegraDestino = repRegrasAutorizacaoAvaria.BuscarRegraPorDestino((from o in solicitacao.Carga.Percursos select o.Destino.Codigo).ToList(), solicitacao.DataAvaria, etapa);
            listaRegras.AddRange(listaRegraDestino);

            int codigoFilial = solicitacao.Carga.Filial?.Codigo ?? 0;
            if (solicitacao.Carga.FilialOrigem != null)
                codigoFilial = solicitacao.Carga.FilialOrigem.Codigo;

            //Regra por Filial de Emissão
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegraFilial = new List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();
            if (codigoFilial > 0)
            {
                listaRegraFilial = repRegrasAutorizacaoAvaria.BuscarRegraPorFilial(codigoFilial, solicitacao.DataAvaria, etapa);
                listaRegras.AddRange(listaRegraFilial);
            }

            //Regra por Transportador
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegraTransportador = new List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();
            if (solicitacao.Transportador != null)
            {
                listaRegraTransportador = repRegrasAutorizacaoAvaria.BuscarRegraPorTransportadora(solicitacao.Transportador.Codigo, solicitacao.DataAvaria, etapa);
                listaRegras.AddRange(listaRegraTransportador);
            }

            //Regra por TipoOperacao
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegraTipoOperacao = new List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();
            if (solicitacao.Carga.TipoOperacao != null)
            {
                listaRegraTipoOperacao = repRegrasAutorizacaoAvaria.BuscarRegraPorTipoOperacao(solicitacao.Carga.TipoOperacao.Codigo, solicitacao.DataAvaria, etapa);
                listaRegras.AddRange(listaRegraTipoOperacao);
            }

            //Regra por valor da ocorrência
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaRegraValor = repRegrasAutorizacaoAvaria.BuscarRegraPorValor(solicitacao.ValorAvaria, solicitacao.DataAvaria, etapa);
            listaRegras.AddRange(listaRegraValor);

            if (listaRegras.Distinct().Count() > 0)
            {
                //List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> novalista = new List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();

                listaFiltrada.AddRange(listaRegras.Distinct());

                foreach (Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorMotivoAvaria)
                    {
                        bool valido = false;
                        if (regra.RegrasMotivoAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.MotivoAvaria.Codigo == solicitacao.MotivoAvaria.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.MotivoAvaria.Codigo == solicitacao.MotivoAvaria.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.MotivoAvaria.Codigo != solicitacao.MotivoAvaria.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.MotivoAvaria.Codigo != solicitacao.MotivoAvaria.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorOrigem)
                    {
                        bool valido = false;
                        if (regra.RegrasOrigem.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.Carga.Pedidos.Any(p => p.Origem.Codigo == o.Origem.Codigo)))
                            valido = true;
                        else if (regra.RegrasOrigem.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.Carga.Pedidos.Any(p => p.Origem.Codigo == o.Origem.Codigo)))
                            valido = true;
                        else if (regra.RegrasOrigem.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.Carga.Pedidos.All(p => p.Origem.Codigo != o.Origem.Codigo)))
                            valido = true;
                        else if (regra.RegrasOrigem.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.Carga.Pedidos.All(p => p.Origem.Codigo != o.Origem.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorDestino)
                    {
                        bool valido = false;
                        if (regra.RegrasDestino.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.Carga.Pedidos.Any(p => p.Destino.Codigo == o.Destino.Codigo)))
                            valido = true;
                        else if (regra.RegrasDestino.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.Carga.Pedidos.Any(p => p.Destino.Codigo == o.Destino.Codigo)))
                            valido = true;
                        else if (regra.RegrasDestino.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.Carga.Pedidos.All(p => p.Destino.Codigo != o.Destino.Codigo)))
                            valido = true;
                        else if (regra.RegrasDestino.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.Carga.Pedidos.All(p => p.Destino.Codigo != o.Destino.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorFilial && codigoFilial > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Filial.Codigo != codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Filial.Codigo != codigoFilial))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTransportadora && solicitacao.Transportador != null)
                    {
                        bool valido = false;
                        if (regra.RegrasTransportadora.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Transportadora.Codigo == solicitacao.Transportador.Codigo))
                            valido = true;
                        else if (regra.RegrasTransportadora.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Transportadora.Codigo == solicitacao.Transportador.Codigo))
                            valido = true;
                        else if (regra.RegrasTransportadora.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Transportadora.Codigo != solicitacao.Transportador.Codigo))
                            valido = true;
                        else if (regra.RegrasTransportadora.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Transportadora.Codigo != solicitacao.Transportador.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTipoOperacao && solicitacao.Carga.TipoOperacao != null)
                    {
                        bool valido = false;
                        if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.TipoOperacao.Codigo == solicitacao.Carga.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.TipoOperacao.Codigo == solicitacao.Carga.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.TipoOperacao.Codigo != solicitacao.Carga.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.TipoOperacao.Codigo != solicitacao.Carga.TipoOperacao.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValor)
                    {

                        bool valido = false;
                        if (regra.RegrasValorAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Valor == solicitacao.ValorAvaria))
                            valido = true;
                        else if (regra.RegrasValorAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Valor == solicitacao.ValorAvaria))
                            valido = true;
                        else if (regra.RegrasValorAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Valor != solicitacao.ValorAvaria))
                            valido = true;
                        else if (regra.RegrasValorAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Valor != solicitacao.ValorAvaria))
                            valido = true;
                        if (regra.RegrasValorAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.ValorAvaria >= o.Valor))
                            valido = true;
                        else if (regra.RegrasValorAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.ValorAvaria >= o.Valor))
                            valido = true;
                        if (regra.RegrasValorAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.ValorAvaria <= o.Valor))
                            valido = true;
                        else if (regra.RegrasValorAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.ValorAvaria <= o.Valor))
                            valido = true;
                        if (regra.RegrasValorAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.ValorAvaria > o.Valor))
                            valido = true;
                        else if (regra.RegrasValorAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.ValorAvaria > o.Valor))
                            valido = true;
                        if (regra.RegrasValorAvaria.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && solicitacao.ValorAvaria < o.Valor))
                            valido = true;
                        else if (regra.RegrasValorAvaria.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && solicitacao.ValorAvaria < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return listaFiltrada;
        }

        public static List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada, Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork, bool notificarUsuario)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao repAutorizacao = new Repositorio.Embarcador.Avarias.SolicitacaoAvariaAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao> autorizacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();

            foreach (Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regra in listaFiltrada)
            {
                foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                {
                    Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao autorizacao = new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao
                    {
                        SolicitacaoAvaria = solicitacao,
                        Usuario = aprovador,
                        RegrasAutorizacaoAvaria = regra,
                        OrigemRegraAvaria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria.Alcada,
                        EtapaAutorizacaoAvaria = regra.EtapaAutorizacaoAvaria,
                        NumeroAprovadores = regra.NumeroAprovadores
                    };
                    repAutorizacao.Inserir(autorizacao);

                    autorizacoes.Add(autorizacao);

                    if (notificarUsuario)
                    {
                        string titulo = Localization.Resources.Avarias.AutorizacaoAvaria.Avaria;
                        string nota = string.Format(Localization.Resources.Avarias.AutorizacaoAvaria.UsuarioSolicitouLiberacaoParaAvariaValorCarga, usuario.Nome, solicitacao.NumeroAvaria.ToString(), solicitacao.ValorAvaria.ToString("n2"), solicitacao.Carga.CodigoCargaEmbarcador);
                        serNotificacao.GerarNotificacaoEmail(aprovador, usuario, solicitacao.Codigo, "Avarias/AutorizacaoAvaria", titulo, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    }
                }
            }

            return autorizacoes;
        }

        public static void CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada, Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            CriarRegrasAutorizacao(listaFiltrada, solicitacao, usuario, tipoServicoMultisoftware, stringConexao, unitOfWork, true);
        }

        #endregion
    }
}
