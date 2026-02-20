using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using AdminMultisoftware.Dominio.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.MDFes
{
    public class MDFes : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio, Dominio.Relatorios.Embarcador.DataSource.MDFe.Mdfe>
    {
        #region Atributos

        private readonly Repositorio.ManifestoEletronicoDeDocumentosFiscais _repositorioMDFe;

        #endregion

        #region Construtores

        public MDFes(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToLis metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.MDFe.Mdfe> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMDFe.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMDFe.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Mdfe/Mdfes";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            if (filtrosPesquisa.DataAutorizacaoInicial.HasValue || filtrosPesquisa.DataAutorizacaoLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataAutorizacaoInicial.HasValue ? $"{filtrosPesquisa.DataAutorizacaoInicial.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataAutorizacaoLimite.HasValue ? $"até {filtrosPesquisa.DataAutorizacaoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("PeriodoAutorizacao", periodo, true));
            }
            else
                parametros.Add(new Parametro("PeriodoAutorizacao", false));

            if (filtrosPesquisa.DataCancelamentoInicial.HasValue || filtrosPesquisa.DataCancelamentoLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataCancelamentoInicial.HasValue ? $"{filtrosPesquisa.DataCancelamentoInicial.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataCancelamentoLimite.HasValue ? $"até {filtrosPesquisa.DataCancelamentoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("PeriodoCancelamento", periodo, true));
            }
            else
                parametros.Add(new Parametro("PeriodoCancelamento", false));

            if (filtrosPesquisa.DataEmissaoInicial.HasValue || filtrosPesquisa.DataEmissaoLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataEmissaoInicial.HasValue ? $"{filtrosPesquisa.DataEmissaoInicial.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataEmissaoLimite.HasValue ? $"até {filtrosPesquisa.DataEmissaoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("PeriodoEmissao", periodo, true));
            }
            else
                parametros.Add(new Parametro("PeriodoEmissao", false));

            if (filtrosPesquisa.DataEncerramentoInicial.HasValue || filtrosPesquisa.DataEncerramentoLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataEncerramentoInicial.HasValue ? $"{filtrosPesquisa.DataEncerramentoInicial.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataEncerramentoLimite.HasValue ? $"até {filtrosPesquisa.DataEncerramentoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("PeriodoEncerramento", periodo, true));
            }
            else
                parametros.Add(new Parametro("PeriodoEncerramento", false));

            if ((filtrosPesquisa.NumeroInicial > 0) || (filtrosPesquisa.NumeroLimite > 0))
            {
                string intervaloValor = $"{((filtrosPesquisa.NumeroInicial > 0) ? $"{filtrosPesquisa.NumeroInicial} " : "")}{((filtrosPesquisa.NumeroLimite > 0) ? $"até {filtrosPesquisa.NumeroLimite}" : "")}";

                parametros.Add(new Parametro("IntervaloNumero", intervaloValor, true));
            }
            else
                parametros.Add(new Parametro("IntervaloNumero", false));


            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa);

                if (_tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                {
                    parametros.Add(new Parametro("Empresa", empresa.Descricao, true));
                    parametros.Add(new Parametro("Transportador", false));
                }
                else
                {
                    parametros.Add(new Parametro("Empresa", false));
                    parametros.Add(new Parametro("Transportador", empresa.Descricao, true));
                }
            }
            else
            {
                parametros.Add(new Parametro("Empresa", false));
                parametros.Add(new Parametro("Transportador", false));
            }

            if (filtrosPesquisa.CodigoSerie > 0)
            {
                Repositorio.EmpresaSerie repositorioEmpresaSerie = new Repositorio.EmpresaSerie(_unitOfWork);
                Dominio.Entidades.EmpresaSerie empresaSerie = repositorioEmpresaSerie.BuscarPorCodigo(filtrosPesquisa.CodigoSerie);

                parametros.Add(new Parametro("Serie", empresaSerie.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Serie", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfMotorista))
                parametros.Add(new Parametro("Motorista", filtrosPesquisa.CpfMotorista, true));
            else
                parametros.Add(new Parametro("Motorista", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoCarregamento))
            {
                Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork);
                Dominio.Entidades.Estado estadoCarregamento = repositorioEstado.BuscarPorSigla(filtrosPesquisa.EstadoCarregamento);

                parametros.Add(new Parametro("EstadoCarregamento", estadoCarregamento.Nome, true));
            }
            else
                parametros.Add(new Parametro("EstadoCarregamento", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDescarregamento))
            {
                Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork);
                Dominio.Entidades.Estado estadoDescarregamento = repositorioEstado.BuscarPorSigla(filtrosPesquisa.EstadoDescarregamento);

                parametros.Add(new Parametro("EstadoDescarregamento", estadoDescarregamento.Nome, true));
            }
            else
                parametros.Add(new Parametro("EstadoDescarregamento", false));

            if (filtrosPesquisa.ListaStatusMdfe?.Count > 0)
            {
                List<string> listaStatusDescricao = new List<string>();

                foreach (StatusMDFe status in filtrosPesquisa.ListaStatusMdfe)
                    listaStatusDescricao.Add(status.ObterDescricao());

                parametros.Add(new Parametro("ListaStatusMdfe", string.Join(", ", listaStatusDescricao), true));
            }
            else
                parametros.Add(new Parametro("ListaStatusMdfe", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo))
                parametros.Add(new Parametro("Veiculo", filtrosPesquisa.PlacaVeiculo, true));
            else
                parametros.Add(new Parametro("Veiculo", false));

            if (filtrosPesquisa.TipoOperacao > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.TipoOperacao);
                parametros.Add(new Parametro("TipoOperacao", tipoOperacao.Descricao, true));
            }
            else
                parametros.Add(new Parametro("TipoOperacao", false));

            if (filtrosPesquisa.NumeroCTe > 0)
                parametros.Add(new Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe.ToString(), true));
            else
                parametros.Add(new Parametro("NumeroCTe", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga, true));
            else
                parametros.Add(new Parametro("NumeroCarga", false));

            if (filtrosPesquisa.ExibirCTes)
                parametros.Add(new Parametro("ExibirCTes", "Sim", true));
            else
                parametros.Add(new Parametro("ExibirCTes", false));

            if (filtrosPesquisa.MDFeVinculadoACarga.HasValue)
                parametros.Add(new Parametro("MDFeVinculadoACarga", filtrosPesquisa.MDFeVinculadoACarga.Value ? "Sim" : "Não", true));
            else
                parametros.Add(new Parametro("MDFeVinculadoACarga", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DataAutorizacaoFormatada")
                return "DataAutorizacao";

            if (propriedadeOrdenarOuAgrupar == "DataCancelamentoFormatada")
                return "DataCancelamento";

            if (propriedadeOrdenarOuAgrupar == "DataEncerramentoFormatada")
                return "DataEncerramento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}