using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_nfsenacional_csharp.Respostas._Genericas
{
    public class DownloadResp
    {
		public string status { get; set; }
		public string motivo { get; set; }
		public Erro erro { get; set; }
		public string xml { get; set; }
		public string pdfDocumento { get; set; }
	}
}
