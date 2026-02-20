using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public class Stage
    {
        public static Dominio.Entidades.Embarcador.Pedidos.Stage ObterStageMaisRelevante(List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> pedidoStage)
        {
            if (pedidoStage == null || pedidoStage.Count == 0)
                return null;

            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = (from obj in pedidoStage select obj.Stage).Distinct().ToList();
            foreach (Dominio.Entidades.Embarcador.Pedidos.Stage stage in stages)
            {
                if (stage.TipoPercurso == Vazio.Todos)
                    return stage;

                if (!stage.RelevanciaCusto)
                    continue;

                if (stage.CargaDT.TipoDocumentoTransporte != null)
                {
                    if (stage.CargaDT.TipoDocumentoTransporte.CodigoIntegracao == "ZA24")
                    {
                        if (stage.TipoPercurso != Vazio.PercursoRegreso)
                            return stage;

                        continue;
                    }

                    if (stage.CargaDT.TipoDocumentoTransporte.CodigoIntegracao == "ZA21")
                    {
                        if ((stage.TipoPercurso != Vazio.PercursoDireto) && (stage.TipoPercurso != Vazio.PercursoRegreso))
                            return stage;

                        continue;
                    }

                    if (stage.CargaDT.TipoDocumentoTransporte.CodigoIntegracao == "ZA18")
                    {
                        bool existeStageRelevantePreliminar = stages.Any(x => x.TipoPercurso == Vazio.PercursoPreliminar && x.RelevanciaCusto);
                        bool existeStageRelevantePrincipal = stages.Any(x => x.TipoPercurso == Vazio.PercursoPrincipal && x.RelevanciaCusto);

                        if(existeStageRelevantePreliminar && existeStageRelevantePrincipal)
                        {
                            //vai a preliminar
                            return stages.Where(x => x.TipoPercurso == Vazio.PercursoPreliminar && x.RelevanciaCusto).FirstOrDefault();
                        }
                      
                        //if ((stage.TipoPercurso != Vazio.PercursoPrincipal) && (stage.TipoPercurso != Vazio.PercursoSubSeQuente))
                        //    return stage;

                        if (stage.TipoPercurso != Vazio.PercursoSubSeQuente)
                            return stage;

                        continue;
                    }

                    if (stage.CargaDT.TipoDocumentoTransporte.CodigoIntegracao == "ZA02")
                    {
                        if (stage.TipoPercurso != Vazio.PercursoDireto)
                            return stage;

                        continue;
                    }
                }

                if (stage.TipoPercurso != Vazio.PercursoRegreso)
                    return stage;
            }

            return null;
        }

        public static Dominio.Entidades.Embarcador.Pedidos.Stage BuscarStagePorCargaCte(int codigoCargaCte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            int codigoStage = repositorioCargaCTe.BuscarCodigoStageRelevantePorCargaCTe(codigoCargaCte);

            return (codigoStage > 0) ? repStage.BuscarPorCodigo(codigoStage, false) : null;
        }

        public static dynamic ObterDocumentoContabelPorStage(IGrouping<(int CodigoStage, string NumeroStage), Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentoAgrupado, Repositorio.UnitOfWork unitOfWork)
        {
            (int CodigoStage, string NumeroStage) dadosStage = documentoAgrupado.Key;
            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis = documentoAgrupado.ToList();
            List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> impostosValorAgregado = documentosContabeis.Select(documento => documento.ImpostoValorAgregado).Distinct().ToList();
            Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = (impostosValorAgregado.Count == 1) ? impostosValorAgregado.FirstOrDefault() : null;

            decimal aliquotaCofins = 0m;
            decimal aliquotaIcms = 0m;
            decimal aliquotaIss = 0m;
            decimal aliquotaPis = 0m;
            decimal valorTotalAdValorem = 0m;
            decimal valorTotalCofins = 0m;
            decimal valorTotalIcms = 0m;
            decimal valorTotalIcmsST = 0m;
            decimal valorTotalIss = 0m;
            decimal valorTotalIssRetido = 0m;
            decimal valorTotalFreteLiquido = 0m;
            decimal valorTotalGris = 0m;
            decimal valorTotalPedagio = 0m;
            decimal valorTotalPis = 0m;
            decimal valorTotalReceber = 0m;
            decimal valorTotalTaxaDescarga = 0m;
            decimal valorTotalTaxaEntrega = 0m;
            decimal valorTotalCustoFixo = 0m;
            decimal valorTotalFreteCaixa = 0m;
            decimal valorTotalFreteKM = 0m;
            decimal valorTotalFretePeso = 0m;
            decimal valorTotalFreteViagem = 0m;
            decimal valorTotalTaxa = 0m;
            decimal valorTotalPernoite = 0m;

            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil in documentosContabeis)
            {
                if (documentoContabil.TipoContaContabil == TipoContaContabil.AdValorem)
                    valorTotalAdValorem += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido || documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido2 || documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido9)
                    valorTotalFreteLiquido += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TotalReceber)
                    valorTotalReceber += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.Pedagio)
                    valorTotalPedagio += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaDescarga)
                    valorTotalTaxaDescarga += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaEntrega)
                    valorTotalTaxaEntrega += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ICMSST)
                    valorTotalIcmsST += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.GRIS)
                    valorTotalGris += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.CustoFixo)
                    valorTotalCustoFixo += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteCaixa)
                    valorTotalFreteCaixa += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteKM)
                    valorTotalFreteKM += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FretePeso)
                    valorTotalFretePeso += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteViagem)
                    valorTotalFreteViagem += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaTotal)
                    valorTotalTaxa += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.Pernoite)
                    valorTotalPernoite += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ISS)
                {
                    aliquotaIss = documentoContabil.AliquotaIss;
                    valorTotalIss += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ISSRetido)
                {
                    aliquotaIss = documentoContabil.AliquotaIss;
                    valorTotalIssRetido += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ICMS)
                {
                    aliquotaIcms = documentoContabil.DocumentoProvisao?.PercentualAliquota ?? 0m;
                    valorTotalIcms += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.PIS)
                {
                    aliquotaPis = documentoContabil.AliquotaPis;
                    valorTotalPis += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.COFINS)
                {
                    aliquotaCofins = documentoContabil.AliquotaCofins;
                    valorTotalCofins += documentoContabil.ValorContabilizacao;
                }
            }

            return new
            {
                CodigoStage = documentoAgrupado.Key.CodigoStage,
                NumeroStage = documentoAgrupado.Key.NumeroStage,
                IVA = impostoValorAgregado?.CodigoIVA ?? "",
                AdValorem = (valorTotalAdValorem > 0) ? valorTotalAdValorem.ToString("n2") : "",
                AliquotaCofins = (aliquotaCofins > 0) ? aliquotaCofins.ToString("n2") : "",
                AliquotaIcms = (aliquotaIcms > 0) ? aliquotaIcms.ToString("n2") : "",
                AliquotaIss = (aliquotaIss > 0) ? aliquotaIss.ToString("n2") : "",
                AliquotaPis = (aliquotaPis > 0) ? aliquotaPis.ToString("n2") : "",
                Cofins = (valorTotalCofins > 0) ? valorTotalCofins.ToString("n2") : "",
                Icms = (valorTotalIcms > 0) ? valorTotalIcms.ToString("n2") : "",
                IcmsST = (valorTotalIcmsST > 0) ? valorTotalIcmsST.ToString("n2") : "",
                Iss = (valorTotalIss > 0) ? valorTotalIss.ToString("n2") : "",
                IssRetido = (valorTotalIssRetido > 0) ? valorTotalIssRetido.ToString("n2") : "",
                CustoFixo = (valorTotalCustoFixo > 0) ? valorTotalCustoFixo.ToString("n2") : "",
                FreteCaixa = (valorTotalFreteCaixa > 0) ? valorTotalFreteCaixa.ToString("n2") : "",
                FreteKM = (valorTotalFreteKM > 0) ? valorTotalFreteKM.ToString("n2") : "",
                FretePeso = (valorTotalFretePeso > 0) ? valorTotalFretePeso.ToString("n2") : "",
                FreteViagem = (valorTotalFreteViagem > 0) ? valorTotalFreteViagem.ToString("n2") : "",
                FreteLiquido = (valorTotalFreteLiquido > 0) ? valorTotalFreteLiquido.ToString("n2") : "",
                FreteTotal = (valorTotalReceber > 0) ? valorTotalReceber.ToString("n2") : "",
                Gris = (valorTotalGris > 0) ? valorTotalGris.ToString("n2") : "",
                Pedagio = (valorTotalPedagio > 0) ? valorTotalPedagio.ToString("n2") : "",
                Pis = (valorTotalPis > 0) ? valorTotalPis.ToString("n2") : "",
                TaxaDescarga = (valorTotalTaxaDescarga > 0) ? valorTotalTaxaDescarga.ToString("n2") : "",
                TaxaEntrega = (valorTotalTaxaEntrega > 0) ? valorTotalTaxaEntrega.ToString("n2") : "",
                TaxaTotal = (valorTotalTaxa > 0) ? valorTotalTaxa.ToString("n2") : "",
                Pernoite = (valorTotalPernoite > 0) ? valorTotalPernoite.ToString("n2") : ""
            };
        }
    }
}