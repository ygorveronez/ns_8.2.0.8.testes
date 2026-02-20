using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Seguro
{
    public class Seguro : ServicoBase
    {
        #region Construtores
       
        public Seguro(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public static void AlertarVencimento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConfiguracaoAlerta.ApoliceSeguro);

            if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
                return;

            DateTime dataUltimoAlerta = DateTime.Now.Date;
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta()
            {
                AlertarAposVencimento = configuracaoAlerta.AlertarAposVencimento,
                DiasAlertarAntesVencimento = configuracaoAlerta.DiasAlertarAntesVencimento,
                DiasRepetirAlerta = configuracaoAlerta.DiasRepetirAlerta
            };
            Repositorio.Embarcador.Seguros.ApoliceSeguro repositorioApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguroAlertar = repositorioApoliceSeguro.BuscarApolicesSeguroAlertar(filtrosPesquisa);

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro in apolicesSeguroAlertar)
            {
                int diasParaVencer = (int)apoliceSeguro.FimVigencia.Subtract(dataUltimoAlerta).TotalDays;
                string mensagem = string.Empty;
                mensagem = string.Format(Localization.Resources.Seguro.Seguro.Embarcador, razaoSocial) + Environment.NewLine;
                if (diasParaVencer > 0)
                    mensagem += string.Format(Localization.Resources.Seguro.Seguro.ApoliceSeguroTransportadorVenceEm, apoliceSeguro.Descricao, (apoliceSeguro.Empresa == null ? "" : string.Format(Localization.Resources.Seguro.Seguro.DoTransportador, apoliceSeguro.Empresa.Descricao) + " "), diasParaVencer, (diasParaVencer == 1 ? "" : "s"));
                else if (diasParaVencer == 0)
                    mensagem += string.Format(Localization.Resources.Seguro.Seguro.ApoliceSeguroTransportadorVenceHoje, apoliceSeguro.Descricao, (apoliceSeguro.Empresa == null ? "" : string.Format(Localization.Resources.Seguro.Seguro.DoTransportador, apoliceSeguro.Empresa.Descricao) + " "));
                else
                    mensagem += string.Format(Localization.Resources.Seguro.Seguro.ApoliceSeguroTransportadorVenceuA, apoliceSeguro.Descricao, (apoliceSeguro.Empresa == null ? "" : string.Format(Localization.Resources.Seguro.Seguro.DoTransportador, apoliceSeguro.Empresa.Descricao) + " "), (-diasParaVencer), (diasParaVencer == -1 ? "" : "s"));

                foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
                {
                    servicoNotificacao.GerarNotificacaoEmail(
                        usuario: usuarioNotificar,
                        usuarioGerouNotificacao: null,
                        codigoObjeto: apoliceSeguro.Codigo,
                        URLPagina: "Seguros/ApoliceSeguro",
                        titulo: Localization.Resources.Seguro.Seguro.VencimentoApoliceSeguro,
                        nota: mensagem,
                        icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.atencao,
                        tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta,
                        tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                        unitOfWork: unitOfWork
                    );
                }

                if (configuracaoAlerta.AlertarTransportador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                    {
                        AssuntoEmail = "Vencimento de Apólice de Seguro",
                        CabecalhoMensagem = "Alerta de Vencimento de Apólice de Seguro",
                        Empresa = apoliceSeguro.Empresa,
                        Mensagem = mensagem
                    };

                    servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
                }

                apoliceSeguro.DataUltimoAlerta = dataUltimoAlerta;

                repositorioApoliceSeguro.Atualizar(apoliceSeguro);
            }
        }

        public static void InformarValorSeguroCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguro, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto repositorioApoliceSeguroDesconto = new Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto(unidadeDeTrabalho);

            carga.DescontoSeguro = 0;
            carga.PercentualDescontoSeguro = 0;

            if (apolicesSeguro.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto> desconto = repositorioApoliceSeguroDesconto.BuscarDescontosPorApolices(apolicesSeguro.Select(o => o.Codigo).ToList(), modeloVeicularCarga?.Codigo ?? 0, carga.Filial?.Codigo ?? 0, carga.TipoOperacao?.Codigo ?? 0);

            if (desconto.Count == 0)
                return;

            carga.DescontoSeguro = desconto.Sum(o => o.ValorDesconto);
            carga.PercentualDescontoSeguro = desconto.Sum(o => o.PercentualDesconto);
        }

        public static void SetarDadosSeguroCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftare, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeDeTrabalho);
            Repositorio.Embarcador.Transportadores.TransportadorAverbacao repTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unidadeDeTrabalho);

            if (carga.EmitirCTeComplementar)
                return;

            if (carga.Carregamento != null && (carga.ApoliceSeguroInformadaManualmente || repositorioCarregamentoApolice.ExistePorCarregamento(carga.Carregamento.Codigo)))
                return;

            if (configuracao.NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> listaAverbacaoPedidos = repPedidoAverbacao.BuscarPorCarga(carga.Codigo);
                if (listaAverbacaoPedidos != null && listaAverbacaoPedidos.Count > 0)
                    return;
            }

            serCargaDadosSumarizados.SetarGrupoPrincipalCarga(cargaPedidos, carga, unidadeDeTrabalho);

            bool possuiSeguroFilialEmissora = false;
            if (carga.EmpresaFilialEmissora != null && carga.EmpresaFilialEmissora.UsarTipoOperacaoApolice && !(carga.TipoOperacao?.AverbarDocumentoDaSubcontratacao ?? false))
            {
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> apolicesTransportadores = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

                if (carga.TipoOperacao != null)
                    apolicesTransportadores = repTransportadorAverbacao.BuscarPorTipoOperacao(carga.EmpresaFilialEmissora.Codigo, carga.TipoOperacao.Codigo);

                if (apolicesTransportadores.Count == 0)
                    apolicesTransportadores = repTransportadorAverbacao.BuscarPorTransportador(carga.EmpresaFilialEmissora.Codigo);


                ExcluirAverbacaoTransportador(carga, unidadeDeTrabalho);

                bool abriuTransacao = false;
                if (!unidadeDeTrabalho.IsActiveTransaction())
                {
                    unidadeDeTrabalho.Start();
                    abriuTransacao = true;
                }

                if (apolicesTransportadores != null && apolicesTransportadores.Count > 0)
                {
                    possuiSeguroFilialEmissora = true;
                    for (var i = 0; i < cargaPedidos.Count; i++)
                        VincularTransportadorAverbacaoCargaPedido(carga, cargaPedidos[i], apolicesTransportadores, true, unidadeDeTrabalho, configuracao);
                }

                if (abriuTransacao)
                    unidadeDeTrabalho.CommitChanges();

                if (carga.DadosSumarizados != null)
                    repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }
            else if (!possuiSeguroFilialEmissora && carga.Empresa != null && carga.Empresa.UsarTipoOperacaoApolice)
            {
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> apolicesTransportadores = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

                // Busca as apolices por tipo operacao
                if (carga.TipoOperacao != null)
                    apolicesTransportadores = repTransportadorAverbacao.BuscarPorTipoOperacao(carga.Empresa.Codigo, carga.TipoOperacao.Codigo);

                // Se nao tiver pra empresa/tipo operacao, busca apenas por empresa
                if (apolicesTransportadores.Count == 0)
                    apolicesTransportadores = repTransportadorAverbacao.BuscarPorTransportador(carga.Empresa.Codigo);

                ExcluirAverbacaoTransportador(carga, unidadeDeTrabalho);
                // Vincula pra todos pedidos


                bool abriuTransacao = false;
                if (!unidadeDeTrabalho.IsActiveTransaction())
                {
                    unidadeDeTrabalho.Start();
                    abriuTransacao = true;
                }

                for (var i = 0; i < cargaPedidos.Count; i++)
                    VincularTransportadorAverbacaoCargaPedido(carga, cargaPedidos[i], apolicesTransportadores, false, unidadeDeTrabalho, configuracao);


                if (abriuTransacao)
                    unidadeDeTrabalho.CommitChanges();

                if (carga.DadosSumarizados != null)
                    repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);

            }
            else
            {
                // Busca apoliceis a partir da configuração do sistema

                ExcluirAverbacaoTransportador(carga, unidadeDeTrabalho);

                List<Dominio.Entidades.Cliente> tomadores = new List<Dominio.Entidades.Cliente>();

                bool abriuTransacao = false;
                if (!unidadeDeTrabalho.IsActiveTransaction())
                {
                    unidadeDeTrabalho.Start();
                    abriuTransacao = true;
                }

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoasTomadores = repGrupoPessoas.BuscarPorCodigos((from obj in cargaPedidos where obj.ObterTomador()?.GrupoPessoas != null select obj.ObterTomador().GrupoPessoas.Codigo).Distinct().ToList());

                for (var i = 0; i < cargaPedidos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];
                    List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> apolicesTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>();
                    if (cargaPedido.Carga.TipoOperacao != null && carga.TipoOperacao.ApolicesSeguro != null && (cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao || tipoServicoMultisoftare != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        apolicesTipoOperacao = (from obj in carga.TipoOperacao.ApolicesSeguro
                                                where
                                                   obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date
                                                   && obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date
                                                   && obj.ApoliceSeguro.Ativa == true
                                                select obj).ToList();
                    }

                    // Usar as apolices quando a config emissão for ativa
                    if (apolicesTipoOperacao.Count > 0)
                    {
                        // Remove as apolices existentes
                        VincularTransportadorAverbacaoCargaPedido(carga, cargaPedidos[i], apolicesTipoOperacao, carga.EmpresaFilialEmissora != null, unidadeDeTrabalho, configuracao, tipoServicoMultisoftare);
                    }
                    else
                    {
                        Dominio.Entidades.Cliente tomador = (from obj in tomadores where obj.CPF_CNPJ == cargaPedido.ObterTomador().CPF_CNPJ select obj).FirstOrDefault();

                        if (tomador == null)
                        {
                            tomador = cargaPedido.ObterTomador();
                            if (tomador != null)
                                tomadores.Add(tomador);
                        }

                        if (tomador != null)
                        {
                            // Remove as apolices existentes
                            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolices = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();
                            if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                            {
                                if (tomador.ApolicesSeguro != null)
                                    apolices = (from obj in tomador.ApolicesSeguro where obj.InicioVigencia.Date <= DateTime.Now.Date && obj.FimVigencia >= DateTime.Now.Date && obj.Ativa == true select obj).ToList();
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = (from obj in grupoPessoasTomadores where tomador.GrupoPessoas.Codigo == obj.Codigo select obj).FirstOrDefault(); //repGrupoPessoas.BuscarPorCodigo(tomador.GrupoPessoas.Codigo);

                                if (grupoPessoas.ApolicesSeguro != null)
                                    apolices = (from obj in grupoPessoas.ApolicesSeguro where obj.InicioVigencia.Date <= DateTime.Now.Date && obj.FimVigencia >= DateTime.Now.Date select obj).ToList();
                            }
                            VincularTransportadorAverbacaoCargaPedido(carga, cargaPedido, apolices, carga.EmpresaFilialEmissora != null, unidadeDeTrabalho, configuracao);
                        }
                    }
                }

                if (abriuTransacao)
                    unidadeDeTrabalho.CommitChanges();

                if (carga.DadosSumarizados != null)
                    repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }
        }

        public async static Task SetarDadosSeguroCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftare, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeDeTrabalho);
            Repositorio.Embarcador.Transportadores.TransportadorAverbacao repTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unidadeDeTrabalho);

            if (carga.EmitirCTeComplementar)
                return;

            if (carga.Carregamento != null && (carga.ApoliceSeguroInformadaManualmente || await repositorioCarregamentoApolice.ExistePorCarregamentoAsync(carga.Carregamento.Codigo)))
                return;

            if (configuracao.NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> listaAverbacaoPedidos = repPedidoAverbacao.BuscarPorCarga(carga.Codigo);
                if (listaAverbacaoPedidos != null && listaAverbacaoPedidos.Count > 0)
                    return;
            }

            if (carga.PreCarga != null && carga.CargaAgrupamento != null)
                return;

            serCargaDadosSumarizados.SetarGrupoPrincipalCarga(cargaPedidos, carga, unidadeDeTrabalho);

            bool possuiSeguroFilialEmissora = false;
            if (carga.EmpresaFilialEmissora != null && carga.EmpresaFilialEmissora.UsarTipoOperacaoApolice && !(carga.TipoOperacao?.AverbarDocumentoDaSubcontratacao ?? false))
            {
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> apolicesTransportadores = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

                if (carga.TipoOperacao != null)
                    apolicesTransportadores = await repTransportadorAverbacao.BuscarPorTipoOperacaoAsync(carga.EmpresaFilialEmissora.Codigo, carga.TipoOperacao.Codigo);

                if (apolicesTransportadores.Count == 0)
                    apolicesTransportadores = await repTransportadorAverbacao.BuscarPorTransportadorAsync(carga.EmpresaFilialEmissora.Codigo);


                ExcluirAverbacaoTransportador(carga, unidadeDeTrabalho);

                bool abriuTransacao = false;
                if (!unidadeDeTrabalho.IsActiveTransaction())
                {
                    await unidadeDeTrabalho.StartAsync();
                    abriuTransacao = true;
                }

                if (apolicesTransportadores != null && apolicesTransportadores.Count > 0)
                {
                    possuiSeguroFilialEmissora = true;
                    for (var i = 0; i < cargaPedidos.Count; i++)
                        await VincularTransportadorAverbacaoCargaPedidoAsync(carga, cargaPedidos[i], apolicesTransportadores, true, unidadeDeTrabalho, configuracao);
                }

                if (abriuTransacao)
                    await unidadeDeTrabalho.CommitChangesAsync();

                if (carga.DadosSumarizados != null)
                    await repCargaDadosSumarizados.AtualizarAsync(carga.DadosSumarizados);
            }
            else if (!possuiSeguroFilialEmissora && carga.Empresa != null && carga.Empresa.UsarTipoOperacaoApolice)
            {
                List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> apolicesTransportadores = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao>();

                // Busca as apolices por tipo operacao
                if (carga.TipoOperacao != null)
                    apolicesTransportadores = await repTransportadorAverbacao.BuscarPorTipoOperacaoAsync(carga.Empresa.Codigo, carga.TipoOperacao.Codigo);

                // Se nao tiver pra empresa/tipo operacao, busca apenas por empresa
                if (apolicesTransportadores.Count == 0)
                    apolicesTransportadores = await repTransportadorAverbacao.BuscarPorTransportadorAsync(carga.Empresa.Codigo);

                ExcluirAverbacaoTransportador(carga, unidadeDeTrabalho);
                // Vincula pra todos pedidos


                bool abriuTransacao = false;
                if (!unidadeDeTrabalho.IsActiveTransaction())
                {
                    await unidadeDeTrabalho.StartAsync();
                    abriuTransacao = true;
                }

                for (var i = 0; i < cargaPedidos.Count; i++)
                    await VincularTransportadorAverbacaoCargaPedidoAsync(carga, cargaPedidos[i], apolicesTransportadores, false, unidadeDeTrabalho, configuracao);


                if (abriuTransacao)
                    await unidadeDeTrabalho.CommitChangesAsync();

                if (carga.DadosSumarizados != null)
                    await repCargaDadosSumarizados.AtualizarAsync(carga.DadosSumarizados);

            }
            else
            {
                // Busca apoliceis a partir da configuração do sistema

                ExcluirAverbacaoTransportador(carga, unidadeDeTrabalho);

                List<Dominio.Entidades.Cliente> tomadores = new List<Dominio.Entidades.Cliente>();

                bool abriuTransacao = false;
                if (!unidadeDeTrabalho.IsActiveTransaction())
                {
                    await unidadeDeTrabalho.StartAsync();
                    abriuTransacao = true;
                }

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoasTomadores = await repGrupoPessoas.BuscarPorCodigosAsync((from obj in cargaPedidos where obj.ObterTomador()?.GrupoPessoas != null select obj.ObterTomador().GrupoPessoas.Codigo).Distinct().ToList());

                for (var i = 0; i < cargaPedidos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];
                    List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> apolicesTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>();
                    if (cargaPedido.Carga.TipoOperacao != null && carga.TipoOperacao.ApolicesSeguro != null && (cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao || tipoServicoMultisoftare != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        apolicesTipoOperacao = (from obj in carga.TipoOperacao.ApolicesSeguro
                                                where
                                                   obj.ApoliceSeguro.InicioVigencia.Date <= DateTime.Now.Date
                                                   && obj.ApoliceSeguro.FimVigencia >= DateTime.Now.Date
                                                   && obj.ApoliceSeguro.Ativa
                                                select obj).ToList();
                    }

                    if (apolicesTipoOperacao.Count > 0)
                    {
                        VincularTransportadorAverbacaoCargaPedido(carga, cargaPedidos[i], apolicesTipoOperacao, carga.EmpresaFilialEmissora != null, unidadeDeTrabalho, configuracao, tipoServicoMultisoftare);
                    }
                    else
                    {
                        Dominio.Entidades.Cliente tomador = (from obj in tomadores where obj.CPF_CNPJ == cargaPedido.ObterTomador().CPF_CNPJ select obj).FirstOrDefault();

                        if (tomador == null)
                        {
                            tomador = cargaPedido.ObterTomador();
                            if (tomador != null)
                                tomadores.Add(tomador);
                        }

                        if (tomador != null)
                        {
                            // Remove as apolices existentes
                            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolices = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();
                            if (tomador.NaoUsarConfiguracaoEmissaoGrupo || tomador.GrupoPessoas == null)
                            {
                                if (tomador.ApolicesSeguro != null)
                                    apolices = (from obj in tomador.ApolicesSeguro where obj.InicioVigencia.Date <= DateTime.Now.Date && obj.FimVigencia >= DateTime.Now.Date && obj.Ativa == true select obj).ToList();
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = (from obj in grupoPessoasTomadores where tomador.GrupoPessoas.Codigo == obj.Codigo select obj).FirstOrDefault(); //repGrupoPessoas.BuscarPorCodigo(tomador.GrupoPessoas.Codigo);

                                if (grupoPessoas.ApolicesSeguro != null)
                                    apolices = (from obj in grupoPessoas.ApolicesSeguro where obj.InicioVigencia.Date <= DateTime.Now.Date && obj.FimVigencia >= DateTime.Now.Date select obj).ToList();
                            }
                            VincularTransportadorAverbacaoCargaPedido(carga, cargaPedido, apolices, carga.EmpresaFilialEmissora != null, unidadeDeTrabalho, configuracao);
                        }
                    }
                }

                if (abriuTransacao)
                    await unidadeDeTrabalho.CommitChangesAsync();

                if (carga.DadosSumarizados != null)
                    await repCargaDadosSumarizados.AtualizarAsync(carga.DadosSumarizados);
            }
        }

        public void ExtornarAutorizacoesApolices(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, string MotivoExtorno)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga repApoliceSeguraAutorizacaoCarga = new Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga> apolicesLimitesSeguro = repApoliceSeguraAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga apoliceSeguraAutorizacaoCarga in apolicesLimitesSeguro)
            {
                if (apoliceSeguraAutorizacaoCarga.SituacaoAutorizacaoApolice == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice.Liberada)
                {
                    apoliceSeguraAutorizacaoCarga.SituacaoAutorizacaoApolice = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice.AutorizacaoExtornada;
                    apoliceSeguraAutorizacaoCarga.MotivoExtornoAutorizacao = MotivoExtorno;
                    repApoliceSeguraAutorizacaoCarga.Atualizar(apoliceSeguraAutorizacaoCarga);
                }
                else
                    repApoliceSeguraAutorizacaoCarga.Deletar(apoliceSeguraAutorizacaoCarga);
            }
        }

        public string VeririficarSeEnecessarioAutorizacaoApolice(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, out bool precisaAutorizacao)
        {
            string mensagem = "";
            precisaAutorizacao = false;

            Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga repApoliceSeguraAutorizacaoCarga = new Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga> apolicesLimitesSeguro = repApoliceSeguraAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga apoliceSeguraAutorizacaoCarga in apolicesLimitesSeguro)
            {
                if (apoliceSeguraAutorizacaoCarga.SituacaoAutorizacaoApolice == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice.AgLiberacao)
                {
                    precisaAutorizacao = true;
                    mensagem = "A apólice de seguro " + apoliceSeguraAutorizacaoCarga.ApoliceSeguro.NumeroApolice + " possui um limite de cobertura no valor de R$ " + apoliceSeguraAutorizacaoCarga.ValorLimiteApolice.ToString("n2") + " e o valor da mercadoria na carga de R$ " + apoliceSeguraAutorizacaoCarga.ValorTotalMercadoria.ToString("n2") + " supera o valor limite da apólice, por isso é necessário a autorização do responsável para continuar.";
                }
            }

            return mensagem;
        }

        public void VerificarSeNecessariaAutorizacaoSeguro(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga repApoliceSeguraAutorizacaoCarga = new Repositorio.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Seguro.ApoliceSeguroLimiteValor> apolicesLimiteValor = new List<Dominio.ObjetosDeValor.Embarcador.Seguro.ApoliceSeguroLimiteValor>();

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaosCarga = repApoliceSeguroAverbacao.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = (from obj in apoliceSeguroAverbacaosCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apolice in apoliceSeguroAverbacaos)
                {
                    if (apolice.ApoliceSeguro == null || apolice.ApoliceSeguro.ValorLimiteApolice <= 0)
                        continue;

                    decimal valorAcumulado;
                    if (cargaPedido.TipoContratacaoCarga != TipoContratacaoCarga.Normal && cargaPedido.TipoContratacaoCarga != TipoContratacaoCarga.SVMProprio)
                        valorAcumulado = repositorioPedidoCTeParaSubContratacao.BuscarValorTotalMercadoriaPorCargaPedido(cargaPedido.Codigo);
                    else
                        valorAcumulado = repPedidoXMLNotaFiscal.BuscarTotalPorCargaPedido(cargaPedido.Codigo);

                    Dominio.ObjetosDeValor.Embarcador.Seguro.ApoliceSeguroLimiteValor apoliceLimite = (from obj in apolicesLimiteValor where obj.ApoliceSeguro.Codigo == apolice.Codigo select obj).FirstOrDefault();
                    if (apoliceLimite == null)
                    {
                        apoliceLimite = new Dominio.ObjetosDeValor.Embarcador.Seguro.ApoliceSeguroLimiteValor();
                        apoliceLimite.ApoliceSeguro = apolice.ApoliceSeguro;
                        apoliceLimite.ValorAcumulado = valorAcumulado;
                        apolicesLimiteValor.Add(apoliceLimite);
                    }
                    else
                        apoliceLimite.ValorAcumulado += valorAcumulado;
                }
            }

            apolicesLimiteValor = apolicesLimiteValor
                .GroupBy(obj => obj.ApoliceSeguro)
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Seguro.ApoliceSeguroLimiteValor()
                {
                    ApoliceSeguro = obj.Key,
                    ValorAcumulado = obj.Sum(a => a.ValorAcumulado)
                }).ToList();

            foreach (Dominio.ObjetosDeValor.Embarcador.Seguro.ApoliceSeguroLimiteValor apoliceLimite in apolicesLimiteValor)
            {
                if (apoliceLimite.ApoliceSeguro.ValorLimiteApolice >= apoliceLimite.ValorAcumulado)
                    continue;

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga apoliceSeguraAutorizacaoCarga = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga();
                apoliceSeguraAutorizacaoCarga.ApoliceSeguro = apoliceLimite.ApoliceSeguro;
                apoliceSeguraAutorizacaoCarga.Carga = carga;
                apoliceSeguraAutorizacaoCarga.DataHora = DateTime.Now;
                apoliceSeguraAutorizacaoCarga.ValorTotalMercadoria = apoliceLimite.ValorAcumulado;
                apoliceSeguraAutorizacaoCarga.ValorLimiteApolice = apoliceLimite.ApoliceSeguro.ValorLimiteApolice;
                apoliceSeguraAutorizacaoCarga.SituacaoAutorizacaoApolice = SituacaoAutorizacaoApolice.AgLiberacao;
                repApoliceSeguraAutorizacaoCarga.Inserir(apoliceSeguraAutorizacaoCarga);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao Importar(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas, Repositorio.UnitOfWork unitOfWork, int codigoApolice, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto repositorioDescontos = new Repositorio.Embarcador.Seguros.ApoliceSeguroDesconto(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repositorioApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);

            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice = repositorioApoliceSeguro.BuscarPorCodigo(codigoApolice);
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            int descontosInseridosComSucesso = 0;

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    string retorno = "";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    if (i != 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaModeloVeicular = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeicular" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaFilial = (from obj in linha.Colunas where obj.NomeCampo == "Filial" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaValorDesconto = (from obj in linha.Colunas where obj.NomeCampo == "ValorDesconto" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaPercentualDesconto = (from obj in linha.Colunas where obj.NomeCampo == "PercentualDesconto" select obj).FirstOrDefault();

                        retorno = VerificaValorIncialDasColuna(colunaModeloVeicular?.Valor, colunaTipoOperacao?.Valor, colunaFilial?.Valor, colunaValorDesconto?.Valor, colunaPercentualDesconto?.Valor);

                        Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto apoliceSeguroDesconto = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroDesconto { ApoliceSeguro = apolice };

                        if (string.IsNullOrWhiteSpace(retorno))
                        {
                            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao(((string)colunaModeloVeicular.Valor));
                            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigoIntegracao(((string)colunaFilial.Valor));
                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigoIntegracao(((string)colunaTipoOperacao.Valor));

                            if (apolice == null)
                                retorno = "Apólice de seguro não encontrada";

                            if (filial == null)
                                retorno = "Filial não encontrada";

                            if (tipoOperacao == null)
                                retorno = "Tipo Operação não encontrada";

                            if (modeloVeicular == null)
                                retorno = "Modelo veicular não encontrado";

                            decimal valorDesconto = ((string)colunaValorDesconto?.Valor ?? "0").ToDecimal();
                            decimal percentualDesconto = ((string)colunaPercentualDesconto?.Valor ?? "0").ToDecimal();

                            if (repositorioDescontos.ExisteDescontoSeguro(modeloVeicular?.CodigoIntegracao ?? string.Empty, filial?.CodigoFilialEmbarcador ?? string.Empty, tipoOperacao?.CodigoIntegracao ?? string.Empty))
                                retorno = "Desconto já existe";

                            if (string.IsNullOrWhiteSpace(retorno))
                            {
                                apoliceSeguroDesconto.ValorDesconto = valorDesconto;
                                apoliceSeguroDesconto.PercentualDesconto = percentualDesconto;

                                if (apoliceSeguroDesconto.ValorDesconto <= 0 && apoliceSeguroDesconto.PercentualDesconto <= 0)
                                    retorno = "É obrigatório informar o valor do desconto do seguro.";

                                apoliceSeguroDesconto.ModeloVeicularCarga = modeloVeicular;
                                apoliceSeguroDesconto.Filial = filial;
                                apoliceSeguroDesconto.TipoOperacao = tipoOperacao;
                                repositorioDescontos.Inserir(apoliceSeguroDesconto);
                            }
                        }
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = false, mensagemFalha = "Registro ignorado" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);
                        unitOfWork.CommitChanges();
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                    }
                    else
                    {
                        descontosInseridosComSucesso++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);
                        unitOfWork.CommitChanges();
                    }
                }
                catch (ServicoException excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count;
            retornoImportacao.Importados = descontosInseridosComSucesso;

            if (descontosInseridosComSucesso > 0)
                Servicos.Auditoria.Auditoria.Auditar(auditado, apolice, null, "Descontos adicionados via importação", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

            return retornoImportacao;
        }

        public void ValidarRastreadorVeiculoEValorSeguroApolice(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguro repositorioApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            DateTime data = carregamento?.DataCriacao ?? carga?.DataCriacaoCarga ?? DateTime.Today;

            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repositorioApoliceSeguro.BuscarPrimeiraApoliceVigenteEmbarcador(data);

            if (apoliceSeguro == null || apoliceSeguro.ValorLimiteApolice == 0)
                return;

            decimal valorValidar;

            Dominio.Entidades.Veiculo veiculo;

            if (carregamento != null)
            {
                valorValidar = repositorioCarregamentoPedido.BuscarValorTotalMercadoriaPorCarregamento(carregamento.Codigo);
                veiculo = carregamento.Veiculo;
            }
            else
            {
                valorValidar = (carga?.DadosSumarizados?.ValorTotalMercadoriaPedidos != 0) ? (carga?.DadosSumarizados?.ValorTotalMercadoriaPedidos ?? 0) : carga?.DadosSumarizados?.ValorTotalProdutos ?? 0;
                veiculo = carga?.Veiculo ?? null;
            }

            if (veiculo == null)
                return;

            if (veiculo?.PossuiRastreador ?? false)
                return;

            if (veiculo == null)
                return;

            if (valorValidar >= apoliceSeguro.ValorLimiteApolice)
                throw new ServicoException(string.Format(Localization.Resources.Cargas.MontagemCargaMapa.VeiculoNaoPossuiRastreador, veiculo?.Placa_Formatada ?? string.Empty));
        }

        #endregion

        #region Métodos Privados

        private static void VincularTransportadorAverbacaoCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolices, bool seguroFilialEmissora, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unidadeDeTrabalho);
            // Filtra as apolices que possuem averbacoes
            //apolices = (from o in apolices where o.SeguradoraAverbacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido select o).ToList();

            // Cria a lista nova de seguros
            for (var i = 0; i < apolices.Count; i++)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao();

                seguro.ApoliceSeguro = apolices[i];
                seguro.CargaPedido = cargaPedido;
                seguro.SeguroFilialEmissora = seguroFilialEmissora;
                seguro.Desconto = null;

                repApoliceSeguroAverbacao.Inserir(seguro);
            }

            if (cargaPedido.Carga.DadosSumarizados != null)
            {
                if (apolices.Any(obj => obj.SeguradoraAverbacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido))
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = true;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = AverbarMDFe(carga.Codigo, configuracao, unidadeDeTrabalho);
                }
                else
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = false;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = false;
                }
            }
        }

        private static void VincularTransportadorAverbacaoCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> apolicesTipoOperacao, bool seguroFilialEmissora, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftare)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unidadeDeTrabalho);

            for (var i = 0; i < apolicesTipoOperacao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao();

                seguro.ApoliceSeguro = apolicesTipoOperacao[i].ApoliceSeguro;
                seguro.CargaPedido = cargaPedido;
                seguro.SeguroFilialEmissora = seguroFilialEmissora;
                seguro.Desconto = apolicesTipoOperacao[i].Desconto;

                repApoliceSeguroAverbacao.Inserir(seguro);
            }

            if (apolicesTipoOperacao.Count > 0)
                Servicos.Embarcador.Seguro.Seguro.InformarValorSeguroCarga(carga, apolicesTipoOperacao.Select(o => o.ApoliceSeguro).Distinct().ToList(), carga.ModeloVeicularCarga, unidadeDeTrabalho);

            if (carga.DadosSumarizados != null)
            {
                if (apolicesTipoOperacao.Any(obj => obj.ApoliceSeguro.SeguradoraAverbacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido))
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = true;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = AverbarMDFe(carga.Codigo, configuracao, unidadeDeTrabalho);
                }
                else
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = false;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = false;
                }
            }
        }

        private static void VincularTransportadorAverbacaoCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> apolicesTransportadores, bool apoliceFilialEmissora, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unidadeDeTrabalho);

            // Filtra as apolices que possuem averbacoes
            apolicesTransportadores = (from o in apolicesTransportadores where o.ApoliceSeguro != null select o).ToList();
            // Cria a lista nova de seguros
            for (var i = 0; i < apolicesTransportadores.Count; i++)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao();

                seguro.ApoliceSeguro = apolicesTransportadores[i].ApoliceSeguro;
                seguro.CargaPedido = cargaPedido;
                seguro.SeguroFilialEmissora = apoliceFilialEmissora;
                seguro.Desconto = apolicesTransportadores[i].Desconto;

                repApoliceSeguroAverbacao.Inserir(seguro);
            }

            if (carga.DadosSumarizados != null)
            {
                if (apolicesTransportadores.Any(obj => obj.ApoliceSeguro.SeguradoraAverbacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido))
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = true;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = AverbarMDFe(carga.Codigo, configuracao, unidadeDeTrabalho);
                }
                else
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = false;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = false;
                }
            }
        }

        private async static Task VincularTransportadorAverbacaoCargaPedidoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> apolicesTransportadores, bool apoliceFilialEmissora, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unidadeDeTrabalho);

            // Filtra as apolices que possuem averbacoes
            apolicesTransportadores = (from o in apolicesTransportadores where o.ApoliceSeguro != null select o).ToList();
            // Cria a lista nova de seguros
            for (var i = 0; i < apolicesTransportadores.Count; i++)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao();

                seguro.ApoliceSeguro = apolicesTransportadores[i].ApoliceSeguro;
                seguro.CargaPedido = cargaPedido;
                seguro.SeguroFilialEmissora = apoliceFilialEmissora;
                seguro.Desconto = apolicesTransportadores[i].Desconto;

                await repApoliceSeguroAverbacao.InserirAsync(seguro);
            }

            if (carga.DadosSumarizados != null)
            {
                if (apolicesTransportadores.Any(obj => obj.ApoliceSeguro.SeguradoraAverbacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido))
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = true;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = await AverbarMDFeAsync(carga.Codigo, configuracao, unidadeDeTrabalho);
                }
                else
                {
                    carga.DadosSumarizados.PossuiAverbacaoCTe = false;
                    carga.DadosSumarizados.PossuiAverbacaoMDFe = false;
                }
            }
        }

        private static void ExcluirAverbacaoTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.TratarErro($"ExcluirAverbacaoTransportador - Removido CarCodigo: {carga.Codigo} - Embarcador: {carga.CodigoCargaEmbarcador}", "RemoveAverbacao");

            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unidadeDeTrabalho);

            repApoliceSeguroAverbacao.DeletarPorCarga(carga.Codigo);
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private static string VerificaValorIncialDasColuna(string codigoModeloVeicular, string codigoTipoOperacao, string codigoFilial, string valorDesconto, string percentualDesconto)
        {
            if (string.IsNullOrEmpty(codigoModeloVeicular))
                return "Código Modelo veicular não informado";

            if (string.IsNullOrEmpty(codigoTipoOperacao))
                return "Código Tipo de Operação não informado";

            if (string.IsNullOrEmpty(codigoFilial))
                return "Código Filial não informado";

            if (string.IsNullOrEmpty(valorDesconto) && string.IsNullOrEmpty(percentualDesconto))
                return "Precisa informar Valor Desconto ou Percentual Desconto";

            if (valorDesconto.ToDecimal() > 0 && percentualDesconto.ToDecimal() > 0)
                return "É permitido informar somente o percentual ou o valor do desconto do seguro, nunca os dois.";

            return null;
        }

        private static bool AverbarMDFe(int codigoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repositorioConfiguracaoCarga.BuscarPrimeiroRegistro();
            bool possuiCIOT = repositorioCargaCIOT.ExisteCIOTPorCarga(codigoCarga);

            return ((configuracao.AverbarMDFe && !configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT) || (configuracao.AverbarMDFe && configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT && possuiCIOT));
        }

        private async static Task<bool> AverbarMDFeAsync(int codigoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = await repositorioConfiguracaoCarga.BuscarPrimeiroRegistroAsync();
            bool possuiCIOT = await repositorioCargaCIOT.ExisteCIOTPorCargaAsync(codigoCarga);

            return ((configuracao.AverbarMDFe && !configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT) || (configuracao.AverbarMDFe && configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT && possuiCIOT));
        }

        #endregion
    }
}