using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Pessoa
{
    public sealed class GrupoPessoasObservacaoNfe
    {
        #region Métodos Privados

        private string ObterLacreContainerDois(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.LacreContainerDois).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        private string ObterLacreContainerTres(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.LacreContainerTres).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        private string ObterLacreContainerUm(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.LacreContainerUm).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        private string ObterNumeroContainer(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.NumeroContainer).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        private string ObterNumeroPedidoEmbarcador(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.NumeroPedido).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        public string ObterNumeroControleCliente(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.NumeroControleCliente).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        public string ObterNumeroControlePedido(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.NumeroControlePedido && (observacaoNfe?.Contains(o.IdentificadorInicio) ?? false)).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        public string ObterNumeroReferenciaEDI(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.NumeroReferenciaEDI).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        private string ObterObservacao(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            string observacao = grupoPessoas.ObservacaoNfe;

            if (string.IsNullOrWhiteSpace(observacao))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(observacaoNfe))
                return observacao;

            List<string> tagsUtilizadasObservacao = ObterTagsFormulasUtilizadasObservacao(observacao);
            Dictionary<string, string> formulasSubstituirObservacaoNfe = new Dictionary<string, string>();

            foreach (string tag in tagsUtilizadasObservacao)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.Tag == tag.Substring(1)).FirstOrDefault();

                if (formula != null)
                    formulasSubstituirObservacaoNfe.Add(tag, ObterValorFormula(formula, observacaoNfe, string.Empty));
            }

            foreach (var formulaSubstituir in formulasSubstituirObservacaoNfe)
                observacao = observacao.Replace(formulaSubstituir.Key, formulaSubstituir.Value);

            return observacao;
        }

        private List<string> ObterTagsFormulasUtilizadasObservacao(string observacao)
        {
            HashSet<string> listaTag = new HashSet<string>();
            Regex expressaoObterTagFormulas = new Regex(@"#\w+");
            MatchCollection tagsEncontradas = expressaoObterTagFormulas.Matches(observacao);

            foreach (Match tag in tagsEncontradas)
                listaTag.Add(tag.Value);

            return listaTag.ToList();
        }

        private string ObterTaraContainer(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string observacaoNfe, string valorAnterior)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula = grupoPessoas.FormulasObservacaoNfe?.Where(o => o.TaraContainer).FirstOrDefault();

            return ObterValorFormula(formula, observacaoNfe, valorAnterior);
        }

        private string ObterValorFormula(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formula, string texto, string valorAnterior)
        {
            if (formula == null)
                return valorAnterior ?? string.Empty;

            int posicaoInicialValorFormula = 0;
            int posicaoFinalValorFormula = texto.Length;

            if (!string.IsNullOrEmpty(formula.IdentificadorInicio))
            {
                posicaoInicialValorFormula = texto.IndexOf(formula.IdentificadorInicio);

                if (posicaoInicialValorFormula < 0)
                    return valorAnterior ?? string.Empty;

                posicaoInicialValorFormula += formula.IdentificadorInicio.Length;
                posicaoFinalValorFormula = formula.IdentificadorFim.Length == 0 ? -1 : texto.IndexOf(formula.IdentificadorFim, posicaoInicialValorFormula);

                if (posicaoFinalValorFormula < 0)
                    posicaoFinalValorFormula = texto.Length;
            }

            int tamanhoValorFormula = posicaoFinalValorFormula - posicaoInicialValorFormula;

            if (formula.QtdMaximoDigitos.HasValue && formula.QtdMaximoDigitos.Value > 0)
                tamanhoValorFormula = formula.QtdMaximoDigitos.Value;

            if (tamanhoValorFormula <= 0)
                return valorAnterior ?? string.Empty;

            if (posicaoInicialValorFormula + tamanhoValorFormula > texto.Length)
                tamanhoValorFormula = texto.Length - posicaoInicialValorFormula;

            string valorFormula = texto.Substring(posicaoInicialValorFormula, tamanhoValorFormula).Trim();

            return string.IsNullOrEmpty(valorFormula) ? !string.IsNullOrWhiteSpace(valorAnterior) ? valorAnterior : string.Empty : valorFormula;
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarDadosNfePorGrupoPessoasEmitente(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente emitenteNfe, string observacaoNfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = null;
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grupoPessoas = cargaPedido.ObterTomador()?.GrupoPessoas;
            else
                grupoPessoas = emitenteNfe?.GrupoPessoas;

            if (grupoPessoas == null)
                return;

            string observacaoNFe = "";
            if (!configuracaoTMS.GerarObservacaoDaNotaPorCTe)
                cargaPedido.Pedido.ObservacaoCTe += ObterObservacao(grupoPessoas, observacaoNfe, string.Empty);
            else
                observacaoNFe = ObterObservacao(grupoPessoas, observacaoNfe, string.Empty);

            if (string.IsNullOrWhiteSpace(observacaoNfe))
            {
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    cargaPedido.Pedido.NumeroContainer = string.Empty;
                    cargaPedido.Pedido.LacreContainerUm = string.Empty;
                    cargaPedido.Pedido.LacreContainerDois = string.Empty;
                    cargaPedido.Pedido.LacreContainerTres = string.Empty;
                    cargaPedido.Pedido.TaraContainer = string.Empty;
                }

                return;
            }

            if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroPedidoEmbarcador))
                cargaPedido.Pedido.NumeroPedidoEmbarcador = ObterNumeroPedidoEmbarcador(grupoPessoas, observacaoNfe, cargaPedido.Pedido.NumeroPedidoEmbarcador);

            string numeroContainer = ObterNumeroContainer(grupoPessoas, observacaoNfe, cargaPedido.Pedido.NumeroContainer);
            if (string.IsNullOrWhiteSpace(numeroContainer) && !string.IsNullOrWhiteSpace(grupoPessoas.ExpressaoContainer))
            {
                string observacaoContainer = observacaoNfe.ToLower();
                System.Text.RegularExpressions.Regex patternContainer = new System.Text.RegularExpressions.Regex(grupoPessoas.ExpressaoContainer, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Match matchContainer = patternContainer.Match(observacaoContainer);
                if (matchContainer != null && !string.IsNullOrWhiteSpace(matchContainer.Value))
                    numeroContainer = matchContainer.Value.ToUpper();
            }

            if (!string.IsNullOrWhiteSpace(numeroContainer))
            {
                numeroContainer = numeroContainer.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                cargaPedido.Pedido.NumeroContainer = numeroContainer.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
            }

            if (configuracaoTMS.UtilizaEmissaoMultimodal && !string.IsNullOrWhiteSpace(numeroContainer) && cargaPedido.Pedido.Container == null)
            {
                cargaPedido.Pedido.Container = repContainer.BuscarPorNumero(numeroContainer);
                if (cargaPedido.Pedido.Container == null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Container novoContainer = new Dominio.Entidades.Embarcador.Pedidos.Container()
                    {
                        ContainerTipo = cargaPedido.Pedido.ContainerTipoReserva,
                        Descricao = numeroContainer,
                        Numero = Utilidades.String.SanitizeString(numeroContainer),
                        Status = true,
                        TipoPropriedade = (serPedido.ValidarDigitoContainerNumero(numeroContainer) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer.Proprio : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer.Soc)                        
                    };
                    novoContainer.DataUltimaAtualizacao = DateTime.Now;
                    novoContainer.Integrado = false;
                    repContainer.Inserir(novoContainer, auditado);
                    cargaPedido.Pedido.Container = repContainer.BuscarPorCodigo(novoContainer.Codigo);
                }
            }

            cargaPedido.Pedido.LacreContainerUm = ObterLacreContainerUm(grupoPessoas, observacaoNfe, cargaPedido.Pedido.LacreContainerUm);
            if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerUm) && !string.IsNullOrWhiteSpace(configuracaoTMS.ExpressaoLacreContainer))
            {
                string observacaoLacre = observacaoNfe.ToLower();
                string numeroLacre = "";
                System.Text.RegularExpressions.Regex patternContainer = new System.Text.RegularExpressions.Regex(configuracaoTMS.ExpressaoLacreContainer, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Match matchContainer = patternContainer.Match(observacaoLacre);
                if (matchContainer != null && !string.IsNullOrWhiteSpace(matchContainer.Value))
                {
                    numeroLacre = matchContainer.Value.ToUpper();
                    if (!string.IsNullOrWhiteSpace(numeroLacre))
                    {
                        numeroLacre = numeroLacre.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();
                        cargaPedido.Pedido.LacreContainerUm = numeroLacre;
                    }
                }
            }

            cargaPedido.Pedido.LacreContainerDois = ObterLacreContainerDois(grupoPessoas, observacaoNfe, cargaPedido.Pedido.LacreContainerDois);
            cargaPedido.Pedido.LacreContainerTres = ObterLacreContainerTres(grupoPessoas, observacaoNfe, cargaPedido.Pedido.LacreContainerTres);
            cargaPedido.Pedido.TaraContainer = ObterTaraContainer(grupoPessoas, observacaoNfe, cargaPedido.Pedido.TaraContainer);

            string numeroControlePedido = ObterNumeroControlePedido(grupoPessoas, observacaoNfe, cargaPedido.Pedido.NumeroControle);
            string numeroControleCliente = ObterNumeroControleCliente(grupoPessoas, observacaoNfe, xmlNotaFiscal.NumeroControleCliente);
            string numeroReferenciaEDI = ObterNumeroReferenciaEDI(grupoPessoas, observacaoNfe, xmlNotaFiscal.NumeroReferenciaEDI);

            if (!string.IsNullOrWhiteSpace(numeroControleCliente) || 
                !string.IsNullOrWhiteSpace(numeroReferenciaEDI) || 
                !string.IsNullOrWhiteSpace(observacaoNFe) || 
                !string.IsNullOrWhiteSpace(numeroControlePedido))
            {
                if (!string.IsNullOrWhiteSpace(numeroControleCliente))
                    xmlNotaFiscal.NumeroControleCliente = numeroControleCliente;

                if (!string.IsNullOrWhiteSpace(numeroReferenciaEDI))
                    xmlNotaFiscal.NumeroReferenciaEDI = numeroReferenciaEDI;

                if (!string.IsNullOrWhiteSpace(numeroControlePedido))
                    xmlNotaFiscal.NumeroControlePedido = numeroControlePedido;

                if (!string.IsNullOrWhiteSpace(observacaoNFe))
                    xmlNotaFiscal.ObservacaoNotaFiscalParaCTe = observacaoNFe;


                repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
            }
        }

        #endregion
    }
}
