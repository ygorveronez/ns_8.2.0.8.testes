using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Ocorrencias
{
    public class OcorrenciaEntrega : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega, Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaEntrega>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Ocorrencias.CargaOcorrencia _repositorioCargaOcorrencia;

        #endregion

        #region Construtores

        public OcorrenciaEntrega(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {

            _repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaEntrega> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaOcorrencia.ConsultarRelatorioEntrega(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaOcorrencia.ContarConsultaRelatorioEntrega(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Ocorrencias/OcorrenciaEntrega";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EtapaEstadia", ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("OcorrenciaEstadia", ""));

            if (filtrosPesquisa.NumeroOcorrenciaInicial > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrenciaInicial", filtrosPesquisa.NumeroOcorrenciaInicial.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrenciaInicial", false));

            if (filtrosPesquisa.NumeroOcorrenciaFinal > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrenciaFinal", filtrosPesquisa.NumeroOcorrenciaFinal.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrenciaFinal", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrenciaCliente", false));

            if (filtrosPesquisa.SituacoesOcorrencia?.Count > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", filtrosPesquisa.SituacoesOcorrencia.Select(o => o.ObterDescricao())), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

            if (filtrosPesquisa.SituacoesCancelamento?.Count > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCancelamento", string.Join(", ", filtrosPesquisa.SituacoesCancelamento.Select(o => o.ObterDescricao())), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCancelamento", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa.CodigoCargaEmbarcador, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", false));

            if (filtrosPesquisa.CodigoRecebedor > 0)
            {
                Dominio.Entidades.Usuario recebedor = repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoRecebedor);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", recebedor.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Operador", false));

            if (filtrosPesquisa.CodigosGrupoOcorrencia.Count > 0)
            {
                Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repositorioGrupoOcorrencia = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe> grupoOcorrencia = repositorioGrupoOcorrencia.BuscarPorCodigos(filtrosPesquisa.CodigosGrupoOcorrencia, false);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoOcorrencia", string.Join(", ", grupoOcorrencia.Select(o => o.Descricao)), true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoOcorrencia", false));
            }

            if (filtrosPesquisa.CodigosOcorrencia.Count > 0)
            {
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrencia = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigos(filtrosPesquisa.CodigosOcorrencia);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ocorrencia", string.Join(", ", tiposDeOcorrencia.Select(o => o.Descricao)), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ocorrencia", false));

            if (filtrosPesquisa.CodigoSolicitante.Count > 0)
            {
                List<Dominio.Entidades.Usuario> solicitante = repositorioUsuario.BuscarPorCodigos(filtrosPesquisa.CodigoSolicitante);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Solicitante", string.Join(", ", solicitante.Select(o => o.Nome)), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Solicitante", false));

            if (filtrosPesquisa.CodigoTransportadorChamado > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportadorChamado);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TransportadorChamado", transportador.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TransportadorChamado", false));


            if (filtrosPesquisa.NumeroNotaFiscal > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNotaFiscal", filtrosPesquisa.NumeroNotaFiscal.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNotaFiscal", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ResponsavelChamado", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataSolicitacaoInicial", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataSolicitacaoFinal", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataOcorrenciaInicial", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataOcorrenciaFinal", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCancelamentoInicial", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCancelamentoFinal", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTeOriginal", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCTeGerado", false));



            if (filtrosPesquisa.CodigosFilial.Count > 0)
            {
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", string.Join(", ", filiais.Select(f => f.Descricao)), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repositorioGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", false));

            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente pessoa = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjPessoa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorInicial", false));


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorFinal", false));


            if (filtrosPesquisa.TiposOperacaoCarga.Count > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.TiposOperacaoCarga);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacaoCarga", string.Join(", ", tiposOperacao.Select(o => o.Descricao)), true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacaoCarga", false));
            }

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCreditoDebito", string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDocumento", string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CNPJTransportadora")
                return "CNPJEmpresa";

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoCancelamento")
                return "SituacaoCancelamento";

            if (propriedadeOrdenarOuAgrupar == "Operador")
                return "Operadores";

            return propriedadeOrdenarOuAgrupar;
        }

    }
}
