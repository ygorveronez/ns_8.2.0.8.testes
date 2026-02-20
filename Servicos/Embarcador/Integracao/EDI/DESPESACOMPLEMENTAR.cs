using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class DESPESACOMPLEMENTAR
    {
        public Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.DNE ConverterParaDESPESACOMPLEMENTAR(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, DateTime dataOcorrencia, string observacao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (ctes == null || ctes.Count <= 0)
                return null;

            Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.DNE dne = new Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.DNE();

            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unidadeTrabalho);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            dne.Notas = new List<Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.Nota>();
            dne.Despesa = new Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.Despesa();

            Dominio.Entidades.Empresa empresa = ctes.FirstOrDefault().Empresa;
            Dominio.Entidades.ParticipanteCTe tomador = ctes.FirstOrDefault().Tomador;

            //decimal valortotal = 0;
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota in cte.XMLNotaFiscais.ToList())
                {
                    Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.Nota notaDespesa = new Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR.Nota();
                    notaDespesa.NotaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
                    notaDespesa.NotaFiscal.Numero = xmlNota.Numero;
                    notaDespesa.NotaFiscal.Serie = xmlNota.Serie;
                    notaDespesa.NotaFiscal.Transportador = serEmpresa.ConverterObjetoEmpresa(cte.Empresa);
                    notaDespesa.NotaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(xmlNota.Emitente);
                    dne.Notas.Add(notaDespesa);
                }
                //valortotal += cte.ValorAReceber;
            }

            dne.Despesa.NumeroDespesa = cargaOcorrencia.NumeroOcorrencia.ToString();
            dne.Despesa.Evento = cargaOcorrencia.TipoOcorrencia.CodigoProceda;
            dne.Despesa.ValorDespesa = cargaOcorrencia.ValorOcorrencia;
            return dne;
        }
    }
}
