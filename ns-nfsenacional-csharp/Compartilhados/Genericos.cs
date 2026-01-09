using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ns_nfsenacional_csharp.Compartilhados
{
    public class Genericos
    {
		public static void salvarXML(string xml, string caminho, string nome, string tpEvento = "", string nSeqEvento = "")
		{
			string localParaSalvar = caminho + tpEvento + nome + nSeqEvento + ".xml";
			string ConteudoSalvar = "";
			ConteudoSalvar = xml.Replace(@"\""", "");
			File.WriteAllText(localParaSalvar, ConteudoSalvar);
		}

		public static void salvarJSON(string json, string caminho, string nome, string tpEvento = "", string nSeqEvento = "")
		{
			string localParaSalvar = caminho + tpEvento + nome + nSeqEvento + ".json";
			File.WriteAllText(localParaSalvar, json);
		}

		public static void salvarJSONemissao(string json, string caminho, string nome)
		{
			string localParaSalvar = caminho + nome + ".json";
			File.WriteAllText(localParaSalvar, json);
		}

		public static void salvarPDF(string pdf, string caminho, string nome, string tpEvento = "", string nSeqEvento = "")
		{
			string localParaSalvar = caminho + tpEvento + nome + nSeqEvento + ".pdf";
			byte[] bytes = Convert.FromBase64String(pdf);
			if (File.Exists(localParaSalvar))
				File.Delete(localParaSalvar);
			FileStream stream = new FileStream(localParaSalvar, FileMode.CreateNew);
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(bytes, 0, bytes.Length);
			writer.Close();
		}

		public static void gravarLinhaLog(string modelo, string conteudo)
		{
			try
			{
				string diretorioBase = AppDomain.CurrentDomain.BaseDirectory;
				string caminhoPasta = Path.Combine(diretorioBase, "log");

				if (!Directory.Exists(caminhoPasta))
					Directory.CreateDirectory(caminhoPasta);

				string dataParaNome = DateTime.Now.ToString("ddMMyyyy");

				string modeloLimpo = modelo.Replace(":", "").Replace("\\", "").Replace("/", "");

				string nomeArq = $"{dataParaNome}_{modeloLimpo}.txt";
				string caminhoCompleto = Path.Combine(caminhoPasta, nomeArq);

				using (StreamWriter outputFile = new StreamWriter(caminhoCompleto, true))
				{
					outputFile.WriteLine($"{DateTime.Now} - {conteudo}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro ao gravar log: {ex.Message}");
			}
		}

		public static X509Certificate2 buscaCertificado(String cnpj)
		{
			X509Certificate2Collection lcerts;
			X509Store lStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

			lStore.Open(OpenFlags.ReadOnly);

			lcerts = lStore.Certificates;
			X509Certificate2 cert = null;
			foreach (X509Certificate2 elemento in lcerts)
			{
				if (elemento.Subject.Contains(cnpj))
				{
					cert = elemento;
					lStore.Close();
					return cert;
				}
			}
			lStore.Close();
			return cert;
		}

		public static string assinaXML(string XMLString, string RefUri, X509Certificate2 X509Cert)
		{
			XmlDocument XMLDoc;
			try
			{

				string _xnome = "";
				if (X509Cert != null)
				{
					_xnome = X509Cert.Subject.ToString();
				}
				X509Certificate2 _X509Cert = new X509Certificate2();
				X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
				store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
				X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
				X509Certificate2Collection collection1 = (X509Certificate2Collection)collection.Find(X509FindType.FindBySubjectDistinguishedName, _xnome, false);
				if (collection1.Count == 0)
				{
					throw new Exception("Problemas no certificado digital");
				}
				else
				{
					_X509Cert = collection1[0];
					string x;
					x = _X509Cert.GetKeyAlgorithm().ToString();
					XmlDocument doc = new XmlDocument();
					doc.PreserveWhitespace = false;

					try
					{
						doc.LoadXml(XMLString);
						int qtdeRefUri = doc.GetElementsByTagName(RefUri).Count;

						if (qtdeRefUri == 0)
						{
							throw new Exception("A tag de assinatura " + RefUri.Trim() + " inexiste");
						}
						else
						{
							if (qtdeRefUri > 1)
							{
								throw new Exception("A tag de assinatura " + RefUri.Trim() + " não é unica");
							}
							else
							{
								try
								{

									SignedXml signedXml = new SignedXml(doc);

									signedXml.SigningKey = _X509Cert.PrivateKey;

									Reference reference = new Reference();

									XmlAttributeCollection _Uri = doc.GetElementsByTagName(RefUri).Item(0).Attributes;
									foreach (XmlAttribute _atributo in _Uri)
									{
										if (_atributo.Name == "Id")
										{
											reference.Uri = "#" + _atributo.InnerText;
										}
									}

									XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
									reference.AddTransform(env);

									XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
									reference.AddTransform(c14);

									signedXml.AddReference(reference);

									KeyInfo keyInfo = new KeyInfo();

									keyInfo.AddClause(new KeyInfoX509Data(_X509Cert));

									signedXml.KeyInfo = keyInfo;

									signedXml.ComputeSignature();

									XmlElement xmlDigitalSignature = signedXml.GetXml();

									doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
									XMLDoc = new XmlDocument();
									XMLDoc.PreserveWhitespace = false;
									XMLDoc = doc;
									return XMLDoc.OuterXml;
								}
								catch (Exception caught)
								{
									throw new Exception("Erro: Ao assinar o documento - " + caught.Message);
								}
							}
						}
					}
					catch (Exception caught)
					{
						throw new Exception("Erro: XML mal formado - " + caught.Message);
					}
				}
			}
			catch (Exception caught)
			{
				throw new Exception("Erro: Problema ao acessar o certificado digital" + caught.Message);
			}
		}
	}
}
