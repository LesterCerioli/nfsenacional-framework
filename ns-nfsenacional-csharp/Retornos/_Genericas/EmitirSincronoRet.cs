using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_nfsenacional_csharp.Retornos._Genericas
{
    public class EmitirSincronoRet
    {
		public string statusEnvio;
		public string statusConsulta;
		public string statusDownload;
		public string cStat;
		public string nProt;
		public string motivo;
		public string nsNRec;
		public IList<string> erros { get; set; }
	}
}
