using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ReturnDocumentV1
    {
        public string Message { get; set; }
        public DocumentV1 Document { get; set; }
    }

    public class DocumentV1
    {
        public string Label { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public string DocumentType { get; set; }
        public string Travel { get; set; }
        public string _Id { get; set; } // "_id" do JSON
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Code { get; set; }
    }
}
