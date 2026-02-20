using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class CTeEmitido : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido, Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioConhecimentoDeTransporteEletronico;

        #endregion

        #region Construtores

        public CTeEmitido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }
        public CTeEmitido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, cancellationToken);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CpfCnpjEmbarcador > 0 ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjEmbarcador) : null;
            string cpfCnpjEmbarcadorSemFormato = cliente != null ? cliente.CPF_CNPJ_SemFormato : null;

            List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> listaCTesEmitidosPorEmbarcador = _repositorioConhecimentoDeTransporteEletronico.RelatorioCTesEmitidosPorEmbarcador(
                                                                                                                                                                        codigoEmpresaPai: filtrosPesquisa.CodigoEmpresaPai,
                                                                                                                                                                        codigoEmpresa: filtrosPesquisa.CodigoTransportador,
                                                                                                                                                                        cpfCnpjEmbarcador: cpfCnpjEmbarcadorSemFormato,
                                                                                                                                                                        dataAutorizacaoInicial: filtrosPesquisa.DataAutorizacaoInicial,
                                                                                                                                                                        dataAutorizacaoFinal: filtrosPesquisa.DataAutorizacaoFinal,
                                                                                                                                                                        dataEmissaoInicial: filtrosPesquisa.DataEmissaoInicial,
                                                                                                                                                                        dataEmissaoFinal: filtrosPesquisa.DataEmissaoFinal,
                                                                                                                                                                        propGrupo: parametrosConsulta.PropriedadeAgrupar,
                                                                                                                                                                        dirOrdenacaoGrupo: parametrosConsulta.DirecaoAgrupar,
                                                                                                                                                                        propOrdenacao: parametrosConsulta.PropriedadeOrdenar,
                                                                                                                                                                        dirOrdenacao: parametrosConsulta.DirecaoOrdenar,
                                                                                                                                                                        inicioRegistros: parametrosConsulta.InicioRegistros,
                                                                                                                                                                        maximoRegistros: parametrosConsulta.LimiteRegistros,
                                                                                                                                                                        paginar: false).ToList();

            foreach (var cteEmitido in listaCTesEmitidosPorEmbarcador)
            {
                cteEmitido.Status = !string.IsNullOrWhiteSpace(cteEmitido.Status) ? cteEmitido.Status : "";
                cteEmitido.Log = !string.IsNullOrWhiteSpace(cteEmitido.Log) ? cteEmitido.Log : "";
                cteEmitido.CPFCNPJRemetente = !string.IsNullOrWhiteSpace(cteEmitido.CPFCNPJRemetente) ? cteEmitido.CPFCNPJRemetente : "";
                cteEmitido.Remetente = !string.IsNullOrWhiteSpace(cteEmitido.Remetente) ? cteEmitido.Remetente : "";
                cteEmitido.CPFCNPJDestinatario = !string.IsNullOrWhiteSpace(cteEmitido.CPFCNPJDestinatario) ? cteEmitido.CPFCNPJDestinatario : "";
                cteEmitido.Destinatario = !string.IsNullOrWhiteSpace(cteEmitido.Destinatario) ? cteEmitido.Destinatario : "";
                cteEmitido.NumeroNotaFiscal = !string.IsNullOrWhiteSpace(cteEmitido.NumeroNotaFiscal) ? cteEmitido.NumeroNotaFiscal : "";
                cteEmitido.CNPJTransportador = !string.IsNullOrWhiteSpace(cteEmitido.CNPJTransportador) ? cteEmitido.CNPJTransportador : "";
                cteEmitido.Transportador = !string.IsNullOrWhiteSpace(cteEmitido.Transportador) ? cteEmitido.Transportador : "";
                cteEmitido.Observacao = !string.IsNullOrWhiteSpace(cteEmitido.Observacao) ? cteEmitido.Observacao : "";
                cteEmitido.PlacaVeiculo = !string.IsNullOrWhiteSpace(cteEmitido.PlacaVeiculo) ? cteEmitido.PlacaVeiculo : "";
                cteEmitido.UFRemetente = !string.IsNullOrWhiteSpace(cteEmitido.UFRemetente) ? cteEmitido.UFRemetente : "";
                cteEmitido.UFDestinatario = !string.IsNullOrWhiteSpace(cteEmitido.UFDestinatario) ? cteEmitido.UFDestinatario : "";
                cteEmitido.UFTransportador = !string.IsNullOrWhiteSpace(cteEmitido.UFTransportador) ? cteEmitido.UFTransportador : "";
                cteEmitido.CNF = !string.IsNullOrWhiteSpace(cteEmitido.CNF) ? cteEmitido.CNF : "";
                cteEmitido.ChaveNotaFiscal = !string.IsNullOrEmpty(cteEmitido.ChaveNotaFiscal) && !string.IsNullOrWhiteSpace(cteEmitido.ChaveNotaFiscal) ? cteEmitido.ChaveNotaFiscal : string.Empty;
                cteEmitido.ChaveCTe = !string.IsNullOrEmpty(cteEmitido.ChaveCTe) && !string.IsNullOrWhiteSpace(cteEmitido.ChaveCTe) ? cteEmitido.ChaveCTe : string.Empty;
                cteEmitido.CodIntegracao = !string.IsNullOrEmpty(cteEmitido.CodIntegracao) && !string.IsNullOrWhiteSpace(cteEmitido.CodIntegracao) ? cteEmitido.CodIntegracao : string.Empty;
                cteEmitido.CPFCNPJTomador = !string.IsNullOrEmpty(cteEmitido.CPFCNPJTomador) && !string.IsNullOrWhiteSpace(cteEmitido.CPFCNPJTomador) ? cteEmitido.CPFCNPJTomador : string.Empty;
                cteEmitido.Tomador = !string.IsNullOrEmpty(cteEmitido.Tomador) && !string.IsNullOrWhiteSpace(cteEmitido.Tomador) ? cteEmitido.Tomador : string.Empty;
                cteEmitido.Contrato = !string.IsNullOrEmpty(cteEmitido.Contrato) && !string.IsNullOrWhiteSpace(cteEmitido.Contrato) ? cteEmitido.Contrato : string.Empty;
            }

            return listaCTesEmitidosPorEmbarcador;
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CpfCnpjEmbarcador > 0 ? repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjEmbarcador) : null;
            string cpfCnpjEmbarcadorSemFormato = cliente != null ? cliente.CPF_CNPJ_SemFormato : null;

            return _repositorioConhecimentoDeTransporteEletronico.ContarRelatorioCTesEmitidosPorEmbarcador(
                codigoEmpresaPai: filtrosPesquisa.CodigoEmpresaPai,
                codigoEmpresa: filtrosPesquisa.CodigoTransportador,
                cpfCnpjEmbarcador: cpfCnpjEmbarcadorSemFormato,
                dataAutorizacaoInicial: filtrosPesquisa.DataAutorizacaoInicial,
                dataAutorizacaoFinal: filtrosPesquisa.DataAutorizacaoFinal,
                dataEmissaoInicial: filtrosPesquisa.DataEmissaoInicial,
                dataEmissaoFinal: filtrosPesquisa.DataEmissaoFinal);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/CTeEmitido";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);
            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CpfCnpjEmbarcador > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjEmbarcador) : null;

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", "(" + empresa.CNPJ_Formatado + ") " + empresa.RazaoSocial, true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));
            }

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue || filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue ? filtrosPesquisa.DataEmissaoInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataEmissaoFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", data, true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", false));
            }

            if (filtrosPesquisa.DataAutorizacaoInicial != DateTime.MinValue || filtrosPesquisa.DataAutorizacaoFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataAutorizacaoInicial != DateTime.MinValue ? filtrosPesquisa.DataAutorizacaoInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataAutorizacaoFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataAutorizacaoFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAutorizacao", data, true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAutorizacao", false));
            }

            if (filtrosPesquisa.CpfCnpjEmbarcador > 0)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cliente", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cliente", false));
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar, true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));
            }

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
