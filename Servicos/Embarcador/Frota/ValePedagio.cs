using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Frota
{
    public sealed class ValePedagio
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio ObterConfiguracaoPorCriterios(int codigoTipoOperacao, int codigoFilial, TipoIntegracao tipoIntegracao, string cnpjTransportador = "")
        {
            Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> configuracoesFiltradas = new List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();
            List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> configuracoes = repValePedagio.BuscarPorTipoIntegracaoTipoOperacaoEFilial(tipoIntegracao, codigoTipoOperacao, codigoFilial, cnpjTransportador);

            // 1 - Com Tipo Operação e Filial
            configuracoesFiltradas = (from o in configuracoes where o.TipoOperacao != null && o.Filial != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 2 - Com Tipo Operação
            configuracoesFiltradas = (from o in configuracoes where o.TipoOperacao != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 3 - Com Filial
            configuracoesFiltradas = (from o in configuracoes where o.Filial != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            return configuracoes.FirstOrDefault();
        }

        private Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio ObterConfiguracaoPorCriteriosTMS(int codigoTipoOperacao, int codigoGrupoPessoas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> configuracoesFiltradas = new List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio>();
            List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> configuracoes = repValePedagio.BuscarPorTipoIntegracaoTipoOperacaoEGrupoPessoas(tipoIntegracao, codigoTipoOperacao, codigoGrupoPessoas);

            // 1 - Com Tipo Operação e Grupo Pessoas
            configuracoesFiltradas = (from o in configuracoes where o.TipoOperacao != null && o.GrupoPessoas != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 2 - Com Tipo Operação
            configuracoesFiltradas = (from o in configuracoes where o.TipoOperacao != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            // 3 - Com Grupo Pessoas
            configuracoesFiltradas = (from o in configuracoes where o.GrupoPessoas != null select o).ToList();
            if (configuracoesFiltradas.Count() > 0)
                return configuracoesFiltradas.FirstOrDefault();

            return configuracoes.FirstOrDefault();
        }

        private Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio ObterPrimeiraConfiguracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(_unitOfWork);

            return repValePedagio.BuscarPrimeiraConfiguracao(tipoIntegracao);
        }

        private bool PermitirGerarValePedagioFreteProprio(bool freteDeTerceiro, Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao)
        {
            return !(!freteDeTerceiro && !(configuracao?.PermitirGerarValePedagioVeiculoProprio ?? false));
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans ObterIntegracaoDBTrans(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = (carga.FilialCargaAgrupadaValePedagio?.Codigo ?? carga.Filial?.Codigo) ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.DBTrans);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.DBTrans);

            return configuracao?.IntegracaoDBTrans;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem ObterIntegracaoPagbem(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, bool freteDeTerceiro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = tipoOperacao?.Codigo ?? 0;
            int codigoFilial = filial?.Codigo ?? 0;
            int codigoGrupoPessoas = grupoPessoas?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.PagBem);
                if (!PermitirGerarValePedagioFreteProprio(freteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.PagBem);

            return configuracao?.IntegracaoPagbem;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom ObterIntegracaoRepom(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.Repom);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.Repom);

            return configuracao?.IntegracaoRepom;
        }

        public bool UtilizarConsultaValePedagioIntegracaoRepom(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.Repom);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return false;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.Repom);

            return configuracao.ConsultarValorPedagioAntesAutorizarEmissao;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard ObterIntegracaoPamcard(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.Pamcard);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.Pamcard);

            return configuracao?.IntegracaoPamcard;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget ObterIntegracaoTarget(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.Target);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.Target);

            return configuracao?.IntegracaoTarget;
        }

        public bool UtilizarConsultaValePedagioIntegracaoTarget(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.Target);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return false;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.Target);

            return configuracao.ConsultarValorPedagioAntesAutorizarEmissao;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar ObterIntegracaoSemParar(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.SemParar);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.SemParar);

            return configuracao?.IntegracaoSemParar;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar ObterIntegracaoSemPararParaAutenticacao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = 0;
            int codigoFilial = 0;
            int codigoGrupoPessoas = 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.SemParar);
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.SemParar);

            if (configuracao == null)
                configuracao = ObterPrimeiraConfiguracao(TipoIntegracao.SemParar);

            return configuracao?.IntegracaoSemParar;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP ObterIntegracaoQualP(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.QualP);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.QualP);

            return configuracao?.IntegracaoQualP;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete ObterIntegracaoEFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.EFrete);

                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);

                string cnpjTransportador = repositorioConfiguracaoGeralCarga.ExisteConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete()
                    ? carga.Empresa?.CNPJ
                    : string.Empty;

                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.EFrete, cnpjTransportador);
            }

            return configuracao?.IntegracaoEFrete;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo ObterIntegracaoNDDCargo(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.NDDCargo);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.NDDCargo);

            return configuracao?.IntegracaoNDDCargo;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete ObterIntegracaoEFrete()
        {
            return ObterConfiguracaoPorCriterios(0, 0, TipoIntegracao.EFrete)?.IntegracaoEFrete;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio ObterIntegracaoExtratta(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.Extratta);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.Extratta);

            return configuracao?.IntegracaoExtratta;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.ConfiguracaoIntegracao ObterIntegracaoDigitalCom(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom repIntegracaoDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom(_unitOfWork);

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.ConfiguracaoIntegracao configuracaoRetorno = null;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.DigitalCom);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.DigitalCom);

            if (configuracao != null)
            {
                configuracaoRetorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.ConfiguracaoIntegracao();
                configuracaoRetorno.IntegracaoDigitalComValePedagio = configuracao.IntegracaoDigitalCom;
                configuracaoRetorno.IntegracaoDigitalCom = repIntegracaoDigitalCom.Buscar();
            }

            return configuracaoRetorno;
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio ObterIntegracaoAmbipar(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? 0;

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracao;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracao = ObterConfiguracaoPorCriteriosTMS(codigoTipoOperacao, codigoGrupoPessoas, TipoIntegracao.Ambipar);
                if (!PermitirGerarValePedagioFreteProprio(carga.FreteDeTerceiro, configuracao))
                    return null;
            }
            else
                configuracao = ObterConfiguracaoPorCriterios(codigoTipoOperacao, codigoFilial, TipoIntegracao.Ambipar);

            return configuracao?.IntegracaoAmbipar;
        }

        public void AdicionarCargaValePedagioParaMDFe(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Cliente fornecedorValePedagio, Repositorio.UnitOfWork unitOfWork, bool vincularCargaValePedagioIntegracao = true)
        {
            if (fornecedorValePedagio == null)
                return;

            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagioParaMDFe = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagioParaMDFe = repCargaValePedagioParaMDFe.BuscarPorCodigo(cargaValePedagio.Carga.Codigo);

            if (cargaValePedagioParaMDFe == null)
                cargaValePedagioParaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaValePedagio();

            cargaValePedagioParaMDFe.Carga = cargaValePedagio.Carga;
            cargaValePedagioParaMDFe.Fornecedor = fornecedorValePedagio;
            cargaValePedagioParaMDFe.NumeroComprovante = !string.IsNullOrWhiteSpace(cargaValePedagio.CodigoEmissaoValePedagioANTT) ? cargaValePedagio.CodigoEmissaoValePedagioANTT : cargaValePedagio.NumeroValePedagio;
            cargaValePedagioParaMDFe.Valor = cargaValePedagio.ValorValePedagio;
            cargaValePedagioParaMDFe.Responsavel = cargaValePedagio.Carga.Pedidos.FirstOrDefault().Pedido.ObterTomador();
            cargaValePedagioParaMDFe.QuantidadeEixos = cargaValePedagio.QuantidadeEixos;
            cargaValePedagioParaMDFe.TipoCompra = cargaValePedagio.TipoCompra;

            if (vincularCargaValePedagioIntegracao)
                cargaValePedagioParaMDFe.CargaIntegracaoValePedagio = cargaValePedagio;

            if (cargaValePedagioParaMDFe.Responsavel == null)
                cargaValePedagioParaMDFe.Responsavel = cargaValePedagio.Carga.Pedidos.FirstOrDefault().Pedido.Remetente;

            if (cargaValePedagioParaMDFe.Codigo > 0)
                repCargaValePedagioParaMDFe.Atualizar(cargaValePedagioParaMDFe);
            else
                repCargaValePedagioParaMDFe.Inserir(cargaValePedagioParaMDFe);
        }

        #endregion
    }
}
