using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns_nfsenacional_csharp.Compartilhados
{
    public class Endpoints
    {
        public string NFSeEnvio { get; set; } = "https://nfsenacionalhml.ns.eti.br/nfse/issue";
        public string NFSeConsStatusProcessamento { get; set; } = "https://nfsenacionalhml.ns.eti.br/nfse/issue/status";
        public string NFSeDownload { get; set; } = "https://nfsenacionalhml.ns.eti.br/nfse/get";
    }
}
