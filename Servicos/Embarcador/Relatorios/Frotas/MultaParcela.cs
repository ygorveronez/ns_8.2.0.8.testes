using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class MultaParcela : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela, Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.Infracao _repositorioInfracao;

        #endregion

        #region Construtores

        public MultaParcela(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
        }

        public MultaParcela(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, 
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioInfracao.ConsultarRelatorioMultaParcelaAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioInfracao.ConsultarRelatorioMultaParcela(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioInfracao.ContarConsultaRelatorioMultaParcela(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/MultaParcela";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Frota.TipoInfracao repTipoInfracao = new Repositorio.Embarcador.Frota.TipoInfracao(_unitOfWork);

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo);

                parametros.Add(new Parametro("Veiculo", veiculo.Placa, true));
            }
            else
                parametros.Add(new Parametro("Veiculo", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoTipoInfracao))
                parametros.Add(new Parametro("TipoTipoInfracao", ((TipoInfracaoTransito)filtrosPesquisa.TipoTipoInfracao.ToInt()).ObterDescricao(), true));
            else
                parametros.Add(new Parametro("TipoTipoInfracao", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NivelInfracao))
                parametros.Add(new Parametro("NivelInfracao", ((NivelInfracaoTransito)filtrosPesquisa.NivelInfracao.ToInt()).ObterDescricao(), true));
            else
                parametros.Add(new Parametro("NivelInfracao", false));

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroAtuacao))
                parametros.Add(new Parametro("NumeroInfracao", filtrosPesquisa.NumeroAtuacao, true));
            else
                parametros.Add(new Parametro("NumeroInfracao", false));

            if (filtrosPesquisa.NumeroMulta > 0)
                parametros.Add(new Parametro("NumeroMulta", filtrosPesquisa.NumeroMulta.ToString("n0"), true));
            else
                parametros.Add(new Parametro("NumeroMulta", false));

            if ((int)filtrosPesquisa.PagoPor > 0)
                parametros.Add(new Parametro("PagoPor", filtrosPesquisa.PagoPor.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("PagoPor", false));

            if (filtrosPesquisa.CodigoCidade > 0)
            {
                Dominio.Entidades.Localidade obj = repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoCidade);

                parametros.Add(new Parametro("Cidade", obj.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Cidade", false));

            if (filtrosPesquisa.CodigosTipoInfracoes?.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Frota.TipoInfracao> obj = repTipoInfracao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoInfracoes);

                parametros.Add(new Parametro("TipoInfracao", string.Join(", ", obj.Select(o => o.Descricao))));
            }
            else
                parametros.Add(new Parametro("TipoInfracao", false));

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                Dominio.Entidades.Usuario obj = repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista);

                parametros.Add(new Parametro("Motorista", obj.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Motorista", false));

            if (filtrosPesquisa.CnpjPessoa > 0)
            {
                Dominio.Entidades.Cliente obj = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoa);

                parametros.Add(new Parametro("Pessoa", obj.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Pessoa", false));

            parametros.Add(new Parametro("DataVencimento", filtrosPesquisa.DataVencimentoInicial, filtrosPesquisa.DataVencimentoFinal));

            if (filtrosPesquisa.CodigoTitulo > 0)
                parametros.Add(new Parametro("Titulo", filtrosPesquisa.CodigoTitulo.ToString("n0"), true));
            else
                parametros.Add(new Parametro("Titulo", false));

            if ((int)filtrosPesquisa.StatusMulta > 0)
                parametros.Add(new Parametro("StatusMulta", filtrosPesquisa.StatusMulta.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("StatusMulta", false));

            parametros.Add(new Parametro("DataVencimentoPagar", filtrosPesquisa.DataVencimentoInicialPagar, filtrosPesquisa.DataVencimentoFinalPagar));

            if (filtrosPesquisa.CnpjFornecedorPagar > 0)
            {
                Dominio.Entidades.Cliente obj = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjFornecedorPagar);

                parametros.Add(new Parametro("FornecedorPagar", obj.Descricao, true));
            }
            else
                parametros.Add(new Parametro("FornecedorPagar", false));

            parametros.Add(new Parametro("DataLimiteInfracao", filtrosPesquisa.DataLimiteInicial, filtrosPesquisa.DataLimiteFinal, false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoOcorrenciaInfracao))
                parametros.Add(new Parametro("TipoOcorrenciaInfracao", ((TipoOcorrenciaInfracao)filtrosPesquisa.TipoOcorrenciaInfracao.ToInt()).ObterDescricao(), true));
            else
                parametros.Add(new Parametro("TipoOcorrenciaInfracao", false));

            parametros.Add(new Parametro("DataLancamento", filtrosPesquisa.DataLancamentoInicial, filtrosPesquisa.DataLancamentoFinal));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Descricao"))
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
