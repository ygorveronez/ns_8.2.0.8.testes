using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class FechamentoDiario
    {
        #region Métodos Públicos

        public static bool RealizarFechamento(out string erro, int codigoEmpresa, DateTime dataFechamento, bool bloquearApenasDocumentoEntrada, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unidadeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);

            if (dataFechamento >= DateTime.Now.Date)
            {
                erro = "A data do fechamento não pode ser posterior ou igual à data atual.";
                return false;
            }

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento? tipoDocumento = null;
            if (bloquearApenasDocumentoEntrada)
                tipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada;

            if (VerificarSeExisteFechamento(codigoEmpresa, dataFechamento, unidadeTrabalho, tipoDocumento))
            {
                erro = "Já existe um fechamento em uma data igual ou posterior à " + dataFechamento.ToString("dd/MM/yyyy") + ".";
                return false;
            }

            List<long> documentosPendentes = repDocumentoEntrada.ConsultarDocumentoPendente(codigoEmpresa, dataFechamento, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto });
            if (documentosPendentes.Count > 0)
            {
                erro = "Os seguintes documentos de entrada anteriores à data de fechamento estão pendentes: " + string.Join(", ", documentosPendentes);
                return false;
            }

            if (!bloquearApenasDocumentoEntrada)
            {
                List<KeyValuePair<string, string>> ctePendentes = repCTe.ConsultarSeExisteCTePendente(codigoEmpresa, dataFechamento, new string[] { "P", "E", "R", "S", "K", "L" });
                if (ctePendentes.Count > 0)
                {
                    erro = "Os seguintes documentos, anteriores à data de fechamento, estão pendentes: ";

                    List<string> modelos = ctePendentes.Select(o => o.Key).Distinct().ToList();

                    foreach (string modelo in modelos)
                        erro += $"{modelo} {string.Join(", ", ctePendentes.Where(o => o.Key == modelo).Select(o => o.Value))} / ";

                    erro = erro.Remove(erro.Length - 3) + ".";

                    return false;
                }

                List<int> nfsePendentes = repNFSe.ConsultarSeExisteNFSePendente(codigoEmpresa, dataFechamento, new Dominio.Enumeradores.StatusNFSe[] { Dominio.Enumeradores.StatusNFSe.EmCancelamento, Dominio.Enumeradores.StatusNFSe.EmDigitacao, Dominio.Enumeradores.StatusNFSe.Enviado, Dominio.Enumeradores.StatusNFSe.Pendente, Dominio.Enumeradores.StatusNFSe.Rejeicao });
                if (nfsePendentes.Count > 0)
                {
                    erro = "As seguintes NFS-es anteriores à data de fechamento estão pendentes: " + string.Join(", ", nfsePendentes);
                    return false;
                }

                List<string> cargasPendentes = repCarga.ConsultarSeExisteCargaPendente(codigoEmpresa, dataFechamento, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos });
                if (cargasPendentes.Count > 0)
                {
                    erro = "As seguintes cargas anteriores à data de fechamento estão com pendência na emissão:" + string.Join(", ", cargasPendentes);
                    return false;
                }

                List<int> contratoFrete = repContratoFrete.ConsultarSeExisteContratoPendente(codigoEmpresa, dataFechamento, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao });
                if (contratoFrete.Count > 0)
                {
                    erro = "Os seguintes contratos de frete anteriores à data de fechamento aguardando aprovação: " + string.Join(", ", contratoFrete);
                    return false;
                }

                List<int> acertosViagem = repAcertoViagem.ConsultarSeExisteAcertoPendente(codigoEmpresa, dataFechamento, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento });
                if (acertosViagem.Count > 0)
                {
                    erro = "Os seguintes acertos de viagem anteriores à data de fechamento em andamento: " + string.Join(", ", acertosViagem);
                    return false;
                }

                List<int> faturasPendentes = repFatura.ConsultarSeExisteFaturaPendente(codigoEmpresa, dataFechamento, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento });
                if (faturasPendentes.Count > 0)
                {
                    erro = "As seguintes faturas anteriores à data de fechamento em andamento: " + string.Join(", ", faturasPendentes);
                    return false;
                }

                List<int> tituloBaixa = repTituloBaixa.ConsultarSeExisteTituloPendente(codigoEmpresa, dataFechamento, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.EmNegociacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada });
                if (tituloBaixa.Count > 0)
                {
                    erro = "Os seguintes títulos anteriores à data de fechamento em andamento: " + string.Join(", ", tituloBaixa);
                    return false;
                }

                List<int> abastecimentoPendentes = repAbastecimento.ConsultarSeExisteAbastecimentoPendente(codigoEmpresa, dataFechamento, new string[] { "A", "I" });
                if (abastecimentoPendentes.Count > 0)
                {
                    erro = "Os seguintes abastecimentos anteriores à data de fechamento estão pendentes: " + string.Join(", ", abastecimentoPendentes);
                    return false;
                }

                List<int> pedagiosPendentes = repPedagio.ConsultarSeExistePedagioPendente(codigoEmpresa, dataFechamento, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos });
                if (pedagiosPendentes.Count > 0)
                {
                    erro = "Os seguintes pedágios anteriores à data de fechamento estão pendentes: " + string.Join(", ", pedagiosPendentes);
                    return false;
                }
            }

            Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario fechamentoDiario = new Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario()
            {
                DataGeracao = DateTime.Now,
                DataFechamento = dataFechamento,
                Usuario = usuario,
                BloquearApenasDocumentoEntrada = bloquearApenasDocumentoEntrada,
                Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null
            };

            repFechamentoDiario.Inserir(fechamentoDiario, Auditado);

            erro = string.Empty;
            return true;
        }

        public static bool ReabrirFechamento(out string erro, Dominio.Entidades.Embarcador.Financeiro.FechamentoDiario fechamentoDiario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unitOfWork);

            if (repFechamentoDiario.VerificarSeExisteFechamentoPosterior(fechamentoDiario.Empresa?.Codigo ?? 0, fechamentoDiario.DataFechamento))
            {
                erro = "Existe um fechamento com data posterior ao informado, não sendo possível remover o fechamento.";
                return false;
            }

            repFechamentoDiario.Deletar(fechamentoDiario, Auditado);

            erro = string.Empty;
            return true;
        }

        public static bool VerificarSeExisteFechamento(int codigoEmpresa, DateTime data, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento? tipoDocumento = null, List<Dominio.ObjetosDeValor.Embarcador.Financeiro.FechamentoDiario> listaControleJaSelecionados = null)
        {
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unidadeTrabalho);

            if (listaControleJaSelecionados != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FechamentoDiario fechamentoDiario = listaControleJaSelecionados.Where(o => o.Data == data.Date).FirstOrDefault();

                if (fechamentoDiario != null)
                    return fechamentoDiario.Existe;
                else
                {
                    bool existe = repFechamentoDiario.VerificarSeExistePorDataFechamento(codigoEmpresa, data, tipoDocumento);

                    if (!existe)
                        existe = repFechamentoDiario.VerificarSeExistePorDataFechamento(0, data, tipoDocumento);

                    listaControleJaSelecionados.Add(new Dominio.ObjetosDeValor.Embarcador.Financeiro.FechamentoDiario() { Data = data.Date, Existe = existe });

                    return existe;
                }
            }

            if (repFechamentoDiario.VerificarSeExistePorDataFechamento(codigoEmpresa, data, tipoDocumento))
                return true;
            else if (repFechamentoDiario.VerificarSeExistePorDataFechamento(0, data, tipoDocumento))
                return true;

            return false;
        }

        #endregion
    }
}
